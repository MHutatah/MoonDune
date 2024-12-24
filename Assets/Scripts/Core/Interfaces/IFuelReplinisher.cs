using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines the contract for objects that can replenish fuel.
    /// </summary>
    public interface IFuelReplenisher
    {
        /// <summary>
        /// Replenishes fuel by a specified amount.
        /// </summary>
        /// <param name="amount">Amount of fuel to replenish.</param>
        void ReplenishFuel(float amount);

        /// <summary>
        /// Gets the amount of fuel that can be replenished.
        /// </summary>
        float FuelAmount { get; }
    }
}
