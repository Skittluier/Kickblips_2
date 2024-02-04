namespace KickblipsTwo.UI
{
    using UnityEngine;

    public class ScreenManager : MonoBehaviour
    {
        [SerializeField, Tooltip("All the screens in the game.")]
        private Screen[] screens;

        private Screen activeScreen;


        /// <summary>
        /// Setting up all the screens with this screen manager as their manager.
        /// </summary>
        private void Awake()
        {
            for (int i = 0; i < screens.Length; i++)
                screens[i].Setup(this);
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

                    activeScreen = screens[i];
                    activeScreen.gameObject.SetActive(true);

                    break;
                }
                else
                    screens[i].gameObject.SetActive(false);
            }
        }
    }
}