namespace KickblipsTwo.UI
{
    using UnityEngine;

    public class Screen : MonoBehaviour
    {
        internal enum ScreenType
        {
            Title, SongSelect, Options, Game, PopUp
        }

        [field: SerializeField, Tooltip("The screen type")]
        internal ScreenType Type { get; private set; }

        internal ScreenManager ScreenManager { get; private set; }


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
