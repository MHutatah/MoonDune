using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    [Header("Laser Settings")]
    public float laserRange = 10f;                // Max range
    public float laserDamagePerSecond = 10f;      // Damage per second
    public float drainRate = 5f;                  // Energy drained per second
    public float rechargeRate = 3f;               // Energy replenished per second
    public float maxLaserEnergy = 100f;           // Max laser energy
    public float overheatDuration = 5f;           // Overheat time in seconds

    [Header("Laser Growth Settings")]
    public float growthRate = 20f;                // Units/sec growth
    private float currentLaserRange = 0f;

    [Header("Internal State")]
    private float currentLaserEnergy;
    private bool isOverheating = false;
    private float overheatTimer = 0f;
    private bool isFiring = false;
    private bool isRetracting = false;

    // We only lock direction AFTER we stop firing
    private Vector3 lockedDirection = Vector3.forward;

    [Header("Laser Visuals")]
    public LineRenderer laserBeam;
    public Transform firePoint;

    void Start()
    {
        currentLaserEnergy = maxLaserEnergy;
        currentLaserRange = 0f;
        isRetracting = false;

        if (laserBeam)
        {
            laserBeam.positionCount = 2;
            laserBeam.enabled = false;
        }
    }

    void Update()
    {
        if (isOverheating)
        {
            HandleOverheat();
            return;
        }

        // Check if the user is pressing Fire1 and if we have energy
        bool canFire = Input.GetButton("Fire1") && currentLaserEnergy > 0f;

        if (canFire)
        {
            // If we were retracting, stop and reset
            if (isRetracting)
            {
                isRetracting = false;
                currentLaserRange = Mathf.Clamp(currentLaserRange, 0f, laserRange);
            }

            // Laser direction should follow the real-time forward
            Vector3 liveDirection = firePoint.forward;

            GrowLaser();
            FireLaserBeam(liveDirection);
        }
        else
        {
            // If we were previously firing, we now lock the direction for retraction
            if (isFiring && !isRetracting)
            {
                isRetracting = true;
                // Store the LAST real-time direction we had
                lockedDirection = firePoint.forward;
            }

            RetractLaser();
        }

        // Overheat check
        if (currentLaserEnergy <= 0f && !isOverheating)
        {
            StartOverheat();
        }

        isFiring = canFire; // Remember our firing state for next frame
    }

    private void GrowLaser()
    {
        // Drain energy
        currentLaserEnergy -= drainRate * Time.deltaTime;
        currentLaserEnergy = Mathf.Clamp(currentLaserEnergy, 0f, maxLaserEnergy);

        // Grow laser range
        currentLaserRange += growthRate * Time.deltaTime;
        currentLaserRange = Mathf.Clamp(currentLaserRange, 0f, laserRange);

        // Update UI
        UIManager.Instance.UpdateLaserBar(currentLaserEnergy, maxLaserEnergy);
    }

    private void FireLaserBeam(Vector3 currentDirection)
    {
        if (!laserBeam || !firePoint) return;

        if (!laserBeam.enabled)
            laserBeam.enabled = true;

        Vector3 startPos = firePoint.position;
        Vector3 endPos = startPos + currentDirection * currentLaserRange;

        // Raycast
        Ray ray = new Ray(startPos, currentDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, currentLaserRange))
        {
            endPos = hit.point;
            // If it's a crystal, apply damage
            LunarCrystal crystal = hit.collider.GetComponent<LunarCrystal>();
            if (crystal)
            {
                crystal.TakeDamage(laserDamagePerSecond * Time.deltaTime);
            }
        }

        laserBeam.SetPosition(0, startPos);
        laserBeam.SetPosition(1, endPos);
    }

    private void RetractLaser()
    {
        if (!isRetracting)
        {
            // If we are not in retraction mode, just update energy passively
            if (currentLaserEnergy < maxLaserEnergy)
            {
                currentLaserEnergy += rechargeRate * Time.deltaTime;
                currentLaserEnergy = Mathf.Clamp(currentLaserEnergy, 0f, maxLaserEnergy);
                UIManager.Instance.UpdateLaserBar(currentLaserEnergy, maxLaserEnergy);
            }

            // If the beam was on but range is not zero, we might keep it?
            // Or you can force-laserBeam.enabled = false when not firing
            if (currentLaserRange <= 0f && laserBeam.enabled)
                laserBeam.enabled = false;

            return; 
        }

        // We are in retraction mode
        if (currentLaserRange > 0f)
        {
            float retractSpeed = growthRate * 3f;
            currentLaserRange -= retractSpeed * Time.deltaTime;
            currentLaserRange = Mathf.Max(0f, currentLaserRange);
            UpdateRetractionVisual();
        }
        else
        {
            // fully retracted
            if (laserBeam && laserBeam.enabled)
                laserBeam.enabled = false;

            isRetracting = false;
        }

        // Passive recharge
        if (currentLaserEnergy < maxLaserEnergy)
        {
            currentLaserEnergy += rechargeRate * Time.deltaTime;
            currentLaserEnergy = Mathf.Clamp(currentLaserEnergy, 0f, maxLaserEnergy);
            UIManager.Instance.UpdateLaserBar(currentLaserEnergy, maxLaserEnergy);
        }
    }

    private void UpdateRetractionVisual()
    {
        if (!laserBeam || !firePoint) return;
        if (!laserBeam.enabled)
            laserBeam.enabled = true;

        // Use the locked direction for retraction
        Vector3 startPos = firePoint.position;
        Vector3 endPos = startPos + lockedDirection * currentLaserRange;

        Ray ray = new Ray(startPos, lockedDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, currentLaserRange))
        {
            endPos = hit.point;
        }
        Debug.Log("Retracting!");
        laserBeam.SetPosition(0, startPos);
        laserBeam.SetPosition(1, endPos);
    }

    private void StartOverheat()
    {
        isOverheating = true;
        overheatTimer = overheatDuration;
        Debug.Log("Player Laser is overheating!");

        // Disable beam immediately
        if (laserBeam && laserBeam.enabled)
            laserBeam.enabled = false;

        // Force range to zero
        currentLaserRange = 0f;
        isRetracting = false;
    }

    private void HandleOverheat()
    {
        overheatTimer -= Time.deltaTime;
        if (overheatTimer <= 0f)
        {
            isOverheating = false;
            Debug.Log("Player Laser is ready again.");
        }
    }

    public void ReplenishLaserEnergy(float amount)
    {
        currentLaserEnergy = Mathf.Min(currentLaserEnergy + amount, maxLaserEnergy);
        UIManager.Instance.UpdateLaserBar(currentLaserEnergy, maxLaserEnergy);
        Debug.Log("Laser Energy Replenished by: " + amount);
    }
}
