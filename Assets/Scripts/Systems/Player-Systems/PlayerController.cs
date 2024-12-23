using UnityEngine;
using Core.Interfaces;

namespace PlayerSystems
{
    /// <summary>
    /// Controls the player's movement and rotation using a CharacterController.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, IMovable
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
        
        #endregion

        #region Private Variables

        private CharacterController charController;
        private Vector3 moveDirection;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            charController = GetComponent<CharacterController>();
            if (charController == null)
            {
                Debug.LogError("PlayerController: No CharacterController found.");
            }
        }

        private void Update()
        {
            HandleRotation();
            HandleMovement();
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

            if (charController.isGrounded)
            {
                moveDirection = forwardMovement;
                if (Input.GetButtonDown("Jump"))
                {
                    moveDirection.y = jumpForce;
                }
                else
                {
                    moveDirection.y = 0f;
                }
            }
            else
            {
                // Retain current horizontal movement in the air
                moveDirection.x = forwardMovement.x;
                moveDirection.z = forwardMovement.z;
            }

            // Apply Moon gravity over time
            moveDirection.y -= gravity * Time.deltaTime;

            // Move the character
            charController.Move(moveDirection * Time.deltaTime);
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

        #region Additional Movement Handling

        /// <summary>
        /// Handles player input for rotation.
        /// </summary>
        private void HandleRotation()
        {
            float horizontalInput = Input.GetAxis("Horizontal");  // A/D or Left/Right arrows
            Rotate(horizontalInput * turnSpeed);
        }

        /// <summary>
        /// Handles player input for movement.
        /// </summary>
        private void HandleMovement()
        {
            float verticalInput = Input.GetAxis("Vertical");    // W/S or Up/Down arrows
            Move(new Vector3(0f, 0f, verticalInput));
        }

        #endregion
    }
}
