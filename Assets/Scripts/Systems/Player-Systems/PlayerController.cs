using UnityEngine;
using Core.Interfaces;
using Systems.VehicleSystems;
using Core.Managers;
using Systems.UISystems;

namespace Systems.PlayerSystems
{
    /// <summary>
    /// Controls the player's movement and rotation using a CharacterController.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, IMovable, IFuelReplenisher
    {
        #region Inspector Variables

        [Header("Movement Settings")]
        [Tooltip("Movement speed when moving forward/backward.")]
        [SerializeField] private float moveSpeed = 3f;
        
        [Tooltip("Rotation speed in degrees per second.")]
        [SerializeField] private float turnSpeed = 90f;
        
        [Tooltip("Jump force upward.")]
        [SerializeField] private float jumpForce = 5f;
        
        [Tooltip("Effective gravity on the Moon (m/s^2).")]
        [SerializeField] private float gravity = 1.62f;

        [Header("Firing Settings")]
        [Tooltip("Key to fire the laser.")]
        [SerializeField] private KeyCode fireKey = KeyCode.Mouse0; // Left mouse button by default

        [SerializeField] public VehicleController vehicleController;

        [SerializeField] public HUDController PlayerHUD;

        [SerializeField] public HUDController VehicleHUD;

        #endregion

        #region Private Variables

        private CharacterController charController;
        private Vector3 moveDirection;

        [HideInInspector]
        public PlayerLaser playerLaser; // Reference to the PlayerLaser component

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            charController = GetComponent<CharacterController>();
            if (charController == null)
            {
                Debug.LogError("PlayerController: No CharacterController found.");
            }

            playerLaser = GetComponent<PlayerLaser>();
            if (playerLaser == null)
            {
                Debug.LogError("PlayerController: No PlayerLaser component found.");
            }
        }

        private void Update()
        {
            HandleRotation();
            HandleMovement();
            HandleFiring();
        }

        #endregion

        #region IMovable Implementation

        /// <summary>
        /// Moves the player in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        public void Move(Vector3 direction)
        {
            if (charController == null) return;

            float verticalInput = direction.z; // Assuming forward/backward movement
            Vector3 forwardMovement = transform.forward * verticalInput * moveSpeed;
            Vector3 rightMovement = transform.right * direction.x * moveSpeed;

            moveDirection.x = rightMovement.x + forwardMovement.x;
            moveDirection.z = rightMovement.z + forwardMovement.z;
        }

        /// <summary>
        /// Rotates the player around the Y-axis.
        /// </summary>
        /// <param name="angle">Angle in degrees to rotate.</param>
        public void Rotate(float angle)
        {
            transform.Rotate(0f, angle * Time.deltaTime, 0f);
        }

        #endregion

        #region IFuelReplenisher Implementation

        /// <summary>
        /// Replenishes the vehicle's gravity fuel by the specified amount.
        /// </summary>
        /// <param name="amount">Amount of fuel to add.</param>
        public void ReplenishFuel(float amount)
        {
            vehicleController.currentGravityFuel += amount;
            vehicleController.currentGravityFuel = Mathf.Clamp(vehicleController.currentGravityFuel, 0f, vehicleController.maxGravityFuel);
            UpdateHUD();
            Debug.Log($"Vehicle Gravity Fuel Replenished by: {amount}");

            // Optionally, play a sound or effect
            SoundManager.Instance?.PlayCrystalPickup();
        }
        
        private void UpdateHUD()
        {
            if (PlayerHUD != null && VehicleHUD != null)
            {
                PlayerHUD.UpdateGravityFuel(vehicleController.currentGravityFuel, vehicleController.maxGravityFuel);
                VehicleHUD.UpdateGravityFuel(vehicleController.currentGravityFuel, vehicleController.maxGravityFuel);
            }
        }

        /// <summary>
        /// Gets the current gravity fuel level.
        /// </summary>
        /// <returns>Current gravity fuel.</returns>
        public float FuelAmount => vehicleController.currentGravityFuel;

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles player input for rotation.
        /// </summary>
        private void HandleRotation()
        {
            float horizontalInput = Input.GetAxis("Horizontal");  // A/D or Left/Right arrows
            Rotate(horizontalInput * turnSpeed);
        }

        /// <summary>
        /// Handles player input for movement and jumping.
        /// </summary>
        private void HandleMovement()
        {
            float verticalInput = Input.GetAxis("Vertical");    // W/S or Up/Down arrows
            Move(new Vector3(0f, 0f, verticalInput));

            if (charController.isGrounded)
            {
                // Reset Y-axis movement when grounded, unless jumping
                if (Input.GetButtonDown("Jump"))
                {
                    moveDirection.y = jumpForce;
                    Debug.Log("Player Jumped!");
                }
                else
                {
                    moveDirection.y = 0f;
                }
            }
            else
            {
                // Apply gravity if not grounded
                moveDirection.y -= gravity * Time.deltaTime;
            }

            // Move the character
            charController.Move(moveDirection * Time.deltaTime);
        }

        /// <summary>
        /// Handles player input for firing the laser.
        /// </summary>
        private void HandleFiring()
        {
            if (playerLaser == null) return;

            if (Input.GetKeyDown(fireKey))
            {
                Vector3 fireDirection = transform.forward;
                playerLaser.Fire(fireDirection);
                Debug.Log("Laser Fired!");
            }

            if (Input.GetKeyUp(fireKey))
            {
                playerLaser.StopFiring();
                Debug.Log("Laser Stopped Firing!");
            }
        }

        #endregion
    }
}
