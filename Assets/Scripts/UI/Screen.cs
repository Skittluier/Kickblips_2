namespace KickblipsTwo.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class Screen : MonoBehaviour
    {
        internal enum ScreenType
        {
            Title, SongSelect, Options, Game, PopUp, Results
        }

        [field: SerializeField, Tooltip("The screen type")]
        internal ScreenType Type { get; private set; }

        internal ScreenManager ScreenManager { get; private set; }

        [SerializeField, Tooltip("The first button to be selected when this screen is being enabled.")]
        protected Button firstSelectedButton;


        protected virtual void OnEnable()
        {
            if (firstSelectedButton != null)
                EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }

        /// <summary>
        /// Defines the screen manager.
        /// </summary>
        /// <param name="screenManager">The screen manager in question</param>
        internal void Setup(ScreenManager screenManager)
        {
            ScreenManager = screenManager;
        }
    }
}
