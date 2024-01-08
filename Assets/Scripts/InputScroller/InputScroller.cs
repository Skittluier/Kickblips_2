namespace KickblipsTwo.InputScroller
{
    using KickblipsTwo.Managers;
    using System;
    using System.Collections;
    using UnityEngine;

    public class InputScroller : MonoBehaviour
    {
        [Serializable]
        public struct InputRange
        {
            [SerializeField, Tooltip("The button that will be used with this range.")]
            internal Input button;

            [SerializeField, Tooltip("The note range belonging to this button.")]
            internal Vector2Int noteRange;
        }

        [SerializeField, Tooltip("The transition time from the moment the input combination is being spawned until the moment the buttons should be pressed.")]
        private float transitionTime = 1;

        [SerializeField, Tooltip("The fade out time of the buttons whenever they pass the input moment.")]
        private float fadeOutTimeOnSuccessfullPress = 0.5f;

        [SerializeField, Tooltip("The position of where the input should start.")]
        private RectTransform inputStartPosition;

        [SerializeField, Tooltip("The position of where the input should be pressed.")]
        private RectTransform inputTargetPosition;

        [SerializeField, Tooltip("The pool with all the input combinations.")]
        private InputCombinationPool inputCombinationPool;


        /// <summary>
        /// Spawns an input combination.
        /// </summary>
        /// <param name="midiEventOne">The first midi event connected to it</param>
        /// <param name="midiEventTwo">The second midi event connected to it</param>
        internal void SpawnInputCombination(MidiEvent midiEventOne, MidiEvent midiEventTwo, InputManager inputManager)
        {
            // Setting the input combination.
            InputCombination inputCombination = inputCombinationPool.SpawnFromPool(inputStartPosition.position);
            KickblipsTwo.Input.Input firstInput = null;
            KickblipsTwo.Input.Input secondInput = null;

            if (midiEventOne != null)
                for (int i = 0; i < inputManager.PossibleInputs.Length; i++)
                    if (inputManager.PossibleInputs[i].MidiNote == midiEventOne.Note)
                        firstInput = inputManager.PossibleInputs[i];

            if (midiEventTwo != null)
                for (int i = 0; i < inputManager.PossibleInputs.Length; i++)
                    if (inputManager.PossibleInputs[i].MidiNote == midiEventTwo.Note)
                        secondInput = inputManager.PossibleInputs[i];

            inputCombination.UpdateInputDisplay(firstInput, secondInput);

            // Scroll the input combination through the scroller.
            StartCoroutine(DoScroll(inputCombination));
            IEnumerator DoScroll(InputCombination inputCombination)
            {
                float currVal = 0;
                float startYPos = inputStartPosition.position.y;
                float targetYPos = inputTargetPosition.position.y;

                while (currVal < 1)
                {
                    currVal += Time.deltaTime / transitionTime;
                    inputCombination.transform.position = new Vector2(inputCombination.transform.position.x, Mathf.Lerp(startYPos, targetYPos, currVal));

                    if (currVal >= 1)
                    {
                        inputCombination.transform.position = new Vector2(inputCombination.transform.position.x, targetYPos);
                        inputCombinationPool.ReturnToPool(inputCombination);
                    }

                    yield return null;
                }
            }
        }
    }
}