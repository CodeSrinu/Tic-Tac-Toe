using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SetSFXVolume(float volume)
    {
        sfxAudioSource.volume = volume;
    }
    public void SetMusicVolume(float volume)
    {
        musicAudioSource.volume = volume;
    }

    public void DisableMusicAudioSource()
    {
        musicAudioSource.enabled = false;
    }

    public void DisableSFXAudioSource()
    {
        sfxAudioSource.enabled = false;
    }

    public void EnableMusicAudioSource()
    {
        musicAudioSource.enabled = true;
    }

    public void EnableSFXAudioSource()
    {
        sfxAudioSource.enabled = true;
    }
}
