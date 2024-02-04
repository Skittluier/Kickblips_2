namespace KickblipsTwo.InputScroller
{
    using UnityEngine;
    using UnityEngine.UI;

    public class InputCombination : MonoBehaviour
    {
        internal uint UID { get; private set; }
        internal KickblipsTwo.Input.Input FirstInput { get; private set; }
        internal KickblipsTwo.Input.Input SecondInput { get; private set; }


        [SerializeField, Tooltip("The first input image.")]
        private Image firstInputImg;

        [SerializeField, Tooltip("The second input image.")]
        private Image secondInputImg;


        /// <summary>
        /// Set-up the InputCombination for their unique ID.
        /// </summary>
        /// <param name="uid">The unique ID of the input combination</param>
        internal void Setup(uint uid)
        {
            UID = uid;
        }

        /// <summary>
        /// Updates the input images.
        /// </summary>
        /// <param name="firstInput">First input listener</param>
        /// <param name="secondInput">Second input listener</param>
        internal void UpdateInputDisplay(KickblipsTwo.Input.Input firstInput, KickblipsTwo.Input.Input secondInput)
        {
            if (firstInput != null)
            {
                FirstInput = firstInput;
                firstInputImg.sprite = firstInput.GetDeviceVisualInput().inputSprite;
            }

            if (secondInput != null)
            {
                SecondInput = secondInput;
                secondInputImg.sprite = secondInput.GetDeviceVisualInput().inputSprite;
            }
            else
                secondInputImg.sprite = firstInputImg.sprite;
        }
    }
}