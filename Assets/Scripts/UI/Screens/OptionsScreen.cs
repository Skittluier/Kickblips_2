namespace KickblipsTwo.UI.Screens
{
    using UnityEngine;
    using UnityEngine.UI;

    public class OptionsScreen : UI.Screen
    {
        [SerializeField, Tooltip("The quit button")]
        private Button quitButton;

        [SerializeField, Tooltip("The back button")]
        private Button backButton;


        private void Awake()
        {
            quitButton.onClick.AddListener(Application.Quit);
            backButton.onClick.AddListener(ScreenManager.GoToPreviousScreen);
        }
    }
}