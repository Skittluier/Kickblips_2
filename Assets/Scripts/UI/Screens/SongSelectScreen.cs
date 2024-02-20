namespace KickblipsTwo.UI.Screens
{
    using KickblipsTwo.IO;
    using KickblipsTwo.UI.Screens.SongSelection;
    using System.Collections;
    using System.IO;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SongSelectScreen : UI.Screen
    {
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

                        FileHandler.HighlightFolder(directoryInfo.Name);
                        FileHandler.FetchMidi((midiFile) =>
                        {
                            SongInfo songInfo = new SongInfo(midiFile.Tracks);

                            midiValid = songInfo.ValidateSong();
                            validatingMidiFile = false;
                        });

                        while (validatingMidiFile)
                            yield return null;

                        // It seems that the Midi is invalid. That means we won't list this song.
                        if (!midiValid)
                            continue;

                        SongItemWindow songItemWindow = Instantiate(songItem, songContentArea);
                        songItemWindow.SetupWindow(directoryInfo.Name, "Always Easy.", -1);

                        // We have now instantiated the object. We don't need to check any more files.
                        break;
                    }
                }
            }

            loadingText.gameObject.SetActive(false);
        }
    }
}