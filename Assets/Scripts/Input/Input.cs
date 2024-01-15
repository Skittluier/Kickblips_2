namespace KickblipsTwo.Input
{
    using KickblipsTwo.Managers;
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [CreateAssetMenu(fileName = "New Input", menuName = "Input Type/New Input")]
    public class Input : ScriptableObject
    {
        [Serializable]
        public struct DeviceVisualInput
        {
            [field: SerializeField, Tooltip("The device layout belonging to this input.")]
            public InputManager.DeviceLayout DeviceLayout { get; private set; }
            public Sprite inputSprite;
        }

        [SerializeField, Tooltip("The visual inputs for the device.")]
        private DeviceVisualInput[] visualInputs;

        [field: SerializeField, Tooltip("The input action reference for the InputSystem.")]
        internal InputActionReference InputActionReference { get; private set; }

        [field: SerializeField, Tooltip("The midi note for this input. (48 - 55)")]
        internal uint MidiNote { get; private set; }



        /// <summary>
        /// Fetches the visual input for the current device.
        /// </summary>
        /// <returns>The visual input for the current device</returns>
        internal DeviceVisualInput GetDeviceVisualInput()
        {
            if (InputManager.CurrentlyUsedDevice != null)
                for (int i = 0; i < visualInputs.Length; i++)
                    if (InputManager.CurrentlyUsedDevice.layout.Contains(visualInputs[i].DeviceLayout.ToString()))
                        return visualInputs[i];

            // Just return the first one if the input is missing. However, do give a message.
            Debug.LogError("[Input] Missing input for the connected devices.");

            return visualInputs[0];
        }
    }
}