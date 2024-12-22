using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake()
    {
        // Basic Singleton setup
        if (Instance == null) 
        {
            Instance = this;
        } 
        else 
        {
            Destroy(gameObject);
        }
    }

    // Example: Crystal Collection Event
    public event Action<int> OnCrystalCollected; 
    public void CrystalCollected(int amount)
    {
        OnCrystalCollected?.Invoke(amount);
    }

    // Example: Oxygen Low Event
    public event Action OnOxygenLow;
    public void TriggerOxygenLow()
    {
        OnOxygenLow?.Invoke();
    }
}
