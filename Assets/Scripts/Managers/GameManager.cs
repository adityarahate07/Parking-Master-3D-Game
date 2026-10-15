using UnityEngine;

/// <summary>
/// Central game coordinator for Parking Master 3D.
/// Controls overall game flow, level transitions, win/loss evaluation, and persistence.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, LevelSelect, Garage, Playing, Paused, LevelComplete, LevelFailed }
    public GameState currentState = GameState.Playing;

    public int CurrentLevel => LevelManager.Instance != null ? LevelManager.Instance.currentLevel : 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartLevel(int level)
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;

        LevelData data = LevelData.GetLevel(level);

        // Reset systems
        ScoreManager.Instance?.ResetScore();
        CollisionManager.Instance?.ResetCollisions(data.maxCollisions);
        TimerManager.Instance?.StartTimer(data.timeLimitSeconds);
        AudioManager.Instance?.StartEngineSound();

        // Update UI
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.InitHUD(level, data.levelName, data.timeLimitSeconds, data.maxCollisions);
        }
    }

    public void OnLevelSuccess()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.LevelComplete;

        TimerManager.Instance?.StopTimer();
        AudioManager.Instance?.StopEngineSound();
        AudioManager.Instance?.PlayLevelComplete();

        float remainingTime = TimerManager.Instance != null ? TimerManager.Instance.timeRemaining : 0f;
        float elapsedTime = TimerManager.Instance != null ? TimerManager.Instance.ElapsedTime : 0f;
        int collisions = CollisionManager.Instance != null ? CollisionManager.Instance.collisionCount : 0;
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.finalScore : 1000;
        int stars = ScoreManager.Instance != null ? ScoreManager.Instance.starsEarned : 3;

        // Persist records
        int lvl = CurrentLevel;
        SaveManager.SetStars(lvl, stars);
        SaveManager.SetBestScore(lvl, score);
        SaveManager.SetBestTime(lvl, elapsedTime);
        LevelUnlockManager.UnlockNextLevel(lvl);

        // Show UI dialog
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.ShowLevelComplete(score, TimerManager.Instance != null ? TimerManager.Instance.GetFormattedElapsedTime() : "00:00", collisions, stars);
        }
    }

    public void OnTooManyCollisions()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.LevelFailed;

        TimerManager.Instance?.StopTimer();
        AudioManager.Instance?.StopEngineSound();
        AudioManager.Instance?.PlayLevelFailed();

        // Disable car controls
        CarController car = FindObjectOfType<CarController>();
        if (car != null) car.SetControlsEnabled(false);

        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.ShowLevelFailed("TOO MANY COLLISIONS!");
        }
    }

    public void OnTimeUp()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.LevelFailed;

        TimerManager.Instance?.StopTimer();
        AudioManager.Instance?.StopEngineSound();
        AudioManager.Instance?.PlayLevelFailed();

        // Disable car controls
        CarController car = FindObjectOfType<CarController>();
        if (car != null) car.SetControlsEnabled(false);

        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.ShowLevelFailed("TIME'S UP!");
        }
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        LevelManager.Instance?.RestartCurrentLevel();
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        LevelManager.Instance?.LoadNextLevel();
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.StopEngineSound();
        LevelManager.Instance?.LoadLevelSelect();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.StopEngineSound();
        LevelManager.Instance?.LoadMainMenu();
    }

    public void GoToGarage()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.StopEngineSound();
        LevelManager.Instance?.LoadGarage();
    }

    public void QuitGame()
    {
        AudioManager.Instance?.PlayButtonClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
