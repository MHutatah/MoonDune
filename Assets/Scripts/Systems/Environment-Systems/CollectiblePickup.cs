// File: Assets/Scripts/Systems/Environment-Systems/CollectiblePickup.cs
using UnityEngine;
using Core.Interfaces;
using Core.Managers;
using Systems.VehicleSystems;
using Core.Utilities;

namespace Systems.EnvironmentSystems
{
    [RequireComponent(typeof(Collider))]
    public class CollectiblePickup : MonoBehaviour, IPickup, IPooledObject
    {
        [Header("Pickup Settings")]
        [SerializeField] private float gravityFuelAmount = 20f;
        [SerializeField] private bool destroyOnPickup = false;

        [Header("Feedback Settings")]
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private ParticleSystem pickupEffectPrefab;
        [SerializeField] private string poolTag = "Collectible";

        private bool isCollected = false;

        private void Awake()
        {
            Collider col = GetComponent<Collider>();
            if (!col.isTrigger)
            {
                col.isTrigger = true;
                Debug.LogWarning($"{gameObject.name}: Collider was not set as a trigger. Automatically setting it to trigger.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            if (other.CompareTag("Player") || other.CompareTag("Vehicle"))
            {
                OnPickup(other);
            }
        }

        public void OnPickup(Collider collector)
        {
            if (isCollected) return;

            IFuelReplenisher fuelReplenisher = collector.GetComponentInParent<IFuelReplenisher>();
            if (fuelReplenisher != null)
            {
                fuelReplenisher.ReplenishFuel(gravityFuelAmount);

                SoundManager.Instance?.PlayCrystalPickup();

                if (pickupEffectPrefab != null)
                {
                    Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                }

                UIManager.Instance?.ShowPickupMessage($"Gravity Fuel +{gravityFuelAmount}");

                EventManager.Instance?.CrystalCollected(1);

                isCollected = true;

                ObjectPooler.Instance?.ReturnToPool(poolTag, gameObject);

                Debug.Log($"{gameObject.name}: Pickup collected by {collector.name}.");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: IFuelReplenisher not found on collector.");
            }
        }

        public float GetPickupAmount()
        {
            return gravityFuelAmount;
        }

        public void OnObjectSpawn()
        {
            isCollected = false;
            gameObject.SetActive(true);
        }

        public void OnObjectReturn()
        {
            isCollected = false;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(false);
        }
    }
}
