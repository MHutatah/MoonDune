using UnityEngine;

/// <summary>
/// Manages all game sounds, ensuring organized and centralized audio control.
/// </summary>
namespace Core.Managers
{

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Clips")]
        [SerializeField] private AudioClip laserShotClip;
        [SerializeField] private AudioClip crystalPickupClip;
        [SerializeField] private AudioClip lowOxygenClip;
        [SerializeField] private AudioClip crystalBreakClip;
        // ... More clips as needed

        private AudioSource audioSource;

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

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("SoundManager: No AudioSource component found.");
            }
        }

        /// <summary>
        /// Plays the laser shot sound effect.
        /// </summary>
        public void PlayLaserShot()
        {
            if (laserShotClip != null && audioSource != null)
                audioSource.PlayOneShot(laserShotClip);
            else
                Debug.LogWarning("SoundManager: LaserShotClip or AudioSource is not assigned.");
        }

        /// <summary>
        /// Plays the crystal pickup sound effect.
        /// </summary>
        public void PlayCrystalPickup()
        {
            if (crystalPickupClip != null && audioSource != null)
                audioSource.PlayOneShot(crystalPickupClip);
            else
                Debug.LogWarning("SoundManager: CrystalPickupClip or AudioSource is not assigned.");
        }

        /// <summary>
        /// Plays the low oxygen alert sound effect.
        /// </summary>
        public void PlayLowOxygenAlert()
        {
            if (lowOxygenClip != null && audioSource != null)
                audioSource.PlayOneShot(lowOxygenClip);
            else
                Debug.LogWarning("SoundManager: LowOxygenClip or AudioSource is not assigned.");
        }

        /// <summary>
        /// Plays the crystal break sound effect.
        /// </summary>
        public void PlayCrystalBreak()
        {
            if (crystalBreakClip != null && audioSource != null)
                audioSource.PlayOneShot(crystalBreakClip);
            else
                Debug.LogWarning("SoundManager: CrystalBreakClip or AudioSource is not assigned.");
        }

        // Add methods for additional audio clips as needed
    }
}