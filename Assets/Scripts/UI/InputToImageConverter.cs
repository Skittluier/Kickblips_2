namespace KickblipsTwo.UI
{
    using KickblipsTwo.Managers;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    public class InputToImageConverter : MonoBehaviour
    {
        [SerializeField, Tooltip("The input that should be converted to a sprite. Empty means that nothing will be shown. (programmer will update this then)")]
        private KickblipsTwo.Input.Input input;

        private InputDevice currentlyKnownInputDevice;

        [SerializeField, Tooltip("The reference to the UI image component.")]
        private Image inputImage;


        private void OnEnable()
        {
            if (input != null)
                UpdateButtonSprite(input);
        }

        /// <summary>
        /// Checking every frame whether the input device has changed. If thats the case, then update the button sprite.
        /// </summary>
        private void Update()
        {
            if (InputManager.CurrentlyUsedDevice != currentlyKnownInputDevice)
                UpdateButtonSprite(input);
        }

        /// <summary>
        /// Translates the current device input into a sprite.
        /// </summary>
        internal void UpdateButtonSprite(KickblipsTwo.Input.Input input)
        {
            currentlyKnownInputDevice = InputManager.CurrentlyUsedDevice;

            this.input = input;
            inputImage.sprite = input.GetDeviceVisualInput().inputSprite;
        }
    }
}