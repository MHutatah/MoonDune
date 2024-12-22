using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static UIManager Instance;

    [Header("HUD Sliders")]
    [Tooltip("Slider to display the player's current oxygen level.")]
    public Slider oxygenSlider;

    [Tooltip("Slider to display the laser's remaining energy.")]
    public Slider laserSlider;

    [Tooltip("Slider to display the gravity field's remaining energy.")]
    public Slider gravityFuelSlider;

    [Header("HUD Labels")]
    [Tooltip("Text label for the Oxygen bar.")]
    public Text oxygenLabel;

    [Tooltip("Text label for the Laser Energy bar.")]
    public Text laserLabel;

    [Tooltip("Text label for the Gravity Fuel bar.")]
    public Text gravityFuelLabel;

    [Header("HUD Text")]
    [Tooltip("Displays how many crystals the player has collected.")]
    public Text crystalCountText;

    [Header("Screens")]
    [Tooltip("GameObject that appears when the player runs out of oxygen or loses.")]
    public GameObject gameOverScreen;

    [Tooltip("GameObject that appears when the player wins (e.g., plants the flag).")]
    public GameObject gameWinScreen;

    private int crystalCount = 0;

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // Optionally, persist across scenes
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Public Methods for Updating HUD

    /// <summary>
    /// Updates the oxygen slider on the HUD.
    /// </summary>
    /// <param name="currentOxygen">Current oxygen level.</param>
    /// <param name="maxOxygen">Maximum oxygen level.</param>
    public void UpdateOxygenBar(float currentOxygen, float maxOxygen)
    {
        if (oxygenSlider != null)
        {
            oxygenSlider.value = currentOxygen / maxOxygen;
        }
    }

    /// <summary>
    /// Updates the laser energy slider on the HUD.
    /// </summary>
    /// <param name="currentEnergy">Current laser energy.</param>
    /// <param name="maxEnergy">Maximum laser energy.</param>
    public void UpdateLaserBar(float currentEnergy, float maxEnergy)
    {
        if (laserSlider != null)
        {
            laserSlider.value = currentEnergy / maxEnergy;
        }
    }

    /// <summary>
    /// Updates the gravity fuel slider on the HUD.
    /// </summary>
    /// <param name="currentEnergy">Current gravity fuel.</param>
    /// <param name="maxEnergy">Maximum gravity fuel.</param>
    public void UpdateGravityFuelBar(float currentEnergy, float maxEnergy)
    {
        if (gravityFuelSlider != null)
        {
            gravityFuelSlider.value = currentEnergy / maxEnergy;
        }
    }

    /// <summary>
    /// Updates the crystal count text on the HUD.
    /// </summary>
    /// <param name="amount">Number of crystals collected.</param>
    public void UpdateCrystalCount(int amount)
    {
        crystalCount += amount;
        if (crystalCountText != null)
        {
            crystalCountText.text = "Crystals: " + crystalCount.ToString();
        }
    }

    #endregion

    #region Public Methods for Screens

    /// <summary>
    /// Shows the Game Over screen.
    /// </summary>
    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Shows the Victory screen.
    /// </summary>
    public void ShowGameWinScreen()
    {
        if (gameWinScreen != null)
        {
            gameWinScreen.SetActive(true);
        }
    }

    #endregion

    #region Optional Methods for Labels and Warnings

    /// <summary>
    /// Optionally updates labels for HUD sliders.
    /// </summary>
    /// <param name="oxygenText">Text for Oxygen label.</param>
    /// <param name="laserText">Text for Laser label.</param>
    /// <param name="gravityFuelText">Text for Gravity Fuel label.</param>
    public void UpdateHUDLabels(string oxygenText, string laserText, string gravityFuelText)
    {
        if (oxygenLabel != null)
        {
            oxygenLabel.text = oxygenText;
        }

        if (laserLabel != null)
        {
            laserLabel.text = laserText;
        }

        if (gravityFuelLabel != null)
        {
            gravityFuelLabel.text = gravityFuelText;
        }
    }

    /// <summary>
    /// Displays an oxygen warning (e.g., flashing the oxygen bar).
    /// </summary>
    public void ShowOxygenWarning()
    {
        Debug.Log("Oxygen is running low!");
        // Implement visual warning, such as changing the oxygen bar color or flashing it
    }

    #endregion
}
