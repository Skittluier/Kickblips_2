namespace KickblipsTwo.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class HPBar : MonoBehaviour
    {
        [SerializeField, Tooltip("The filling component of the health bar.")]
        private Image hpBarFill;


        /// <summary>
        /// Sets the HP bar status.
        /// </summary>
        /// <param name="hpBarValue">The value of the HP bar</param>
        internal void UpdateHPBarStatus(int hpBarValue)
        {
            hpBarFill.fillAmount = hpBarValue * 0.01f;
        }

        /// <summary>
        /// Resets the HP bar to 1.
        /// </summary>
        internal void ResetHPBar()
        {
            hpBarFill.fillAmount = 1;
        }
    }
}