namespace KickblipsTwo.UI.Screens
{
    using KickblipsTwo.IO;
    using KickblipsTwo.Platform;
    using KickblipsTwo.UI.Screens.SongSelection;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    public class SongSelectScreen : UI.Screen
    {
        [Header("References")]
        [SerializeField, Tooltip("The loading text.")]
        private TMP_Text loadingText;

        [SerializeField, Tooltip("The song item prefab that will be instantiated & initialized after we found a valid song.")]
        private SongItemWindow songItem;

        [SerializeField, Tooltip("The area where all the songs will be listed.")]
        private Transform songContentArea;

        [SerializeField, Tooltip("The quit button")]
        private Button quitButton;

        [SerializeField, Tooltip("The options button")]
        private Button optionsButton;

        [Header("Settings")]
        [SerializeField, Tooltip("Difficulty names")]
        private string[] difficulties;

        [SerializeField, Tooltip("The control for changing difficulty.")]
        private InputActionReference difficultyChangeAction;

        private List<SongItemWindow> songItems = new List<SongItemWindow>();

        private Coroutine waitForSongsCoroutine;

        private GameObject lastKnownSelectedGameObject;


        private IEnumerator Start()
        {
            loadingText.gameObject.SetActive(true);
            quitButton.onClick.AddListener(Application.Quit);
            optionsButton.onClick.AddListener(() => ScreenManager.ChangeScreen(ScreenType.Options));

            // BROWSE THROUGH ALL THE SONGS.
            string[] directories = Directory.GetDirectories($"{Application.streamingAssetsPath}/{FileHandler.DefaultMusicFolder}/");

            for (int i = 0; i < directories.Length; i++)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directories[i]);

                bool containsMidiFile = false;
                bool containsAudioFile = false;
                bool validatingMidiFile = false;

                string[] files = Directory.GetFiles(directories[i]);

                for (int j = 0; j < files.Length; j++)
                {
                    FileInfo fileInfo = new FileInfo(files[j]);

                    // Looking for a Midi file.
                    if (!containsMidiFile)
                        if (Equals(fileInfo.Extension, ".mid"))
                            containsMidiFile = true;

                    // Looking for an audio file.
                    if (!containsAudioFile)
                        if (Equals(fileInfo.Extension, ".wav") || Equals(fileInfo.Extension, ".mp3"))
                            containsAudioFile = true;

                    // If it contains a midi & audio file, then validate the Midi content.
                    if (containsMidiFile && containsAudioFile)
                    {
                        bool midiValid = true;
                        validatingMidiFile = true;
                        string originalArtistName = null;
                        SongInfo songInfo = null;

                        FileHandler.HighlightFolder(directoryInfo.Name);
                        FileHandler.FetchMidi((midiFile) =>
                        {
                            songInfo = new SongInfo(directoryInfo.Name, midiFile.Tracks, originalArtistName, PlatformHandler.GetHighscore(directoryInfo.Name));
                            SongInfoManager.SongList.Add(songInfo);

                            midiValid = songInfo.ValidateSong(out originalArtistName) == SongInfo.ErrorCode.Success;
                            validatingMidiFile = false;
                        });

                        while (validatingMidiFile)
                            yield return null;

                        // It seems that the Midi is invalid. That means we won't list this song.
                        if (!midiValid)
                            continue;

                        SongItemWindow songItemWindow = Instantiate(songItem, songContentArea);
                        songItemWindow.SetupWindow(songInfo, directoryInfo.Name, difficulties[Game.Instance.PreferredDifficulty], originalArtistName);

                        songItems.Add(songItemWindow);

                        // We have now instantiated the object. We don't need to check any more files.
                        break;
                    }
                }
            }

            loadingText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Checks everytime the last known selected game object changes if the song supports the current difficulty.
        /// </summary>
        private void Update()
        {
            if (!Equals(lastKnownSelectedGameObject, EventSystem.current.currentSelectedGameObject))
            {
                lastKnownSelectedGameObject = EventSystem.current.currentSelectedGameObject;

                if (lastKnownSelectedGameObject.GetComponent<SongItemWindow>() is SongItemWindow songItemWindow && songItemWindow.SongInfo.Tracks.Length < Game.Instance.PreferredDifficulty)
                    Game.Instance.PreferredDifficulty = 0;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            difficultyChangeAction.action.started += OnDifficultyChanged;
            difficultyChangeAction.action.Enable();

            waitForSongsCoroutine = StartCoroutine(DoWaitForSongs());
            IEnumerator DoWaitForSongs()
            {
                // Wait for those songs.
                while (loadingText.gameObject.activeInHierarchy)
                    yield return null;

                // Song items initialized? Then select the first game object.
                if (firstSelectedButton == null && songItems.Count > 0)
                    EventSystem.current.SetSelectedGameObject(songItems[0].gameObject);
                else if (songItems.Count == 0)
                    EventSystem.current.SetSelectedGameObject(quitButton.gameObject);

                for (int i = 0; i < songItems.Count; i++)
                    songItems[i].UpdateHighscore(songItems[i].SongInfo.Highscore);
            }
        }

        private void OnDisable()
        {
            difficultyChangeAction.action.started -= OnDifficultyChanged;
            difficultyChangeAction.action.Disable();

            if (waitForSongsCoroutine != null)
            {
                StopCoroutine(waitForSongsCoroutine);
                waitForSongsCoroutine = null;
            }
        }

        /// <summary>
        /// Will be executed whenever the difficulty changes.
        /// </summary>
        private void OnDifficultyChanged(InputAction.CallbackContext obj)
        {
            Game.Instance.PreferredDifficulty++;

            if (Game.Instance.PreferredDifficulty >= difficulties.Length)
                Game.Instance.PreferredDifficulty = 0;

            string difficultyName = difficulties[Game.Instance.PreferredDifficulty];

            Debug.Log("[SongSelectScreen] Change difficulty to: " + difficultyName);

            // Only change difficulty when it's supported!
            for (int i = 0; i < songItems.Count; i++)
                if (songItems[i].SongInfo.Tracks.Length - 1 > Game.Instance.PreferredDifficulty)
                    songItems[i].UpdateDifficulty(difficultyName);
        }
    }
}