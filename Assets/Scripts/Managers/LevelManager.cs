using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level transitions, scene loading, and dynamic environment instantiation for all 10 levels.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int currentLevel = 1;

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

    public void LoadLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, 10);
        string sceneName = string.Format("Level{0:00}", currentLevel);

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            // Dynamic fallback: reload current scene and build level
            LevelEnvironmentBuilder builder = FindObjectOfType<LevelEnvironmentBuilder>();
            if (builder != null)
            {
                builder.BuildLevel(currentLevel);
            }
            else
            {
                SceneManager.LoadScene("Level01");
            }
        }
    }

    public void LoadNextLevel()
    {
        if (currentLevel < 10)
        {
            LoadLevel(currentLevel + 1);
        }
        else
        {
            LoadMainMenu();
        }
    }

    public void RestartCurrentLevel()
    {
        LoadLevel(currentLevel);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void LoadGarage()
    {
        SceneManager.LoadScene("Garage");
    }
}
