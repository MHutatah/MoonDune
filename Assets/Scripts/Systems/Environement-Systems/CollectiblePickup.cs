using UnityEngine;

public class CollectiblePickup : MonoBehaviour
{
    [Header("Collectible Settings")]
    public float gravityEnergyAmount = 20f; // Amount of gravity energy to replenish

    private void OnTriggerEnter(Collider other)
    {
        GravityFieldGenerator gravityGenerator = other.GetComponent<GravityFieldGenerator>();

        if (gravityGenerator != null)
        {
            // Replenish gravity field energy
            gravityGenerator.ReplenishGravityEnergy(gravityEnergyAmount);

            // Optionally play a pickup sound or effect
            // SoundManager.Instance.PlayPickupSound();

            // Optionally update crystal count if crystals are also tracked here
            // UIManager.Instance.UpdateCrystalCount(1); // Example increment

            // Destroy the collectible after pickup
            Destroy(gameObject);
            Debug.Log("Collected Gravity Fuel and replenished " + gravityEnergyAmount + " energy.");
        }
    }
}
