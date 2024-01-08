namespace KickblipsTwo.Input
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    [CreateAssetMenu(fileName = "New Input", menuName = "Input Type/New Input")]
    public class Input : ScriptableObject
    {
        [field: SerializeField, Tooltip("The image of the input button.")]
        internal Sprite InputSprite { get; private set; }

        [field: SerializeField, Tooltip("The input action reference for the InputSystem.")]
        internal InputActionReference InputActionReference { get; private set; }

        [field: SerializeField, Tooltip("The midi note for this input. (48 - 55)")]
        internal uint MidiNote { get; private set; }
    }
}