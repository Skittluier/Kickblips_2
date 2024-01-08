namespace KickblipsTwo.Managers
{
    using UnityEngine;

    public class InputManager : MonoBehaviour
    {
        [field: SerializeField, Tooltip("All the possible inputs and their sprites.")]
        internal KickblipsTwo.Input.Input[] PossibleInputs { get; private set; }
    }
}