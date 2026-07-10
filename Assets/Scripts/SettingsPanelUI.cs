using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : MonoBehaviour
{
    [SerializeField] private Button musicSourceBtn;
    [SerializeField] private Button sfxSourceBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private Sprite musicSourceTurnedOnSprite;
    [SerializeField] private Sprite musicSourceTurnedOffSprite;
    [SerializeField] private Sprite sfxSourceTurnedOnSprite;
    [SerializeField] private Sprite sfxSourceTurnedOffSprite;

    private bool _isMusicSourceOn;
    private bool _isSfxSourceOn;
    private float _musicVolume;
    private float _sfxVolume;
    private void Start()
    {
        _isMusicSourceOn = PlayerPrefs.GetString("isMusicSourceOn", "true") == "true";
        UpdateMusicSourceBtn();

        _isSfxSourceOn = PlayerPrefs.GetString("isSfxSourceOn", "true") == "false";
        UpdateSFXSourceBtn();

        _musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        musicVolumeSlider.value = _musicVolume;

        _sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);
        sfxVolumeSlider.value = _sfxVolume;

        musicSourceBtn.onClick.AddListener(() =>
        {
            _isMusicSourceOn = !_isMusicSourceOn;
            PlayerPrefs.SetString("isMusicSourceOn", _isMusicSourceOn.ToString());
            PlayerPrefs.Save();
            UpdateMusicSourceBtn();
        });

        sfxSourceBtn.onClick.AddListener(() =>
        {
            _isSfxSourceOn = !_isSfxSourceOn;
            PlayerPrefs.SetString("isSfxSourceOn", _isSfxSourceOn.ToString());
            PlayerPrefs.Save();
            UpdateSFXSourceBtn();
        });

        musicVolumeSlider.onValueChanged.AddListener((value) =>
        {
            AudioManager.instance.SetMusicVolume(value);
            PlayerPrefs.SetFloat("musicVolume", value);
            PlayerPrefs.Save();
        });

        sfxVolumeSlider.onValueChanged.AddListener((value) =>
        {
            AudioManager.instance.SetSFXVolume(value);
            PlayerPrefs.SetFloat("sfxVolume", value);
            PlayerPrefs.Save();
        });

        cancelBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void UpdateMusicSourceBtn()
    {
        Sprite sprite;
        if(_isMusicSourceOn)
        {
            sprite = musicSourceTurnedOnSprite;
            AudioManager.instance.EnableMusicAudioSource();
        }
        else
        {
            sprite = musicSourceTurnedOffSprite;
            AudioManager.instance.DisableMusicAudioSource();
        }
        musicSourceBtn.GetComponent<Image>().sprite = sprite;
    }

    private void UpdateSFXSourceBtn()
    {
        Sprite sprite;
        if(_isSfxSourceOn)
        {
            sprite = sfxSourceTurnedOnSprite;
            AudioManager.instance.EnableSFXAudioSource();
        }
        else
        {
            sprite = sfxSourceTurnedOffSprite;
            AudioManager.instance.DisableSFXAudioSource();
        }
        sfxSourceBtn.GetComponent<Image>().sprite = sprite;
    }
}
