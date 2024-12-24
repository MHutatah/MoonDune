using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines the contract for collectible pickup objects.
    /// </summary>
    public interface IPickup
    {
        /// <summary>
        /// Handles the pickup action when interacted with by another object.
        /// </summary>
        /// <param name="collector">Collider of the object collecting the pickup.</param>
        void OnPickup(Collider collector);

        /// <summary>
        /// Gets the amount or type of pickup this object provides.
        /// </summary>
        /// <returns>Pickup amount or type.</returns>
        float GetPickupAmount();
    }
}
