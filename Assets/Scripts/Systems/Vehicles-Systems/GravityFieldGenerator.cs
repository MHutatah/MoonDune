using UnityEngine;
using System;

[RequireComponent(typeof(SphereCollider))]
public class GravityFieldGenerator : MonoBehaviour
{
    #region Inspector Variables

    [Header("Gravity Settings")]
    [SerializeField] private float gravityStrength = 9.81f;        // Strength of the gravitational pull
    [SerializeField] private float gravityRadius = 10f;           // Radius of the gravitational field
    [SerializeField] private float maxGravityForce = 100f;        // Maximum gravity force applied
    [SerializeField] private LayerMask affectedLayers;           // Layers to be affected by the gravity field

    [Header("Force Settings")]
    [SerializeField] private ForceMode forceMode = ForceMode.Acceleration; // Type of force applied

    [Header("Visual Settings")]
    [SerializeField] private GameObject gravityFieldVisualPrefab;   // Prefab for visualizing the gravity field
    [SerializeField] private Color fieldColor = Color.blue;        // Color of the gravity field visualization
    [SerializeField] private float visualScale = 1f;               // Scale multiplier for the visualization

    #endregion

    #region Private Variables

    private VehicleController vehicleController;                    // Reference to the VehicleController script
    private SphereCollider sphereCollider;                          // SphereCollider used as a trigger for detecting nearby objects
    private GameObject gravityFieldVisualInstance;                  // Instance of the gravity field visualization
    private Rigidbody vehicleRigidbody;                             // Rigidbody of the vehicle to exclude from gravity effects

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        // Ensure the SphereCollider is set as a trigger and has the correct radius
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = gravityRadius;
    }

    private void Start()
    {
        // Find the VehicleController script in the parent GameObject
        vehicleController = GetComponentInParent<VehicleController>();
        if (vehicleController == null)
        {
            Debug.LogError("GravityFieldGenerator: VehicleController script not found in parent GameObject.");
            enabled = false; // Disable this script if VehicleController is not found
            return;
        }

        // Get the Rigidbody component from the VehicleController's GameObject
        vehicleRigidbody = vehicleController.GetComponent<Rigidbody>();
        if (vehicleRigidbody == null)
        {
            Debug.LogError("GravityFieldGenerator: Rigidbody component not found in VehicleController's GameObject.");
            enabled = false; // Disable this script if Rigidbody is not found
            return;
        }

        // Initialize gravity field visualization
        InitializeGravityFieldVisual();

        // Subscribe to gravity state changes
        vehicleController.OnGravityStateChanged += HandleGravityStateChanged;

        // Set initial state
        bool initialState = vehicleController.IsGravityOn && vehicleController.GetCurrentGravityFuel() > 0f;
        SetGravityFieldActive(initialState);
    }

    private void Update()
    {
        // Dynamic scaling of the gravity field visualization
        if (gravityFieldVisualInstance != null)
        {
            gravityFieldVisualInstance.transform.localScale = Vector3.one * gravityRadius * visualScale;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (vehicleController == null || vehicleRigidbody == null) return;

        // Ignore the vehicle itself
        if (other.attachedRigidbody == null || other.attachedRigidbody == vehicleRigidbody) return;

        // Check if the object is on an affected layer
        if (((1 << other.gameObject.layer) & affectedLayers) == 0) return;

        // Apply gravitational force towards the vehicle
        Vector3 direction = (transform.position - other.transform.position).normalized;
        float distance = Vector3.Distance(transform.position, other.transform.position);

        // Inverse Square Law for gravity strength
        float adjustedGravity = gravityStrength / Mathf.Max(distance * distance, 1f); // Prevent division by zero
        adjustedGravity = Mathf.Clamp(adjustedGravity, 0f, maxGravityForce);

        // Apply force to the other object's Rigidbody
        other.attachedRigidbody.AddForce(direction * adjustedGravity, forceMode);
    }

    private void OnDestroy()
    {
        if (vehicleController != null)
        {
            vehicleController.OnGravityStateChanged -= HandleGravityStateChanged;
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the gravity field visualization.
    /// </summary>
    private void InitializeGravityFieldVisual()
    {
        if (gravityFieldVisualPrefab != null)
        {
            gravityFieldVisualInstance = Instantiate(gravityFieldVisualPrefab, transform.position, Quaternion.identity, transform);
            gravityFieldVisualInstance.transform.localScale = Vector3.one * gravityRadius * visualScale;

            // Set the color of the visualization
            Renderer[] renderers = gravityFieldVisualInstance.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                rend.material.color = fieldColor;
                // Optionally, set material properties for transparency
                rend.material.SetFloat("_Mode", 3); // Transparent
                rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                rend.material.SetInt("_ZWrite", 0);
                rend.material.DisableKeyword("_ALPHATEST_ON");
                rend.material.EnableKeyword("_ALPHABLEND_ON");
                rend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                rend.material.renderQueue = 3000;
            }
        }
        else
        {
            Debug.LogWarning("GravityFieldGenerator: GravityFieldVisualPrefab is not assigned.");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Activates or deactivates the gravity field.
    /// </summary>
    /// <param name="active">True to activate, false to deactivate.</param>
    public void SetGravityFieldActive(bool active)
    {
        if (sphereCollider != null)
        {
            sphereCollider.enabled = active;
        }

        if (gravityFieldVisualInstance != null)
        {
            gravityFieldVisualInstance.SetActive(active);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles gravity field activation based on the vehicle's state.
    /// </summary>
    /// <param name="isActive">True to activate, false to deactivate.</param>
    private void HandleGravityStateChanged(bool isActive)
    {
        SetGravityFieldActive(isActive);
    }

    #endregion
}
