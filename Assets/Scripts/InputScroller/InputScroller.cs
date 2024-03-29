namespace KickblipsTwo.InputScroller
{
    using KickblipsTwo.Input;
    using System;
    using System.Collections;
    using UnityEngine;

    public class InputScroller : MonoBehaviour
    {
        [Serializable]
        public struct InputRange
        {
            [SerializeField, Tooltip("The button that will be used with this range.")]
            internal KickblipsTwo.Input.Input button;

            [SerializeField, Tooltip("The note range belonging to this button.")]
            internal Vector2Int noteRange;
        }

        [field: SerializeField, Tooltip("The transition time from the moment the input combination is being spawned until the moment the buttons should be pressed.")]
        internal float TransitionTime { get; private set; } = 2;

        [SerializeField, Tooltip("The position of where the input should start.")]
        private RectTransform inputStartPosition;

        [field: SerializeField, Tooltip("The position of where the input will end.")]
        private RectTransform inputStopPosition;

        [field: SerializeField, Tooltip("The position of where the input should be pressed.")]
        internal RectTransform InputTargetPosition { get; private set; }

        [SerializeField, Tooltip("The pool with all the input combinations.")]
        private InputCombinationPool inputCombinationPool;


        /// <summary>
        /// Spawns an input combination.
        /// </summary>
        /// <param name="midiEventOne">The first midi event connected to it</param>
        /// <param name="midiEventTwo">The second midi event connected to it</param>
        /// <param name="inputManager">The input manager</param>
        /// <returns>The input combination being returned.</returns>
        internal InputCombination SpawnInputCombination(MidiEvent midiEventOne, MidiEvent midiEventTwo, InputManager inputManager)
        {
            // Setting the input combination.
            InputCombination inputCombination = inputCombinationPool.SpawnFromPool(inputStartPosition.position);

            KickblipsTwo.Input.Input firstInput = null;
            KickblipsTwo.Input.Input secondInput = null;

            if (midiEventOne != null)
                for (int i = 0; i < inputManager.PossibleNoteInputs.Length; i++)
                    if (inputManager.PossibleNoteInputs[i].MidiNote == midiEventOne.Note)
                        firstInput = inputManager.PossibleNoteInputs[i];

            if (midiEventTwo != null)
                for (int i = 0; i < inputManager.PossibleNoteInputs.Length; i++)
                    if (inputManager.PossibleNoteInputs[i].MidiNote == midiEventTwo.Note)
                        secondInput = inputManager.PossibleNoteInputs[i];

            inputCombination.UpdateInputDisplay(firstInput, secondInput);

            // Scroll the input combination through the scroller.
            StartCoroutine(DoScroll(inputCombination));
            IEnumerator DoScroll(InputCombination inputCombination)
            {
                float currVal = 0;
                float startYPos = inputStartPosition.position.y;
                float targetYPos = inputStopPosition.position.y;

                while (currVal < 1 && inputCombination.gameObject.activeInHierarchy)
                {
                    currVal += Time.deltaTime / TransitionTime;
                    inputCombination.transform.position = new Vector2(inputCombination.transform.position.x, Mathf.Lerp(startYPos, targetYPos, currVal));

                    if (currVal >= 1)
                    {
                        inputCombination.transform.position = new Vector2(inputCombination.transform.position.x, targetYPos);

                        inputCombinationPool.ReturnToPool(inputCombination);
                        Game.Instance.DeductHealth();
                    }

                    yield return null;
                }
            }

            return inputCombination;
        }
    }
}