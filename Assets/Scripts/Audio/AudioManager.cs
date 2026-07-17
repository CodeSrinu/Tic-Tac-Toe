using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip panelSlideSound;
    [SerializeField] private AudioClip panelSlideBackSound;
    [SerializeField] private AudioClip btnClickSound;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

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

    public void SetMusicSourceEnabledStatus(bool isOn)
    {
        musicAudioSource.enabled = isOn;
    }
    public void SetSFXSourceEnabledStatus(bool isOn)
    {
        sfxAudioSource.enabled = isOn;
    }


    public void PlayPanelSlideSound()
    {
        sfxAudioSource.PlayOneShot(panelSlideSound, 0.7f);
    }
    public void PlayPanelSlideBackSound()
    {
        sfxAudioSource.PlayOneShot(panelSlideBackSound, 0.45f);
    }


    public void PlayBtnClickSound()
    {
        sfxAudioSource.PlayOneShot(btnClickSound);
    }
}
