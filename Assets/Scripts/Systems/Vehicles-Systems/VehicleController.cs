using UnityEngine;
using Core.Interfaces;
using Core.Managers;

namespace Systems.VehicleSystems
{
    /// <summary>
    /// Controls the vehicle's movement, floating mechanics, gravity toggling, and gravity fuel management.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : MonoBehaviour, IMovable
    {
        #region Inspector Variables

        [Header("Floating Settings")]
        [SerializeField] private float floatingHeight = 5f;            // Desired height when floating
        [SerializeField] private float floatingForce = 10f;            // Force applied to maintain floating
        [SerializeField] private float floatingDamping = 2f;           // Damping to smooth floating motion

        [Header("Grounded Settings")]
        [SerializeField] private LayerMask groundLayer;                // Layer representing the ground
        [SerializeField] private float groundCheckDistance = 1f;       // Distance to check for ground

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;                // Movement speed of the vehicle
        [SerializeField] private float rotationSpeed = 100f;           // Rotation speed of the vehicle

        [Header("Gravity Fuel Settings")]
        [SerializeField] private float maxGravityFuel = 100f;          // Maximum gravity fuel capacity
        [SerializeField] private float gravityFuelDrainRate = 10f;     // Fuel drained per second while gravity is on
        [SerializeField] private float gravityFuelRechargeRate = 5f;   // Fuel replenished per second when gravity is off
        [SerializeField] private KeyCode gravityToggleKey = KeyCode.G; // Key to toggle gravity

        [Header("Gravity Field Settings")]
        [SerializeField] private GravityFieldGenerator gravityFieldGenerator; // Reference to the GravityFieldGenerator script

        #endregion

        #region Private Variables

        private Rigidbody rb;                                           // Reference to the Rigidbody component
        public bool isGravityOn = true;                                // Current state of gravity
        private bool isGrounded = false;                                // Whether the vehicle is grounded
        private float currentGravityFuel;                               // Current gravity fuel level

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("VehicleController: No Rigidbody found.");
            }
        }

        private void Start()
        {
            InitializeVehicle();
            if (EventManager.Instance != null)
            {
                EventManager.Instance.EventOnOxygenLow += HandleOxygenLow;
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.EventOnOxygenLow -= HandleOxygenLow;
            }
        }

        private void Update()
        {
            HandleGravityToggle();
            HandleMovementInput();
            CheckGroundedStatus();
            ManageGravityFuel();
        }

        private void FixedUpdate()
        {
            if (isGravityOn && currentGravityFuel > 0f)
            {
                MaintainGroundedState();
            }
            else
            {
                MaintainFloatingState();
                SuspendVehicleMotion();
            }
        }

        #endregion

        #region IMovable Implementation

        /// <summary>
        /// Moves the vehicle in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        public void Move(Vector3 direction)
        {
            // Assuming 'direction' is a normalized vector representing movement input
            Vector3 movement = direction * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }

        /// <summary>
        /// Rotates the vehicle around the Y-axis.
        /// </summary>
        /// <param name="angle">Angle in degrees to rotate.</param>
        public void Rotate(float angle)
        {
            Quaternion rotation = Quaternion.Euler(0f, angle * Time.deltaTime, 0f);
            rb.MoveRotation(rb.rotation * rotation);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the vehicle's Rigidbody settings and gravity fuel.
        /// </summary>
        private void InitializeVehicle()
        {
            rb.useGravity = isGravityOn;
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent the vehicle from tipping over
            currentGravityFuel = maxGravityFuel;

            // Initialize the Gravity Field Generator
            if (gravityFieldGenerator != null)
            {
                gravityFieldGenerator.SetGravityFieldActive(isGravityOn && currentGravityFuel > 0f);
            }
            else
            {
                Debug.LogError("VehicleController: GravityFieldGenerator reference is not assigned.");
            }
        }

        #endregion

        #region Gravity Toggle Logic

        /// <summary>
        /// Handles input to toggle gravity on and off based on fuel availability.
        /// </summary>
        private void HandleGravityToggle()
        {
            if (Input.GetKeyDown(gravityToggleKey))
            {
                if (isGravityOn)
                {
                    // Allow turning off gravity at any time
                    ToggleGravity();
                }
                else
                {
                    // Allow turning on gravity only if enough fuel is available
                    if (currentGravityFuel > 0f)
                    {
                        ToggleGravity();
                    }
                    else
                    {
                        Debug.Log("Insufficient Gravity Fuel to Enable Gravity!");
                        // Provide UI feedback to the player
                        UIManager.Instance?.ShowPickupMessage("Insufficient Gravity Fuel!");
                    }
                }
            }
        }

        /// <summary>
        /// Toggles the gravity state of the vehicle.
        /// </summary>
        private void ToggleGravity()
        {
            isGravityOn = !isGravityOn;
            rb.useGravity = isGravityOn;

            if (isGravityOn)
            {
                Debug.Log("Gravity Enabled: Vehicle is now grounded.");
                // Reset velocities to prevent unintended motion when gravity is re-enabled
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // Update Gravity Field Generator
                if (gravityFieldGenerator != null)
                {
                    gravityFieldGenerator.SetGravityFieldActive(isGravityOn && currentGravityFuel > 0f);
                }

                // Show feedback message
                UIManager.Instance?.ShowPickupMessage("Gravity Enabled!");
            }
            else
            {
                Debug.Log("Gravity Disabled: Vehicle is now floating.");

                // Update Gravity Field Generator
                if (gravityFieldGenerator != null)
                {
                    gravityFieldGenerator.SetGravityFieldActive(isGravityOn && currentGravityFuel > 0f);
                }

                // Show feedback message
                UIManager.Instance?.ShowPickupMessage("Gravity Disabled!");
            }
        }

        #endregion

        #region Grounded State Management

        /// <summary>
        /// Checks whether the vehicle is grounded by casting a ray downward.
        /// </summary>
        private void CheckGroundedStatus()
        {
            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 0.1f; // Slightly above to prevent immediate collision

            if (Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        /// <summary>
        /// Maintains the vehicle's grounded state by ensuring it stays on the ground.
        /// </summary>
        private void MaintainGroundedState()
        {
            if (isGrounded)
            {
                // Freeze vertical movement to keep the vehicle stationary on the ground
                Vector3 velocity = rb.velocity;
                velocity.y = 0f;
                rb.velocity = velocity;
            }
            else
            {
                // Snap the vehicle back to the ground to prevent unintended floating
                Vector3 position = transform.position;
                position.y = Mathf.Max(position.y, 0f); // Assuming ground is at y=0
                transform.position = position;
            }
        }

        #endregion

        #region Floating State Management

        /// <summary>
        /// Maintains the vehicle's floating state by applying upward force to reach the desired height.
        /// </summary>
        private void MaintainFloatingState()
        {
            float currentHeight = transform.position.y;
            float heightDifference = floatingHeight - currentHeight;

            // Apply a force proportional to the height difference to maintain floating
            Vector3 force = Vector3.up * (heightDifference * floatingForce) - Vector3.up * (rb.velocity.y * floatingDamping);
            rb.AddForce(force, ForceMode.Acceleration);
        }

        #endregion

        #region Movement Logic

        /// <summary>
        /// Handles player input for vehicle movement.
        /// </summary>
        private void HandleMovementInput()
        {
            // Only allow movement when gravity is on, the vehicle is grounded, and there's sufficient fuel
            if (isGravityOn && isGrounded && currentGravityFuel > 0f)
            {
                // Get input axes
                float moveInput = Input.GetAxis("Vertical");     // W/S or Up/Down arrows
                float rotateInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows

                // Calculate movement direction
                Vector3 moveDirectionInput = new Vector3(0f, 0f, moveInput).normalized;

                // Implementing the IMovable interface methods
                Move(moveDirectionInput);
                Rotate(rotateInput * rotationSpeed);
            }
        }

        #endregion

        #region Gravity Fuel Management

        /// <summary>
        /// Manages gravity fuel depletion and replenishment.
        /// </summary>
        private void ManageGravityFuel()
        {
            if (isGravityOn && currentGravityFuel > 0f)
            {
                // Deplete gravity fuel while gravity is on
                currentGravityFuel -= gravityFuelDrainRate * Time.deltaTime;
                currentGravityFuel = Mathf.Clamp(currentGravityFuel, 0f, maxGravityFuel);

                // Update UI
                UIManager.Instance?.UpdateGravityFuelBar(currentGravityFuel, maxGravityFuel);

                // Check if gravity fuel is depleted
                if (currentGravityFuel <= 0f)
                {
                    Debug.Log("Gravity Fuel Depleted! Disabling Gravity.");
                    // Automatically disable gravity
                    ToggleGravity();
                }
            }
            else if (!isGravityOn && currentGravityFuel < maxGravityFuel)
            {
                // Replenish gravity fuel when gravity is off
                currentGravityFuel += gravityFuelRechargeRate * Time.deltaTime;
                currentGravityFuel = Mathf.Clamp(currentGravityFuel, 0f, maxGravityFuel);

                // Update UI
                UIManager.Instance?.UpdateGravityFuelBar(currentGravityFuel, maxGravityFuel);
            }
        }

        /// <summary>
        /// Retrieves the current gravity fuel level.
        /// </summary>
        /// <returns>Current gravity fuel.</returns>
        public float GetCurrentGravityFuel()
        {
            return currentGravityFuel;
        }

        #endregion

        #region Suspension of Vehicle Motion

        /// <summary>
        /// Gradually suspends vehicle motion by reducing velocity to zero.
        /// </summary>
        private void SuspendVehicleMotion()
        {
            if (rb.velocity != Vector3.zero)
            {
                // Reduce velocity gradually
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 2f); // Adjust the multiplier for speed
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 2f);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles low oxygen events to modify vehicle functionality.
        /// </summary>
        private void HandleOxygenLow()
        {
            // Example: Reduce movement speed temporarily
            moveSpeed *= 0.8f;
            Debug.LogWarning("VehicleController: Low oxygen detected! Movement speed reduced.");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Replenishes the vehicle's gravity fuel. Can be called externally.
        /// </summary>
        /// <param name="amount">Amount of gravity fuel to add.</param>
        public void ReplenishGravityFuel(float amount)
        {
            currentGravityFuel = Mathf.Min(currentGravityFuel + amount, maxGravityFuel);
            UIManager.Instance?.UpdateGravityFuelBar(currentGravityFuel, maxGravityFuel);
            Debug.Log($"Gravity Fuel Replenished by: {amount}");
        }

        #endregion
    }
}
