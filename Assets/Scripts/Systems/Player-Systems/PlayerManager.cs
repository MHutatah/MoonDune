using UnityEngine;

namespace Systems.PlayerSystems
{
    /// <summary>
    /// Manages the player's state, enabling or disabling controls as needed.
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        #region Private Variables

        private PlayerController playerController;
        private PlayerLaser playerLaser;
        private PlayerOxygen playerOxygen;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            // Fetch references to the necessary components
            playerController = GetComponent<PlayerController>();
            playerLaser = GetComponent<PlayerLaser>();
            playerOxygen = GetComponent<PlayerOxygen>();

            // Validate component references
            if (playerController == null)
            {
                Debug.LogError($"{gameObject.name}: PlayerController component is missing.");
            }

            if (playerLaser == null)
            {
                Debug.LogError($"{gameObject.name}: PlayerLaser component is missing.");
            }

            if (playerOxygen == null)
            {
                Debug.LogError($"{gameObject.name}: PlayerOxygen component is missing.");
            }

            //SetPlayerControl(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enables or disables player controls and related components.
        /// </summary>
        /// <param name="enable">True to enable controls, false to disable.</param>
        public void SetPlayerControl(bool enable)
        {
            if (playerController != null)
            {
                playerController.enabled = enable;
                gameObject.SetActive(enable);
                Debug.Log($"PlayerController enabled: {enable}");
            }

            if (playerLaser != null)
            {
                playerLaser.enabled = enable;
                Debug.Log($"PlayerLaser enabled: {enable}");

                // Optionally, stop firing when controls are disabled
                if (!enable && playerLaser.IsFiring)
                {
                    playerLaser.StopFiring();
                }
            }

            if (playerOxygen != null)
            {
                playerOxygen.enabled = enable;
                Debug.Log($"PlayerOxygen enabled: {enable}");
            }

            // Optionally, handle other components like UI, animations, etc.
        }

        #endregion
    }
}
