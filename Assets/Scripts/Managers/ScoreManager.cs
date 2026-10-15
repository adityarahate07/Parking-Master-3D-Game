using UnityEngine;

/// <summary>
/// Calculates score and star ratings for each level in Parking Master 3D.
/// Handles base score, time bonus, collision penalties, and star ratings (1-3).
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Breakdown")]
    public int baseScore = 1000;
    public int timeBonus = 0;
    public int parkingBonus = 300;
    public int collisionPenalty = 0;
    public int finalScore = 0;
    public int starsEarned = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void ResetScore()
    {
        baseScore = GameConstants.BASE_SCORE;
        timeBonus = 0;
        parkingBonus = GameConstants.PERFECT_PARKING_BONUS;
        collisionPenalty = 0;
        finalScore = baseScore;
        starsEarned = 0;
    }

    public void OnCollisionDeduction()
    {
        collisionPenalty += GameConstants.COLLISION_PENALTY;
        finalScore = Mathf.Max(0, baseScore - collisionPenalty);
    }

    /// <summary>
    /// Computes score and star rating upon successful parking.
    /// </summary>
    public int CalculateFinalScore(float remainingTime, float timeLimit, int collisionCount, bool isAccurate)
    {
        baseScore = GameConstants.BASE_SCORE;
        timeBonus = Mathf.Max(0, Mathf.FloorToInt(remainingTime * GameConstants.TIME_BONUS_PER_SECOND));
        parkingBonus = isAccurate ? GameConstants.PERFECT_PARKING_BONUS : 150;
        collisionPenalty = collisionCount * GameConstants.COLLISION_PENALTY;

        finalScore = Mathf.Max(100, baseScore + timeBonus + parkingBonus - collisionPenalty);

        // Star rating determination
        float timeRatio = remainingTime / Mathf.Max(1f, timeLimit);

        if (collisionCount == 0 && timeRatio >= 0.35f && isAccurate)
        {
            starsEarned = 3;
        }
        else if (collisionCount <= 1 && timeRatio >= 0.15f)
        {
            starsEarned = 2;
        }
        else
        {
            starsEarned = 1;
        }

        return finalScore;
    }
}
