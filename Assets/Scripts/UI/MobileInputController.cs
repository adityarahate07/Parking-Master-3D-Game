using UnityEngine;

/// <summary>
/// Collects touch inputs from on-screen buttons (Left, Right, Accelerate, Brake, Reverse)
/// and blends them with keyboard inputs (A/D, W/S, Space, R) to drive CarController.
/// </summary>
public class MobileInputController : MonoBehaviour
{
    [Header("Target Vehicle")]
    public CarController targetCar;

    [Header("On-Screen Touch Buttons")]
    public TouchButton btnLeft;
    public TouchButton btnRight;
    public TouchButton btnAccelerate;
    public TouchButton btnBrake;
    public TouchButton btnReverse;

    // Smoothed input values
    private float smoothedSteer = 0f;
    private float smoothedThrottle = 0f;

    private void Start()
    {
        if (targetCar == null)
        {
            targetCar = FindObjectOfType<CarController>();
        }
    }

    public void SetTargetCar(CarController car)
    {
        targetCar = car;
    }

    private void Update()
    {
        if (targetCar == null)
        {
            targetCar = FindObjectOfType<CarController>();
            if (targetCar == null) return;
        }

        // 1. Raw Steering Input
        float rawSteer = 0f;
        if (btnLeft != null && btnLeft.IsPressed) rawSteer -= 1f;
        if (btnRight != null && btnRight.IsPressed) rawSteer += 1f;

        // Blend with keyboard (A / D, Left / Right)
        float keySteer = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(keySteer) > 0.01f) rawSteer = keySteer;

        // Smooth steering return
        smoothedSteer = Mathf.MoveTowards(smoothedSteer, rawSteer, Time.deltaTime * 6f);

        // 2. Throttle Input (Accelerate vs Reverse)
        float rawThrottle = 0f;
        if (btnAccelerate != null && btnAccelerate.IsPressed) rawThrottle += 1f;
        if (btnReverse != null && btnReverse.IsPressed) rawThrottle -= 1f;

        // Blend with keyboard (W / S, Up / Down, R)
        float keyVertical = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(keyVertical) > 0.01f) rawThrottle = keyVertical;
        if (Input.GetKey(KeyCode.R)) rawThrottle = -1f;

        smoothedThrottle = Mathf.MoveTowards(smoothedThrottle, rawThrottle, Time.deltaTime * 8f);

        // 3. Brake Input
        bool isBraking = false;
        if (btnBrake != null && btnBrake.IsPressed) isBraking = true;
        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Jump")) isBraking = true;

        // Feed into CarController
        targetCar.SetInput(smoothedSteer, smoothedThrottle, isBraking);

        // Audio pitch & reverse beeper sync
        if (AudioManager.Instance != null)
        {
            float normSpeed = targetCar.SpeedMagnitude / Mathf.Max(1f, targetCar.maxSpeed);
            AudioManager.Instance.UpdateEnginePitch(normSpeed);
            AudioManager.Instance.SetReverseBeeping(targetCar.IsReversing);
        }

        // Hotkeys for Camera and Pause
        if (Input.GetKeyDown(KeyCode.C))
        {
            FindObjectOfType<CameraFollow>()?.ToggleCameraMode();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FindObjectOfType<PauseManager>()?.TogglePause();
        }
    }
}
