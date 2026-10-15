using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data configuration for each level in Parking Master 3D.
/// Contains road layout parameters, spawn points, obstacles, and parking zone specifications.
/// </summary>
[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public string levelName;
    public string subtitle;
    public float timeLimitSeconds;
    public int maxCollisions;
    public bool isNightLevel;
    public bool requireReverseParking;

    // Player spawn
    public Vector3 playerStartPosition;
    public Vector3 playerStartRotation;

    // Target Parking Space
    public Vector3 parkingTargetPosition;
    public Vector3 parkingTargetRotation; // Target orientation (degrees)
    public Vector3 parkingZoneSize;       // Width, Height, Length

    // Obstacle placement data
    public List<ObstaclePlacement> obstacles = new List<ObstaclePlacement>();

    // Moving obstacles
    public List<MovingObstaclePlacement> movingObstacles = new List<MovingObstaclePlacement>();

    [System.Serializable]
    public class ObstaclePlacement
    {
        public enum ObstacleType { TrafficCone, ConcreteBarrier, ParkedCar, Wall }
        public ObstacleType type;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public int carColorIndex; // for parked cars
    }

    [System.Serializable]
    public class MovingObstaclePlacement
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public float speed;
    }

    /// <summary>
    /// Returns the configuration for any level 1-10.
    /// </summary>
    public static LevelData GetLevel(int level)
    {
        LevelData data = new LevelData();
        data.levelNumber = level;
        data.parkingZoneSize = new Vector3(2.6f, 0.5f, 5.0f);

        switch (level)
        {
            case 1:
                data.levelName = "Easy Parking";
                data.subtitle = "Straight road, wide parking bay";
                data.timeLimitSeconds = 90f;
                data.maxCollisions = 3;
                data.isNightLevel = false;
                data.requireReverseParking = false;
                data.playerStartPosition = new Vector3(0, 0.1f, -18f);
                data.playerStartRotation = Vector3.zero;
                data.parkingTargetPosition = new Vector3(0, 0.05f, 10f);
                data.parkingTargetRotation = Vector3.zero;
                data.parkingZoneSize = new Vector3(3.2f, 0.5f, 5.5f);
                // Simple soft guide cones
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(-2.8f, 0, 8f), rotation = Vector3.zero, scale = Vector3.one });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(2.8f, 0, 8f), rotation = Vector3.zero, scale = Vector3.one });
                break;

            case 2:
                data.levelName = "Basic Turning";
                data.subtitle = "90-degree right turn between parked cars";
                data.timeLimitSeconds = 90f;
                data.maxCollisions = 3;
                data.isNightLevel = false;
                data.requireReverseParking = false;
                data.playerStartPosition = new Vector3(0, 0.1f, -16f);
                data.playerStartRotation = Vector3.zero;
                data.parkingTargetPosition = new Vector3(10f, 0.05f, 0f);
                data.parkingTargetRotation = new Vector3(0, 90f, 0);
                // Parked cars on sides of target
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(10f, 0, 3.2f), rotation = new Vector3(0, 90f, 0), scale = Vector3.one, carColorIndex = 0 });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(10f, 0, -3.2f), rotation = new Vector3(0, 90f, 0), scale = Vector3.one, carColorIndex = 1 });
                // Corner curb cones
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(-2f, 0, 4f), rotation = Vector3.zero, scale = Vector3.one });
                break;

            case 3:
                data.levelName = "Narrow Parking";
                data.subtitle = "Thread through tight cone alleyway";
                data.timeLimitSeconds = 80f;
                data.maxCollisions = 3;
                data.isNightLevel = false;
                data.requireReverseParking = false;
                data.playerStartPosition = new Vector3(0, 0.1f, -18f);
                data.playerStartRotation = Vector3.zero;
                data.parkingTargetPosition = new Vector3(0, 0.05f, 12f);
                data.parkingTargetRotation = Vector3.zero;
                data.parkingZoneSize = new Vector3(2.5f, 0.5f, 4.8f);
                // Narrow corridor of cones
                for (float z = -10f; z <= 8f; z += 3.5f)
                {
                    data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(-1.7f, 0, z), rotation = Vector3.zero, scale = Vector3.one });
                    data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(1.7f, 0, z), rotation = Vector3.zero, scale = Vector3.one });
                }
                break;

            case 4:
                data.levelName = "Reverse Parking";
                data.subtitle = "Back into the bay carefully";
                data.timeLimitSeconds = 90f;
                data.maxCollisions = 3;
                data.isNightLevel = false;
                data.requireReverseParking = true;
                data.playerStartPosition = new Vector3(0, 0.1f, 6f);
                data.playerStartRotation = Vector3.zero;
                data.parkingTargetPosition = new Vector3(0, 0.05f, -12f);
                data.parkingTargetRotation = Vector3.zero; // facing forward means car reversed in (heading 0)
                // Flanking parked cars
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(-2.8f, 0, -12f), rotation = Vector3.zero, scale = Vector3.one, carColorIndex = 2 });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(2.8f, 0, -12f), rotation = Vector3.zero, scale = Vector3.one, carColorIndex = 0 });
                // Rear wall
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ConcreteBarrier, position = new Vector3(0, 0, -15.5f), rotation = Vector3.zero, scale = new Vector3(3f, 1f, 1f) });
                break;

            case 5:
                data.levelName = "Parallel Parking";
                data.subtitle = "Parallel park along the street curb";
                data.timeLimitSeconds = 85f;
                data.maxCollisions = 3;
                data.isNightLevel = false;
                data.requireReverseParking = false;
                data.playerStartPosition = new Vector3(3.5f, 0.1f, -14f);
                data.playerStartRotation = Vector3.zero;
                data.parkingTargetPosition = new Vector3(-1.0f, 0.05f, 2f);
                data.parkingTargetRotation = Vector3.zero;
                data.parkingZoneSize = new Vector3(2.4f, 0.5f, 6.0f);
                // Front and rear parked cars
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(-1.0f, 0, 7.8f), rotation = Vector3.zero, scale = Vector3.one, carColorIndex = 1 });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(-1.0f, 0, -4.0f), rotation = Vector3.zero, scale = Vector3.one, carColorIndex = 2 });
                // Curb barrier
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ConcreteBarrier, position = new Vector3(-3.0f, 0, 2f), rotation = new Vector3(0, 90f, 0), scale = new Vector3(12f, 1f, 1f) });
                break;

            case 6:
                data.levelName = "Multi-Turn Parking";
                data.subtitle = "Navigate S-curve chicane to parking spot";
                data.timeLimitSeconds = 100f;
                data.maxCollisions = 3;
                data.isNightLevel = false;
                data.requireReverseParking = false;
                data.playerStartPosition = new Vector3(-8f, 0.1f, -18f);
                data.playerStartRotation = Vector3.zero;
                data.parkingTargetPosition = new Vector3(8f, 0.05f, 14f);
                data.parkingTargetRotation = Vector3.zero;
                // S-curve chicane barriers
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ConcreteBarrier, position = new Vector3(-3f, 0, -8f), rotation = Vector3.zero, scale = new Vector3(6f, 1f, 1f) });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ConcreteBarrier, position = new Vector3(3f, 0, 2f), rotation = Vector3.zero, scale = new Vector3(6f, 1f, 1f) });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(5f, 0, 11f), rotation = Vector3.zero, scale = Vector3.one });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(11f, 0, 11f), rotation = Vector3.zero, scale = Vector3.one });
                break;

            case 7:
                data.levelName = "Obstacle Parking";
                data.subtitle = "Avoid cones, barriers, and moving obstacle";
                data.timeLimitSeconds = 90f;
                data.maxCollisions = 3;
                data.isNightLevel = false;
                data.requireReverseParking = false;
                data.playerStartPosition = new Vector3(0, 0.1f, -18f);
                data.playerStartRotation = Vector3.zero;
                data.parkingTargetPosition = new Vector3(0, 0.05f, 15f);
                data.parkingTargetRotation = Vector3.zero;
                // Moving patrol barrier
                data.movingObstacles.Add(new MovingObstaclePlacement
                {
                    startPos = new Vector3(-6f, 0.5f, 0f),
                    endPos = new Vector3(6f, 0.5f, 0f),
                    speed = 2.5f
                });
                // Cones
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(-2.5f, 0, -8f), rotation = Vector3.zero, scale = Vector3.one });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(2.5f, 0, -8f), rotation = Vector3.zero, scale = Vector3.one });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ConcreteBarrier, position = new Vector3(-4f, 0, 8f), rotation = Vector3.zero, scale = new Vector3(3f, 1f, 1f) });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ConcreteBarrier, position = new Vector3(4f, 0, 8f), rotation = Vector3.zero, scale = new Vector3(3f, 1f, 1f) });
                break;

            case 8:
                data.levelName = "Night Parking";
                data.subtitle = "Dark lot with street lights & headlights";
                data.timeLimitSeconds = 85f;
                data.maxCollisions = 3;
                data.isNightLevel = true;
                data.requireReverseParking = false;
                data.playerStartPosition = new Vector3(-10f, 0.1f, -12f);
                data.playerStartRotation = new Vector3(0, 45f, 0);
                data.parkingTargetPosition = new Vector3(8f, 0.05f, 8f);
                data.parkingTargetRotation = new Vector3(0, 90f, 0);
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(8f, 0, 11.5f), rotation = new Vector3(0, 90f, 0), scale = Vector3.one, carColorIndex = 0 });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(8f, 0, 4.5f), rotation = new Vector3(0, 90f, 0), scale = Vector3.one, carColorIndex = 2 });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(-2f, 0, 0), rotation = Vector3.zero, scale = Vector3.one });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(2f, 0, -4f), rotation = Vector3.zero, scale = Vector3.one });
                break;

            case 9:
                data.levelName = "Advanced Parking";
                data.subtitle = "Tight space crowded by multiple vehicles";
                data.timeLimitSeconds = 80f;
                data.maxCollisions = 3;
                data.isNightLevel = false;
                data.requireReverseParking = false;
                data.playerStartPosition = new Vector3(12f, 0.1f, -16f);
                data.playerStartRotation = new Vector3(0, -90f, 0);
                data.parkingTargetPosition = new Vector3(-8f, 0.05f, 6f);
                data.parkingTargetRotation = new Vector3(0, 0, 0);
                data.parkingZoneSize = new Vector3(2.4f, 0.5f, 4.8f);
                // Parked cars
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(-11f, 0, 6f), rotation = Vector3.zero, scale = Vector3.one, carColorIndex = 1 });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(-5f, 0, 6f), rotation = Vector3.zero, scale = Vector3.one, carColorIndex = 0 });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(0, 0, -6f), rotation = new Vector3(0, 90f, 0), scale = Vector3.one, carColorIndex = 2 });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ConcreteBarrier, position = new Vector3(-8f, 0, 9.5f), rotation = Vector3.zero, scale = new Vector3(3f, 1f, 1f) });
                break;

            case 10:
            default:
                data.levelName = "Parking Master";
                data.subtitle = "The ultimate test: tight reverse park with moving obstacle";
                data.timeLimitSeconds = 110f;
                data.maxCollisions = 3;
                data.isNightLevel = false;
                data.requireReverseParking = true;
                data.playerStartPosition = new Vector3(-12f, 0.1f, -18f);
                data.playerStartRotation = Vector3.zero;
                data.parkingTargetPosition = new Vector3(10f, 0.05f, 12f);
                data.parkingTargetRotation = new Vector3(0, 180f, 0);
                data.parkingZoneSize = new Vector3(2.4f, 0.5f, 4.8f);
                // Flanking cars
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(7.2f, 0, 12f), rotation = new Vector3(0, 180f, 0), scale = Vector3.one, carColorIndex = 0 });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ParkedCar, position = new Vector3(12.8f, 0, 12f), rotation = new Vector3(0, 180f, 0), scale = Vector3.one, carColorIndex = 2 });
                // Moving obstacle in central corridor
                data.movingObstacles.Add(new MovingObstaclePlacement
                {
                    startPos = new Vector3(-4f, 0.5f, -2f),
                    endPos = new Vector3(6f, 0.5f, -2f),
                    speed = 2.0f
                });
                // Cones & barriers
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(-6f, 0, 5f), rotation = Vector3.zero, scale = Vector3.one });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.TrafficCone, position = new Vector3(0, 0, 8f), rotation = Vector3.zero, scale = Vector3.one });
                data.obstacles.Add(new ObstaclePlacement { type = ObstaclePlacement.ObstacleType.ConcreteBarrier, position = new Vector3(10f, 0, 15.5f), rotation = Vector3.zero, scale = new Vector3(3.5f, 1f, 1f) });
                break;
        }

        return data;
    }
}
