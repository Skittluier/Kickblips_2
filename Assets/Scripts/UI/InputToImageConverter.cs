namespace KickblipsTwo.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class InputToImageConverter : MonoBehaviour
    {
        [SerializeField, Tooltip("The input that should be converted to a sprite. Empty means that nothing will be shown. (programmer will update this then)")]
        private KickblipsTwo.Input.Input input;

        [SerializeField, Tooltip("The reference to the UI image component.")]
        private Image inputImage;


        private void OnEnable()
        {
            if (input != null)
                UpdateButtonSprite(input);
        }

        /// <summary>
        /// Translates the current device input into a sprite.
        /// </summary>
        internal void UpdateButtonSprite(KickblipsTwo.Input.Input input)
        {
            inputImage.sprite = input.GetDeviceVisualInput().inputSprite;
        }
    }
}