namespace KickblipsTwo.UI
{
    using TMPro;
    using UnityEngine;

    public class ScoreCounter : MonoBehaviour
    {
        [SerializeField, Tooltip("The score UI object")]
        private TMP_Text scoreText;

        /// <summary>
        /// The current score.
        /// </summary>
        internal int Score { get; private set; }


        /// <summary>
        /// Updates the score counter UI object.
        /// </summary>
        /// <param name="score">The new score</param>
        internal void UpdateScoreCounter(int score)
        {
            Score = score;
            scoreText.text = score.ToString();
        }
    }
}
