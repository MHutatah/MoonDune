using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 50f;

    private Rigidbody rb;
    private bool gravityFieldActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Basic forward/backward movement
        float moveInput = Input.GetAxis("Vertical") * moveSpeed;
        rb.AddForce(transform.forward * moveInput, ForceMode.Force);

        // Turning
        float turnInput = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
        transform.Rotate(0, turnInput, 0);

        // Toggle Gravity Field
        if (Input.GetKeyDown(KeyCode.G))
        {
            gravityFieldActive = !gravityFieldActive;
        }
    }

    void FixedUpdate()
    {
        // Additional downward force if gravity field is active
        if (gravityFieldActive)
        {
            rb.AddForce(Vector3.down * 9.81f, ForceMode.Acceleration);
            // Adjust or scale the force to feel right for “Moon gravity + artificial push”
        }
    }
}
