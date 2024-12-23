using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines the contract for collectible pickup objects.
    /// </summary>
    public interface IPickup
    {
        /// <summary>
        /// Handles the pickup action when interacted with by the player.
        /// </summary>
        /// <param name="other">Collider of the object interacting with the pickup.</param>
        void OnPickup(Collider other);
    }
}
