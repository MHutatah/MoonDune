using UnityEngine;
using System; // Import System for Action

/// <summary>
/// Controls the vehicle's movement, floating mechanics, gravity toggling, and gravity fuel management.
/// Enhanced for smooth driving on uneven terrain.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    #region Inspector Variables

    [Header("Floating Settings")]
    [SerializeField] private float floatingHeight = 2f;
    [SerializeField] private float floatingForce = 10f;
    [SerializeField] private float floatingDamping = 5f;

    [Header("Raycast Settings")]
    [SerializeField] private Transform[] groundRayOrigins; // Assign transforms at each wheel position
    [SerializeField] private float rayLength = 1.5f;
    [SerializeField] private float suspensionForceMultiplier = 10f;

    [Header("Grounded Settings")]
    [SerializeField] private LayerMask groundLayer; // Layer representing the terrain or ground

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 500f;
    [SerializeField] private float rotationSpeed = 200f; // Increased for better responsiveness
    [SerializeField] private float maxMoveSpeed = 20f;

    [Header("Gravity Fuel Settings")]
    [SerializeField] private float maxGravityFuel = 100f;
    [SerializeField] private float gravityFuelDrainRate = 10f;
    [SerializeField] private float gravityFuelRechargeRate = 5f;
    [SerializeField] private KeyCode gravityToggleKey = KeyCode.G;

    [Header("Gravity Field Settings")]
    [SerializeField] private GravityFieldGenerator gravityFieldGenerator;

    #endregion

    #region Private Variables

    private Rigidbody rb;
    private bool isGrounded;
    public bool IsGravityOn { get; private set; } = true; // Public property for gravity state
    private float currentGravityFuel;

    private Vector3 averageNormal;

    #endregion

    #region Events

    /// <summary>
    /// Event triggered when the gravity state changes.
    /// Passes the new gravity state as a boolean.
    /// </summary>
    public event Action<bool> OnGravityStateChanged;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InitializeVehicle();
    }

    private void Update()
    {
        HandleGravityToggle();
        HandleMovementInput();
        ManageGravityFuel();
    }

    private void FixedUpdate()
    {
        CheckGroundedStatus();

        if (IsGravityOn && currentGravityFuel > 0f)
        {
            ApplySuspensionForAllWheels();
        }
        else
        {
            MaintainFloatingState();
            SuspendVehicleMotion();
        }

        AlignToTerrain();
        ApplyDamping();
        LimitSpeed();
    }

    #endregion

    #region Initialization

    private void InitializeVehicle()
    {
        rb.useGravity = IsGravityOn;
        // Freeze rotation on X and Z axes to prevent tipping, allow Y-axis rotation
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        currentGravityFuel = maxGravityFuel;

        if (gravityFieldGenerator != null)
        {
            gravityFieldGenerator.SetGravityFieldActive(IsGravityOn && currentGravityFuel > 0f);
        }
        else
        {
            Debug.LogError("VehicleController: GravityFieldGenerator reference is not assigned.");
        }
    }

    #endregion

    #region Gravity Toggle Logic

    private void HandleGravityToggle()
    {
        if (Input.GetKeyDown(gravityToggleKey))
        {
            if (IsGravityOn || currentGravityFuel > 0f)
            {
                ToggleGravity();
            }
            else
            {
                Debug.Log("Insufficient Gravity Fuel to Enable Gravity!");
            }
        }
    }

    private void ToggleGravity()
    {
        IsGravityOn = !IsGravityOn;
        rb.useGravity = IsGravityOn;

        if (gravityFieldGenerator != null)
        {
            gravityFieldGenerator.SetGravityFieldActive(IsGravityOn && currentGravityFuel > 0f);
        }

        // Invoke the event to notify subscribers of the gravity state change
        OnGravityStateChanged?.Invoke(IsGravityOn && currentGravityFuel > 0f);

        if (IsGravityOn)
        {
            Debug.Log("Gravity Enabled: Vehicle is now grounded.");
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // Reset angular velocity to prevent residual spinning
        }
        else
        {
            Debug.Log("Gravity Disabled: Vehicle is now floating.");
        }
    }

    #endregion

    #region Grounded State Management

    private void CheckGroundedStatus()
    {
        isGrounded = false;

        for (int i = 0; i < groundRayOrigins.Length; i++)
        {
            if (Physics.Raycast(groundRayOrigins[i].position, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
            {
                isGrounded = true;
                break;
            }
        }

        // Optional: Visualize the grounded status (can be removed in production)
        Debug.DrawRay(transform.position, Vector3.down * rayLength, isGrounded ? Color.green : Color.red);
    }

    #endregion

    #region Floating State Management

    private void MaintainFloatingState()
    {
        float averageHeight = 0f;
        int hitCount = 0;

        for (int i = 0; i < groundRayOrigins.Length; i++)
        {
            if (Physics.Raycast(groundRayOrigins[i].position, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
            {
                averageHeight += hit.distance;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            averageHeight /= hitCount;
            float heightDifference = floatingHeight - averageHeight;
            Vector3 force = Vector3.up * (heightDifference * floatingForce) - Vector3.up * (rb.velocity.y * floatingDamping);
            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    #endregion

    #region Movement Logic

    private void HandleMovementInput()
    {
        if (IsGravityOn && isGrounded && currentGravityFuel > 0f)
        {
            float moveInput = Input.GetAxis("Vertical");
            float rotateInput = Input.GetAxis("Horizontal");

            Vector3 forwardForce = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.AddForce(forwardForce, ForceMode.Acceleration);

            float rotation = rotateInput * rotationSpeed * Time.fixedDeltaTime;
            rb.AddTorque(Vector3.up * rotation, ForceMode.Acceleration);
        }
    }

    private void LimitSpeed()
    {
        if (rb.velocity.magnitude > maxMoveSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxMoveSpeed;
        }
    }

    #endregion

    #region Gravity Fuel Management

    private void ManageGravityFuel()
    {
        if (IsGravityOn && currentGravityFuel > 0f)
        {
            currentGravityFuel -= gravityFuelDrainRate * Time.deltaTime;
            currentGravityFuel = Mathf.Clamp(currentGravityFuel, 0f, maxGravityFuel);

            UIManager.Instance?.UpdateGravityFuelBar(currentGravityFuel, maxGravityFuel);

            if (currentGravityFuel <= 0f)
            {
                Debug.Log("Gravity Fuel Depleted! Disabling Gravity.");
                ToggleGravity();
            }
        }
        else if (!IsGravityOn && currentGravityFuel < maxGravityFuel)
        {
            currentGravityFuel += gravityFuelRechargeRate * Time.deltaTime;
            currentGravityFuel = Mathf.Clamp(currentGravityFuel, 0f, maxGravityFuel);

            UIManager.Instance?.UpdateGravityFuelBar(currentGravityFuel, maxGravityFuel);
        }
    }

    public float GetCurrentGravityFuel()
    {
        return currentGravityFuel;
    }

    public void ReplenishGravityFuel(float amount)
    {
        currentGravityFuel = Mathf.Min(currentGravityFuel + amount, maxGravityFuel);
        UIManager.Instance?.UpdateGravityFuelBar(currentGravityFuel, maxGravityFuel);
        Debug.Log($"Gravity Fuel Replenished by: {amount}");
    }

    #endregion

    #region Suspension System

    private void ApplySuspensionForAllWheels()
    {
        averageNormal = Vector3.zero;
        int groundedWheels = 0;

        for (int i = 0; i < groundRayOrigins.Length; i++)
        {
            if (Physics.Raycast(groundRayOrigins[i].position, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
            {
                float compression = rayLength - hit.distance;
                Vector3 suspensionForce = Vector3.up * compression * floatingForce * suspensionForceMultiplier;
                rb.AddForceAtPosition(suspensionForce, groundRayOrigins[i].position, ForceMode.Acceleration);

                averageNormal += hit.normal;
                groundedWheels++;
                Debug.DrawRay(groundRayOrigins[i].position, Vector3.down * hit.distance, Color.green);
            }
            else
            {
                Debug.DrawRay(groundRayOrigins[i].position, Vector3.down * rayLength, Color.red);
            }
        }

        if (groundedWheels > 0)
        {
            averageNormal /= groundedWheels;
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    #endregion

    #region Terrain Alignment

    private void AlignToTerrain()
    {
        if (averageNormal != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    #endregion

    #region Suspension of Vehicle Motion

    private void SuspendVehicleMotion()
    {
        if (rb.velocity != Vector3.zero)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 2f);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 2f);
        }
    }

    #endregion

    #region Damping and Speed Control

    private void ApplyDamping()
    {
        rb.velocity *= 0.99f; // Adjust damping factor as needed
        rb.angularVelocity *= 0.95f;
    }

    #endregion
}
