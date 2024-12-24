using UnityEngine;
using Core.Managers;
using Core.Interfaces;
using Systems.EnvironmentSystems;
using Systems.UISystems;

namespace Systems.PlayerSystems
{
    /// <summary>
    /// Handles the player's laser firing mechanics, including energy management and overheating.
    /// Inherits from LaserBase for shared functionalities.
    /// </summary>
    public class PlayerLaser : LaserBase
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

            if (base.IsFiring)
            {
                Debug.Log("IsFiring is true");
                FireLaserBeam();
                UpdateHUD();
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
            Debug.Log("Laser Fired");
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

            Debug.Log("FireLaserBeam()");

            Vector3 startPos = firePoint.position;
            Vector3 endPos = startPos + firePoint.forward * currentLaserRange;

            Ray ray = new Ray(startPos, firePoint.forward);
            Debug.Log(ray);
            if (Physics.Raycast(ray, out RaycastHit hit, currentLaserRange, laserImpactLayers))
            {
                endPos = hit.point;
                Debug.Log($"Hit: {hit.collider.name}");

                // Apply damage to the hit object if it implements IDamageable
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(laserDamagePerSecond * Time.deltaTime, transform);
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
                hudController.UpdatePlayerLaserBar(currentLaserEnergy, maxLaserEnergy);
            }
        }
        #endregion
    }
}
