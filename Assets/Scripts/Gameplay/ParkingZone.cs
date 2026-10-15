using UnityEngine;

/// <summary>
/// Handles parking zone detection, tolerance validation (position, rotation, speed),
/// visual pulsing shader effects, and parking success confirmation.
/// </summary>
public class ParkingZone : MonoBehaviour
{
    [Header("Tolerances")]
    public Vector3 zoneDimensions = new Vector3(2.6f, 1.0f, 5.0f);
    public float rotationToleranceDegrees = 15f;
    public float maxStationarySpeed = 0.25f;
    public float requiredStationaryDuration = 1.0f;
    public bool requireReverseOrientation = false;

    [Header("Target Rotation")]
    public float targetHeadingEulerY = 0f;

    [Header("Visuals")]
    public Renderer zoneMeshRenderer;
    public Material guideMaterial;
    public Light pulseLight;

    // Runtime state
    public bool IsCarInside { get; private set; } = false;
    public bool IsRotationCorrect { get; private set; } = false;
    public bool IsStationary { get; private set; } = false;
    public bool IsParkedSuccessfully { get; private set; } = false;

    private float stationaryTimer = 0f;
    private CarController currentCar;
    private float pulseTime = 0f;

    private void Awake()
    {
        targetHeadingEulerY = transform.eulerAngles.y;

        // Auto-assign components if present
        if (zoneMeshRenderer == null) zoneMeshRenderer = GetComponentInChildren<Renderer>();
        if (pulseLight == null) pulseLight = GetComponentInChildren<Light>();
    }

    public void SetupZone(Vector3 dimensions, float targetHeadingY, bool reverseRequired)
    {
        zoneDimensions = dimensions;
        targetHeadingEulerY = targetHeadingY;
        requireReverseOrientation = reverseRequired;
        transform.rotation = Quaternion.Euler(0, targetHeadingY, 0);

        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            col.isTrigger = true;
            col.size = zoneDimensions;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponentInParent<CarController>();
        if (car != null)
        {
            currentCar = car;
            IsCarInside = true;
            stationaryTimer = 0f;
            FindObjectOfType<UIManager>()?.ShowStatusBanner("PARKING DETECTED", 1.8f);
            AudioManager.Instance?.PlayCountdownTick();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarController car = other.GetComponentInParent<CarController>();
        if (car != null && car == currentCar)
        {
            currentCar = null;
            IsCarInside = false;
            stationaryTimer = 0f;
            IsStationary = false;
            IsRotationCorrect = false;
        }
    }

    private void Update()
    {
        // Animate zone guide glow pulse
        pulseTime += Time.deltaTime * 3.5f;
        float pulse = 0.5f + 0.5f * Mathf.Sin(pulseTime);
        if (pulseLight != null)
        {
            pulseLight.intensity = 1.5f + pulse * 1.5f;
        }

        if (IsParkedSuccessfully || currentCar == null || !IsCarInside) return;

        // 1. Position Check: verify car center is closely aligned
        Vector3 localCarPos = transform.InverseTransformPoint(currentCar.transform.position);
        bool posValid = Mathf.Abs(localCarPos.x) <= (zoneDimensions.x * 0.45f) &&
                         Mathf.Abs(localCarPos.z) <= (zoneDimensions.z * 0.45f);

        // 2. Speed Check: car is almost completely stopped
        float speed = currentCar.SpeedMagnitude;
        IsStationary = speed <= maxStationarySpeed;

        // 3. Rotation Check: verify alignment with target heading
        float carHeading = currentCar.transform.eulerAngles.y;
        float angleDiff = Mathf.DeltaAngle(carHeading, targetHeadingEulerY);

        if (requireReverseOrientation)
        {
            // Reverse parking: front faces opposite or 180 deg
            float reverseDiff = Mathf.DeltaAngle(carHeading, targetHeadingEulerY + 180f);
            IsRotationCorrect = Mathf.Abs(reverseDiff) <= rotationToleranceDegrees;
        }
        else
        {
            // Standard forward parking: allow forward or reverse if not strictly mandated
            float forwardDiff = Mathf.Abs(angleDiff);
            float reverseDiff = Mathf.Abs(Mathf.DeltaAngle(carHeading, targetHeadingEulerY + 180f));
            IsRotationCorrect = forwardDiff <= rotationToleranceDegrees || reverseDiff <= rotationToleranceDegrees;
        }

        // 4. Time Check: must hold position
        if (posValid && IsStationary && IsRotationCorrect)
        {
            stationaryTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(stationaryTimer / requiredStationaryDuration);

            if (stationaryTimer >= requiredStationaryDuration)
            {
                OnSuccessParking();
            }
        }
        else
        {
            stationaryTimer = Mathf.Max(0f, stationaryTimer - Time.deltaTime * 2f);
        }
    }

    private void OnSuccessParking()
    {
        IsParkedSuccessfully = true;
        currentCar.SetControlsEnabled(false);

        FindObjectOfType<UIManager>()?.ShowStatusBanner("PERFECT PARKING!", 3.0f);
        AudioManager.Instance?.PlayParkingSuccess();

        // Notify ParkingManager / GameManager
        ParkingManager parkingManager = FindObjectOfType<ParkingManager>();
        if (parkingManager != null)
        {
            parkingManager.OnParkingCompleted();
        }
        else
        {
            GameManager.Instance?.OnLevelSuccess();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.9f, 0.3f, 0.4f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, zoneDimensions);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, zoneDimensions);
    }
}
