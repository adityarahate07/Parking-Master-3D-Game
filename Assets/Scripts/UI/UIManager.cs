using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls all in-game HUD elements, popups, pause menu, and level results screens.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("HUD Displays")]
    public Text levelText;
    public Text timerText;
    public Text scoreText;
    public Text collisionsText;

    [Header("Status Banner")]
    public GameObject bannerPanel;
    public Text bannerText;

    [Header("Dialog Panels")]
    public GameObject hudRoot;
    public GameObject pausePanel;
    public GameObject levelCompletePanel;
    public GameObject levelFailedPanel;
    public GameObject settingsPanel;

    [Header("Level Complete Details")]
    public Text completeTitleText;
    public Text completeScoreText;
    public Text completeTimeText;
    public Text completeCollisionsText;
    public GameObject[] starIcons; // 3 star objects

    [Header("Level Failed Details")]
    public Text failureReasonText;

    private Coroutine bannerCoroutine;

    private void Start()
    {
        HideAllDialogs();
    }

    public void InitHUD(int levelNum, string levelName, float timeLimit, int maxCollisions)
    {
        if (levelText != null) levelText.text = string.Format("LEVEL {0:00}\n<size=14>{1}</size>", levelNum, levelName);
        if (timerText != null) timerText.text = "TIME\n00:00";
        if (scoreText != null) scoreText.text = "SCORE\n1000";
        if (collisionsText != null) collisionsText.text = string.Format("Collisions: 0 / {0}", maxCollisions);

        if (hudRoot != null) hudRoot.SetActive(true);
        HideAllDialogs();
    }

    private void Update()
    {
        // Update live timer
        if (TimerManager.Instance != null && TimerManager.Instance.isTimerRunning && timerText != null)
        {
            timerText.text = "TIME\n" + TimerManager.Instance.GetFormattedTime();
        }

        // Update live score
        if (ScoreManager.Instance != null && scoreText != null)
        {
            scoreText.text = "SCORE\n" + ScoreManager.Instance.finalScore;
        }
    }

    public void UpdateCollisionDisplay(int current, int max)
    {
        if (collisionsText != null)
        {
            collisionsText.text = string.Format("Collisions: {0} / {1}", current, max);
        }
    }

    public void ShowStatusBanner(string message, float duration = 2.0f)
    {
        if (bannerPanel == null || bannerText == null) return;

        if (bannerCoroutine != null) StopCoroutine(bannerCoroutine);
        bannerCoroutine = StartCoroutine(AnimateBanner(message, duration));
    }

    private IEnumerator AnimateBanner(string msg, float duration)
    {
        bannerText.text = msg;
        bannerPanel.SetActive(true);
        yield return UIAnimations.PopIn(bannerPanel.transform, 0.2f);
        yield return new WaitForSeconds(duration);
        bannerPanel.SetActive(false);
    }

    // ==========================================
    // DIALOGS
    // ==========================================
    public void ShowPauseMenu(bool show)
    {
        if (pausePanel != null) pausePanel.SetActive(show);
        if (show)
        {
            PauseManager.Instance?.PauseGame();
        }
        else
        {
            PauseManager.Instance?.ResumeGame();
        }
    }

    public void ShowLevelComplete(int score, string timeStr, int collisions, int stars)
    {
        if (hudRoot != null) hudRoot.SetActive(false);
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            if (completeScoreText != null) completeScoreText.text = "Score: " + score;
            if (completeTimeText != null) completeTimeText.text = "Time: " + timeStr;
            if (completeCollisionsText != null) completeCollisionsText.text = "Collisions: " + collisions;

            StartCoroutine(UIAnimations.RevealStars(starIcons, stars, 0.35f));
            StartCoroutine(UIAnimations.PopIn(levelCompletePanel.transform, 0.3f));
        }
    }

    public void ShowLevelFailed(string reason)
    {
        if (hudRoot != null) hudRoot.SetActive(false);
        if (levelFailedPanel != null)
        {
            levelFailedPanel.SetActive(true);
            if (failureReasonText != null) failureReasonText.text = reason;
            StartCoroutine(UIAnimations.PopIn(levelFailedPanel.transform, 0.3f));
        }
    }

    public void HideAllDialogs()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (bannerPanel != null) bannerPanel.SetActive(false);
    }

    // ==========================================
    // UI BUTTON HOOKS
    // ==========================================
    public void OnPauseButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        ShowPauseMenu(true);
    }

    public void OnResumeButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        ShowPauseMenu(false);
    }

    public void OnRestartButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameManager.Instance?.RestartCurrentLevel();
    }

    public void OnNextLevelButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameManager.Instance?.NextLevel();
    }

    public void OnLevelSelectButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameManager.Instance?.GoToLevelSelect();
    }

    public void OnMainMenuButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameManager.Instance?.GoToMainMenu();
    }

    public void OnCameraSwitchButtonClicked()
    {
        FindObjectOfType<CameraFollow>()?.ToggleCameraMode();
    }

    public void OnSettingsButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void OnCloseSettingsClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
}
