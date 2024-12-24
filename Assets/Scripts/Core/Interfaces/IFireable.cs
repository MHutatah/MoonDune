using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines the contract for objects that can fire projectiles or beams.
    /// </summary>
    public interface IFireable
    {
        /// <summary>
        /// Initiates the firing action in a specified direction.
        /// </summary>
        /// <param name="direction">Direction in which to fire.</param>
        void Fire(Vector3 direction);

        /// <summary>
        /// Stops the firing action.
        /// </summary>
        void StopFiring();

        /// <summary>
        /// Indicates whether the object is currently firing.
        /// </summary>
        bool IsFiring { get; }
    }
}
