using UnityEngine;

/// <summary>
/// Global constants and key strings for Parking Master 3D.
/// </summary>
public static class GameConstants
{
    public const string GAME_TITLE = "Parking Master 3D";
    public const string GAME_TAGLINE = "Park Perfectly. Drive Carefully.";
    public const string PACKAGE_NAME = "com.adityarahate.parkingmaster3d";

    // PlayerPrefs Keys
    public const string PREF_UNLOCKED_LEVEL = "PM3D_UnlockedLevel";
    public const string PREF_SELECTED_CAR = "PM3D_SelectedCar";
    public const string PREF_LEVEL_STARS_PREFIX = "PM3D_Stars_Level_";
    public const string PREF_LEVEL_SCORE_PREFIX = "PM3D_Score_Level_";
    public const string PREF_LEVEL_TIME_PREFIX = "PM3D_Time_Level_";
    public const string PREF_SOUND_ENABLED = "PM3D_SoundEnabled";
    public const string PREF_MUSIC_ENABLED = "PM3D_MusicEnabled";
    public const string PREF_VIBRATION_ENABLED = "PM3D_VibrationEnabled";
    public const string PREF_GRAPHICS_QUALITY = "PM3D_GraphicsQuality";
    public const string PREF_CONTROL_TYPE = "PM3D_ControlType";

    // Tags
    public const string TAG_PLAYER = "Player";
    public const string TAG_PARKING_ZONE = "ParkingZone";
    public const string TAG_OBSTACLE = "Obstacle";
    public const string TAG_PARKED_CAR = "ParkedCar";
    public const string TAG_BARRIER = "Barrier";
    public const string TAG_CONE = "Cone";
    public const string TAG_GROUND = "Ground";

    // Scoring
    public const int BASE_SCORE = 1000;
    public const int COLLISION_PENALTY = 100;
    public const int PERFECT_PARKING_BONUS = 300;
    public const int TIME_BONUS_PER_SECOND = 10;
    public const int DEFAULT_MAX_COLLISIONS = 3;
}
