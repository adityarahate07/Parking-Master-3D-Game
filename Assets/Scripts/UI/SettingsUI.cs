using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Settings Menu interface for audio toggles, vibration, and graphics fidelity.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Toggles & Selectors")]
    public Toggle soundToggle;
    public Toggle musicToggle;
    public Toggle vibrationToggle;
    public Dropdown graphicsDropdown;
    public Button backButton;

    private void Start()
    {
        InitializeValues();

        if (soundToggle != null) soundToggle.onValueChanged.AddListener(OnSoundToggled);
        if (musicToggle != null) musicToggle.onValueChanged.AddListener(OnMusicToggled);
        if (vibrationToggle != null) vibrationToggle.onValueChanged.AddListener(OnVibrationToggled);
        if (graphicsDropdown != null) graphicsDropdown.onValueChanged.AddListener(OnGraphicsChanged);
        if (backButton != null) backButton.onClick.AddListener(OnBackClicked);
    }

    private void InitializeValues()
    {
        if (soundToggle != null) soundToggle.isOn = SaveManager.IsSoundEnabled();
        if (musicToggle != null) musicToggle.isOn = SaveManager.IsMusicEnabled();
        if (vibrationToggle != null) vibrationToggle.isOn = SaveManager.IsVibrationEnabled();
        if (graphicsDropdown != null) graphicsDropdown.value = SaveManager.GetGraphicsQuality();
    }

    public void OnSoundToggled(bool value)
    {
        SettingsManager.Instance?.SetSound(value);
        AudioManager.Instance?.PlayButtonClick();
    }

    public void OnMusicToggled(bool value)
    {
        SettingsManager.Instance?.SetMusic(value);
        AudioManager.Instance?.PlayButtonClick();
    }

    public void OnVibrationToggled(bool value)
    {
        SettingsManager.Instance?.SetVibration(value);
        AudioManager.Instance?.PlayButtonClick();
    }

    public void OnGraphicsChanged(int value)
    {
        SettingsManager.Instance?.SetGraphicsQuality(value);
        AudioManager.Instance?.PlayButtonClick();
    }

    public void OnBackClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        gameObject.SetActive(false);
    }
}
