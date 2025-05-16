using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private Resolution[] resolutions;

    private const string VolumeKey = "GameVolume";
    private const string QualityKey = "GameQuality";
    private const string FullscreenKey = "Fullscreen";
    private const string ResolutionKey = "ResolutionIndex";

    void Start()
    {
        InitResolutions();
        LoadSettings();
    }

    void InitResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt(ResolutionKey, currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    public void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
    }

    public void OnQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(QualityKey, qualityIndex);
    }

    public void OnFullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
    }

    public void OnResolutionChanged(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt(ResolutionKey, resolutionIndex);
    }

    public void LoadSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        int savedQuality = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
        int savedFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0);

        volumeSlider.value = savedVolume;
        qualityDropdown.value = savedQuality;
        fullscreenToggle.isOn = savedFullscreen == 1;

        AudioListener.volume = savedVolume;
        QualitySettings.SetQualityLevel(savedQuality);
        Screen.fullScreen = savedFullscreen == 1;
    }
}
