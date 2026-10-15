using UnityEngine;

/// <summary>
/// Manages player options: Sound, Music, Vibration, Graphics Quality (Low, Medium, High).
/// Persists choices and applies system-level graphics and audio configurations.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyAllSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplyAllSettings()
    {
        // 1. Audio
        AudioManager.Instance?.ApplySettings();

        // 2. Graphics
        int quality = SaveManager.GetGraphicsQuality();
        QualitySettings.SetQualityLevel(quality, true);
    }

    public void SetSound(bool enabled)
    {
        SaveManager.SetSoundEnabled(enabled);
        AudioManager.Instance?.ApplySettings();
    }

    public void SetMusic(bool enabled)
    {
        SaveManager.SetMusicEnabled(enabled);
        AudioManager.Instance?.ApplySettings();
    }

    public void SetVibration(bool enabled)
    {
        SaveManager.SetVibrationEnabled(enabled);
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        SaveManager.SetGraphicsQuality(qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }
}
