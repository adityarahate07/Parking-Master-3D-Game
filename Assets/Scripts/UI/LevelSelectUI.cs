using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls Level Selection UI: generates 10 level cards with star ratings (1-3 stars),
/// lock/unlock indicators, and handles level loading.
/// </summary>
public class LevelSelectUI : MonoBehaviour
{
    [Header("UI Grid Container")]
    public Transform gridContainer;
    public Button backButton;

    [Header("Level Card Template / Items")]
    public GameObject[] levelCardItems; // Can be manually assigned or auto-populated

    private void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }

        PopulateLevels();
    }

    public void PopulateLevels()
    {
        int unlockedLevel = SaveManager.GetUnlockedLevel();

        if (levelCardItems != null && levelCardItems.Length >= 10)
        {
            for (int i = 0; i < 10; i++)
            {
                int lvlNum = i + 1;
                bool isUnlocked = lvlNum <= unlockedLevel;
                int stars = SaveManager.GetStars(lvlNum);

                GameObject card = levelCardItems[i];
                if (card == null) continue;

                // Update text
                Text titleText = card.transform.Find("TitleText")?.GetComponent<Text>();
                if (titleText != null) titleText.text = "LEVEL " + lvlNum;

                // Update stars
                Text starsText = card.transform.Find("StarsText")?.GetComponent<Text>();
                if (starsText != null)
                {
                    if (!isUnlocked) starsText.text = "🔒 LOCKED";
                    else if (stars == 3) starsText.text = "⭐⭐⭐";
                    else if (stars == 2) starsText.text = "⭐⭐";
                    else if (stars == 1) starsText.text = "⭐";
                    else starsText.text = "NEW";
                }

                // Lock overlay
                GameObject lockOverlay = card.transform.Find("LockOverlay")?.gameObject;
                if (lockOverlay != null) lockOverlay.SetActive(!isUnlocked);

                // Button click
                Button btn = card.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = isUnlocked;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => OnLevelCardClicked(lvlNum, isUnlocked));
                }
            }
        }
    }

    public void OnLevelCardClicked(int levelNum, bool isUnlocked)
    {
        if (isUnlocked)
        {
            AudioManager.Instance?.PlayButtonClick();
            LevelManager.Instance?.LoadLevel(levelNum);
        }
        else
        {
            AudioManager.Instance?.PlayCountdownTick();
        }
    }

    public void OnBackClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        LevelManager.Instance?.LoadMainMenu();
    }
}
