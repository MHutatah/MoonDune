using UnityEngine;
using Core.Managers;
using Systems.UISystems;

namespace Systems.PlayerSystems
{
    /// <summary>
    /// Manages the player's oxygen levels, including depletion and replenishment.
    /// </summary>
    public class PlayerOxygen : MonoBehaviour
    {
        #region Inspector Variables

        [Header("Oxygen Settings")]
        public float maxOxygen = 100f;
        [Range(0f, 100f)]
        public float currentOxygen;
        public float depletionRate = 1f; // Oxygen lost per second

        [Header("HUD Reference")]
        public HUDController hudController;

        #endregion

        #region Private Variables

        private bool isOxygenLow = false;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            currentOxygen = maxOxygen;
            UpdateHUD();
        }

        private void Update()
        {
            DepleteOxygen();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Depletes the player's oxygen over time.
        /// </summary>
        private void DepleteOxygen()
        {
            currentOxygen -= depletionRate * Time.deltaTime;
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
            UpdateHUD();

            if (currentOxygen <= 20f && !isOxygenLow)
            {
                isOxygenLow = true;
                UIManager.Instance?.ShowOxygenWarning();
                SoundManager.Instance?.PlayLowOxygenAlert();
            }

            if (currentOxygen <= 0f)
            {
                // Handle game over via GameManager
                GameManager.Instance?.GameOver();
                Debug.Log("Player has run out of oxygen!");
            }
        }

        /// <summary>
        /// Updates the HUD with the current oxygen level.
        /// </summary>
        private void UpdateHUD()
        {
            if (hudController != null)
            {
                hudController.UpdateOxygenBar(currentOxygen, maxOxygen);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Replenishes the player's oxygen. Can be called externally.
        /// </summary>
        /// <param name="amount">Amount of oxygen to add.</param>
        public void ReplenishOxygen(float amount)
        {
            currentOxygen += amount;
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
            UpdateHUD();

            if (currentOxygen > 20f && isOxygenLow)
            {
                isOxygenLow = false;
                // Optionally, hide oxygen warning if implemented
            }
        }

        #endregion
    }
}
