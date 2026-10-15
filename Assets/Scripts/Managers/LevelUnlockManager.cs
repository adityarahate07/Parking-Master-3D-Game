using UnityEngine;

/// <summary>
/// Controls progression gating for levels and vehicles.
/// Levels unlock sequentially upon completion; vehicles unlock upon reaching milestones.
/// </summary>
public static class LevelUnlockManager
{
    public static bool IsLevelUnlocked(int level)
    {
        if (level <= 1) return true;
        return SaveManager.GetUnlockedLevel() >= level;
    }

    public static void UnlockNextLevel(int completedLevel)
    {
        if (completedLevel >= 1 && completedLevel < 10)
        {
            SaveManager.SetUnlockedLevel(completedLevel + 1);
        }
    }

    public static bool IsCarUnlocked(int carIndex)
    {
        // Car 0: Starter (Always Free)
        // Car 1: Sport Car (Unlocked after Level 3)
        // Car 2: SUV (Unlocked after Level 7)
        int highestUnlockedLevel = SaveManager.GetUnlockedLevel();
        switch (carIndex)
        {
            case 0: return true;
            case 1: return highestUnlockedLevel > 3 || SaveManager.GetStars(3) > 0;
            case 2: return highestUnlockedLevel > 7 || SaveManager.GetStars(7) > 0;
            default: return false;
        }
    }

    public static string GetCarUnlockRequirementDescription(int carIndex)
    {
        switch (carIndex)
        {
            case 0: return "FREE";
            case 1: return "Complete Level 3 to Unlock";
            case 2: return "Complete Level 7 to Unlock";
            default: return "";
        }
    }
}
