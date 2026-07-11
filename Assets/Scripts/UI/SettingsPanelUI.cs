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

        _isSfxSourceOn = PlayerPrefs.GetString("isSfxSourceOn", "true") == "true";
        UpdateSFXSourceBtn();

        _musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        musicVolumeSlider.value = _musicVolume;

        _sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);
        sfxVolumeSlider.value = _sfxVolume;

        musicSourceBtn.onClick.AddListener(() =>
        {
            _isMusicSourceOn = !_isMusicSourceOn;
            PlayerPrefs.SetString("isMusicSourceOn", _isMusicSourceOn.ToString().ToLower());
            PlayerPrefs.Save();
            UpdateMusicSourceBtn();
        });

        sfxSourceBtn.onClick.AddListener(() =>
        {
            _isSfxSourceOn = !_isSfxSourceOn;
            PlayerPrefs.SetString("isSfxSourceOn", _isSfxSourceOn.ToString().ToLower());
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
            PanelAnimator.Hide(gameObject);
        });
    }

    private void UpdateMusicSourceBtn()
    {
        Sprite sprite = _isMusicSourceOn ? musicSourceTurnedOnSprite : musicSourceTurnedOffSprite;
        AudioManager.instance.SetMusicSourceEnabledStatus(_isMusicSourceOn);
        musicVolumeSlider.interactable = _isMusicSourceOn;
        musicSourceBtn.GetComponent<Image>().sprite = sprite;
    }


    private void UpdateSFXSourceBtn()
    {
        Sprite sprite = _isSfxSourceOn ? sfxSourceTurnedOnSprite : sfxSourceTurnedOffSprite;
        AudioManager.instance.SetSFXSourceEnabledStatus(_isSfxSourceOn);
        sfxVolumeSlider.interactable = _isSfxSourceOn;
        sfxSourceBtn.GetComponent<Image>().sprite = sprite;
    }
}
