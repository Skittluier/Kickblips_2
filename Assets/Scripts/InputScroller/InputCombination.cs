namespace KickblipsTwo.InputScroller
{
    using UnityEngine;
    using UnityEngine.UI;

    public class InputCombination : MonoBehaviour
    {
        [SerializeField, Tooltip("The first input image.")]
        private Image firstInputImg;

        [SerializeField, Tooltip("The second input image.")]
        private Image secondInputImg;


        /// <summary>
        /// Updates the input images.
        /// </summary>
        /// <param name="firstInput">First input listener</param>
        /// <param name="secondInput">Second input listener</param>
        internal void UpdateInputDisplay(KickblipsTwo.Input.Input firstInput, KickblipsTwo.Input.Input secondInput)
        {
            if (firstInput != null)
                firstInputImg.sprite = firstInput.GetDeviceVisualInput().inputSprite;

            if (secondInput != null)
                secondInputImg.sprite = secondInput.GetDeviceVisualInput().inputSprite;
            else
                secondInputImg.sprite = firstInputImg.sprite;
        }
    }
}