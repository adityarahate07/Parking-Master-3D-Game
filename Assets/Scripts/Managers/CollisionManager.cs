using UnityEngine;

/// <summary>
/// Monitors and tallies collisions with cones, barriers, walls, and parked cars.
/// Handles audio cues, score deductions, UI warnings, and triggers level failure when exceeded.
/// </summary>
public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance { get; private set; }

    [Header("Collision Tracking")]
    public int collisionCount = 0;
    public int maxAllowedCollisions = 3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void ResetCollisions(int maxAllowed = 3)
    {
        collisionCount = 0;
        maxAllowedCollisions = maxAllowed;
    }

    public void RegisterCollision(GameObject obstacleHit, string obstacleType)
    {
        collisionCount++;

        // Audio and Score penalty
        AudioManager.Instance?.PlayCollision();
        ScoreManager.Instance?.OnCollisionDeduction();

        // UI Alert banner
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.ShowStatusBanner("COLLISION! (-100 PTS)", 1.5f);
            ui.UpdateCollisionDisplay(collisionCount, maxAllowedCollisions);
        }

        // Check if exceeded threshold
        if (collisionCount >= maxAllowedCollisions)
        {
            GameManager.Instance?.OnTooManyCollisions();
        }
    }
}
