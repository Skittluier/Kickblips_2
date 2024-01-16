namespace KickblipsTwo.Managers
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputManager : MonoBehaviour
    {
        public enum DeviceLayout
        {
            XInput, DualShock, Switch, Keyboard, END
        }

        [field: SerializeField, Tooltip("All the possible inputs and their sprites.")]
        internal KickblipsTwo.Input.Input[] PossibleInputs { get; private set; }

        internal static InputDevice CurrentlyUsedDevice { get; private set; }


        /// <summary>
        /// Define the last connected device as the currently used device.
        /// </summary>
        private void Awake()
        {
            CurrentlyUsedDevice = InputSystem.devices.LastOrDefault();

            // Is the mouse the last device connected? Then jump back to the keyboard.
            if (Equals(CurrentlyUsedDevice.layout, "Mouse"))
            {
                string keyboardName = DeviceLayout.Keyboard.ToString();
                for (int i = 0; i < InputSystem.devices.Count; i++)
                    if (Equals(InputSystem.devices[i].layout, keyboardName))
                    {
                        CurrentlyUsedDevice = InputSystem.devices[i];
                        break;
                    }
            }

            InputSystem.onDeviceChange += OnDeviceChange;
        }

        /// <summary>
        /// Will be called whenever an input device changes.
        /// This will update the currently used device.
        /// </summary>
        /// <param name="inputDevice">The input device that matters in this event</param>
        /// <param name="inputDeviceChange">The kind of change that happened</param>
        private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            if (inputDeviceChange == InputDeviceChange.Disconnected)
            {
                // Checking if it's a relevant input device.
                if (Equals(CurrentlyUsedDevice.deviceId, inputDevice.deviceId))
                {
                    // Resetting the currently used device to the default one.
                    for (int i = 0; i < InputSystem.devices.Count; i++)
                        if (InputSystem.devices[i].layout.Contains(DeviceLayout.Keyboard.ToString()))
                            CurrentlyUsedDevice = InputSystem.devices[i];
                }
            }
            else if (inputDeviceChange == InputDeviceChange.Reconnected || inputDeviceChange == InputDeviceChange.Added)
                CurrentlyUsedDevice = inputDevice;
        }
    }
}