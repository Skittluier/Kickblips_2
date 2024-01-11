namespace KickblipsTwo.UI
{
    using TMPro;
    using UnityEngine;

    public class ScoreCounter : MonoBehaviour
    {
        [SerializeField, Tooltip("The score UI object")]
        private TMP_Text scoreText;


        /// <summary>
        /// Updates the score counter UI object.
        /// </summary>
        /// <param name="score">The new score</param>
        internal void UpdateScoreCounter(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
