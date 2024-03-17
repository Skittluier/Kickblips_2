namespace KickblipsTwo.Input
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputManager : MonoBehaviour
    {
        public enum DeviceLayout
        {
            XInput, DualShock, Switch, Keyboard, END
        }

        [field: SerializeField, Tooltip("All the possible note inputs and their sprites.")]
        internal Input[] PossibleNoteInputs { get; private set; }

        [field: SerializeField, Tooltip("The possible note inputs which should preferably be on the right side of the track.")]
        private Input[] preferredRightNoteInputs;

        internal static InputDevice CurrentlyUsedDevice { get; private set; }


        [SerializeField, Tooltip("The action map for the game.")]
        private InputActionAsset actionMap;

        [SerializeField, Tooltip("The index within the action map for game input.")]
        private int gameActionMapNo = 0;

        [SerializeField, Tooltip("The index within the action map for UI input.")]
        private int uiActionMapNo = 1;


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

        /// <summary>
        /// Switches the active input to the game action map.
        /// </summary>
        internal void SwitchToGameActionMap()
        {
            for (int i = 0; i < actionMap.actionMaps.Count; i++)
            {
                if (i == gameActionMapNo)
                    actionMap.actionMaps[i].Enable();
                else
                    actionMap.actionMaps[i].Disable();
            }
        }

        /// <summary>
        /// Switches the active input to the UI action map.
        /// </summary>
        internal void SwitchToUIActionMap()
        {
            for (int i = 0; i < actionMap.actionMaps.Count; i++)
            {
                if (i == uiActionMapNo)
                    actionMap.actionMaps[i].Enable();
                else
                    actionMap.actionMaps[i].Disable();
            }
        }

        /// <summary>
        /// Checks if the note is preferably on the right side of the track.
        /// </summary>
        /// <param name="noteNumber">The number of the note</param>
        /// <returns>Is the note preferably on the right side of the track?</returns>
        internal bool NoteIsPreferablyOnRightSide(int noteNumber)
        {
            for (int i = 0; i < preferredRightNoteInputs.Length; i++)
                if (preferredRightNoteInputs[i].MidiNote == noteNumber)
                    return true;

            return false;
        }
    }
}