using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed when moving forward/backward.")]
    public float moveSpeed = 3f;
    
    [Tooltip("Rotation speed in degrees per second.")]
    public float turnSpeed = 90f;
    
    [Tooltip("Jump force upward.")]
    public float jumpForce = 5f;
    
    [Tooltip("Effective gravity on the Moon (m/s^2).")]
    public float gravity = 1.62f;
    
    private CharacterController charController;
    private Vector3 moveDirection;

    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get input
        float h = Input.GetAxis("Horizontal");  // used for turning
        float v = Input.GetAxis("Vertical");    // used for forward/backward movement

        // Rotate the player left/right based on horizontal input
        transform.Rotate(0f, h * turnSpeed * Time.deltaTime, 0f);

        // Determine forward movement in the direction the player is facing
        Vector3 forward = transform.forward * v * moveSpeed;

        // If we're on the ground, set horizontal movement + handle jumping
        if (charController.isGrounded)
        {
            moveDirection = forward;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }
        else
        {
            // Retain current horizontal movement in the air
            moveDirection.x = forward.x;
            moveDirection.z = forward.z;
        }

        // Apply Moon gravity over time
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the character
        charController.Move(moveDirection * Time.deltaTime);
    }
}


