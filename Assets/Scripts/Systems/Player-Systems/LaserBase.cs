using UnityEngine;
using Core.Managers;
using Core.Interfaces;

namespace Systems.PlayerSystems
{
    /// <summary>
    /// Base class for laser functionalities shared between player and vehicle.
    /// </summary>
    public abstract class LaserBase : MonoBehaviour, IFireable
    {
        #region IFireable Implementation

        public bool IsFiring { get; private set; } = false;

        #endregion

        #region Protected Variables

        [SerializeField] protected float laserRange = 15f;                  // Maximum range of the laser
        [SerializeField] protected float laserDamagePerSecond = 15f;        // Damage dealt per second
        [SerializeField] protected float drainRate = 7f;                    // Energy drained per second while firing
        [SerializeField] protected float rechargeRate = 4f;                 // Energy replenished per second when not firing
        [SerializeField] protected float maxLaserEnergy = 150f;             // Maximum laser energy capacity
        [SerializeField] protected float overheatDuration = 6f;             // Duration (in seconds) laser is disabled after overheating
        [SerializeField] protected float growthRate = 25f;                  // Units per second the laser grows

        [Header("Laser Visuals")]
        [SerializeField] protected LineRenderer laserBeam;                  // Reference to the Line Renderer component
        [SerializeField] protected Transform firePoint;                     // The origin point of the laser

        #endregion

        #region Protected Variables for Energy Management

        protected float currentLaserRange = 0f;                            // Current dynamic range of the laser
        protected float currentLaserEnergy;                                // Current laser energy
        protected bool isOverheating = false;                              // Indicates if the laser is in overheating state
        protected float overheatTimer = 0f;                                // Timer for overheating duration

        #endregion

        #region Unity Callbacks

        protected virtual void Start()
        {
            InitializeLaser();
        }

        protected virtual void Update()
        {
            if (isOverheating)
            {
                HandleOverheat();
                return;
            }

            if (IsFiring)
            {
                FireLaser();
            }
            
            else
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
        protected void InitializeLaser()
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
                Debug.LogWarning("LaserBase: LaserBeam is not assigned in the Inspector.");
            }

            if (firePoint == null)
            {
                firePoint = transform;
                Debug.LogWarning("LaserBase: FirePoint not assigned. Using laser's transform.");
            }
        }

        #endregion

        #region Firing Logic

        /// <summary>
        /// Abstract method to handle firing mechanics. Implemented in derived classes.
        /// </summary>
        public abstract void Fire(Vector3 direction);

        /// <summary>
        /// Abstract method to handle stopping firing. Implemented in derived classes.
        /// </summary>
        //public abstract void StopFiring();

        protected void SetIsFiring(bool value)

        {

            IsFiring = value;

        }
        
        /// <summary>
        /// Handles the laser firing logic.
        /// </summary>
        protected virtual void FireLaser()
        {
            laserBeam.enabled = true;
            // Drain energy
            currentLaserEnergy -= drainRate * Time.deltaTime;
            currentLaserEnergy = Mathf.Clamp(currentLaserEnergy, 0f, maxLaserEnergy);

            // Grow laser range
            currentLaserRange += growthRate * Time.deltaTime;
            currentLaserRange = Mathf.Clamp(currentLaserRange, 0f, laserRange);
        }

        /// <summary>
        /// Stops firing by disabling the laser beam and resetting its range.
        /// </summary>
        public virtual void StopFiring()
        {
            if (laserBeam != null && laserBeam.enabled)
                laserBeam.enabled = false;

            currentLaserRange = 0f;
            IsFiring = false;
        }

        #endregion

        #region Energy Management

        /// <summary>
        /// Recharges the laser's energy when not firing.
        /// </summary>
        protected virtual void RechargeLaser()
        {
            if (currentLaserEnergy < maxLaserEnergy)
            {
                currentLaserEnergy += rechargeRate * Time.deltaTime;
                currentLaserEnergy = Mathf.Clamp(currentLaserEnergy, 0f, maxLaserEnergy);
                UpdateHUD();
            }
        }

        #endregion

        #region Overheat Handling

        /// <summary>
        /// Checks if the laser should enter the overheating state.
        /// </summary>
        protected void CheckOverheatCondition()
        {
            if (currentLaserEnergy <= 0f && !isOverheating)
            {
                StartOverheat();
            }
        }

        /// <summary>
        /// Initiates the overheating state.
        /// </summary>
        protected void StartOverheat()
        {
            isOverheating = true;
            overheatTimer = overheatDuration;
            Debug.Log($"{this.GetType().Name}: Laser is overheating!");

            StopFiring();
        }

        /// <summary>
        /// Manages the overheating timer and resets the laser once cooldown is complete.
        /// </summary>
        protected void HandleOverheat()
        {
            overheatTimer -= Time.deltaTime;
            if (overheatTimer <= 0f)
            {
                isOverheating = false;
                Debug.Log($"{this.GetType().Name}: Laser is ready again.");
            }
        }

        #endregion

        #region HUD Update

        /// <summary>
        /// Updates the HUD with the current laser energy.
        /// </summary>
        protected virtual void UpdateHUD()
        {
            // Implement in derived classes if specific HUD updates are needed
        }

        #endregion
    }
}
