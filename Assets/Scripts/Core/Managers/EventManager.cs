using System;
using UnityEngine;

namespace Core.Managers
{
    /// <summary>
    /// Manages game-wide events to facilitate communication between different systems.
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        #region Unity Callbacks

        private void Awake()
        {
            // Singleton setup
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

        #endregion

        #region Events

        // Crystal Collection Event
        public event Action<int> OnCrystalCollected;

        public void CrystalCollected(int amount)
        {
            OnCrystalCollected?.Invoke(amount);
        }

        // Oxygen Low Event
        public event Action OnOxygenLow;

        public void TriggerOxygenLow()
        {
            OnOxygenLow?.Invoke();
        }

        // Player Entered Vehicle Zone Event
        public event Action OnPlayerEnteredVehicleZone;

        public void TriggerPlayerEnterVehicleZone()
        {
            OnPlayerEnteredVehicleZone?.Invoke();
        }

        // Player Exited Vehicle Zone Event
        public event Action OnPlayerExitedVehicleZone;

        public void TriggerPlayerExitVehicleZone()
        {
            OnPlayerExitedVehicleZone?.Invoke();
        }

        // Player Entered Vehicle Event
        public event Action OnPlayerEnteredVehicle;

        public void TriggerPlayerEnteredVehicle()
        {
            OnPlayerEnteredVehicle?.Invoke();
        }

        // Player Exited Vehicle Event
        public event Action OnPlayerExitedVehicle;

        public void TriggerPlayerExitedVehicle()
        {
            OnPlayerExitedVehicle?.Invoke();
        }

        public event Action OnCrystalDestroyed;

        public void CrystalDestroyed()
        {
            OnCrystalDestroyed?.Invoke();
        }
        #endregion
    }
}
