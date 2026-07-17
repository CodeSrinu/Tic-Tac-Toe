using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip panelSlideSound;
    [SerializeField] private AudioClip panelSlideBackSound;
    [SerializeField] private AudioClip btnClickSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip circleDrawSound;
    [SerializeField] private AudioClip crossDrawSound;
    [SerializeField] private AudioClip loadingSound;
    [SerializeField] private AudioClip lobbyJoinedSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip tieSound;
    [SerializeField] private AudioClip rematchTriggerSound;
    [SerializeField] private AudioClip turnChangeSound;

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

    public void PlayErrorSound()
    {
        sfxAudioSource.PlayOneShot(errorSound);
    }
    public void PlayCircleDrawSound()
    {
        sfxAudioSource.PlayOneShot(circleDrawSound);
    }
    public void PlayCrossDrawSound()
    {
        sfxAudioSource.PlayOneShot(crossDrawSound);
    }
    public void PlayLoadingSound()
    {
        sfxAudioSource.clip = loadingSound;
        sfxAudioSource.loop = true;
        sfxAudioSource.Play();
    }
    public void StopLoadingSound()
    {
        sfxAudioSource.DOFade(0, 0.2f).OnComplete(() =>
        {
            sfxAudioSource.Stop();
            sfxAudioSource.clip = null;
            sfxAudioSource.loop = false;
        });
        
    }
    public void PlayLobbyJoinedSound()
    {
        sfxAudioSource.PlayOneShot(lobbyJoinedSound);
    }
    public void PlayWinSound()
    {
        sfxAudioSource.PlayOneShot(winSound,0.3f);
    }
    public void PlayLoseSound()
    {
        sfxAudioSource.PlayOneShot(loseSound);
    }
    public void PlayTieSound()
    {
        sfxAudioSource.PlayOneShot(tieSound);
    }
    public void PlayRematchTriggerSound()
    {
        sfxAudioSource.PlayOneShot(rematchTriggerSound);
    }
    public void PlatTurnChangeSound()
    {
        sfxAudioSource.PlayOneShot(turnChangeSound, 0.15f);
    }
}
