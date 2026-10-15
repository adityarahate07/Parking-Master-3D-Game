using UnityEngine;

/// <summary>
/// Data defining vehicle attributes for the Garage and CarController.
/// </summary>
[System.Serializable]
public class CarStats
{
    public string carName;
    public string category; // Starter, Sport, SUV
    public int unlockLevelRequirement;
    public float maxSpeed;
    public float acceleration;
    public float steerAngle;
    public float brakeForce;
    public Color bodyColor;
    public Vector3 carDimensions;

    public static CarStats[] GetDefaultCars()
    {
        return new CarStats[]
        {
            new CarStats
            {
                carName = "Apex Cruiser",
                category = "Starter Car",
                unlockLevelRequirement = 1, // Free
                maxSpeed = 12f,
                acceleration = 9f,
                steerAngle = 36f,
                brakeForce = 18f,
                bodyColor = new Color(0.92f, 0.16f, 0.16f), // Sport Red
                carDimensions = new Vector3(1.8f, 1.4f, 4.0f)
            },
            new CarStats
            {
                carName = "Veloce GT",
                category = "Sport Car",
                unlockLevelRequirement = 3, // Unlock after Level 3
                maxSpeed = 16f,
                acceleration = 13f,
                steerAngle = 40f,
                brakeForce = 22f,
                bodyColor = new Color(0.12f, 0.48f, 0.95f), // Electric Blue
                carDimensions = new Vector3(1.85f, 1.25f, 4.2f)
            },
            new CarStats
            {
                carName = "Titan Ridge 4x4",
                category = "SUV",
                unlockLevelRequirement = 7, // Unlock after Level 7
                maxSpeed = 11f,
                acceleration = 8f,
                steerAngle = 33f,
                brakeForce = 24f,
                bodyColor = new Color(0.14f, 0.14f, 0.16f), // Obsidian Black
                carDimensions = new Vector3(2.0f, 1.7f, 4.4f)
            }
        };
    }
}
