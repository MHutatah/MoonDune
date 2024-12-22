using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Victory Settings")]
    public Transform flagPlantPoint;    // Position where the flag is planted
    public GameObject flagPrefab;       // Flag prefab to instantiate

    private bool hasPlantedFlag = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally persist across scenes
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles crystal collection.
    /// </summary>
    /// <param name="amount">Number of crystals collected.</param>
    public void CollectCrystal(int amount)
    {
        UIManager.Instance.UpdateCrystalCount(amount);
        // Additional logic, such as tracking total crystals
    }

    /// <summary>
    /// Triggers Game Over state.
    /// </summary>
    public void GameOver()
    {
        // Disable player controls, show Game Over screen, etc.
        UIManager.Instance.ShowGameOverScreen();
        // Optionally stop the game or reset the level
        Debug.Log("Game Over!");
    }

    /// <summary>
    /// Triggers Victory state by planting the flag.
    /// </summary>
    public void Victory()
    {
        if (!hasPlantedFlag)
        {
            Instantiate(flagPrefab, flagPlantPoint.position, flagPlantPoint.rotation);
            hasPlantedFlag = true;
            UIManager.Instance.ShowGameWinScreen();
            // Optionally stop the game or transition to a new scene
            Debug.Log("Victory! Flag Planted.");
        }
    }
}
