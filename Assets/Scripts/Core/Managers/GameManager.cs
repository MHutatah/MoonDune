using UnityEngine;

namespace Core.Managers
{
    /// <summary>
    /// Oversees the overall game state, including victory and game over conditions.
    /// </summary>
    [DefaultExecutionOrder(-100)] // Ensures GameManager initializes after EventManager
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        #region Inspector Variables

        [Header("Victory Settings")]
        [SerializeField] private Transform flagPlantPoint;    // Position where the flag is planted
        [SerializeField] private GameObject flagPrefab;       // Flag prefab to instantiate

        #endregion

        #region Private Variables

        private int totalCrystals = 0;       // Total crystals collected
        private bool hasPlantedFlag = false;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            // Singleton setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Persist across scenes if needed
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Ensure EventManager exists
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnCrystalCollected += OnCrystalCollected;
            }
            else
            {
                Debug.LogError("GameManager: EventManager instance not found in the scene.");
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnCrystalCollected -= OnCrystalCollected;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles crystal collection.
        /// </summary>
        /// <param name="amount">Number of crystals collected.</param>
        private void OnCrystalCollected(int amount)
        {
            totalCrystals += amount;
            UIManager.Instance?.UpdateCrystalCount(totalCrystals);
            // Additional logic, such as checking for victory conditions
            Debug.Log($"Total Crystals Collected: {totalCrystals}");
        }

        #endregion

        #region Game State Management

        /// <summary>
        /// Triggers Game Over state.
        /// </summary>
        public void GameOver()
        {
            // Disable player controls, show Game Over screen, etc.
            UIManager.Instance?.ShowGameOverScreen();
            // Optionally stop the game or reset the level
            Debug.Log("Game Over!");
        }

        /// <summary>
        /// Triggers Victory state by planting the flag.
        /// </summary>
        public void Victory()
        {
            if (!hasPlantedFlag && flagPlantPoint != null && flagPrefab != null)
            {
                Instantiate(flagPrefab, flagPlantPoint.position, flagPlantPoint.rotation);
                hasPlantedFlag = true;
                UIManager.Instance?.ShowGameWinScreen();
                // Optionally stop the game or transition to a new scene
                Debug.Log("Victory! Flag Planted.");
            }
            else
            {
                Debug.LogWarning("GameManager: FlagPlantPoint or FlagPrefab is not assigned.");
            }
        }

        #endregion
    }
}
