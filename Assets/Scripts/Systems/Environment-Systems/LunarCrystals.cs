using UnityEngine;
using Core.Interfaces;
using UnityEngine.UI;
using Core.Managers;

namespace Systems.EnvironmentSystems
{
    /// <summary>
    /// Represents a lunar crystal that can be damaged and destroyed, spawning collectibles upon destruction.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class LunarCrystal : MonoBehaviour, IDamageable
    {
        #region Inspector Variables

        [Header("Crystal Settings")]
        [SerializeField] private int crystalValue = 5;                  // Number of crystals it yields
        [SerializeField] private float maxCrystalHealth = 50f;
        [SerializeField] private Canvas rockCanvas;
        [SerializeField] private Slider rockHealthSlider;
        [SerializeField] private float healthBarDuration = 2f;

        [Header("Collectible Settings")]
        [SerializeField] private GameObject collectiblePrefab;          // Assign the CollectiblePrefab here
        [SerializeField] private int collectiblesPerDestruction = 5;    // Number of collectibles to spawn
        [SerializeField] private float spawnRadius = 2f;                // Radius within which collectibles are spawned
        [SerializeField] private float minForce = 5f;                   // Minimum force applied to collectibles
        [SerializeField] private float maxForce = 15f;                  // Maximum force applied to collectibles

        #endregion

        #region Private Variables

        private float crystalHealth;                                      // Current health of the crystal
        private float healthBarTimer;                                     // Timer to hide the health bar
        private bool isDestroyed = false;                                 // Flag to prevent multiple destructions

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            crystalHealth = maxCrystalHealth;
            rockCanvas.enabled = false;

            // Initialize health slider
            if (rockHealthSlider != null)
            {
                rockHealthSlider.maxValue = maxCrystalHealth;
                rockHealthSlider.value = crystalHealth;
            }
            else
            {
                Debug.LogWarning("LunarCrystal: rockHealthSlider is not assigned.");
            }
        }

        private void Update()
        {
            HandleHealthBarVisibility();
        }

        #endregion

        #region IDamageable Implementation

        /// <summary>
        /// Applies damage to the crystal.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        /// <param name="source">Transform of the source dealing damage.</param>
        public void TakeDamage(float damage, Transform source)
        {
            if (isDestroyed) return;

            // Update health
            crystalHealth -= damage;
            crystalHealth = Mathf.Clamp(crystalHealth, 0f, maxCrystalHealth);

            // Update health slider
            if (rockHealthSlider != null)
            {
                rockHealthSlider.value = crystalHealth;
            }

            // Show health bar
            rockCanvas.enabled = true;
            healthBarTimer = healthBarDuration;

            // Rotate health bar to face the player
            if (rockCanvas != null && source != null)
            {
                rockCanvas.transform.LookAt(source);
            }

            // Check for destruction
            if (crystalHealth <= 0f)
            {
                BreakCrystal();
            }
        }

        #endregion

        #region Crystal Destruction

        /// <summary>
        /// Handles the destruction of the crystal.
        /// </summary>
        private void BreakCrystal()
        {
            if (isDestroyed) return;
            isDestroyed = true;

            // Trigger crystal collection logic
            GameManager.Instance?.CrystalCollected(crystalValue);

            // Play crystal break sound and effects
            SoundManager.Instance?.PlayCrystalBreak();

            // Spawn collectibles
            SpawnCollectibles();

            // Destroy the crystal
            Destroy(gameObject);
            Debug.Log("Lunar Crystal broken and collected.");
        }

        /// <summary>
        /// Spawns collectibles around the destroyed crystal.
        /// </summary>
        private void SpawnCollectibles()
        {
            if (collectiblePrefab == null)
            {
                Debug.LogError("LunarCrystal: collectiblePrefab is not assigned.");
                return;
            }

            for (int i = 0; i < collectiblesPerDestruction; i++)
            {
                // Random position within spawnRadius
                Vector3 randomDirection = Random.insideUnitSphere.normalized;
                Vector3 spawnPosition = transform.position + randomDirection * Random.Range(0f, spawnRadius);

                // Instantiate collectible
                GameObject collectible = Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);

                // Apply force
                Rigidbody rb = collectible.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Random force magnitude
                    float forceMagnitude = Random.Range(minForce, maxForce);

                    // Apply force in random direction within a sphere
                    Vector3 forceDirection = Random.onUnitSphere;
                    rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
                }
                else
                {
                    Debug.LogWarning("LunarCrystal: CollectiblePrefab does not have a Rigidbody component.");
                }
            }
        }

        #endregion

        #region Health Bar Management

        /// <summary>
        /// Manages the visibility of the health bar based on the timer.
        /// </summary>
        private void HandleHealthBarVisibility()
        {
            if (rockCanvas.enabled)
            {
                healthBarTimer -= Time.deltaTime;
                if (healthBarTimer <= 0f)
                {
                    rockCanvas.enabled = false;
                }
            }
        }

        #endregion
    }
}
