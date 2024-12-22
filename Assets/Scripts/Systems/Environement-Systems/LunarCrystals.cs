using UnityEngine;

public class LunarCrystal : MonoBehaviour
{
    public float crystalHealth = 50f;
    public int crystalValue = 5; // Number of crystals it yields

    public void TakeDamage(float damage)
    {
        crystalHealth -= damage;
        if (crystalHealth <= 0)
        {
            BreakCrystal();
        }
    }

    private void BreakCrystal()
    {
        // Trigger crystal collection logic, such as incrementing crystal count
        GameManager.Instance.CollectCrystal(crystalValue);

        // Play crystal break sound and effects
        // SoundManager.Instance.PlayCrystalBreak();

        // Optionally spawn particles or visual effects
        // Instantiate(crystalBreakEffect, transform.position, Quaternion.identity);

        // Destroy the crystal
        Destroy(gameObject);
        Debug.Log("Lunar Crystal broken and collected.");
    }
}
