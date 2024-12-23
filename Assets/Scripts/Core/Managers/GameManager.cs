using UnityEngine;

/// <summary>
/// Oversees the overall game state, including victory and game over conditions.
/// </summary>
namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Victory Settings")]
        [SerializeField] private Transform flagPlantPoint;    // Position where the flag is planted
        [SerializeField] private GameObject flagPrefab;       // Flag prefab to instantiate

        [Header("Crystal Management")]
        [SerializeField] private int totalCrystals = 0;       // Total crystals collected

        private bool hasPlantedFlag = false;

        protected virtual void Awake()
        {
            // Enhanced Singleton setup
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
            // Subscribe to relevant events
            if (EventManager.Instance != null)
            {
                EventManager.Instance.EventOnCrystalCollected += OnCrystalCollected;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (EventManager.Instance != null)
            {
                EventManager.Instance.EventOnCrystalCollected -= OnCrystalCollected;
            }
        }

        /// <summary>
        /// Handles crystal collection.
        /// </summary>
        /// <param name="amount">Number of crystals collected.</param>
        private void OnCrystalCollected(int amount)
        {
            totalCrystals += amount;
            UIManager.Instance?.UpdateCrystalCount(totalCrystals);
            // Additional logic, such as tracking total crystals
        }

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
        public void CrystalCollected(int crystalValue)
        {

            // Implement the logic for when a crystal is collected

            Debug.Log($"Crystal collected with value: {crystalValue}");

        }

    }
}