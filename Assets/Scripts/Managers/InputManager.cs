namespace KickblipsTwo.Managers
{
    using UnityEngine;

    public class InputManager : MonoBehaviour
    {
        public enum Devices
        {
            Xbox, PlayStation, Switch, Keyboard, END
        }

        [field: SerializeField, Tooltip("All the possible inputs and their sprites.")]
        internal KickblipsTwo.Input.Input[] PossibleInputs { get; private set; }
    }
}