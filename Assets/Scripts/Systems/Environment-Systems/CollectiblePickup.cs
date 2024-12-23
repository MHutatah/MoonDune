// File: Assets/Scripts/Systems/Environment-Systems/CollectiblePickup.cs
using UnityEngine;
using Core.Interfaces;
using Systems.EnvironmentSystems;
using Systems.VehicleSystems;
using Core.Managers;

namespace EnvironmentSystems
{
    /// <summary>
    /// Represents a collectible item that replenishes the player's gravity fuel upon pickup.
    /// Implements the IPickup interface.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CollectiblePickup : MonoBehaviour, IPickup
    {
        #region Inspector Variables

        [Header("Pickup Settings")]
        [SerializeField] private float gravityFuelAmount = 20f;            // Amount of gravity fuel to replenish
        [SerializeField] private bool destroyOnPickup = true;             // Whether to destroy the pickup after collection

        [Header("Feedback Settings")]
        [SerializeField] private AudioClip pickupSound;                    // Sound to play upon pickup
        [SerializeField] private ParticleSystem pickupEffectPrefab;        // Particle effect to instantiate upon pickup

        #endregion

        #region Private Variables

        private bool isCollected = false;                                 // Flag to prevent multiple pickups

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            // Ensure the Collider is set as a trigger
            Collider col = GetComponent<Collider>();
            if (!col.isTrigger)
            {
                col.isTrigger = true;
                Debug.LogWarning($"{gameObject.name}: Collider was not set as a trigger. Automatically setting it to trigger.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return; // Prevent multiple pickups

            // Check if the collider belongs to the player
            if (other.CompareTag("Player"))
            {
                OnPickup(other);
            }
        }

        #endregion

        #region IPickup Implementation

        /// <summary>
        /// Handles the pickup action when interacted with by the player.
        /// </summary>
        /// <param name="other">Collider of the object interacting with the pickup.</param>
        public void OnPickup(Collider other)
        {
            if (isCollected) return; // Prevent multiple pickups

            // Find the VehicleController in the player's hierarchy
            VehicleController vehicleController = other.GetComponentInChildren<VehicleController>();
            if (vehicleController != null)
            {
                // Replenish gravity fuel
                vehicleController.ReplenishGravityFuel(gravityFuelAmount);

                // Play pickup sound
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // Instantiate pickup effect
                if (pickupEffectPrefab != null)
                {
                    Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                }

                // Provide UI feedback
                EventManager.Instance?.CrystalCollected(1); // Notify about crystal collection
                UIManager.Instance?.ShowPickupMessage($"Gravity Fuel +{gravityFuelAmount}");

                // Trigger crystal collection event
                // Already done above via EventManager

                // Set the pickup as collected
                isCollected = true;

                // Destroy or deactivate the pickup
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }

                Debug.Log($"{gameObject.name}: Pickup collected by Player.");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: VehicleController not found on Player.");
            }
        }

        #endregion
    }
}
