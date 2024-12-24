using UnityEngine;

public class VehicleInteraction : MonoBehaviour
{
    #region Inspector Variables

    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;           // Key to enter/exit vehicle

    [Header("References")]
    [SerializeField] private GameObject playerObject;                      // Reference to the Player GameObject
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            ShowInteractionPrompt(false);
        }
    }

    #endregion

    #region Interaction Handling

    /// <summary>
    /// Handles player input for entering/exiting the vehicle.
    /// </summary>
    private void HandleInteractionInput()
    {
        if (isPlayerInRange && !isPlayerInside)
        {
            // Player can enter the vehicle
            if (Input.GetKeyDown(interactionKey))
            {
                EnterVehicle();
            }
        }
        else if (isPlayerInside)
        {
            // Player can exit the vehicle
            if (Input.GetKeyDown(interactionKey))
            {
                ExitVehicle();
            }
        }
    }

    /// <summary>
    /// Handles entering the vehicle.
    /// </summary>
    private void EnterVehicle()
    {
        if (playerObject == null || vehicleController == null || gravityFieldGenerator == null)
        {
            Debug.LogError("VehicleInteraction: Missing references. Please assign all required references in the Inspector.");
            return;
        }

        // Deactivate the player
        playerObject.SetActive(false);

        // Activate the vehicle controller
        vehicleController.enabled = true;

        // Activate the gravity field
        gravityFieldGenerator.SetGravityFieldActive(true);

        // Update state
        isPlayerInside = true;

        // Optionally, adjust camera or UI settings here

        // Hide interaction prompt
        ShowInteractionPrompt(false);

        Debug.Log("Player has entered the vehicle.");
    }

    /// <summary>
    /// Handles exiting the vehicle.
    /// </summary>
    private void ExitVehicle()
    {
        if (playerObject == null || vehicleController == null || gravityFieldGenerator == null || exitPoint == null)
        {
            Debug.LogError("VehicleInteraction: Missing references. Please assign all required references in the Inspector.");
            return;
        }

        // Deactivate the vehicle controller
        vehicleController.enabled = false;

        // Deactivate the gravity field
        gravityFieldGenerator.SetGravityFieldActive(false);

        // Reactivate the player at the exit point
        playerObject.SetActive(true);
        playerObject.transform.position = exitPoint.position;
        playerObject.transform.rotation = exitPoint.rotation;

        // Update state
        isPlayerInside = false;

        // Optionally, adjust camera or UI settings here

        // Show interaction prompt if player is still in range
        ShowInteractionPrompt(true);

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
        }
    }

    #endregion
}


