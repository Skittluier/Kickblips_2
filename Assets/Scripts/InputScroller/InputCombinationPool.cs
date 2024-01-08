namespace KickblipsTwo.InputScroller
{
    using System.Collections.Generic;
    using UnityEngine;

    public class InputCombinationPool : MonoBehaviour
    {
        [SerializeField, Tooltip("The pre-made input combinations.")]
        private List<InputCombination> inputCombinations = new List<InputCombination>();

        [SerializeField, Tooltip("The input combination prefab.")]
        private InputCombination inputCombinationPrefab;


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
                    return inputCombinations[i];
                }
            }

            // This is the moment where no game objects are available. So we'll just make the pool larger.
            InputCombination inputCombination = Instantiate(inputCombinationPrefab);
            inputCombination.transform.position = targetPos;

            inputCombinations.Add(inputCombination);

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
        }
    }
}