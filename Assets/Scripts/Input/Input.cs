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
            [field: SerializeField, Tooltip("The device belonging to this input.")]
            public InputManager.Devices Device { get; private set; }
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
            string bindingDisplayString = InputActionRebindingExtensions.GetBindingDisplayString(InputActionReference, InputBinding.DisplayStringOptions.DontOmitDevice);

            for (int i = 0; i < InputSystem.devices.Count; i++)
                for (int j = 0; j < visualInputs.Length; j++)
                {
                    if (bindingDisplayString.Contains(visualInputs[j].Device.ToString()))
                        return visualInputs[j];
                }

            // Just return the first one if the input is missing. However, do give a message.
            Debug.LogError("[Input] Missing input for the connected devices.");

            return visualInputs[0];
        }

#if UNITY_EDITOR
        // TODO: Finish this. (Look at ControllerLayoutWindow.cs)
        internal void EDITOR_ApplyNewLayouts(string keyboardLayoutFormat, string playStationLayoutFormat, string xboxLayoutFormat, string switchLayoutFormat)
        {
            for (int i = 0; i < visualInputs.Length; i++)
            {
                switch (visualInputs[i].Device)
                {
                    case InputManager.Devices.Keyboard:
                        break;
                    case InputManager.Devices.PlayStation:

                        break;
                        case InputManager.Devices.Xbox:

                        break;
                    case InputManager.Devices.Switch:

                        break;
                }
            }
        }
#endif
    }
}