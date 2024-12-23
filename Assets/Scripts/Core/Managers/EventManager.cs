using System;
using UnityEngine;

/// <summary>
/// Manages game-wide events to facilitate communication between different systems.
/// </summary>
namespace Core.Managers
{

    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        protected virtual void Awake()
        {
            // Enhanced Singleton setup
            if (Instance == null) 
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Persist across scenes if needed
            } 
            else 
            {
                Destroy(gameObject);
            }
        }

        // Crystal Collection Event
        public event Action<int> EventOnCrystalCollected; 
        public void CrystalCollected(int amount)
        {
            EventOnCrystalCollected?.Invoke(amount);
        }

        // Oxygen Low Event
        public event Action EventOnOxygenLow;
        public void TriggerOxygenLow()
        {
            EventOnOxygenLow?.Invoke();
        }

        // Player Entered Vehicle Zone Event
        public event Action EventOnPlayerEnteredVehicleZone;
        public void TriggerPlayerEnterVehicleZone()
        {
            EventOnPlayerEnteredVehicleZone?.Invoke();
        }

        // Player Exited Vehicle Zone Event
        public event Action EventOnPlayerExitedVehicleZone;
        public void TriggerPlayerExitVehicleZone()
        {
            EventOnPlayerExitedVehicleZone?.Invoke();
        }

        // Player Entered Vehicle Event
        public event Action EventOnPlayerEnteredVehicle;
        public void TriggerPlayerEnteredVehicle()
        {
            EventOnPlayerEnteredVehicle?.Invoke();
        }

        // Player Exited Vehicle Event
        public event Action EventOnPlayerExitedVehicle;
        public void TriggerPlayerExitedVehicle()
        {
            EventOnPlayerExitedVehicle?.Invoke();
        }
    }
}