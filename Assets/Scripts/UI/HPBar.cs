namespace KickblipsTwo.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class HPBar : MonoBehaviour
    {
        [SerializeField, Tooltip("The filling component of the health bar.")]
        private Image hpBarFill;

        internal const int defaultPlayerHealth = 100;

        /// <summary>
        /// The health of the player. 0 = game over.
        /// </summary>
        internal int PlayerHealth { get; private set; } = defaultPlayerHealth;


        /// <summary>
        /// Sets the HP bar status.
        /// </summary>
        /// <param name="hpBarValue">The value of the HP bar</param>
        internal void UpdateHPBarStatus(int hpBarValue)
        {
            PlayerHealth = hpBarValue;
            hpBarFill.fillAmount = hpBarValue * 0.01f;
        }

        /// <summary>
        /// Resets the HP bar to 1.
        /// </summary>
        internal void ResetHPBar()
        {
            PlayerHealth = defaultPlayerHealth;
            hpBarFill.fillAmount = 1;
        }
    }
}