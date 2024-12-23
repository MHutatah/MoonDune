using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines the contract for interactable objects.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Executes the interaction logic.
        /// </summary>
        void Interact();
    }
}
