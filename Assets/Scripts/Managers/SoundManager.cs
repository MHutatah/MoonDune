using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Clips")]
    public AudioClip laserShotClip;
    public AudioClip crystalPickupClip;
    public AudioClip lowOxygenClip;
    // ... More clips

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayLaserShot()
    {
        audioSource.PlayOneShot(laserShotClip);
    }

    public void PlayCrystalPickup()
    {
        audioSource.PlayOneShot(crystalPickupClip);
    }

    public void PlayLowOxygenAlert()
    {
        audioSource.PlayOneShot(lowOxygenClip);
    }
}


