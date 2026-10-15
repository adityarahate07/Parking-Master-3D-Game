using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dynamically constructs the complete 3D environment, asphalt roadways,
/// parking lines, street lights, obstacles, target parking zone, and player vehicle.
/// Guarantees that any level 1-10 functions completely whether running in editor or on Android.
/// </summary>
public class LevelEnvironmentBuilder : MonoBehaviour
{
    [Header("Environment Materials")]
    public Material asphaltMat;
    public Material whiteLineMat;
    public Material yellowLineMat;
    public Material sidewalkMat;
    public Material curbMat;
    public Material wallMat;
    public Material parkingGuideMat;
    public Material coneOrangeMat;
    public Material coneWhiteMat;
    public Material barrierMat;
    public Material streetPoleMat;
    public Material foliageMat;
    public Material trunkMat;

    [Header("Vehicle Materials")]
    public Material[] carPaintMaterials;
    public Material carGlassMat;
    public Material carTireMat;
    public Material carRimMat;
    public Material headlightMat;
    public Material taillightMat;

    private GameObject levelEnvironmentRoot;
    private GameObject playerCarInstance;
    private Light directionalSunlight;

    private void Awake()
    {
        LoadAllMaterials();
    }

    private Material SafeLoadMaterial(string resPath, Color fallbackColor, float metallic = 0.1f, float smoothness = 0.2f)
    {
        Material mat = Resources.Load<Material>(resPath);
        if (mat != null) return mat;

        Shader shader = Shader.Find("Standard") ?? Shader.Find("Mobile/Diffuse") ?? Shader.Find("Diffuse");
        mat = new Material(shader);
        mat.color = fallbackColor;
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", fallbackColor);
        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
        return mat;
    }

    private void LoadAllMaterials()
    {
        asphaltMat = SafeLoadMaterial("Materials/Road_Asphalt", new Color(0.18f, 0.19f, 0.22f), 0.1f, 0.2f);
        whiteLineMat = SafeLoadMaterial("Materials/Road_Marking_White", new Color(0.95f, 0.95f, 0.95f), 0.1f, 0.5f);
        yellowLineMat = SafeLoadMaterial("Materials/Road_Marking_Yellow", new Color(0.95f, 0.8f, 0.1f), 0.1f, 0.5f);
        sidewalkMat = SafeLoadMaterial("Materials/Concrete_Sidewalk", new Color(0.55f, 0.55f, 0.55f), 0.0f, 0.1f);
        curbMat = SafeLoadMaterial("Materials/Concrete_Curb", new Color(0.45f, 0.45f, 0.48f), 0.0f, 0.1f);
        wallMat = SafeLoadMaterial("Materials/Building_Wall", new Color(0.35f, 0.38f, 0.42f), 0.0f, 0.1f);
        parkingGuideMat = SafeLoadMaterial("Materials/Parking_Zone_Guide", new Color(0.2f, 0.95f, 0.4f, 0.45f), 0.0f, 0.8f);
        coneOrangeMat = SafeLoadMaterial("Materials/Traffic_Cone_Orange", new Color(0.98f, 0.38f, 0.05f), 0.0f, 0.4f);
        coneWhiteMat = SafeLoadMaterial("Materials/Traffic_Cone_White", new Color(0.95f, 0.95f, 0.95f), 0.0f, 0.4f);
        barrierMat = SafeLoadMaterial("Materials/Barrier_Concrete", new Color(0.6f, 0.6f, 0.62f), 0.0f, 0.1f);
        streetPoleMat = SafeLoadMaterial("Materials/Street_Pole", new Color(0.25f, 0.28f, 0.3f), 0.6f, 0.7f);
        foliageMat = SafeLoadMaterial("Materials/Tree_Foliage", new Color(0.18f, 0.55f, 0.2f), 0.0f, 0.1f);
        trunkMat = SafeLoadMaterial("Materials/Tree_Trunk", new Color(0.4f, 0.26f, 0.15f), 0.0f, 0.1f);

        carGlassMat = SafeLoadMaterial("Materials/Car_Glass", new Color(0.1f, 0.25f, 0.4f, 0.7f), 0.8f, 0.95f);
        carTireMat = SafeLoadMaterial("Materials/Car_Tire_Rubber", new Color(0.12f, 0.12f, 0.12f), 0.0f, 0.1f);
        carRimMat = SafeLoadMaterial("Materials/Car_Rim_Chrome", new Color(0.85f, 0.85f, 0.88f), 0.9f, 0.85f);
        headlightMat = SafeLoadMaterial("Materials/Car_Headlight_On", new Color(1.0f, 0.98f, 0.85f), 0.0f, 0.9f);
        taillightMat = SafeLoadMaterial("Materials/Car_Taillight_On", new Color(0.9f, 0.1f, 0.1f), 0.0f, 0.9f);

        carPaintMaterials = new Material[]
        {
            SafeLoadMaterial("Materials/Car_Starter_Red", new Color(0.92f, 0.16f, 0.16f), 0.4f, 0.7f),
            SafeLoadMaterial("Materials/Car_Sport_Blue", new Color(0.12f, 0.48f, 0.95f), 0.6f, 0.8f),
            SafeLoadMaterial("Materials/Car_SUV_Black", new Color(0.14f, 0.14f, 0.16f), 0.3f, 0.6f),
            SafeLoadMaterial("Materials/ParkedCar_Silver", new Color(0.75f, 0.75f, 0.78f), 0.7f, 0.7f),
            SafeLoadMaterial("Materials/ParkedCar_Navy", new Color(0.08f, 0.15f, 0.35f), 0.4f, 0.6f),
            SafeLoadMaterial("Materials/ParkedCar_White", new Color(0.92f, 0.92f, 0.94f), 0.3f, 0.7f)
        };
    }

    /// <summary>
    /// Builds the complete level from LevelData.
    /// </summary>
    public void BuildLevel(int levelNumber)
    {
        LevelData data = LevelData.GetLevel(levelNumber);

        // Clean previous environment
        if (levelEnvironmentRoot != null) Destroy(levelEnvironmentRoot);
        if (playerCarInstance != null) Destroy(playerCarInstance);

        levelEnvironmentRoot = new GameObject("Environment_Level_" + levelNumber);

        // 1. Setup Lighting
        SetupLighting(data.isNightLevel);

        // 2. Build Parking Lot Ground & Perimeter
        BuildParkingLotBase();

        // 3. Build Target Parking Zone
        BuildTargetParkingZone(data);

        // 4. Build Obstacles (Cones, Barriers, Parked Cars)
        BuildObstacles(data);

        // 5. Build Moving Obstacles
        BuildMovingObstacles(data);

        // 6. Spawn Player Car & Attach Camera / Mobile Controls
        SpawnPlayerCar(data);
    }

    private void SetupLighting(bool isNight)
    {
        if (directionalSunlight == null)
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional)
                {
                    directionalSunlight = l;
                    break;
                }
            }
            if (directionalSunlight == null)
            {
                GameObject sunObj = new GameObject("Directional Light");
                directionalSunlight = sunObj.AddComponent<Light>();
                directionalSunlight.type = LightType.Directional;
            }
        }

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = isNight ? new Color(0.06f, 0.08f, 0.12f) : new Color(0.18f, 0.22f, 0.28f);
        }

        if (isNight)
        {
            RenderSettings.ambientLight = new Color(0.12f, 0.15f, 0.22f);
            directionalSunlight.color = new Color(0.3f, 0.4f, 0.6f);
            directionalSunlight.intensity = 0.25f;
            directionalSunlight.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        }
        else
        {
            RenderSettings.ambientLight = new Color(0.55f, 0.58f, 0.62f);
            directionalSunlight.color = new Color(1.0f, 0.96f, 0.88f);
            directionalSunlight.intensity = 1.15f;
            directionalSunlight.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        }
    }

    private void BuildParkingLotBase()
    {
        // Main Asphalt Ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Asphalt_Ground";
        ground.transform.SetParent(levelEnvironmentRoot.transform);
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(8f, 1f, 8f); // 80x80 meters
        if (asphaltMat != null) ground.GetComponent<Renderer>().material = asphaltMat;
        ground.tag = GameConstants.TAG_GROUND;

        // Perimeter Walls
        float wallLength = 70f;
        CreateWall(new Vector3(0, 1.2f, 35f), new Vector3(wallLength, 2.4f, 1f));
        CreateWall(new Vector3(0, 1.2f, -35f), new Vector3(wallLength, 2.4f, 1f));
        CreateWall(new Vector3(35f, 1.2f, 0), new Vector3(1f, 2.4f, wallLength));
        CreateWall(new Vector3(-35f, 1.2f, 0), new Vector3(1f, 2.4f, wallLength));

        // Decorative Street Lamps
        Vector3[] lampPositions = new Vector3[]
        {
            new Vector3(-18f, 0, -18f),
            new Vector3(18f, 0, -18f),
            new Vector3(-18f, 0, 18f),
            new Vector3(18f, 0, 18f),
            new Vector3(0, 0, 25f),
            new Vector3(0, 0, -25f)
        };
        foreach (var pos in lampPositions)
        {
            CreateStreetLamp(pos);
        }

        // Sidewalk curbs and trees around edges
        for (float z = -25f; z <= 25f; z += 12f)
        {
            CreateTree(new Vector3(-30f, 0, z));
            CreateTree(new Vector3(30f, 0, z));
        }
    }

    private void CreateWall(Vector3 pos, Vector3 size)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Perimeter_Wall";
        wall.transform.SetParent(levelEnvironmentRoot.transform);
        wall.transform.position = pos;
        wall.transform.localScale = size;
        if (wallMat != null) wall.GetComponent<Renderer>().material = wallMat;
        wall.tag = GameConstants.TAG_OBSTACLE;
        wall.AddComponent<Obstacle>().hazardType = Obstacle.HazardType.Wall;
    }

    private void CreateStreetLamp(Vector3 pos)
    {
        GameObject lamp = new GameObject("StreetLamp");
        lamp.transform.SetParent(levelEnvironmentRoot.transform);
        lamp.transform.position = pos;

        // Pole
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.transform.SetParent(lamp.transform);
        pole.transform.localPosition = new Vector3(0, 3f, 0);
        pole.transform.localScale = new Vector3(0.25f, 3f, 0.25f);
        if (streetPoleMat != null) pole.GetComponent<Renderer>().material = streetPoleMat;

        // Light Head
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.transform.SetParent(lamp.transform);
        head.transform.localPosition = new Vector3(0, 6f, 0.4f);
        head.transform.localScale = new Vector3(0.6f, 0.25f, 1.0f);
        if (headlightMat != null) head.GetComponent<Renderer>().material = headlightMat;

        // Spotlight
        GameObject lightObj = new GameObject("LampSpotlight");
        lightObj.transform.SetParent(head.transform);
        lightObj.transform.localPosition = Vector3.zero;
        lightObj.transform.localRotation = Quaternion.Euler(75f, 0, 0);
        Light spot = lightObj.AddComponent<Light>();
        spot.type = LightType.Spot;
        spot.range = 18f;
        spot.spotAngle = 65f;
        spot.color = new Color(1.0f, 0.95f, 0.8f);
        spot.intensity = 2.5f;
    }

    private void CreateTree(Vector3 pos)
    {
        GameObject tree = new GameObject("Deco_Tree");
        tree.transform.SetParent(levelEnvironmentRoot.transform);
        tree.transform.position = pos;

        // Trunk
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
        trunk.transform.localScale = new Vector3(0.4f, 1.5f, 0.4f);
        if (trunkMat != null) trunk.GetComponent<Renderer>().material = trunkMat;

        // Foliage
        GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage.transform.SetParent(tree.transform);
        foliage.transform.localPosition = new Vector3(0, 3.8f, 0);
        foliage.transform.localScale = new Vector3(2.5f, 2.8f, 2.5f);
        if (foliageMat != null) foliage.GetComponent<Renderer>().material = foliageMat;
    }

    private void BuildTargetParkingZone(LevelData data)
    {
        GameObject zoneObj = new GameObject("TargetParkingZone");
        zoneObj.transform.SetParent(levelEnvironmentRoot.transform);
        zoneObj.transform.position = data.parkingTargetPosition;
        zoneObj.transform.rotation = Quaternion.Euler(data.parkingTargetRotation);
        zoneObj.tag = GameConstants.TAG_PARKING_ZONE;

        ParkingZone pz = zoneObj.AddComponent<ParkingZone>();
        pz.SetupZone(data.parkingZoneSize, data.parkingTargetRotation.y, data.requireReverseParking);

        // Ground Guide Quad / Box
        GameObject visualBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visualBox.name = "ZoneGuideVisual";
        visualBox.transform.SetParent(zoneObj.transform);
        visualBox.transform.localPosition = new Vector3(0, 0.04f, 0);
        visualBox.transform.localScale = new Vector3(data.parkingZoneSize.x, 0.08f, data.parkingZoneSize.z);
        if (parkingGuideMat != null) visualBox.GetComponent<Renderer>().material = parkingGuideMat;
        Destroy(visualBox.GetComponent<Collider>()); // Trigger collider is on parent
        pz.zoneMeshRenderer = visualBox.GetComponent<Renderer>();

        // Yellow Parking Lines
        CreateParkingLines(zoneObj.transform, data.parkingZoneSize);

        // Pulsing Point Light
        GameObject lightObj = new GameObject("ZoneGlowLight");
        lightObj.transform.SetParent(zoneObj.transform);
        lightObj.transform.localPosition = new Vector3(0, 0.8f, 0);
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 7f;
        light.color = new Color(0.2f, 1.0f, 0.4f);
        light.intensity = 2.0f;
        pz.pulseLight = light;

        ParkingManager.Instance?.SetActiveZone(pz);
    }

    private void CreateParkingLines(Transform parent, Vector3 size)
    {
        float w = size.x;
        float l = size.z;
        float lineThickness = 0.12f;

        // Left Line
        CreateLineStrip(parent, new Vector3(-w * 0.5f, 0.05f, 0), new Vector3(lineThickness, 0.02f, l));
        // Right Line
        CreateLineStrip(parent, new Vector3(w * 0.5f, 0.05f, 0), new Vector3(lineThickness, 0.02f, l));
        // Back Line / Stopper
        CreateLineStrip(parent, new Vector3(0, 0.05f, l * 0.5f), new Vector3(w, 0.02f, lineThickness));
    }

    private void CreateLineStrip(Transform parent, Vector3 localPos, Vector3 localScale)
    {
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line.name = "ParkingLine";
        line.transform.SetParent(parent);
        line.transform.localPosition = localPos;
        line.transform.localScale = localScale;
        if (yellowLineMat != null) line.GetComponent<Renderer>().material = yellowLineMat;
        Destroy(line.GetComponent<Collider>());
    }

    private void BuildObstacles(LevelData data)
    {
        foreach (var obs in data.obstacles)
        {
            switch (obs.type)
            {
                case LevelData.ObstaclePlacement.ObstacleType.TrafficCone:
                    CreateTrafficCone(obs.position);
                    break;
                case LevelData.ObstaclePlacement.ObstacleType.ConcreteBarrier:
                    CreateConcreteBarrier(obs.position, obs.rotation, obs.scale);
                    break;
                case LevelData.ObstaclePlacement.ObstacleType.ParkedCar:
                    CreateParkedCar(obs.position, obs.rotation, obs.carColorIndex);
                    break;
                case LevelData.ObstaclePlacement.ObstacleType.Wall:
                    CreateWall(obs.position, obs.scale);
                    break;
            }
        }
    }

    private void CreateTrafficCone(Vector3 pos)
    {
        GameObject cone = new GameObject("TrafficCone");
        cone.transform.SetParent(levelEnvironmentRoot.transform);
        cone.transform.position = pos;
        cone.tag = GameConstants.TAG_CONE;

        // Cone Base
        GameObject baseBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseBox.transform.SetParent(cone.transform);
        baseBox.transform.localPosition = new Vector3(0, 0.04f, 0);
        baseBox.transform.localScale = new Vector3(0.5f, 0.08f, 0.5f);
        if (coneOrangeMat != null) baseBox.GetComponent<Renderer>().material = coneOrangeMat;
        Destroy(baseBox.GetComponent<Collider>());

        // Cone Body
        GameObject coneBody = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        coneBody.transform.SetParent(cone.transform);
        coneBody.transform.localPosition = new Vector3(0, 0.45f, 0);
        coneBody.transform.localScale = new Vector3(0.35f, 0.45f, 0.35f);
        if (coneOrangeMat != null) coneBody.GetComponent<Renderer>().material = coneOrangeMat;
        Destroy(coneBody.GetComponent<Collider>());

        // White Stripe
        GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stripe.transform.SetParent(cone.transform);
        stripe.transform.localPosition = new Vector3(0, 0.5f, 0);
        stripe.transform.localScale = new Vector3(0.36f, 0.12f, 0.36f);
        if (coneWhiteMat != null) stripe.GetComponent<Renderer>().material = coneWhiteMat;
        Destroy(stripe.GetComponent<Collider>());

        // Collider & Obstacle script
        CapsuleCollider col = cone.AddComponent<CapsuleCollider>();
        col.height = 0.9f;
        col.radius = 0.25f;
        col.center = new Vector3(0, 0.45f, 0);

        cone.AddComponent<Obstacle>().hazardType = Obstacle.HazardType.TrafficCone;
    }

    private void CreateConcreteBarrier(Vector3 pos, Vector3 rot, Vector3 scale)
    {
        GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
        barrier.name = "ConcreteBarrier";
        barrier.transform.SetParent(levelEnvironmentRoot.transform);
        barrier.transform.position = pos + Vector3.up * 0.45f;
        barrier.transform.rotation = Quaternion.Euler(rot);
        barrier.transform.localScale = new Vector3(scale.x * 2.2f, 0.9f, 0.6f);
        if (barrierMat != null) barrier.GetComponent<Renderer>().material = barrierMat;
        barrier.tag = GameConstants.TAG_BARRIER;
        barrier.AddComponent<Obstacle>().hazardType = Obstacle.HazardType.Barrier;
    }

    private void CreateParkedCar(Vector3 pos, Vector3 rot, int colorIndex)
    {
        GameObject car = new GameObject("ParkedCar");
        car.transform.SetParent(levelEnvironmentRoot.transform);
        car.transform.position = pos;
        car.transform.rotation = Quaternion.Euler(rot);
        car.tag = GameConstants.TAG_PARKED_CAR;

        // Body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.transform.SetParent(car.transform);
        body.transform.localPosition = new Vector3(0, 0.55f, 0);
        body.transform.localScale = new Vector3(1.8f, 0.7f, 3.8f);
        int matIdx = Mathf.Clamp(colorIndex + 3, 0, carPaintMaterials.Length - 1);
        if (carPaintMaterials != null && carPaintMaterials[matIdx] != null)
        {
            body.GetComponent<Renderer>().material = carPaintMaterials[matIdx];
        }

        // Cabin
        GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cabin.transform.SetParent(car.transform);
        cabin.transform.localPosition = new Vector3(0, 1.05f, -0.2f);
        cabin.transform.localScale = new Vector3(1.5f, 0.6f, 2.1f);
        if (carGlassMat != null) cabin.GetComponent<Renderer>().material = carGlassMat;

        // Wheels
        CreateWheelVisual(car.transform, new Vector3(-0.9f, 0.35f, 1.1f));
        CreateWheelVisual(car.transform, new Vector3(0.9f, 0.35f, 1.1f));
        CreateWheelVisual(car.transform, new Vector3(-0.9f, 0.35f, -1.1f));
        CreateWheelVisual(car.transform, new Vector3(0.9f, 0.35f, -1.1f));

        // Collider
        BoxCollider col = car.AddComponent<BoxCollider>();
        col.size = new Vector3(1.9f, 1.4f, 4.0f);
        col.center = new Vector3(0, 0.7f, 0);

        car.AddComponent<Obstacle>().hazardType = Obstacle.HazardType.ParkedCar;
    }

    private void CreateMovingObstacle(LevelData.MovingObstaclePlacement p)
    {
        GameObject moving = GameObject.CreatePrimitive(PrimitiveType.Cube);
        moving.name = "MovingObstacle";
        moving.transform.SetParent(levelEnvironmentRoot.transform);
        moving.transform.localScale = new Vector3(1.6f, 1.1f, 1.6f);
        if (coneOrangeMat != null) moving.GetComponent<Renderer>().material = coneOrangeMat;

        moving.tag = GameConstants.TAG_OBSTACLE;
        moving.AddComponent<Obstacle>().hazardType = Obstacle.HazardType.Barrier;

        MovingObstacle mover = moving.AddComponent<MovingObstacle>();
        mover.Setup(p.startPos, p.endPos, p.speed);
    }

    private void BuildMovingObstacles(LevelData data)
    {
        foreach (var m in data.movingObstacles)
        {
            CreateMovingObstacle(m);
        }
    }

    private void SpawnPlayerCar(LevelData data)
    {
        int selectedCarIndex = SaveManager.GetSelectedCar();
        CarStats stats = GarageManager.Instance != null ? GarageManager.Instance.GetCar(selectedCarIndex) : CarStats.GetDefaultCars()[selectedCarIndex];

        playerCarInstance = new GameObject("PlayerCar");
        playerCarInstance.transform.position = data.playerStartPosition;
        playerCarInstance.transform.rotation = Quaternion.Euler(data.playerStartRotation);
        playerCarInstance.tag = GameConstants.TAG_PLAYER;

        // Rigidbody
        Rigidbody rb = playerCarInstance.AddComponent<Rigidbody>();
        CarController controller = playerCarInstance.AddComponent<CarController>();
        controller.SetStats(stats);

        // Vehicle Visual hierarchy
        GameObject visualRoot = new GameObject("VisualModel");
        visualRoot.transform.SetParent(playerCarInstance.transform);
        visualRoot.transform.localPosition = Vector3.zero;

        // Chassis / Lower Body
        GameObject chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chassis.name = "Chassis";
        chassis.transform.SetParent(visualRoot.transform);
        chassis.transform.localPosition = new Vector3(0, 0.55f, 0);
        chassis.transform.localScale = new Vector3(stats.carDimensions.x, 0.65f, stats.carDimensions.z);
        Destroy(chassis.GetComponent<Collider>());

        // Paint Material
        int paintIdx = Mathf.Clamp(selectedCarIndex, 0, carPaintMaterials.Length - 1);
        if (carPaintMaterials != null && carPaintMaterials[paintIdx] != null)
        {
            chassis.GetComponent<Renderer>().material = carPaintMaterials[paintIdx];
        }

        // Cabin / Roof
        GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cabin.name = "Cabin";
        cabin.transform.SetParent(visualRoot.transform);
        cabin.transform.localPosition = new Vector3(0, 1.05f, -0.2f);
        cabin.transform.localScale = new Vector3(stats.carDimensions.x * 0.85f, 0.6f, stats.carDimensions.z * 0.55f);
        Destroy(cabin.GetComponent<Collider>());
        if (carGlassMat != null) cabin.GetComponent<Renderer>().material = carGlassMat;

        // Wheels
        WheelVisualizer wv = playerCarInstance.AddComponent<WheelVisualizer>();
        float halfW = stats.carDimensions.x * 0.5f;
        float axleZ = stats.carDimensions.z * 0.28f;

        wv.frontLeftWheel = CreateWheelVisual(visualRoot.transform, new Vector3(-halfW, 0.35f, axleZ)).transform;
        wv.frontRightWheel = CreateWheelVisual(visualRoot.transform, new Vector3(halfW, 0.35f, axleZ)).transform;
        wv.rearLeftWheel = CreateWheelVisual(visualRoot.transform, new Vector3(-halfW, 0.35f, -axleZ)).transform;
        wv.rearRightWheel = CreateWheelVisual(visualRoot.transform, new Vector3(halfW, 0.35f, -axleZ)).transform;

        // Lights
        CarLights lights = playerCarInstance.AddComponent<CarLights>();
        lights.leftHeadlight = CreateHeadlightSpot(visualRoot.transform, new Vector3(-0.6f, 0.6f, stats.carDimensions.z * 0.5f));
        lights.rightHeadlight = CreateHeadlightSpot(visualRoot.transform, new Vector3(0.6f, 0.6f, stats.carDimensions.z * 0.5f));
        lights.SetHeadlights(data.isNightLevel);

        // Box Collider on Car
        BoxCollider boxCol = playerCarInstance.AddComponent<BoxCollider>();
        boxCol.size = new Vector3(stats.carDimensions.x, stats.carDimensions.y, stats.carDimensions.z);
        boxCol.center = new Vector3(0, stats.carDimensions.y * 0.5f, 0);

        // Attach CameraFollow
        CameraFollow cam = FindObjectOfType<CameraFollow>();
        if (cam != null)
        {
            cam.SetTarget(playerCarInstance.transform);
        }

        // Wire to Mobile Input Controller
        MobileInputController mobileInput = FindObjectOfType<MobileInputController>();
        if (mobileInput != null)
        {
            mobileInput.SetTargetCar(controller);
        }
    }

    private GameObject CreateWheelVisual(Transform parent, Vector3 localPos)
    {
        GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheel.name = "Wheel";
        wheel.transform.SetParent(parent);
        wheel.transform.localPosition = localPos;
        wheel.transform.localRotation = Quaternion.Euler(0, 0, 90f);
        wheel.transform.localScale = new Vector3(0.7f, 0.25f, 0.7f);
        if (carTireMat != null) wheel.GetComponent<Renderer>().material = carTireMat;
        Destroy(wheel.GetComponent<Collider>());
        return wheel;
    }

    private Light CreateHeadlightSpot(Transform parent, Vector3 localPos)
    {
        GameObject lightObj = new GameObject("HeadlightSpot");
        lightObj.transform.SetParent(parent);
        lightObj.transform.localPosition = localPos;
        lightObj.transform.localRotation = Quaternion.Euler(10f, 0, 0);

        Light spot = lightObj.AddComponent<Light>();
        spot.type = LightType.Spot;
        spot.range = 22f;
        spot.spotAngle = 55f;
        spot.color = new Color(1.0f, 0.98f, 0.9f);
        spot.intensity = 2.0f;
        spot.enabled = false;
        return spot;
    }
}
