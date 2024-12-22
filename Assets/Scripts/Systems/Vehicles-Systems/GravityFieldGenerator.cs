using UnityEngine;

public class GravityFieldGenerator : MonoBehaviour
{
    [Header("Gravity Field Settings")]
    public float maxGravityEnergy = 100f;
    public float currentGravityEnergy;
    public float drainRate = 2f; // Energy drained per second when active

    private bool fieldActive = false;

    //[Header("HUD References")]
    // Already handled by UIManager

    void Start()
    {
        currentGravityEnergy = maxGravityEnergy;
    }

    void Update()
    {
        if (fieldActive)
        {
            // Drain energy while the field is active
            currentGravityEnergy -= drainRate * Time.deltaTime;
            currentGravityEnergy = Mathf.Clamp(currentGravityEnergy, 0f, maxGravityEnergy);

            if (currentGravityEnergy <= 0f)
            {
                fieldActive = false;
                // Optionally notify the player that the Gravity Field has been deactivated
                // EventManager.Instance.GravityFieldDeactivated();
                Debug.Log("Gravity Field Generator has been deactivated due to energy depletion.");
            }
        }

        // Update HUD
        UIManager.Instance.UpdateGravityFuelBar(currentGravityEnergy, maxGravityEnergy);
    }

    /// <summary>
    /// Toggles the Gravity Field Generator on or off.
    /// </summary>
    public void ToggleGravityField()
    {
        if (currentGravityEnergy > 0f)
        {
            fieldActive = !fieldActive;
            // Optionally trigger visual or audio feedback
            // SoundManager.Instance.PlayGravityToggleSound();
            Debug.Log("Gravity Field Generator toggled: " + (fieldActive ? "ON" : "OFF"));
        }
        else
        {
            Debug.Log("Not enough Gravity Fuel to activate the Gravity Field Generator.");
        }
    }

    /// <summary>
    /// Replenishes Gravity Field energy.
    /// </summary>
    /// <param name="amount">Amount of energy to add.</param>
    public void ReplenishGravityEnergy(float amount)
    {
        currentGravityEnergy = Mathf.Min(currentGravityEnergy + amount, maxGravityEnergy);
        UIManager.Instance.UpdateGravityFuelBar(currentGravityEnergy, maxGravityEnergy);
        Debug.Log("Gravity Fuel Replenished by: " + amount);
    }
}
