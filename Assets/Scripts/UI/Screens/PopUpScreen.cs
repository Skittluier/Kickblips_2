namespace KickblipsTwo.UI.Screens
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class PopUpScreen : UI.Screen
    {
        internal struct PopUpScreenButtonSettings
        {
            /// <summary>
            /// The name of the button.
            /// </summary>
            internal string ButtonName;

            /// <summary>
            /// The action of the button. Null = disabled
            /// </summary>
            internal UnityAction ButtonAction;
        }

        internal struct PopUpScreenSettings
        {
            /// <summary>
            /// The title of the pop-up screen.
            /// </summary>
            internal string Title;

            /// <summary>
            /// The description of the pop-up screen.
            /// </summary>
            internal string Description;

            /// <summary>
            /// The settings for the left button.
            /// </summary>
            internal PopUpScreenButtonSettings LeftButtonSettings;

            /// <summary>
            /// The settings for the right button.
            /// </summary>
            internal PopUpScreenButtonSettings RightButtonSettings;
        }

        [Header("References")]
        [SerializeField, Tooltip("The title of the pop-up screen.")]
        private TMP_Text popUpTitle;

        [SerializeField, Tooltip("The description of the pop-up screen.")]
        private TMP_Text popUpDescription;

        [SerializeField, Tooltip("The button on the pop-up screen.")]
        private Button leftButton, rightButton;

        [SerializeField, Tooltip("The text for the button on the pop-up screen.")]
        private TMP_Text leftButtonText, rightButtonText;


        /// <summary>
        /// Setting up a pop-up for on top of the UI.
        /// </summary>
        /// <param name="buttonSettings">The button settings to build up the pop-up itself.</param>
        internal void SetupPopUp(PopUpScreenSettings buttonSettings)
        {
            popUpTitle.text = buttonSettings.Title;
            popUpDescription.text = buttonSettings.Description;

            // Setup the left button.
            if (buttonSettings.LeftButtonSettings.ButtonAction == null)
                leftButton.gameObject.SetActive(false);
            else
            {
                leftButton.gameObject.SetActive(true);
                leftButton.onClick.RemoveAllListeners();
                leftButton.onClick.AddListener(buttonSettings.LeftButtonSettings.ButtonAction);
            }

            // Setup the right button.
            if (buttonSettings.RightButtonSettings.ButtonAction == null)
                rightButton.gameObject.SetActive(false);
            else
            {
                rightButton.gameObject.SetActive(true);
                rightButton.onClick.RemoveAllListeners();
                rightButton.onClick.AddListener(buttonSettings.RightButtonSettings.ButtonAction);
            }
        }
    }
}