namespace KickblipsTwo.UI.Screens.SongSelection
{
    using KickblipsTwo.IO;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SongItemWindow : MonoBehaviour
    {
        internal SongInfo SongInfo { get; private set; }

        [Header("References")]
        [SerializeField, Tooltip("A reference to the title text.")]
        private TMP_Text titleText;

        [SerializeField, Tooltip("The original artist's name text.")]
        private TMP_Text originalArtistText;

        [SerializeField, Tooltip("A reference to the difficulty text.")]
        private TMP_Text difficultyText;

        [SerializeField, Tooltip("A reference to the highscore text.")]
        private TMP_Text highscoreText;

        [SerializeField, Tooltip("A reference to the button.")]
        private Button button;


        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        /// <summary>
        /// Setting up the song item window.
        /// </summary>
        /// <param name="title">Title of the song</param>
        /// <param name="difficulty">Difficulty of the song</param>
        /// <param name="originalArtistName">The name of the original artist</param>
        internal void SetupWindow(SongInfo songInfo, string title, string difficulty, string originalArtistName)
        {
            SongInfo = songInfo;
            titleText.text = title;
            difficultyText.text = difficulty;
            originalArtistText.text = originalArtistName;
        }

        /// <summary>
        /// Updates the difficulty name.
        /// </summary>
        /// <param name="difficultyName">The name of the difficulty</param>
        internal void UpdateDifficulty(string difficultyName)
        {
            difficultyText.text = difficultyName;
        }

        /// <summary>
        /// Updates the highscore text component of the song.
        /// </summary>
        /// <param name="highscore">Highscore of the player for that song</param>
        internal void UpdateHighscore(int highscore)
        {
            highscoreText.text = highscore.ToString();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            Game.Instance.PlaySong(titleText.text);
        }
    }
}