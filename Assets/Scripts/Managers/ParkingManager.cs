using UnityEngine;

/// <summary>
/// Oversees active parking zone state and coordinates win condition evaluation
/// with ScoreManager, TimerManager, and GameManager.
/// </summary>
public class ParkingManager : MonoBehaviour
{
    public static ParkingManager Instance { get; private set; }

    public ParkingZone activeParkingZone;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void SetActiveZone(ParkingZone zone)
    {
        activeParkingZone = zone;
    }

    public void OnParkingCompleted()
    {
        // Gather stats
        float remainingTime = TimerManager.Instance != null ? TimerManager.Instance.timeRemaining : 0f;
        float totalTime = TimerManager.Instance != null ? TimerManager.Instance.totalLevelTime : 90f;
        int collisions = CollisionManager.Instance != null ? CollisionManager.Instance.collisionCount : 0;
        bool accurate = activeParkingZone != null && activeParkingZone.IsRotationCorrect;

        // Calculate score & stars
        int score = ScoreManager.Instance != null
            ? ScoreManager.Instance.CalculateFinalScore(remainingTime, totalTime, collisions, accurate)
            : 1000;

        int stars = ScoreManager.Instance != null ? ScoreManager.Instance.starsEarned : 2;

        // Stop timer
        TimerManager.Instance?.StopTimer();

        // Notify GameManager to show win dialog and save progress
        GameManager.Instance?.OnLevelSuccess();
    }
}
