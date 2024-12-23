using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines the contract for objects that can take damage.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Applies damage to the object.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        /// <param name="source">Transform of the source dealing damage.</param>
        void TakeDamage(float damage, Transform source);
    }
}
