namespace KickblipsTwo.UI.Screens.SongSelection
{
    using KickblipsTwo.Managers;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SongItemWindow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("A reference to the title text.")]
        private TMP_Text titleText;

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
        /// <param name="highscore">Highscore of the player for that song</param>
        internal void SetupWindow(string title, string difficulty, int highscore)
        {
            titleText.text = title;
            difficultyText.text = difficulty;
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