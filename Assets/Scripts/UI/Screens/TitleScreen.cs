namespace KickblipsTwo.UI.Screens
{
    using KickblipsTwo.Managers;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class TitleScreen : UI.Screen
    {
        [SerializeField, Tooltip("A reference to the input to image converter for the start button.")]
        private InputToImageConverter startButtonConverter;

        [SerializeField, Tooltip("The input for proceeding in the title screen.")]
        private KickblipsTwo.Input.Input startInput;

        [SerializeField, Tooltip("The quit button")]
        private Button quitButton;

        [SerializeField, Tooltip("The options button")]
        private Button optionsButton;


        private IEnumerator Start()
        {
            while (InputManager.CurrentlyUsedDevice == null)
                yield return null;

            startButtonConverter.UpdateButtonSprite(startInput);

            quitButton.onClick.AddListener(Application.Quit);
            optionsButton.onClick.AddListener(() => ScreenManager.ChangeScreen(ScreenType.Options));
        }

        /// <summary>
        /// Listening to the user input until they proceed within the game.
        /// </summary>
        private void Update()
        {
            if (startInput.InputActionReference.action.WasPressedThisFrame())
                ScreenManager.ChangeScreen(ScreenType.SongSelect);
        }
    }
}