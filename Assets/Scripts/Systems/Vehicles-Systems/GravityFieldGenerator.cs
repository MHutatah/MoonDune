using UnityEngine;
using Core.Interfaces;
using Systems.VehicleSystems;
using System.Collections.Generic;

namespace Systems.VehicleSystems
{
    /// <summary>
    /// Generates a gravitational field around the vehicle, affecting nearby objects.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class GravityFieldGenerator : MonoBehaviour
    {
        #region Inspector Variables

        [Header("Gravity Settings")]
        [SerializeField] private float gravityStrength = 9.81f;        // Strength of the gravitational pull
        [SerializeField] private float gravityRadius = 10f;           // Radius of the gravitational field

        [Header("Force Settings")]
        [SerializeField] private ForceMode forceMode = ForceMode.Acceleration; // Type of force applied

        [Header("Visual Settings")]
        [SerializeField] private GameObject gravityFieldVisualPrefab;   // Prefab for visualizing the gravity field
        [SerializeField] private Color fieldColor = Color.blue;         // Color of the gravity field visualization
        [SerializeField] private float visualScale = 1f;               // Scale multiplier for the visualization

        #endregion

        #region Private Variables

        private VehicleController vehicleController;                    // Reference to the VehicleController script
        private SphereCollider sphereCollider;                          // SphereCollider used as a trigger for detecting nearby objects
        private GameObject gravityFieldVisualInstance;                  // Instance of the gravity field visualization
        private Rigidbody vehicleRigidbody;                             // Rigidbody of the vehicle to exclude from gravity effects

        private List<Rigidbody> affectedRigidbodies = new List<Rigidbody>(); // List of currently affected rigidbodies

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
        }

        private void FixedUpdate()
        {
            ApplyGravityToAffectedRigidbodies();
        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null && rb != vehicleRigidbody && !affectedRigidbodies.Contains(rb))
            {
                affectedRigidbodies.Add(rb);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null && affectedRigidbodies.Contains(rb))
            {
                affectedRigidbodies.Remove(rb);
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
                    // Set material properties for transparency
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

        #region Gravity Application

        /// <summary>
        /// Applies gravitational force to all affected rigidbodies.
        /// </summary>
        private void ApplyGravityToAffectedRigidbodies()
        {
            foreach (Rigidbody rb in affectedRigidbodies)
            {
                if (rb != null)
                {
                    Vector3 direction = (transform.position - rb.transform.position).normalized;
                    float distance = Vector3.Distance(transform.position, rb.transform.position);
                    float forceMagnitude = gravityStrength / Mathf.Max(distance, 1f); // Prevent division by zero

                    rb.AddForce(direction * forceMagnitude, forceMode);
                }
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
            sphereCollider.enabled = active;
            if (gravityFieldVisualInstance != null)
            {
                gravityFieldVisualInstance.SetActive(active);
            }

            if (!active)
            {
                // Clear the list of affected rigidbodies when deactivating the field
                affectedRigidbodies.Clear();
            }
        }

        #endregion
    }
}
