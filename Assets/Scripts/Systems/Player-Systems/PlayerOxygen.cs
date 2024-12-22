using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float depletionRate = 1f; // Oxygen lost per second

    private void Start()
    {
        currentOxygen = maxOxygen;
    }

    private void Update()
    {
        // Deplete oxygen over time
        currentOxygen -= depletionRate * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        // Update the Oxygen bar in the HUD
        UIManager.Instance.UpdateOxygenBar(currentOxygen, maxOxygen);

        // Check for low oxygen
        if (currentOxygen <= maxOxygen * 0.2f)
        {
            UIManager.Instance.ShowOxygenWarning();
            // Optionally trigger additional warnings or effects
        }

        // Check for oxygen depletion
        if (currentOxygen <= 0f)
        {
            GameManager.Instance.GameOver();
        }
    }

    /// <summary>
    /// Replenishes oxygen.
    /// </summary>
    /// <param name="amount">Amount of oxygen to add.</param>
    public void ReplenishOxygen(float amount)
    {
        currentOxygen = Mathf.Min(currentOxygen + amount, maxOxygen);
        UIManager.Instance.UpdateOxygenBar(currentOxygen, maxOxygen);
    }
}
