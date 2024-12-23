using Core.Managers;
using UnityEngine;

namespace Systems.PlayerSystems
{
    /// <summary>
    /// Manages the player's oxygen levels, handling depletion and replenishment.
    /// </summary>
    public class PlayerOxygen : MonoBehaviour
    {
        #region Inspector Variables

        [Header("Oxygen Settings")]
        [SerializeField] private float maxOxygen = 100f;
        [SerializeField] private float depletionRate = 1f; // Oxygen lost per second

        #endregion

        #region Private Variables

        private float currentOxygen;
        private bool hasTriggeredLowOxygen = false;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            currentOxygen = maxOxygen;
            UIManager.Instance?.UpdateOxygenBar(currentOxygen, maxOxygen);
        }

        private void Update()
        {
            HandleOxygenDepletion();
        }

        #endregion

        #region Oxygen Management

        /// <summary>
        /// Handles oxygen depletion over time and triggers game over when oxygen is depleted.
        /// </summary>
        private void HandleOxygenDepletion()
        {
            // Deplete oxygen over time
            currentOxygen -= depletionRate * Time.deltaTime;
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

            // Update the Oxygen bar in the HUD
            UIManager.Instance?.UpdateOxygenBar(currentOxygen, maxOxygen);

            // Check for low oxygen
            if (currentOxygen <= maxOxygen * 0.2f && !hasTriggeredLowOxygen)
            {
                UIManager.Instance?.ShowOxygenWarning();
                EventManager.Instance?.TriggerOxygenLow();
                hasTriggeredLowOxygen = true;
            }

            // Check for oxygen depletion
            if (currentOxygen <= 0f)
            {
                GameManager.Instance?.GameOver();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Replenishes oxygen.
        /// </summary>
        /// <param name="amount">Amount of oxygen to add.</param>
        public void ReplenishOxygen(float amount)
        {
            currentOxygen = Mathf.Min(currentOxygen + amount, maxOxygen);
            UIManager.Instance?.UpdateOxygenBar(currentOxygen, maxOxygen);
            hasTriggeredLowOxygen = false;
        }

        #endregion
    }
}