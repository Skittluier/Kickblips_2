namespace KickblipsTwo.InputScroller
{
    using System.Collections.Generic;
    using UnityEngine;

    public class InputCombinationPool : MonoBehaviour
    {
        private uint lastKnownUID;

        [SerializeField, Tooltip("The pre-made input combinations.")]
        private List<InputCombination> inputCombinations = new List<InputCombination>();

        /// <summary>
        /// A list of all the visible input combinations.
        /// </summary>
        internal List<InputCombination> VisibleInputCombinations { get; private set; } = new List<InputCombination>();

        [SerializeField, Tooltip("The input combination prefab.")]
        private InputCombination inputCombinationPrefab;

        internal delegate void OnReturnToPoolMethod(InputCombination inputCombination);
        internal OnReturnToPoolMethod OnReturnToPool;


        /// <summary>
        /// Fetches and activates an input combination from the pool.
        /// </summary>
        /// <param name="targetPos">The target position for the object</param>
        /// <returns>An input combination game object</returns>
        internal InputCombination SpawnFromPool(Vector2 targetPos)
        {
            for (int i = 0; i < inputCombinations.Count; i++)
            {
                if (!inputCombinations[i].gameObject.activeInHierarchy)
                {
                    inputCombinations[i].transform.position = targetPos;
                    inputCombinations[i].gameObject.SetActive(true);

                    inputCombinations[i].Setup(lastKnownUID);
                    lastKnownUID++;

                    VisibleInputCombinations.Add(inputCombinations[i]);

                    return inputCombinations[i];
                }
            }

            // This is the moment where no game objects are available. So we'll just make the pool larger.
            InputCombination inputCombination = Instantiate(inputCombinationPrefab);
            inputCombination.transform.position = targetPos;
            inputCombination.Setup(lastKnownUID);
            lastKnownUID++;

            inputCombinations.Add(inputCombination);
            VisibleInputCombinations.Add(inputCombination);

            return inputCombination;
        }

        /// <summary>
        /// Returns an input combination instance back to the pool.
        /// </summary>
        /// <param name="inputCombination">The input combination that should be sent back.</param>
        internal void ReturnToPool(InputCombination inputCombination)
        {
            inputCombination.gameObject.SetActive(false);
            inputCombination.transform.position = Vector2.zero;

            VisibleInputCombinations.Remove(inputCombination);

            OnReturnToPool?.Invoke(inputCombination);
        }
    }
}