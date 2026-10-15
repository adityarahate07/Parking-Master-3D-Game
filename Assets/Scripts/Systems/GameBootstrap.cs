using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Master runtime bootstrap for Parking Master 3D.
/// Initializes singletons, builds UI Canvas, mobile touch controls,
/// and sets up gameplay environments for any active scene.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    public static Font GetAppFont()
    {
        Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (f == null)
        {
            Font[] allFonts = Resources.FindObjectsOfTypeAll<Font>();
            if (allFonts != null && allFonts.Length > 0) f = allFonts[0];
        }
        if (f == null) f = Font.CreateDynamicFontFromOSFont(new string[] { "Arial", "Roboto", "Segoe UI", "Tahoma" }, 16);
        return f;
    }

    private void EnsureDirectionalLight()
    {
        Light[] lights = FindObjectsOfType<Light>();
        foreach (var l in lights)
        {
            if (l.type == LightType.Directional) return;
        }
        GameObject sunObj = new GameObject("Directional Light");
        Light sun = sunObj.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.color = new Color(1.0f, 0.96f, 0.88f);
        sun.intensity = 1.15f;
        sun.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
    }

    private void Awake()
    {
        EnsureCoreSingletons();
        EnsureEventSystem();
    }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Equals("MainMenu", System.StringComparison.OrdinalIgnoreCase))
        {
            SetupMainMenuScene();
        }
        else if (sceneName.Equals("LevelSelect", System.StringComparison.OrdinalIgnoreCase))
        {
            SetupLevelSelectScene();
        }
        else if (sceneName.Equals("Garage", System.StringComparison.OrdinalIgnoreCase))
        {
            SetupGarageScene();
        }
        else
        {
            // Gameplay Level (Level01 to Level10)
            int levelNum = ExtractLevelNumber(sceneName);
            SetupGameplayScene(levelNum);
        }
    }

    private int ExtractLevelNumber(string sceneName)
    {
        string digits = System.Text.RegularExpressions.Regex.Replace(sceneName, @"[^\d]", "");
        if (int.TryParse(digits, out int val) && val >= 1 && val <= 10)
        {
            return val;
        }
        return LevelManager.Instance != null ? LevelManager.Instance.currentLevel : 1;
    }

    private void EnsureCoreSingletons()
    {
        if (AudioManager.Instance == null && FindObjectOfType<AudioManager>() == null)
            new GameObject("AudioManager").AddComponent<AudioManager>();

        if (SettingsManager.Instance == null && FindObjectOfType<SettingsManager>() == null)
            new GameObject("SettingsManager").AddComponent<SettingsManager>();

        if (GarageManager.Instance == null && FindObjectOfType<GarageManager>() == null)
            new GameObject("GarageManager").AddComponent<GarageManager>();

        if (GameManager.Instance == null && FindObjectOfType<GameManager>() == null)
            new GameObject("GameManager").AddComponent<GameManager>();

        if (LevelManager.Instance == null && FindObjectOfType<LevelManager>() == null)
            new GameObject("LevelManager").AddComponent<LevelManager>();

        if (ScoreManager.Instance == null && FindObjectOfType<ScoreManager>() == null)
            new GameObject("ScoreManager").AddComponent<ScoreManager>();

        if (TimerManager.Instance == null && FindObjectOfType<TimerManager>() == null)
            new GameObject("TimerManager").AddComponent<TimerManager>();

        if (CollisionManager.Instance == null && FindObjectOfType<CollisionManager>() == null)
            new GameObject("CollisionManager").AddComponent<CollisionManager>();

        if (ParkingManager.Instance == null && FindObjectOfType<ParkingManager>() == null)
            new GameObject("ParkingManager").AddComponent<ParkingManager>();

        if (PauseManager.Instance == null && FindObjectOfType<PauseManager>() == null)
            new GameObject("PauseManager").AddComponent<PauseManager>();
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    // ==========================================
    // 1. MAIN MENU SETUP
    // ==========================================
    private void SetupMainMenuScene()
    {
        EnsureDirectionalLight();

        // Showcase environment
        GameObject bgObj = new GameObject("Showcase_Background");
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.SetParent(bgObj.transform);
        plane.transform.localScale = new Vector3(4f, 1f, 4f);
        Material asphalt = Resources.Load<Material>("Materials/Road_Asphalt");
        if (asphalt == null)
        {
            asphalt = new Material(Shader.Find("Standard") ?? Shader.Find("Mobile/Diffuse") ?? Shader.Find("Diffuse"));
            asphalt.color = new Color(0.2f, 0.22f, 0.25f);
        }
        plane.GetComponent<Renderer>().material = asphalt;

        // Showcase Car
        GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
        car.name = "ShowcaseCar";
        car.transform.SetParent(bgObj.transform);
        car.transform.position = new Vector3(0, 0.6f, 0);
        car.transform.localScale = new Vector3(1.8f, 0.7f, 3.8f);
        Material carMat = Resources.Load<Material>("Materials/Car_Starter_Red");
        if (carMat == null)
        {
            carMat = new Material(Shader.Find("Standard") ?? Shader.Find("Mobile/Diffuse") ?? Shader.Find("Diffuse"));
            carMat.color = new Color(0.9f, 0.2f, 0.2f);
        }
        car.GetComponent<Renderer>().material = carMat;

        // Camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.12f, 0.15f, 0.22f);
        cam.transform.position = new Vector3(0, 2.5f, -6f);
        cam.transform.LookAt(car.transform.position + Vector3.up * 0.5f);

        // Build UI Canvas
        CreateMainMenuCanvas(car.transform, cam.transform);
    }

    private void CreateMainMenuCanvas(Transform showcaseCar, Transform showcaseCam)
    {
        GameObject canvasObj = CreateBaseCanvas("MainMenuCanvas");
        Canvas canvas = canvasObj.GetComponent<Canvas>();

        MainMenuUI menuUI = canvasObj.AddComponent<MainMenuUI>();
        menuUI.showcaseCar = showcaseCar;
        menuUI.showcaseCamera = showcaseCam;

        // Title Panel
        GameObject titleObj = CreateUIElement("TitleHeader", canvasObj.transform, new Vector2(0.5f, 0.82f), new Vector2(0.5f, 0.82f), new Vector2(0, 0), new Vector2(600, 160));
        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = GetAppFont();
        titleText.fontSize = 46;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.text = "<b>PARKING\nMASTER 3D</b>";

        // Tagline
        GameObject tagObj = CreateUIElement("Tagline", titleObj.transform, new Vector2(0.5f, -0.1f), new Vector2(0.5f, -0.1f), new Vector2(0, -20), new Vector2(500, 40));
        Text tagText = tagObj.AddComponent<Text>();
        tagText.font = GetAppFont();
        tagText.fontSize = 20;
        tagText.alignment = TextAnchor.MiddleCenter;
        tagText.color = new Color(0.9f, 0.75f, 0.2f);
        tagText.text = "<i>\"Park Perfectly. Drive Carefully.\"</i>";

        // Buttons Container
        GameObject btnGroup = CreateUIElement("ButtonsGroup", canvasObj.transform, new Vector2(0.5f, 0.38f), new Vector2(0.5f, 0.38f), new Vector2(0, 0), new Vector2(320, 280));
        menuUI.playButton = CreateMenuButton("PlayBtn", btnGroup.transform, new Vector2(0, 80), "▶  PLAY", new Color(0.18f, 0.8f, 0.44f));
        menuUI.levelsButton = CreateMenuButton("LevelsBtn", btnGroup.transform, new Vector2(0, 25), "🎮  LEVELS", new Color(0.2f, 0.6f, 0.86f));
        menuUI.garageButton = CreateMenuButton("GarageBtn", btnGroup.transform, new Vector2(0, -30), "🚗  GARAGE", new Color(0.9f, 0.5f, 0.13f));
        menuUI.settingsButton = CreateMenuButton("SettingsBtn", btnGroup.transform, new Vector2(0, -85), "⚙  SETTINGS", new Color(0.45f, 0.5f, 0.58f));
        menuUI.exitButton = CreateMenuButton("ExitBtn", btnGroup.transform, new Vector2(0, -140), "❌  EXIT", new Color(0.85f, 0.25f, 0.2f));

        // Settings Dialog
        menuUI.settingsDialog = CreateSettingsDialog(canvasObj.transform);
    }

    // ==========================================
    // 2. LEVEL SELECT SETUP
    // ==========================================
    private void SetupLevelSelectScene()
    {
        EnsureDirectionalLight();
        CreateCameraIfNone(new Vector3(0, 5, -10), Quaternion.Euler(20, 0, 0));

        GameObject canvasObj = CreateBaseCanvas("LevelSelectCanvas");
        LevelSelectUI selectUI = canvasObj.AddComponent<LevelSelectUI>();

        // Header Title
        GameObject titleObj = CreateUIElement("HeaderTitle", canvasObj.transform, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), Vector2.zero, new Vector2(500, 70));
        Text t = titleObj.AddComponent<Text>();
        t.font = GetAppFont();
        t.fontSize = 38;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.text = "<b>SELECT LEVEL</b>";

        // Back Button
        selectUI.backButton = CreateMenuButton("BackBtn", canvasObj.transform, new Vector2(-420, 260), "◀ BACK", new Color(0.4f, 0.45f, 0.5f), new Vector2(140, 50));

        // Grid of 10 Levels (2 rows of 5)
        selectUI.levelCardItems = new GameObject[10];
        float startX = -360f;
        float spacingX = 180f;

        for (int i = 0; i < 10; i++)
        {
            int row = i < 5 ? 0 : 1;
            int col = i % 5;
            Vector2 cardPos = new Vector2(startX + col * spacingX, row == 0 ? 80f : -100f);

            GameObject card = CreateUIElement("LevelCard_" + (i + 1), canvasObj.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), cardPos, new Vector2(150, 140));
            Image img = card.AddComponent<Image>();
            img.color = new Color(0.18f, 0.22f, 0.28f, 0.95f);
            card.AddComponent<Button>();

            // Title
            GameObject cTitle = CreateUIElement("TitleText", card.transform, new Vector2(0.5f, 0.75f), new Vector2(0.5f, 0.75f), Vector2.zero, new Vector2(140, 30));
            Text ct = cTitle.AddComponent<Text>();
            ct.font = GetAppFont();
            ct.fontSize = 20;
            ct.alignment = TextAnchor.MiddleCenter;
            ct.color = Color.white;
            ct.text = "LEVEL " + (i + 1);

            // Stars
            GameObject cStars = CreateUIElement("StarsText", card.transform, new Vector2(0.5f, 0.35f), new Vector2(0.5f, 0.35f), Vector2.zero, new Vector2(140, 30));
            Text cs = cStars.AddComponent<Text>();
            cs.font = GetAppFont();
            cs.fontSize = 18;
            cs.alignment = TextAnchor.MiddleCenter;
            cs.color = new Color(1f, 0.85f, 0.2f);
            cs.text = "⭐⭐⭐";

            // Lock Overlay
            GameObject lockOverlay = CreateUIElement("LockOverlay", card.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(150, 140));
            Image lockImg = lockOverlay.AddComponent<Image>();
            lockImg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);
            lockOverlay.SetActive(false);

            selectUI.levelCardItems[i] = card;
        }

        selectUI.PopulateLevels();
    }

    // ==========================================
    // 3. GARAGE SETUP
    // ==========================================
    private void SetupGarageScene()
    {
        EnsureDirectionalLight();

        // 3D Turntable Platform
        GameObject turntableObj = new GameObject("TurntablePlatform");
        turntableObj.transform.position = Vector3.zero;
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.SetParent(turntableObj.transform);
        cylinder.transform.localPosition = new Vector3(0, 0.1f, 0);
        cylinder.transform.localScale = new Vector3(4.5f, 0.15f, 4.5f);
        Material rimMat = Resources.Load<Material>("Materials/Car_Rim_Chrome");
        if (rimMat == null)
        {
            rimMat = new Material(Shader.Find("Standard") ?? Shader.Find("Mobile/Diffuse") ?? Shader.Find("Diffuse"));
            rimMat.color = new Color(0.85f, 0.85f, 0.88f);
        }
        cylinder.GetComponent<Renderer>().material = rimMat;

        // 3 Preview Cars on turntable
        GameObject[] previewCars = new GameObject[3];
        CarStats[] stats = CarStats.GetDefaultCars();
        Material[] paints = new Material[]
        {
            Resources.Load<Material>("Materials/Car_Starter_Red"),
            Resources.Load<Material>("Materials/Car_Sport_Blue"),
            Resources.Load<Material>("Materials/Car_SUV_Black")
        };

        for (int i = 0; i < 3; i++)
        {
            GameObject pCar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pCar.name = "PreviewCar_" + i;
            pCar.transform.SetParent(turntableObj.transform);
            pCar.transform.localPosition = new Vector3(0, 0.6f, 0);
            pCar.transform.localScale = stats[i].carDimensions;
            if (paints[i] != null) pCar.GetComponent<Renderer>().material = paints[i];
            pCar.SetActive(i == 0);
            previewCars[i] = pCar;
        }

        // Camera
        Camera cam = CreateCameraIfNone(new Vector3(0, 2.5f, -6.5f), Quaternion.Euler(18, 0, 0));

        // UI Canvas
        GameObject canvasObj = CreateBaseCanvas("GarageCanvas");
        GarageUI garageUI = canvasObj.AddComponent<GarageUI>();
        garageUI.turntablePlatform = turntableObj.transform;
        garageUI.carPreviewModels = previewCars;

        // Header Title
        GameObject header = CreateUIElement("GarageHeader", canvasObj.transform, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), Vector2.zero, new Vector2(400, 60));
        Text ht = header.AddComponent<Text>();
        ht.font = GetAppFont();
        ht.fontSize = 38;
        ht.alignment = TextAnchor.MiddleCenter;
        ht.color = Color.white;
        ht.text = "<b>VEHICLE GARAGE</b>";

        // Back Button
        garageUI.backButton = CreateMenuButton("BackBtn", canvasObj.transform, new Vector2(-420, 260), "◀ BACK", new Color(0.4f, 0.45f, 0.5f), new Vector2(140, 50));

        // Car Info Box
        GameObject infoBox = CreateUIElement("InfoBox", canvasObj.transform, new Vector2(0.5f, 0.22f), new Vector2(0.5f, 0.22f), Vector2.zero, new Vector2(500, 180));
        Image infoBg = infoBox.AddComponent<Image>();
        infoBg.color = new Color(0.12f, 0.15f, 0.2f, 0.9f);

        // Car Name
        GameObject nameObj = CreateUIElement("CarName", infoBox.transform, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), Vector2.zero, new Vector2(450, 35));
        garageUI.carNameText = nameObj.AddComponent<Text>();
        garageUI.carNameText.font = GetAppFont();
        garageUI.carNameText.fontSize = 24;
        garageUI.carNameText.alignment = TextAnchor.MiddleCenter;
        garageUI.carNameText.color = Color.white;

        // Category
        GameObject catObj = CreateUIElement("CarCategory", infoBox.transform, new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), Vector2.zero, new Vector2(450, 25));
        garageUI.carCategoryText = catObj.AddComponent<Text>();
        garageUI.carCategoryText.font = GetAppFont();
        garageUI.carCategoryText.fontSize = 16;
        garageUI.carCategoryText.alignment = TextAnchor.MiddleCenter;
        garageUI.carCategoryText.color = new Color(0.2f, 0.8f, 0.9f);

        // Unlock status
        GameObject unlObj = CreateUIElement("UnlockStatus", infoBox.transform, new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.45f), Vector2.zero, new Vector2(450, 25));
        garageUI.unlockRequirementText = unlObj.AddComponent<Text>();
        garageUI.unlockRequirementText.font = GetAppFont();
        garageUI.unlockRequirementText.fontSize = 16;
        garageUI.unlockRequirementText.alignment = TextAnchor.MiddleCenter;
        garageUI.unlockRequirementText.color = Color.yellow;

        // Select Button
        garageUI.selectButton = CreateMenuButton("SelectBtn", infoBox.transform, new Vector2(0, -50), "EQUIPPED", new Color(0.18f, 0.8f, 0.44f), new Vector2(200, 45));
        garageUI.selectButtonText = garageUI.selectButton.GetComponentInChildren<Text>();

        // Prev & Next Buttons
        garageUI.prevCarButton = CreateMenuButton("PrevBtn", canvasObj.transform, new Vector2(-320, 0), "◀", new Color(0.25f, 0.3f, 0.38f), new Vector2(60, 60));
        garageUI.nextCarButton = CreateMenuButton("NextBtn", canvasObj.transform, new Vector2(320, 0), "▶", new Color(0.25f, 0.3f, 0.38f), new Vector2(60, 60));

        garageUI.UpdateGarageView();
    }

    // ==========================================
    // 4. GAMEPLAY LEVEL SETUP
    // ==========================================
    private void SetupGameplayScene(int levelNum)
    {
        CreateCameraIfNone(new Vector3(0, 4, -8), Quaternion.Euler(20, 0, 0));

        // 1. Build 3D Environment, Obstacles, and Spawn Car
        LevelEnvironmentBuilder builder = FindObjectOfType<LevelEnvironmentBuilder>();
        if (builder == null)
        {
            GameObject bObj = new GameObject("LevelEnvironmentBuilder");
            builder = bObj.AddComponent<LevelEnvironmentBuilder>();
        }
        builder.BuildLevel(levelNum);

        // 2. Build In-Game HUD Canvas
        CreateGameplayHUDCanvas(levelNum);

        // 3. Start Level in GameManager
        GameManager.Instance?.StartLevel(levelNum);
    }

    private void CreateGameplayHUDCanvas(int levelNum)
    {
        GameObject canvasObj = CreateBaseCanvas("GameplayHUDCanvas");
        UIManager ui = canvasObj.AddComponent<UIManager>();

        // Mobile Controls Component
        MobileInputController mobileInput = canvasObj.AddComponent<MobileInputController>();

        // HUD Root
        GameObject hudRoot = CreateUIElement("HUDRoot", canvasObj.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        ui.hudRoot = hudRoot;

        // TOP LEFT: Level Text
        GameObject lvlObj = CreateUIElement("LevelText", hudRoot.transform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(120, -50), new Vector2(200, 60));
        ui.levelText = lvlObj.AddComponent<Text>();
        ui.levelText.font = GetAppFont();
        ui.levelText.fontSize = 24;
        ui.levelText.alignment = TextAnchor.MiddleLeft;
        ui.levelText.color = Color.white;

        // TOP CENTER: Timer
        GameObject timeObj = CreateUIElement("TimerText", hudRoot.transform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -50), new Vector2(160, 60));
        ui.timerText = timeObj.AddComponent<Text>();
        ui.timerText.font = GetAppFont();
        ui.timerText.fontSize = 24;
        ui.timerText.alignment = TextAnchor.MiddleCenter;
        ui.timerText.color = new Color(1f, 0.85f, 0.2f);

        // TOP RIGHT: Score & Collisions
        GameObject scoreObj = CreateUIElement("ScoreText", hudRoot.transform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-120, -40), new Vector2(180, 45));
        ui.scoreText = scoreObj.AddComponent<Text>();
        ui.scoreText.font = GetAppFont();
        ui.scoreText.fontSize = 22;
        ui.scoreText.alignment = TextAnchor.MiddleRight;
        ui.scoreText.color = Color.white;

        GameObject colObj = CreateUIElement("CollisionsText", hudRoot.transform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-120, -75), new Vector2(180, 30));
        ui.collisionsText = colObj.AddComponent<Text>();
        ui.collisionsText.font = GetAppFont();
        ui.collisionsText.fontSize = 16;
        ui.collisionsText.alignment = TextAnchor.MiddleRight;
        ui.collisionsText.color = new Color(0.95f, 0.35f, 0.35f);

        // TOP BUTTONS: Pause (⏸) & Camera (📷)
        Button pauseBtn = CreateMenuButton("PauseBtn", hudRoot.transform, new Vector2(-40, -40), "⏸", new Color(0.3f, 0.35f, 0.45f, 0.85f), new Vector2(50, 50));
        pauseBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
        pauseBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        pauseBtn.onClick.AddListener(ui.OnPauseButtonClicked);

        Button camBtn = CreateMenuButton("CamBtn", hudRoot.transform, new Vector2(40, -40), "📷", new Color(0.3f, 0.35f, 0.45f, 0.85f), new Vector2(50, 50));
        camBtn.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        camBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        camBtn.onClick.AddListener(ui.OnCameraSwitchButtonClicked);

        // STATUS BANNER (Uses child object for Text to avoid UGUI graphic conflict)
        GameObject banner = CreateUIElement("StatusBanner", canvasObj.transform, new Vector2(0.5f, 0.75f), new Vector2(0.5f, 0.75f), Vector2.zero, new Vector2(420, 50));
        Image bImg = banner.AddComponent<Image>();
        bImg.color = new Color(0.1f, 0.12f, 0.16f, 0.9f);

        GameObject bTxtObj = CreateUIElement("Text", banner.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Text bTxt = bTxtObj.AddComponent<Text>();
        bTxt.font = GetAppFont();
        bTxt.fontSize = 24;
        bTxt.alignment = TextAnchor.MiddleCenter;
        bTxt.color = new Color(0.2f, 0.95f, 0.4f);
        ui.bannerPanel = banner;
        ui.bannerText = bTxt;
        banner.SetActive(false);

        // MOBILE DRIVING CONTROLS (Bottom left: Steering, Bottom right: Drive / Brake / Reverse)
        CreateMobileTouchControls(canvasObj.transform, mobileInput);

        // DIALOGS (Pause, Complete, Failed, Settings)
        CreatePauseDialog(canvasObj.transform, ui);
        CreateLevelCompleteDialog(canvasObj.transform, ui);
        CreateLevelFailedDialog(canvasObj.transform, ui);
    }

    private void CreateMobileTouchControls(Transform canvasTransform, MobileInputController controller)
    {
        // Steering: Left & Right buttons (bottom-left)
        controller.btnLeft = CreateTouchButton("BtnSteerLeft", canvasTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(75, 75), "◀", new Vector2(90, 90));
        controller.btnRight = CreateTouchButton("BtnSteerRight", canvasTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(185, 75), "▶", new Vector2(90, 90));

        // Pedals: Accelerate, Brake, Reverse (bottom-right)
        controller.btnAccelerate = CreateTouchButton("BtnAccelerate", canvasTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-80, 110), "GAS", new Vector2(90, 110), new Color(0.18f, 0.75f, 0.35f, 0.85f));
        controller.btnBrake = CreateTouchButton("BtnBrake", canvasTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-190, 60), "STOP", new Vector2(85, 80), new Color(0.9f, 0.25f, 0.2f, 0.85f));
        controller.btnReverse = CreateTouchButton("BtnReverse", canvasTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-80, 45), "REV", new Vector2(90, 50), new Color(0.85f, 0.5f, 0.15f, 0.85f));
    }

    private TouchButton CreateTouchButton(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, string label, Vector2 size, Color? btnColor = null)
    {
        GameObject obj = CreateUIElement(name, parent, anchorMin, anchorMax, anchoredPos, size);
        Image img = obj.AddComponent<Image>();
        img.color = btnColor ?? new Color(0.2f, 0.25f, 0.35f, 0.75f);

        GameObject textObj = CreateUIElement("Label", obj.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Text t = textObj.AddComponent<Text>();
        t.font = GetAppFont();
        t.fontSize = 20;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.text = label;

        return obj.AddComponent<TouchButton>();
    }

    // Dialog creation helpers
    private void CreatePauseDialog(Transform parent, UIManager ui)
    {
        GameObject panel = CreateUIElement("PausePanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(400, 360));
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.12f, 0.16f, 0.95f);

        GameObject title = CreateUIElement("Title", panel.transform, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), Vector2.zero, new Vector2(300, 40));
        Text t = title.AddComponent<Text>();
        t.font = GetAppFont();
        t.fontSize = 32;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.text = "PAUSED";

        Button resBtn = CreateMenuButton("ResumeBtn", panel.transform, new Vector2(0, 50), "RESUME", new Color(0.2f, 0.75f, 0.4f), new Vector2(250, 45));
        resBtn.onClick.AddListener(ui.OnResumeButtonClicked);

        Button restBtn = CreateMenuButton("RestartBtn", panel.transform, new Vector2(0, -5), "RESTART", new Color(0.25f, 0.6f, 0.9f), new Vector2(250, 45));
        restBtn.onClick.AddListener(ui.OnRestartButtonClicked);

        Button setBtn = CreateMenuButton("SettingsBtn", panel.transform, new Vector2(0, -60), "SETTINGS", new Color(0.45f, 0.5f, 0.6f), new Vector2(250, 45));
        setBtn.onClick.AddListener(ui.OnSettingsButtonClicked);

        Button menuBtn = CreateMenuButton("MenuBtn", panel.transform, new Vector2(0, -115), "MAIN MENU", new Color(0.8f, 0.3f, 0.25f), new Vector2(250, 45));
        menuBtn.onClick.AddListener(ui.OnMainMenuButtonClicked);

        ui.pausePanel = panel;
        panel.SetActive(false);
    }

    private void CreateLevelCompleteDialog(Transform parent, UIManager ui)
    {
        GameObject panel = CreateUIElement("LevelCompletePanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460, 400));
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.13f, 0.18f, 0.96f);

        GameObject title = CreateUIElement("Title", panel.transform, new Vector2(0.5f, 0.86f), new Vector2(0.5f, 0.86f), Vector2.zero, new Vector2(400, 40));
        ui.completeTitleText = title.AddComponent<Text>();
        ui.completeTitleText.font = GetAppFont();
        ui.completeTitleText.fontSize = 28;
        ui.completeTitleText.alignment = TextAnchor.MiddleCenter;
        ui.completeTitleText.color = new Color(0.2f, 0.95f, 0.45f);
        ui.completeTitleText.text = "🎉 LEVEL COMPLETE!";

        // Stars row
        GameObject starRow = CreateUIElement("StarsRow", panel.transform, new Vector2(0.5f, 0.68f), new Vector2(0.5f, 0.68f), Vector2.zero, new Vector2(240, 50));
        ui.starIcons = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject sObj = CreateUIElement("Star_" + i, starRow.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2((i - 1) * 60f, 0), new Vector2(50, 50));
            Text st = sObj.AddComponent<Text>();
            st.font = GetAppFont();
            st.fontSize = 36;
            st.alignment = TextAnchor.MiddleCenter;
            st.color = new Color(1f, 0.85f, 0.2f);
            st.text = "⭐";
            ui.starIcons[i] = sObj;
        }

        // Stats: Score, Time, Collisions
        GameObject scoreObj = CreateUIElement("Score", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(350, 30));
        ui.completeScoreText = scoreObj.AddComponent<Text>();
        ui.completeScoreText.font = GetAppFont();
        ui.completeScoreText.fontSize = 20;
        ui.completeScoreText.alignment = TextAnchor.MiddleCenter;
        ui.completeScoreText.color = Color.white;

        GameObject timeObj = CreateUIElement("Time", panel.transform, new Vector2(0.5f, 0.42f), new Vector2(0.5f, 0.42f), Vector2.zero, new Vector2(350, 30));
        ui.completeTimeText = timeObj.AddComponent<Text>();
        ui.completeTimeText.font = GetAppFont();
        ui.completeTimeText.fontSize = 18;
        ui.completeTimeText.alignment = TextAnchor.MiddleCenter;
        ui.completeTimeText.color = new Color(0.85f, 0.85f, 0.85f);

        GameObject colObj = CreateUIElement("Collisions", panel.transform, new Vector2(0.5f, 0.34f), new Vector2(0.5f, 0.34f), Vector2.zero, new Vector2(350, 30));
        ui.completeCollisionsText = colObj.AddComponent<Text>();
        ui.completeCollisionsText.font = GetAppFont();
        ui.completeCollisionsText.fontSize = 18;
        ui.completeCollisionsText.alignment = TextAnchor.MiddleCenter;
        ui.completeCollisionsText.color = new Color(0.85f, 0.85f, 0.85f);

        // Buttons: Next, Retry, Level Select
        Button nextBtn = CreateMenuButton("NextBtn", panel.transform, new Vector2(0, -75), "NEXT LEVEL ▶", new Color(0.2f, 0.8f, 0.4f), new Vector2(260, 42));
        nextBtn.onClick.AddListener(ui.OnNextLevelButtonClicked);

        Button retryBtn = CreateMenuButton("RetryBtn", panel.transform, new Vector2(-90, -125), "RETRY ↺", new Color(0.3f, 0.6f, 0.9f), new Vector2(130, 38));
        retryBtn.onClick.AddListener(ui.OnRestartButtonClicked);

        Button lvlBtn = CreateMenuButton("LvlSelectBtn", panel.transform, new Vector2(90, -125), "LEVELS 🎮", new Color(0.45f, 0.5f, 0.6f), new Vector2(130, 38));
        lvlBtn.onClick.AddListener(ui.OnLevelSelectButtonClicked);

        ui.levelCompletePanel = panel;
        panel.SetActive(false);
    }

    private void CreateLevelFailedDialog(Transform parent, UIManager ui)
    {
        GameObject panel = CreateUIElement("LevelFailedPanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420, 340));
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.18f, 0.1f, 0.12f, 0.96f);

        GameObject title = CreateUIElement("Title", panel.transform, new Vector2(0.5f, 0.82f), new Vector2(0.5f, 0.82f), Vector2.zero, new Vector2(350, 40));
        Text t = title.AddComponent<Text>();
        t.font = GetAppFont();
        t.fontSize = 32;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = new Color(0.95f, 0.25f, 0.25f);
        t.text = "LEVEL FAILED";

        GameObject reason = CreateUIElement("Reason", panel.transform, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), Vector2.zero, new Vector2(350, 40));
        ui.failureReasonText = reason.AddComponent<Text>();
        ui.failureReasonText.font = GetAppFont();
        ui.failureReasonText.fontSize = 20;
        ui.failureReasonText.alignment = TextAnchor.MiddleCenter;
        ui.failureReasonText.color = Color.white;
        ui.failureReasonText.text = "TOO MANY COLLISIONS!";

        Button retryBtn = CreateMenuButton("RetryBtn", panel.transform, new Vector2(0, -30), "RETRY ↺", new Color(0.2f, 0.7f, 0.4f), new Vector2(240, 45));
        retryBtn.onClick.AddListener(ui.OnRestartButtonClicked);

        Button lvlBtn = CreateMenuButton("LvlSelectBtn", panel.transform, new Vector2(0, -85), "LEVEL SELECT 🎮", new Color(0.4f, 0.45f, 0.55f), new Vector2(240, 45));
        lvlBtn.onClick.AddListener(ui.OnLevelSelectButtonClicked);

        ui.levelFailedPanel = panel;
        panel.SetActive(false);
    }

    private GameObject CreateSettingsDialog(Transform parent)
    {
        GameObject panel = CreateUIElement("SettingsPanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(400, 380));
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.12f, 0.15f, 0.2f, 0.96f);

        SettingsUI sUI = panel.AddComponent<SettingsUI>();

        GameObject title = CreateUIElement("Title", panel.transform, new Vector2(0.5f, 0.88f), new Vector2(0.5f, 0.88f), Vector2.zero, new Vector2(300, 40));
        Text t = title.AddComponent<Text>();
        t.font = GetAppFont();
        t.fontSize = 28;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.text = "SETTINGS";

        sUI.soundToggle = CreateSettingToggle("SoundToggle", panel.transform, new Vector2(0, 60), "Sound Effects");
        sUI.musicToggle = CreateSettingToggle("MusicToggle", panel.transform, new Vector2(0, 10), "Background Music");
        sUI.vibrationToggle = CreateSettingToggle("VibToggle", panel.transform, new Vector2(0, -40), "Haptic Vibration");

        sUI.backButton = CreateMenuButton("BackBtn", panel.transform, new Vector2(0, -110), "BACK", new Color(0.35f, 0.4f, 0.5f), new Vector2(200, 40));

        panel.SetActive(false);
        return panel;
    }

    private Toggle CreateSettingToggle(string name, Transform parent, Vector2 pos, string label)
    {
        GameObject toggleObj = CreateUIElement(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, new Vector2(280, 35));
        Toggle toggle = toggleObj.AddComponent<Toggle>();

        GameObject labelObj = CreateUIElement("Label", toggleObj.transform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(80, 0), new Vector2(180, 30));
        Text t = labelObj.AddComponent<Text>();
        t.font = GetAppFont();
        t.fontSize = 18;
        t.alignment = TextAnchor.MiddleLeft;
        t.color = Color.white;
        t.text = label;

        GameObject checkBg = CreateUIElement("Background", toggleObj.transform, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-20, 0), new Vector2(28, 28));
        Image bgImg = checkBg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.25f, 0.35f);
        toggle.targetGraphic = bgImg;

        GameObject checkmark = CreateUIElement("Checkmark", checkBg.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(20, 20));
        Image cmImg = checkmark.AddComponent<Image>();
        cmImg.color = new Color(0.2f, 0.9f, 0.4f);
        toggle.graphic = cmImg;

        return toggle;
    }

    // Canvas & Primitive helpers
    private GameObject CreateBaseCanvas(string name)
    {
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();
        return canvasObj;
    }

    private GameObject CreateUIElement(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        return obj;
    }

    private Button CreateMenuButton(string name, Transform parent, Vector2 pos, string label, Color btnColor, Vector2? customSize = null)
    {
        Vector2 size = customSize ?? new Vector2(280, 48);
        GameObject btnObj = CreateUIElement(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);
        Image img = btnObj.AddComponent<Image>();
        img.color = btnColor;
        Button btn = btnObj.AddComponent<Button>();

        GameObject textObj = CreateUIElement("Text", btnObj.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Text t = textObj.AddComponent<Text>();
        t.font = GetAppFont();
        t.fontSize = 20;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.text = "<b>" + label + "</b>";

        return btn;
    }

    private Camera CreateCameraIfNone(Vector3 pos, Quaternion rot)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.12f, 0.15f, 0.22f);
        cam.transform.position = pos;
        cam.transform.rotation = rot;
        if (cam.GetComponent<CameraFollow>() == null)
        {
            cam.gameObject.AddComponent<CameraFollow>();
        }
        return cam;
    }
}
