using UnityEngine;
using Core.Managers;
using Core.Interfaces;
using Systems.UISystems;

namespace Systems.VehicleSystems
{
    /// <summary>
    /// Manages the vehicle's gravity fuel, driving mechanics, and state transitions.
    /// Implements IFuelReplenisher for gravity fuel replenishing capabilities.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : MonoBehaviour, IFuelReplenisher
    {
        #region Inspector Variables

        [Header("Gravity Fuel Settings")]
        [SerializeField] public float maxGravityFuel = 100f;
        [Range(0f, 100f)]
        [SerializeField] public float currentGravityFuel;
        [SerializeField] private float drainRate = 10f;     // Fuel drained per second while gravity is on
        [SerializeField] private float rechargeRate = 5f;   // Fuel replenished per second when gravity is off

        [Header("HUD Reference")]
        [SerializeField] public HUDController PlayerHUD;
        [SerializeField] public HUDController VehicleHUD;

        [Header("Input Settings")]
        [SerializeField] private KeyCode gravityToggleKey = KeyCode.G; // Key to toggle gravity

        [Header("Driving Settings")]
        [SerializeField] private float driveSpeed = 15f;             // Movement speed when driving
        [SerializeField] private float rotationSpeed = 100f;        // Rotation speed
        [SerializeField] private float transitionDuration = 2f;     // Duration for transitioning between states

        #endregion

        #region Private Variables

        // Rigidbody component for physics interactions
        private Rigidbody rb;

        // Gravity state variable
        public bool isGravityOn = true;

        // Transition management variables
        private bool isTransitioning = false;
        private float transitionTimer = 0f;
        private Vector3 initialVelocity;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            // Ensure the Rigidbody component is present
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("VehicleController: Rigidbody component is missing.");
            }
        }

        private void Start()
        {
            currentGravityFuel = maxGravityFuel;
            UpdateHUD();

            // Initially deactivate Vehicle HUD if necessary
            UIManager.Instance?.DeactivateVehicleHUD();
        }

        private void Update()
        {
            HandleUserInput();
            ManageGravityFuel();
            HandleTransition();
            HandleDriving();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles user input for toggling gravity.
        /// </summary>
        private void HandleUserInput()
        {
            // Toggle gravity when the specified key is pressed and not currently transitioning
            if (Input.GetKeyDown(gravityToggleKey) && !isTransitioning)
            {
                StartTransition(!isGravityOn);
            }
        }

        /// <summary>
        /// Manages gravity fuel based on the current gravity state.
        /// </summary>
        private void ManageGravityFuel()
        {
            if (isGravityOn)
            {
                // Drain gravity fuel over time
                currentGravityFuel -= drainRate * Time.deltaTime;
                currentGravityFuel = Mathf.Clamp(currentGravityFuel, 0f, maxGravityFuel);

                if (currentGravityFuel <= 0f)
                {
                    // Automatically disable gravity when fuel is depleted
                    StartTransition(false);
                    SoundManager.Instance?.PlayLowGravityFuelAlert(); // Assumes this method exists
                    Debug.Log("Vehicle's gravity fuel depleted and gravity turned off!");
                }
            }
            else
            {
                // Recharge gravity fuel over time when gravity is off
                currentGravityFuel += rechargeRate * Time.deltaTime;
                currentGravityFuel = Mathf.Clamp(currentGravityFuel, 0f, maxGravityFuel);
            }

            UpdateHUD();
        }

        /// <summary>
        /// Initiates the transition between gravity states.
        /// </summary>
        /// <param name="enableGravity">True to enable gravity, false to disable.</param>
        private void StartTransition(bool enableGravity)
        {
            isTransitioning = true;
            transitionTimer = transitionDuration;
            initialVelocity = rb.velocity;

            // Optionally, trigger transition effects or sounds here
            Debug.Log($"Starting transition to {(enableGravity ? "Gravity On" : "Gravity Off")}");
        }

        /// <summary>
        /// Handles the smooth transition between gravity states, decelerating velocity to zero.
        /// </summary>
        private void HandleTransition()
        {
            if (isTransitioning)
            {
                // Calculate the fraction of transition completed
                float transitionFraction = 1f - (transitionTimer / transitionDuration);

                // Gradually reduce velocity to zero using linear interpolation
                rb.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, transitionFraction);

                // Optionally, handle angular velocity similarly
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, transitionFraction);

                // Decrement the transition timer
                transitionTimer -= Time.deltaTime;

                if (transitionTimer <= 0f)
                {
                    // Transition complete
                    isTransitioning = false;
                    isGravityOn = !isGravityOn;

                    // Toggle Rigidbody gravity based on the new state
                    rb.useGravity = isGravityOn;

                    if (!isGravityOn)
                    {
                        // When gravity is off, make the vehicle float by setting higher drag and freezing rotation
                        rb.drag = 5f; // Increase drag to simulate floating
                        rb.angularDrag = 5f;
                        rb.constraints = RigidbodyConstraints.FreezeRotation;
                    }
                    else
                    {
                        // Reset drag and constraints when gravity is on
                        rb.drag = 0f;
                        rb.angularDrag = 0f;
                        rb.constraints = RigidbodyConstraints.None;
                    }

                    // Trigger relevant events
                    if (isGravityOn)
                    {
                        EventManager.Instance?.TriggerPlayerEnteredVehicle();
                    }
                    else
                    {
                        EventManager.Instance?.TriggerPlayerExitedVehicle();
                    }

                    Debug.Log($"Transition complete: Gravity is now {(isGravityOn ? "On" : "Off")}");
                }
            }
        }

        /// <summary>
        /// Handles vehicle driving mechanics when gravity is enabled.
        /// </summary>
        private void HandleDriving()
        {
            if (isGravityOn && !isTransitioning)
            {
                // Capture input axes
                float moveInput = Input.GetAxis("Vertical");     // Typically W/S or Up/Down arrows
                float rotateInput = Input.GetAxis("Horizontal"); // Typically A/D or Left/Right arrows

                // Calculate movement direction
                Vector3 moveDirection = transform.forward * moveInput * driveSpeed;

                // Apply movement force
                rb.AddForce(moveDirection, ForceMode.Acceleration);

                // Calculate rotation torque
                float rotationTorque = rotateInput * rotationSpeed * Time.deltaTime;

                // Apply rotation torque
                rb.AddTorque(Vector3.up * rotationTorque, ForceMode.Acceleration);
            }
        }

        /// <summary>
        /// Updates the HUD elements with the current gravity fuel levels.
        /// </summary>
        private void UpdateHUD()
        {
            if (PlayerHUD != null)
            {
                PlayerHUD.UpdateGravityFuel(currentGravityFuel, maxGravityFuel);
            }

            if (VehicleHUD != null)
            {
                VehicleHUD.UpdateGravityFuel(currentGravityFuel, maxGravityFuel);
            }
        }

        #endregion

        #region IFuelReplenisher Implementation

        /// <summary>
        /// Replenishes the vehicle's gravity fuel by the specified amount.
        /// </summary>
        /// <param name="amount">Amount of fuel to add.</param>
        public void ReplenishFuel(float amount)
        {
            currentGravityFuel += amount;
            currentGravityFuel = Mathf.Clamp(currentGravityFuel, 0f, maxGravityFuel);
            UpdateHUD();
            Debug.Log($"Vehicle Gravity Fuel Replenished by: {amount}");

            // Optionally, play a sound or effect
            SoundManager.Instance?.PlayCrystalPickup();
        }

        /// <summary>
        /// Gets the current gravity fuel level.
        /// </summary>
        /// <returns>Current gravity fuel.</returns>
        public float FuelAmount => currentGravityFuel;

        #endregion
    }
}
