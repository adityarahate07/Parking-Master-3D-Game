using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the 3D Garage showroom: vehicle preview turntable,
/// stats comparison (speed, acceleration, handling), vehicle selection, and unlock milestones.
/// </summary>
public class GarageUI : MonoBehaviour
{
    [Header("UI Displays")]
    public Text carNameText;
    public Text carCategoryText;
    public Text unlockRequirementText;
    public Button selectButton;
    public Text selectButtonText;
    public Button prevCarButton;
    public Button nextCarButton;
    public Button backButton;

    [Header("Stats Sliders / Bars")]
    public Slider speedSlider;
    public Slider handlingSlider;
    public Slider brakingSlider;

    [Header("3D Turntable Platform")]
    public Transform turntablePlatform;
    public GameObject[] carPreviewModels; // 3 preview vehicle gameObjects
    public float turntableRotateSpeed = 20f;

    private int browsingIndex = 0;

    private void Start()
    {
        browsingIndex = SaveManager.GetSelectedCar();

        if (selectButton != null) selectButton.onClick.AddListener(OnSelectClicked);
        if (prevCarButton != null) prevCarButton.onClick.AddListener(OnPrevClicked);
        if (nextCarButton != null) nextCarButton.onClick.AddListener(OnNextClicked);
        if (backButton != null) backButton.onClick.AddListener(OnBackClicked);

        UpdateGarageView();
    }

    private void Update()
    {
        // Smoothly rotate the turntable platform showcasing the vehicle
        if (turntablePlatform != null)
        {
            turntablePlatform.Rotate(Vector3.up, turntableRotateSpeed * Time.deltaTime, Space.World);
        }
    }

    public void UpdateGarageView()
    {
        CarStats car = GarageManager.Instance != null ? GarageManager.Instance.GetCar(browsingIndex) : CarStats.GetDefaultCars()[browsingIndex];
        bool isUnlocked = GarageManager.Instance != null ? GarageManager.Instance.IsCarUnlocked(browsingIndex) : (browsingIndex == 0);
        bool isSelected = SaveManager.GetSelectedCar() == browsingIndex;

        if (carNameText != null) carNameText.text = car.carName;
        if (carCategoryText != null) carCategoryText.text = car.category;

        // Stats bars (normalized 0 to 1)
        if (speedSlider != null) speedSlider.value = car.maxSpeed / 18f;
        if (handlingSlider != null) handlingSlider.value = car.steerAngle / 45f;
        if (brakingSlider != null) brakingSlider.value = car.brakeForce / 25f;

        // Lock / Select status
        if (unlockRequirementText != null)
        {
            if (isUnlocked)
            {
                unlockRequirementText.text = isSelected ? "<color=#2ECC71>EQUIPPED</color>" : "<color=#3498DB>UNLOCKED</color>";
            }
            else
            {
                unlockRequirementText.text = "<color=#E74C3C>🔒 " + LevelUnlockManager.GetCarUnlockRequirementDescription(browsingIndex) + "</color>";
            }
        }

        if (selectButton != null)
        {
            selectButton.interactable = isUnlocked && !isSelected;
            if (selectButtonText != null)
            {
                if (isSelected) selectButtonText.text = "EQUIPPED";
                else if (isUnlocked) selectButtonText.text = "SELECT CAR";
                else selectButtonText.text = "LOCKED";
            }
        }

        // Toggle 3D preview model
        if (carPreviewModels != null)
        {
            for (int i = 0; i < carPreviewModels.Length; i++)
            {
                if (carPreviewModels[i] != null)
                {
                    carPreviewModels[i].SetActive(i == browsingIndex);
                }
            }
        }
    }

    public void OnPrevClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        browsingIndex--;
        if (browsingIndex < 0) browsingIndex = 2;
        UpdateGarageView();
    }

    public void OnNextClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        browsingIndex++;
        if (browsingIndex > 2) browsingIndex = 0;
        UpdateGarageView();
    }

    public void OnSelectClicked()
    {
        if (GarageManager.Instance != null && GarageManager.Instance.SelectCar(browsingIndex))
        {
            AudioManager.Instance?.PlayParkingSuccess();
            UpdateGarageView();
        }
    }

    public void OnBackClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        LevelManager.Instance?.LoadMainMenu();
    }
}
