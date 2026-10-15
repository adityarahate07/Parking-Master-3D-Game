using UnityEngine;

/// <summary>
/// Manages level countdown timer and notifies GameManager when time runs out.
/// </summary>
public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [Header("Timer State")]
    public float totalLevelTime = 90f;
    public float timeRemaining = 90f;
    public bool isTimerRunning = false;

    public float ElapsedTime => totalLevelTime - timeRemaining;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void StartTimer(float duration)
    {
        totalLevelTime = duration;
        timeRemaining = duration;
        isTimerRunning = true;
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isTimerRunning = false;
            OnTimeExpired();
        }
    }

    private void OnTimeExpired()
    {
        GameManager.Instance?.OnTimeUp();
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public string GetFormattedElapsedTime()
    {
        int minutes = Mathf.FloorToInt(ElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(ElapsedTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
