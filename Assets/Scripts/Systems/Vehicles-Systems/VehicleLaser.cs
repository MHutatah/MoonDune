using UnityEngine;

public class VehicleLaser : MonoBehaviour
{
    [Header("Laser Settings")]
    public float laserRange = 15f;                 
    public float laserDamagePerSecond = 15f;       
    public float drainRate = 7f;                   
    public float rechargeRate = 4f;               
    public float maxLaserEnergy = 150f;            
    public float overheatDuration = 6f;            

    [Header("Laser Growth Settings")]
    public float growthRate = 25f;                 
    private float currentLaserRange = 0f;

    [Header("Internal State")]
    private float currentLaserEnergy;
    private bool isOverheating = false;
    private float overheatTimer = 0f;
    private bool isFiring = false;
    private bool isRetracting = false;

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

        bool canFire = Input.GetButton("FireVehicleLaser") && currentLaserEnergy > 0f;

        if (canFire)
        {
            if (isRetracting)
            {
                isRetracting = false;
                currentLaserRange = Mathf.Clamp(currentLaserRange, 0f, laserRange);
            }

            Vector3 liveDirection = firePoint.forward;

            GrowLaser();
            FireLaserBeam(liveDirection);
        }
        else
        {
            if (isFiring && !isRetracting)
            {
                isRetracting = true;
                lockedDirection = firePoint.forward;
            }

            RetractLaser();
        }

        if (currentLaserEnergy <= 0f && !isOverheating)
        {
            StartOverheat();
        }

        isFiring = canFire;
    }

    private void GrowLaser()
    {
        currentLaserEnergy -= drainRate * Time.deltaTime;
        currentLaserEnergy = Mathf.Clamp(currentLaserEnergy, 0f, maxLaserEnergy);

        currentLaserRange += growthRate * Time.deltaTime;
        currentLaserRange = Mathf.Clamp(currentLaserRange, 0f, laserRange);

        UIManager.Instance.UpdateLaserBar(currentLaserEnergy, maxLaserEnergy);
    }

    private void FireLaserBeam(Vector3 currentDirection)
    {
        if (!laserBeam || !firePoint) return;

        if (!laserBeam.enabled)
            laserBeam.enabled = true;

        Vector3 startPos = firePoint.position;
        Vector3 endPos = startPos + currentDirection * currentLaserRange;

        Ray ray = new Ray(startPos, currentDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, currentLaserRange))
        {
            endPos = hit.point;
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
            // Passive recharge
            if (currentLaserEnergy < maxLaserEnergy)
            {
                currentLaserEnergy += rechargeRate * Time.deltaTime;
                currentLaserEnergy = Mathf.Clamp(currentLaserEnergy, 0f, maxLaserEnergy);
                UIManager.Instance.UpdateLaserBar(currentLaserEnergy, maxLaserEnergy);
            }

            if (currentLaserRange <= 0f && laserBeam.enabled)
                laserBeam.enabled = false;

            return;
        }

        // Retract mode
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

        Vector3 startPos = firePoint.position;
        Vector3 endPos = startPos + lockedDirection * currentLaserRange;

        Ray ray = new Ray(startPos, lockedDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, currentLaserRange))
        {
            endPos = hit.point;
        }

        laserBeam.SetPosition(0, startPos);
        laserBeam.SetPosition(1, endPos);
    }

    private void StartOverheat()
    {
        isOverheating = true;
        overheatTimer = overheatDuration;
        Debug.Log("Vehicle Laser is overheating!");

        if (laserBeam && laserBeam.enabled)
            laserBeam.enabled = false;

        currentLaserRange = 0f;
        isRetracting = false;
    }

    private void HandleOverheat()
    {
        overheatTimer -= Time.deltaTime;
        if (overheatTimer <= 0f)
        {
            isOverheating = false;
            Debug.Log("Vehicle Laser is ready again.");
        }
    }

    public void ReplenishLaserEnergy(float amount)
    {
        currentLaserEnergy = Mathf.Min(currentLaserEnergy + amount, maxLaserEnergy);
        UIManager.Instance.UpdateLaserBar(currentLaserEnergy, maxLaserEnergy);
        Debug.Log("Vehicle Laser Energy Replenished by: " + amount);
    }
}
