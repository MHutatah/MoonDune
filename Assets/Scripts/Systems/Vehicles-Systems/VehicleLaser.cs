using UnityEngine;
using Core.Managers;
using Systems.EnvironmentSystems;

/// <summary>
/// Handles the vehicle's laser firing mechanics, including energy management and overheating.
/// </summary>
namespace Systems.VehicleSystems
{
    public class VehicleLaser : MonoBehaviour
    {
        #region Inspector Variables

        [Header("Laser Settings")]
        [SerializeField] private float laserRange = 15f;                  // Maximum range of the vehicle laser
        [SerializeField] private float laserDamagePerSecond = 15f;        // Damage dealt per second to crystals
        [SerializeField] private float drainRate = 7f;                    // Energy drained per second while firing
        [SerializeField] private float rechargeRate = 4f;                 // Energy replenished per second when not firing
        [SerializeField] private float maxLaserEnergy = 150f;             // Maximum laser energy capacity
        [SerializeField] private float overheatDuration = 6f;             // Duration (in seconds) laser is disabled after overheating

        [Header("Laser Growth Settings")]
        [SerializeField] private float growthRate = 25f;                  // Units per second the laser grows

        [Header("Laser Visuals")]
        [SerializeField] private LineRenderer laserBeam;                  // Reference to the Line Renderer component
        [SerializeField] private Transform firePoint;                     // The origin point of the vehicle's laser

        #endregion

        #region Private Variables

        private float currentLaserRange = 0f;                            // Current dynamic range of the laser
        private float currentLaserEnergy;                                // Current laser energy
        private bool isOverheating = false;                              // Indicates if the laser is in overheating state
        private float overheatTimer = 0f;                                // Timer for overheating duration
        private bool isFiring = false;                                   // Indicates if the vehicle is currently firing

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            InitializeLaser();
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
            if (isOverheating)
            {
                HandleOverheat();
                return;
            }

            HandleFiring();

            if (!isFiring)
            {
                RechargeLaser();
            }

            CheckOverheatCondition();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes laser properties and visuals.
        /// </summary>
        private void InitializeLaser()
        {
            currentLaserEnergy = maxLaserEnergy;
            currentLaserRange = 0f;

            if (laserBeam != null)
            {
                laserBeam.positionCount = 2;
                laserBeam.enabled = false;
            }
            else
            {
                Debug.LogWarning("VehicleLaser: LaserBeam is not assigned in the Inspector.");
            }
        }

        #endregion

        #region Firing Logic

        /// <summary>
        /// Handles the firing logic based on vehicle input.
        /// </summary>
        private void HandleFiring()
        {
            bool canFire = Input.GetButton("Fire1") && currentLaserEnergy > 0f;

            if (canFire)
            {
                isFiring = true;
                GrowLaser();
                FireLaserBeam(firePoint.forward);
            }
            else
            {
                isFiring = false;
                StopFiring();
            }
        }

        /// <summary>
        /// Increases the laser's range and drains energy accordingly.
        /// </summary>
        private void GrowLaser()
        {
            // Drain energy
            currentLaserEnergy -= drainRate * Time.deltaTime;
            currentLaserEnergy = Mathf.Clamp(currentLaserEnergy, 0f, maxLaserEnergy);

            // Grow laser range
            currentLaserRange += growthRate * Time.deltaTime;
            currentLaserRange = Mathf.Clamp(currentLaserRange, 0f, laserRange);

            // Update UI
            UIManager.Instance?.UpdateVehicleLaserBar(currentLaserEnergy, maxLaserEnergy);
        }

        /// <summary>
        /// Renders the laser beam in the current firing direction.
        /// </summary>
        /// <param name="currentDirection">The current direction of the laser.</param>
        private void FireLaserBeam(Vector3 currentDirection)
        {
            if (laserBeam == null || firePoint == null) return;

            if (!laserBeam.enabled)
                laserBeam.enabled = true;

            Vector3 startPos = firePoint.position;
            Vector3 endPos = startPos + currentDirection * currentLaserRange;

            // Raycast to detect hit
            Ray ray = new Ray(startPos, currentDirection);
            if (Physics.Raycast(ray, out RaycastHit hit, currentLaserRange))
            {
                endPos = hit.point;

                // Apply damage to the hit crystal
                LunarCrystal crystal = hit.collider.GetComponent<LunarCrystal>();
                if (crystal != null)
                {
                    crystal.TakeDamage(laserDamagePerSecond * Time.deltaTime, transform);
                }
            }

            // Update Line Renderer positions
            laserBeam.SetPosition(0, startPos);
            laserBeam.SetPosition(1, endPos);
        }

        /// <summary>
        /// Stops firing by disabling the laser beam and resetting its range.
        /// </summary>
        private void StopFiring()
        {
            if (laserBeam != null && laserBeam.enabled)
                laserBeam.enabled = false;

            currentLaserRange = 0f;
        }

        #endregion

        #region Energy Management

        /// <summary>
        /// Recharges the laser's energy when not firing.
        /// </summary>
        private void RechargeLaser()
        {
            if (currentLaserEnergy < maxLaserEnergy)
            {
                currentLaserEnergy += rechargeRate * Time.deltaTime;
                currentLaserEnergy = Mathf.Clamp(currentLaserEnergy, 0f, maxLaserEnergy);
                UIManager.Instance?.UpdateVehicleLaserBar(currentLaserEnergy, maxLaserEnergy);
            }
        }

        #endregion

        #region Overheat Handling

        /// <summary>
        /// Checks if the laser should enter the overheating state.
        /// </summary>
        private void CheckOverheatCondition()
        {
            if (currentLaserEnergy <= 0f && !isOverheating)
            {
                StartOverheat();
            }
        }

        /// <summary>
        /// Initiates the overheating state.
        /// </summary>
        private void StartOverheat()
        {
            isOverheating = true;
            overheatTimer = overheatDuration;
            Debug.Log("Vehicle Laser is overheating!");

            StopFiring();
        }

        /// <summary>
        /// Manages the overheating timer and resets the laser once cooldown is complete.
        /// </summary>
        private void HandleOverheat()
        {
            overheatTimer -= Time.deltaTime;
            if (overheatTimer <= 0f)
            {
                isOverheating = false;
                Debug.Log("Vehicle Laser is ready again.");
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles low oxygen events to modify laser functionality.
        /// </summary>
        private void HandleOxygenLow()
        {
            // Example: Reduce laser damage temporarily
            laserDamagePerSecond *= 0.8f;
            Debug.LogWarning("VehicleLaser: Low oxygen detected! Laser damage reduced.");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Replenishes the laser's energy. Can be called externally.
        /// </summary>
        /// <param name="amount">Amount of energy to add.</param>
        public void ReplenishLaserEnergy(float amount)
        {
            currentLaserEnergy = Mathf.Min(currentLaserEnergy + amount, maxLaserEnergy);
            UIManager.Instance?.UpdateVehicleLaserBar(currentLaserEnergy, maxLaserEnergy);
            Debug.Log($"Vehicle Laser Energy Replenished by: {amount}");
        }

        #endregion
    }
}