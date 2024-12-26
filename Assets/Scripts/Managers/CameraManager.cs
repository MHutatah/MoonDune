using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera playerCamera;    // Assign the player camera in the Inspector
    [SerializeField] private Camera vehicleCamera;  // Assign the vehicle camera in the Inspector

    private void Start()
    {
        // Ensure only the player camera is active at the start
        if (playerCamera != null)
            playerCamera.enabled = true;
        if (vehicleCamera != null)
            vehicleCamera.enabled = false;
    }

    /// <summary>
    /// Switches to the vehicle camera.
    /// </summary>
    public void SwitchToVehicleCamera()
    {
        if (playerCamera != null)
            playerCamera.enabled = false;
        if (vehicleCamera != null)
            vehicleCamera.enabled = true;
    }

    /// <summary>
    /// Switches back to the player camera.
    /// </summary>
    public void SwitchToPlayerCamera()
    {
        if (playerCamera != null)
            playerCamera.enabled = true;
        if (vehicleCamera != null)
            vehicleCamera.enabled = false;
    }
}
