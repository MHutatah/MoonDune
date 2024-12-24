using UnityEngine;
using Core.Managers;
using Systems.EnvironmentSystems;
using Core.Utilities;
using UnityEngine.UI;
using Core.Interfaces;

namespace Systems.EnvironmentSystems
{
    [RequireComponent(typeof(Collider))]
    public class LunarCrystal : MonoBehaviour, IDamageable
    {
        [Header("Crystal Settings")]
        [SerializeField] private int crystalValue = 5;
        [SerializeField] private float maxHealth = 50f;

        [Header("Health Bar Settings")]
        [SerializeField] private Canvas healthCanvas;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private float healthBarDuration = 2f;

        [Header("Collectible Settings")]
        [SerializeField] private string collectiblePoolTag = "Collectible";
        [SerializeField] private int collectiblesPerDestruction = 5;
        [SerializeField] private float spawnRadius = 2f;
        [SerializeField] private float minForce = 5f;
        [SerializeField] private float maxForce = 15f;

        private float currentHealth;
        private float healthBarTimer;
        private bool isDestroyed = false;
        private Transform playerTransform;                               // Reference to the player for health bar rotation

        private void Start()
        {
            currentHealth = maxHealth;

            if (healthCanvas != null)
            {
                healthCanvas.enabled = false;
            }
            else
            {
                Debug.LogWarning("LunarCrystal: HealthCanvas is not assigned.");
            }

            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
            else
            {
                Debug.LogWarning("LunarCrystal: HealthSlider is not assigned.");
            }
        }

        public void Update()
        {
            HandleHealthBarVisibility();
            RotateHealthBarTowardsPlayer();
        }

        public void TakeDamage(float amount, Transform source)
        {
            if (isDestroyed) return;
            playerTransform = source;
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }

            if (healthCanvas != null)
            {
                healthCanvas.enabled = true;
                healthBarTimer = healthBarDuration;
            }

            if (currentHealth <= 0f)
            {
                BreakCrystal();
            }
        }

        private void BreakCrystal()
        {
            if (isDestroyed) return;
            isDestroyed = true;

            EventManager.Instance?.CrystalDestroyed();

            SoundManager.Instance?.PlayCrystalBreak();

            SpawnCollectibles();

            Destroy(gameObject);
            Debug.Log($"{gameObject.name}: Lunar Crystal broken and collected.");
        }

        private void SpawnCollectibles()
        {
            if (string.IsNullOrEmpty(collectiblePoolTag))
            {
                Debug.LogError("LunarCrystal: Collectible Pool Tag is not set.");
                return;
            }

            for (int i = 0; i < collectiblesPerDestruction; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere.normalized;
                Vector3 spawnPosition = transform.position + randomDirection * Random.Range(0f, spawnRadius);

                GameObject collectible = ObjectPooler.Instance?.SpawnFromPool(collectiblePoolTag, spawnPosition, Quaternion.identity);
                if (collectible != null)
                {
                    Rigidbody rb = collectible.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        float forceMagnitude = Random.Range(minForce, maxForce);
                        Vector3 forceDirection = randomDirection;
                        rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
                    }
                    else
                    {
                        Debug.LogWarning("LunarCrystal: CollectiblePrefab does not have a Rigidbody component.");
                    }
                }
                else
                {
                    Debug.LogWarning($"LunarCrystal: Failed to spawn collectible from pool '{collectiblePoolTag}'.");
                }
            }
        }

        #region Health Bar Management

        /// <summary>
        /// Manages the visibility of the health bar based on the timer.
        /// </summary>
        private void HandleHealthBarVisibility()
        {
            if (healthCanvas != null && healthCanvas.enabled)
            {
                healthBarTimer -= Time.deltaTime;
                if (healthBarTimer <= 0f)
                {
                    healthCanvas.enabled = false;
                }
            }
        }

        /// <summary>
        /// Rotates the health bar to face the player.
        /// </summary>
        private void RotateHealthBarTowardsPlayer()
        {
            if (healthCanvas != null && playerTransform != null)
            {
                healthCanvas.transform.rotation = Quaternion.LookRotation(healthCanvas.transform.position - playerTransform.position);
            }
        }

        #endregion
    }
}
