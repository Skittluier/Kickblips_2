namespace KickblipsTwo.UI
{
    using TMPro;
    using UnityEngine;

    public class ComboCounter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("A reference to the highest combo.")]
        private TMP_Text comboHighscore;

        [SerializeField, Tooltip("A reference to the current combo.")]
        private TMP_Text currentComboScore;

        [Header("Settings")]
        [SerializeField, Tooltip("The format for the combo highscore.\n{0} = The combo highscore")]
        private string comboHighscoreTextFormat = "{0} Combo";

        [SerializeField, Tooltip("The format for the current combo.\n{0} = The current combo")]
        private string currentComboTextFormat = "{0}X";

        private int currentComboNo = 0;
        internal int ComboHighscoreNo { get; private set; } = 0;


        /// <summary>
        /// Increases the combo count by 1.
        /// </summary>
        internal void IncreaseComboCount()
        {
            currentComboNo++;

            // Updating the highscore, if necessary.
            if (currentComboNo > ComboHighscoreNo)
            {
                ComboHighscoreNo = currentComboNo;
                comboHighscore.text = string.Format(comboHighscoreTextFormat, ComboHighscoreNo);
            }

            currentComboScore.text = string.Format(currentComboTextFormat, currentComboNo);
        }

        /// <summary>
        /// Resets the combo counter.
        /// </summary>
        internal void ResetCombo()
        {
            currentComboNo = 0;
            currentComboScore.text = string.Format(currentComboTextFormat, currentComboNo);
        }

        /// <summary>
        /// Resets the current combo counter as well as the combo highscore.
        /// </summary>
        internal void ResetEverything()
        {
            ResetCombo();

            ComboHighscoreNo = 0;
            comboHighscore.text = string.Format(comboHighscoreTextFormat, ComboHighscoreNo);
        }
    }
}