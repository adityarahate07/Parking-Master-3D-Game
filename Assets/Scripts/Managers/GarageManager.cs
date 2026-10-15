using UnityEngine;

/// <summary>
/// Manages vehicle catalog, selection, and unlocking states for the Garage screen.
/// </summary>
public class GarageManager : MonoBehaviour
{
    public static GarageManager Instance { get; private set; }

    public CarStats[] allCars;
    public int currentCarIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            allCars = CarStats.GetDefaultCars();
            currentCarIndex = SaveManager.GetSelectedCar();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public CarStats GetCurrentCar()
    {
        if (allCars == null || allCars.Length == 0) allCars = CarStats.GetDefaultCars();
        int idx = Mathf.Clamp(currentCarIndex, 0, allCars.Length - 1);
        return allCars[idx];
    }

    public CarStats GetCar(int index)
    {
        if (allCars == null || allCars.Length == 0) allCars = CarStats.GetDefaultCars();
        int idx = Mathf.Clamp(index, 0, allCars.Length - 1);
        return allCars[idx];
    }

    public bool IsCarUnlocked(int index)
    {
        return LevelUnlockManager.IsCarUnlocked(index);
    }

    public bool SelectCar(int index)
    {
        if (IsCarUnlocked(index))
        {
            currentCarIndex = index;
            SaveManager.SetSelectedCar(index);
            return true;
        }
        return false;
    }
}
