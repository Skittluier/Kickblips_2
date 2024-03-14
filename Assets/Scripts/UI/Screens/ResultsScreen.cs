namespace KickblipsTwo.UI.Screens
{
    using KickblipsTwo.IO;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ResultsScreen : UI.Screen
    {
        [Header("References")]
        [SerializeField, Tooltip("Song clearance text")]
        private TMP_Text songClearanceTitle;

        [SerializeField, Tooltip("The result text component.")]
        private TMP_Text songResultText;

        [SerializeField, Tooltip("The button for choosing another song.")]
        private Button newSongButton;

        [SerializeField, Tooltip("The button for retrying the song.")]
        private Button retryButton;

        [Header("Settings")]
        [SerializeField, Tooltip("Song clearance success text")]
        private string songClearText = "Song Clear!";

        [SerializeField, Tooltip("Song clearance failure text")]
        private string songFailText = "Song Fail";

        [SerializeField, Tooltip("The formatted text for the song results.\n{0}: Song name\n{1}: Song score\n{2}: Max Combo\n{3}: Notes Hit\n{4}: Total Notes\n{5}: Difficulty"), TextArea]
        private string resultFormatText;

        [SerializeField, Tooltip("Difficulty names")]
        private string[] difficulties;


        private void Awake()
        {
            newSongButton.onClick.AddListener(() => ScreenManager.ChangeScreen(ScreenType.SongSelect));
        }

        /// <summary>
        /// Initializing the results.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            songClearanceTitle.text = Game.Instance.HPBar.PlayerHealth > 0 ? songClearText : songFailText;
            songResultText.text = string.Format(resultFormatText, FileHandler.HighlightedFolder, Game.Instance.ScoreCounter.Score, Game.Instance.ComboCounter.ComboHighscoreNo, Game.Instance.NotesHit, Game.Instance.TotalNotes, difficulties[Game.Instance.PreferredDifficulty]);

            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(() => Game.Instance.PlaySong(FileHandler.HighlightedFolder));
        }
    }
}