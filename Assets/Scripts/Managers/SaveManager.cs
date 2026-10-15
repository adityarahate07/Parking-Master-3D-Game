using UnityEngine;

/// <summary>
/// Handles persistent local progress using Unity PlayerPrefs.
/// Stores unlocked levels, star ratings (1-3 stars), high scores, best times,
/// selected garage vehicle, and user audio/visual preferences.
/// </summary>
public static class SaveManager
{
    public static int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_UNLOCKED_LEVEL, 1);
    }

    public static void SetUnlockedLevel(int level)
    {
        int current = GetUnlockedLevel();
        if (level > current)
        {
            PlayerPrefs.SetInt(GameConstants.PREF_UNLOCKED_LEVEL, Mathf.Clamp(level, 1, 10));
            PlayerPrefs.Save();
        }
    }

    public static int GetStars(int level)
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_LEVEL_STARS_PREFIX + level, 0);
    }

    public static void SetStars(int level, int stars)
    {
        int current = GetStars(level);
        if (stars > current)
        {
            PlayerPrefs.SetInt(GameConstants.PREF_LEVEL_STARS_PREFIX + level, Mathf.Clamp(stars, 0, 3));
            PlayerPrefs.Save();
        }
    }

    public static int GetBestScore(int level)
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_LEVEL_SCORE_PREFIX + level, 0);
    }

    public static void SetBestScore(int level, int score)
    {
        int current = GetBestScore(level);
        if (score > current)
        {
            PlayerPrefs.SetInt(GameConstants.PREF_LEVEL_SCORE_PREFIX + level, score);
            PlayerPrefs.Save();
        }
    }

    public static float GetBestTime(int level)
    {
        return PlayerPrefs.GetFloat(GameConstants.PREF_LEVEL_TIME_PREFIX + level, 0f);
    }

    public static void SetBestTime(int level, float time)
    {
        float current = GetBestTime(level);
        if (current <= 0.01f || time < current)
        {
            PlayerPrefs.SetFloat(GameConstants.PREF_LEVEL_TIME_PREFIX + level, time);
            PlayerPrefs.Save();
        }
    }

    public static int GetSelectedCar()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_SELECTED_CAR, 0);
    }

    public static void SetSelectedCar(int index)
    {
        PlayerPrefs.SetInt(GameConstants.PREF_SELECTED_CAR, Mathf.Clamp(index, 0, 2));
        PlayerPrefs.Save();
    }

    // Audio & Settings
    public static bool IsSoundEnabled()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_SOUND_ENABLED, 1) == 1;
    }

    public static void SetSoundEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(GameConstants.PREF_SOUND_ENABLED, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool IsMusicEnabled()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_MUSIC_ENABLED, 1) == 1;
    }

    public static void SetMusicEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(GameConstants.PREF_MUSIC_ENABLED, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool IsVibrationEnabled()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_VIBRATION_ENABLED, 1) == 1;
    }

    public static void SetVibrationEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(GameConstants.PREF_VIBRATION_ENABLED, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static int GetGraphicsQuality()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_GRAPHICS_QUALITY, 1); // 1 = Medium
    }

    public static void SetGraphicsQuality(int quality)
    {
        PlayerPrefs.SetInt(GameConstants.PREF_GRAPHICS_QUALITY, Mathf.Clamp(quality, 0, 2));
        PlayerPrefs.Save();
    }
}
