using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the modern automotive 3D Main Menu:
/// Displays game title, tagline, interactive menu buttons, and background camera orbit.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button playButton;
    public Button levelsButton;
    public Button garageButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Dialog Panels")]
    public GameObject settingsDialog;

    [Header("3D Showcase Environment")]
    public Transform showcaseCar;
    public Transform showcaseCamera;
    public float cameraOrbitSpeed = 4.0f;

    private void Start()
    {
        // Wire up buttons
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (levelsButton != null) levelsButton.onClick.AddListener(OnLevelsClicked);
        if (garageButton != null) garageButton.onClick.AddListener(OnGarageClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);

        if (settingsDialog != null) settingsDialog.SetActive(false);
    }

    private void Update()
    {
        // Soft camera orbit around player's showcase car in background
        if (showcaseCamera != null && showcaseCar != null)
        {
            showcaseCamera.RotateAround(showcaseCar.position, Vector3.up, cameraOrbitSpeed * Time.deltaTime);
            showcaseCamera.LookAt(showcaseCar.position + Vector3.up * 0.7f);
        }
    }

    public void OnPlayClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        // Continue from current unlocked level or level 1
        int unlocked = SaveManager.GetUnlockedLevel();
        LevelManager.Instance?.LoadLevel(unlocked);
    }

    public void OnLevelsClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        LevelManager.Instance?.LoadLevelSelect();
    }

    public void OnGarageClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        LevelManager.Instance?.LoadGarage();
    }

    public void OnSettingsClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsDialog != null) settingsDialog.SetActive(true);
    }

    public void OnCloseSettingsClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsDialog != null) settingsDialog.SetActive(false);
    }

    public void OnExitClicked()
    {
        GameManager.Instance?.QuitGame();
    }
}
