using UnityEngine;
using Core.Interfaces;
using Core.Managers;
using TMPro;
using Systems.PlayerSystems; // Ensure this namespace is included to access PlayerManager

namespace Systems.VehicleSystems
{
    /// <summary>
    /// Handles player interactions with the vehicle, allowing entering and exiting.
    /// Implements the IInteractable interface.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VehicleInteraction : MonoBehaviour, IInteractable
    {
        #region Inspector Variables

        [Header("Interaction Settings")]
        [SerializeField] private KeyCode interactionKey = KeyCode.E;           // Key to enter/exit vehicle

        [Header("References")]
        [SerializeField] private PlayerManager playerManager;                  // Reference to the PlayerManager script
        [SerializeField] private VehicleController vehicleController;          // Reference to the VehicleController script
        [SerializeField] private GravityFieldGenerator gravityFieldGenerator;  // Reference to the GravityFieldGenerator script
        [SerializeField] private Transform exitPoint;                          // Reference to the exit point Transform
        [SerializeField] private Canvas interactionCanvas;                     // UI Canvas for interaction prompts

        #endregion

        #region Private Variables

        private bool isPlayerInRange = false;    // Indicates if the player is near the vehicle
        private bool isPlayerInside = false;     // Indicates if the player is inside the vehicle

        #endregion

        #region Unity Callbacks

        private void Update()
        {
            HandleInteractionInput();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = true;
                ShowInteractionPrompt(true);
                EventManager.Instance?.TriggerPlayerEnterVehicleZone();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = false;
                ShowInteractionPrompt(false);
                EventManager.Instance?.TriggerPlayerExitVehicleZone();
            }
        }

        #endregion

        #region IInteractable Implementation

        /// <summary>
        /// Executes the interaction logic for entering or exiting the vehicle.
        /// </summary>
        public void Interact()
        {
            if (isPlayerInRange)
            {
                if (!isPlayerInside)
                {
                    EnterVehicle();
                }
                else
                {
                    ExitVehicle();
                }
            }
        }

        #endregion

        #region Interaction Handling

        /// <summary>
        /// Handles player input for interacting with the vehicle.
        /// </summary>
        private void HandleInteractionInput()
        {
            if (isPlayerInRange && Input.GetKeyDown(interactionKey))
            {
                Interact();
            }
        }

        /// <summary>
        /// Handles entering the vehicle.
        /// </summary>
        private void EnterVehicle()
        {
            if (playerManager == null || vehicleController == null || gravityFieldGenerator == null)
            {
                Debug.LogError("VehicleInteraction: Missing references. Please assign all required references in the Inspector.");
                return;
            }

            // Disable player controls
            playerManager.SetPlayerControl(false);

            // Activate the vehicle's gravity field
            gravityFieldGenerator.SetGravityFieldActive(true);

            // Update state
            isPlayerInside = true;

            // Update interaction prompt
            ShowInteractionPrompt(false);

            // Optional: Activate vehicle-specific HUD elements
            UIManager.Instance?.ActivateVehicleHUD(); // Uncomment if applicable

            // Trigger event
            EventManager.Instance?.TriggerPlayerEnteredVehicle();

            Debug.Log("Player has entered the vehicle.");
        }

        /// <summary>
        /// Handles exiting the vehicle.
        /// </summary>
        private void ExitVehicle()
        {
            if (playerManager == null || vehicleController == null || gravityFieldGenerator == null || exitPoint == null)
            {
                Debug.LogError("VehicleInteraction: Missing references. Please assign all required references in the Inspector.");
                return;
            }

            // Enable player controls
            playerManager.SetPlayerControl(true);

            // Deactivate the vehicle's gravity field
            gravityFieldGenerator.SetGravityFieldActive(false);

            // Move player to exit point
            playerManager.transform.position = exitPoint.position;
            playerManager.transform.rotation = exitPoint.rotation;

            // Update state
            isPlayerInside = false;

            // Update interaction prompt
            ShowInteractionPrompt(true);

            // Optional: Deactivate vehicle-specific HUD elements
            UIManager.Instance?.DeactivateVehicleHUD(); // Uncomment if applicable

            // Trigger event
            EventManager.Instance?.TriggerPlayerExitedVehicle();

            Debug.Log("Player has exited the vehicle.");
        }

        /// <summary>
        /// Shows or hides the interaction prompt UI.
        /// </summary>
        /// <param name="show">True to show, false to hide.</param>
        private void ShowInteractionPrompt(bool show)
        {
            if (interactionCanvas != null)
            {
                interactionCanvas.gameObject.SetActive(show);
                // Update the prompt text based on the current state
                if (show && interactionCanvas.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI promptText))
                {
                    promptText.text = isPlayerInside ? "Press E to exit vehicle" : "Press E to enter vehicle";
                }
            }
        }

        #endregion
    }
}
