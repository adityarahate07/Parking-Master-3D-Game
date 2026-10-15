#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor toolbar shortcuts for Parking Master 3D.
/// Enables one-click scene setup, environment building, and testing progression states.
/// </summary>
public static class ParkingMasterEditorMenu
{
    [MenuItem("Parking Master 3D/1. Setup Game Bootstrap in Scene", false, 1)]
    public static void SetupBootstrap()
    {
        if (Object.FindObjectOfType<GameBootstrap>() == null)
        {
            GameObject bootObj = new GameObject("_GameBootstrap");
            bootObj.AddComponent<GameBootstrap>();
            Undo.RegisterCreatedObjectUndo(bootObj, "Create GameBootstrap");
        }

        EditorUtility.DisplayDialog("Parking Master 3D", "GameBootstrap has been added to the active scene!", "OK");
    }

    [MenuItem("Parking Master 3D/2. Build Active Level Environment", false, 2)]
    public static void BuildLevelEnvironment()
    {
        LevelEnvironmentBuilder builder = Object.FindObjectOfType<LevelEnvironmentBuilder>();
        if (builder == null)
        {
            GameObject bObj = new GameObject("LevelEnvironmentBuilder");
            builder = bObj.AddComponent<LevelEnvironmentBuilder>();
            Undo.RegisterCreatedObjectUndo(bObj, "Create LevelEnvironmentBuilder");
        }

        int lvl = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : 1;
        builder.BuildLevel(lvl);

        EditorUtility.DisplayDialog("Parking Master 3D", "Level " + lvl + " 3D environment successfully constructed!", "OK");
    }

    [MenuItem("Parking Master 3D/3. Reset Game Progress (Default State)", false, 3)]
    public static void ResetGameProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt(GameConstants.PREF_UNLOCKED_LEVEL, 1);
        PlayerPrefs.Save();

        EditorUtility.DisplayDialog("Parking Master 3D", "All player progress has been reset. Only Level 1 and Starter Car are unlocked.", "OK");
    }

    [MenuItem("Parking Master 3D/4. Unlock All 10 Levels & All Cars (Cheat Mode)", false, 4)]
    public static void UnlockAllContent()
    {
        PlayerPrefs.SetInt(GameConstants.PREF_UNLOCKED_LEVEL, 10);
        for (int i = 1; i <= 10; i++)
        {
            SaveManager.SetStars(i, 3);
            SaveManager.SetBestScore(i, 1450);
            SaveManager.SetBestTime(i, 35.0f);
        }
        PlayerPrefs.Save();

        EditorUtility.DisplayDialog("Parking Master 3D", "All 10 levels and all 3 vehicles are now unlocked with 3 stars!", "OK");
    }
}
#endif
