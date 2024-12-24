using UnityEngine;

namespace Core.Managers
{
    /// <summary>
    /// Manages all game sounds, ensuring organized and centralized audio control.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        #region Inspector Variables

        [Header("Audio Clips")]
        [SerializeField] private AudioClip laserShotClip;
        [SerializeField] private AudioClip crystalPickupClip;
        [SerializeField] private AudioClip lowOxygenClip;
        [SerializeField] private AudioClip crystalBreakClip;
        [SerializeField] private AudioClip backgroundMusicClip;
        [SerializeField] private AudioClip lowGravityFuelAlertClip; // New AudioClip for low gravity fuel alert

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;

        #endregion

        #region Private Variables

        private AudioSource sfxAudioSource;
        private AudioSource musicAudioSource;
        private AudioSource laserAudioSource; // New AudioSource for laser sounds

        #endregion

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
                return;
            }

            // Initialize AudioSources
            InitializeAudioSources();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes additional AudioSources for SFX, Music, and Laser.
        /// </summary>
        private void InitializeAudioSources()
        {
            // Existing SFX AudioSource
            sfxAudioSource = this.gameObject.AddComponent<AudioSource>();
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.volume = sfxVolume;

            // Existing Music AudioSource
            musicAudioSource = this.gameObject.AddComponent<AudioSource>();
            musicAudioSource.playOnAwake = false;
            musicAudioSource.loop = true;
            musicAudioSource.volume = musicVolume;

            // New Laser AudioSource
            laserAudioSource = this.gameObject.AddComponent<AudioSource>();
            laserAudioSource.playOnAwake = false;
            laserAudioSource.loop = true;
            laserAudioSource.volume = sfxVolume; // Could have separate volume settings if desired

            // Play Background Music if assigned
            if (backgroundMusicClip != null)
            {
                musicAudioSource.clip = backgroundMusicClip;
                musicAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("SoundManager: Background music clip is not assigned.");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Plays the laser shot sound effect. Starts looping the laser sound.
        /// </summary>
        public void PlayLaserShot()
        {
            if (laserShotClip != null && laserAudioSource != null)
            {
                if (!laserAudioSource.isPlaying)
                {
                    laserAudioSource.clip = laserShotClip;
                    laserAudioSource.Play();
                }
            }
            else
            {
                Debug.LogWarning("SoundManager: LaserShotClip or laserAudioSource is not assigned.");
            }
        }

        /// <summary>
        /// Stops the laser shot sound effect.
        /// </summary>
        public void StopLaserShot()
        {
            if (laserAudioSource != null && laserAudioSource.isPlaying)
            {
                laserAudioSource.Stop();
            }
        }

        /// <summary>
        /// Plays the crystal pickup sound effect.
        /// </summary>
        public void PlayCrystalPickup()
        {
            PlaySFX(crystalPickupClip);
        }

        /// <summary>
        /// Plays the low oxygen alert sound effect.
        /// </summary>
        public void PlayLowOxygenAlert()
        {
            PlaySFX(lowOxygenClip);
        }

        /// <summary>
        /// Plays the crystal break sound effect.
        /// </summary>
        public void PlayCrystalBreak()
        {
            PlaySFX(crystalBreakClip);
        }

        /// <summary>
        /// Plays the low gravity fuel alert sound effect.
        /// </summary>
        public void PlayLowGravityFuelAlert()
        {
            PlaySFX(lowGravityFuelAlertClip);
        }

        /// <summary>
        /// Plays a specified background music clip.
        /// </summary>
        /// <param name="clip">AudioClip to play as background music.</param>
        public void PlayBackgroundMusic(AudioClip clip)
        {
            if (clip != null)
            {
                musicAudioSource.clip = clip;
                musicAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("SoundManager: Background music clip is null.");
            }
        }

        /// <summary>
        /// Stops the background music.
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (musicAudioSource.isPlaying)
            {
                musicAudioSource.Stop();
            }
        }

        /// <summary>
        /// Sets the master volume for all audio.
        /// </summary>
        /// <param name="volume">Volume level (0 to 1).</param>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            AudioListener.volume = masterVolume;
        }

        /// <summary>
        /// Sets the music volume.
        /// </summary>
        /// <param name="volume">Volume level (0 to 1).</param>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicAudioSource != null)
            {
                musicAudioSource.volume = musicVolume;
            }
        }

        /// <summary>
        /// Sets the sound effects (SFX) volume.
        /// </summary>
        /// <param name="volume">Volume level (0 to 1).</param>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxAudioSource != null)
            {
                sfxAudioSource.volume = sfxVolume;
            }
            if (laserAudioSource != null)
            {
                laserAudioSource.volume = sfxVolume;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Plays a sound effect using the SFX AudioSource.
        /// </summary>
        /// <param name="clip">AudioClip to play.</param>
        private void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxAudioSource != null)
            {
                sfxAudioSource.PlayOneShot(clip, sfxVolume);
            }
            else
            {
                Debug.LogWarning("SoundManager: SFX AudioClip is null or AudioSource is not assigned.");
            }
        }

        #endregion
    }
}
