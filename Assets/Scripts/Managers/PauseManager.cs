using UnityEngine;

/// <summary>
/// Controls pausing and unpausing of the simulation, halting car movement and the countdown timer.
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        TimerManager.Instance?.PauseTimer();
        AudioManager.Instance?.StopEngineSound();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        TimerManager.Instance?.ResumeTimer();
        AudioManager.Instance?.StartEngineSound();
    }

    public void TogglePause()
    {
        if (IsPaused) ResumeGame();
        else PauseGame();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
