using UnityEngine;
using UnityEngine.UI;

public class LunarCrystal : MonoBehaviour
{

    [Header("Crystal Settings")]
    public int crystalValue = 5; // Number of crystals it yields
    public float maxCrystalHealth = 50f;
    public Canvas rockCanvas;
    public Slider rockHealthSlider;
    public float crystalHealth;
    public float healthBarDurathion = 2f;
    private float healthBarTimer;
    
    [Header("Collectible Settings")]
    public GameObject collectiblePrefab;         // Assign the CollectiblePrefab here
    public int collectiblesPerDestruction = 5;   // Number of collectibles to spawn
    public float spawnRadius = 2f;               // Radius within which collectibles are spawned
    public float minForce = 5f;                   // Minimum force applied to collectibles
    public float maxForce = 15f;                  // Maximum force applied to collectibles
    void Start()
    {
        crystalHealth = maxCrystalHealth;
        rockCanvas.enabled = false;
    }

    public void TakeDamage(float damage, Transform player)
    {
        healthBarTimer = healthBarDurathion;
        rockCanvas.enabled = true;
        crystalHealth -= damage;
        if (crystalHealth <= 0)
        {
            BreakCrystal();
        }
        rockHealthSlider.value = crystalHealth/maxCrystalHealth;
        rockCanvas.transform.LookAt(player);
        //DisableRockHealth();
        //rockCanvas.enabled = false;
        //DisableRockHealth();
    }

    private void BreakCrystal()
    {
        // Trigger crystal collection logic, such as incrementing crystal count
        // GameManager.Instance.CollectCrystal(crystalValue);

        // Play crystal break sound and effects
        // SoundManager.Instance.PlayCrystalBreak();

        // Optionally spawn particles or visual effects
        // Instantiate(crystalBreakEffect, transform.position, Quaternion.identity);

        // Destroy the crystal
        Destroy(gameObject);
        SpawnCollectibles();
        Debug.Log("Lunar Crystal broken and collected.");
    }

    private void SpawnCollectibles()
    {
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
        }
    }

    void Update()
    {
        if (healthBarTimer > 0)
        {
            healthBarTimer -= Time.deltaTime;
            if (healthBarTimer <= 0)
            {
                rockCanvas.enabled = false;
            }
        }
    }
}

