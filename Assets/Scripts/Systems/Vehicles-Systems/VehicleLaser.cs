using UnityEngine;
using Core.Managers;
using Core.Interfaces;
using Systems.EnvironmentSystems;
using Systems.PlayerSystems;
using Systems.UISystems;

namespace Systems.VehicleSystems
{
    /// <summary>
    /// Handles the vehicle's laser firing mechanics, including energy management and overheating.
    /// Inherits from LaserBase for shared functionalities.
    /// </summary>
    public class VehicleLaser : LaserBase
    {
        #region Inspector Variables

        [Header("HUD Reference")]
        [SerializeField] private HUDController hudController;

        [Header("Laser Impact Settings")]
        [SerializeField] private LayerMask laserImpactLayers; // Define which layers the laser can affect

        #endregion

        #region Unity Callbacks

        protected override void Update()
        {
            base.Update();

            if (IsFiring)
            {
                FireLaserBeam();
            }
        }

        #endregion

        #region IFireable Implementation

        public override void Fire(Vector3 direction)
        {
            if (isOverheating || currentLaserEnergy <= 0f)
                return;

            SetIsFiring(true);
            SoundManager.Instance?.PlayLaserShot();
        }

        public override void StopFiring()
        {
            base.StopFiring();
            SoundManager.Instance?.StopLaserShot();
        }

        #endregion

        #region Laser Mechanics

        /// <summary>
        /// Renders the laser beam and applies damage to hit targets.
        /// </summary>
        private void FireLaserBeam()
        {
            if (laserBeam == null || firePoint == null) return;

            Vector3 startPos = firePoint.position;
            Vector3 endPos = startPos + firePoint.forward * currentLaserRange;

            Ray ray = new Ray(startPos, firePoint.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, currentLaserRange, laserImpactLayers))
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

        #endregion

        #region HUD Update

        protected override void UpdateHUD()
        {
            base.UpdateHUD();
            if (hudController != null)
            {
                hudController.UpdateVehicleLaserBar(currentLaserEnergy, maxLaserEnergy);
            }
        }

        #endregion
    }
}
