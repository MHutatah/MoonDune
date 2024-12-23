using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines the contract for movable objects.
    /// </summary>
    public interface IMovable
    {
        /// <summary>
        /// Moves the object in a specified direction.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        void Move(Vector3 direction);

        /// <summary>
        /// Rotates the object around the Y-axis.
        /// </summary>
        /// <param name="angle">Angle in degrees to rotate.</param>
        void Rotate(float angle);
    }
}
