namespace KickblipsTwo.UI
{
    using KickblipsTwo.UI.Screens;
    using UnityEngine;
    using static KickblipsTwo.UI.Screens.PopUpScreen;

    public class ScreenManager : MonoBehaviour
    {
        [SerializeField, Tooltip("All the screens in the game.")]
        private Screen[] screens;

        private Screen activeScreen;
        private Screen previousScreen;


        /// <summary>
        /// Setting up all the screens with this screen manager as their manager.
        /// </summary>
        private void Awake()
        {
            for (int i = 0; i < screens.Length; i++)
            {
                if (i == 0)
                {
                    activeScreen = screens[i];
                    previousScreen = screens[i];
                }

                screens[i].Setup(this);
            }
        }

        /// <summary>
        /// Changes the current active screen.
        /// </summary>
        /// <param name="screenType">The screen type</param>
        internal void ChangeScreen(Screen.ScreenType screenType)
        {
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].Type == screenType)
                {
                    if (activeScreen != null)
                        activeScreen.gameObject.SetActive(false);

                    previousScreen = activeScreen;
                    activeScreen = screens[i];
                    activeScreen.gameObject.SetActive(true);

                    break;
                }
                else
                    screens[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Shows a pop-up in front of the UI.
        /// </summary>
        /// <param name="buttonSettings">The pop-up screen button settings</param>
        internal void ShowPopUp(PopUpScreenSettings buttonSettings)
        {
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].Type == Screen.ScreenType.PopUp && screens[i] is PopUpScreen popUpScreen)
                {
                    popUpScreen.SetupPopUp(buttonSettings);
                    popUpScreen.gameObject.SetActive(true);
                    break;
                }
            }
        }

        /// <summary>
        /// Closes the pop-up window.
        /// </summary>
        internal void ClosePopUp()
        {
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].Type == Screen.ScreenType.PopUp)
                {
                    screens[i].gameObject.SetActive(false);
                    break;
                }
            }
        }

        /// <summary>
        /// Goes to the previous screen.
        /// </summary>
        internal void GoToPreviousScreen()
        {
            ChangeScreen(previousScreen.Type);
        }
    }
}