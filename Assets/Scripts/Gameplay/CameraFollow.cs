using UnityEngine;

/// <summary>
/// Smooth third-person chase camera with dual-view support (Third-person chase & Top-down parking view).
/// Prevents jitter, follows car rotation smoothly, and switches modes on camera button press.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public enum CameraMode { ThirdPerson, TopDownParking }

    [Header("Target")]
    public Transform target;

    [Header("Chase Mode Offsets")]
    public Vector3 chaseOffset = new Vector3(0f, 3.8f, -6.5f);
    public float chaseSmoothSpeed = 8f;
    public float rotationSmoothSpeed = 5f;
    public float lookAheadDistance = 2.0f;

    [Header("Top-Down Mode Offsets")]
    public Vector3 topDownOffset = new Vector3(0f, 12.0f, -2.5f);
    public float topDownPitchAngle = 72f;

    [Header("Mode")]
    public CameraMode currentMode = CameraMode.ThirdPerson;

    private void Start()
    {
        if (target == null)
        {
            CarController car = FindObjectOfType<CarController>();
            if (car != null) target = car.transform;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ToggleCameraMode()
    {
        currentMode = (currentMode == CameraMode.ThirdPerson) ? CameraMode.TopDownParking : CameraMode.ThirdPerson;
        AudioManager.Instance?.PlayButtonClick();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            CarController car = FindObjectOfType<CarController>();
            if (car != null) target = car.transform;
            else return;
        }

        if (currentMode == CameraMode.ThirdPerson)
        {
            UpdateThirdPersonView();
        }
        else
        {
            UpdateTopDownView();
        }
    }

    private void UpdateThirdPersonView()
    {
        // Calculate desired position in target's coordinate frame
        Vector3 targetPos = target.position;
        Quaternion targetRot = Quaternion.Euler(0, target.eulerAngles.y, 0);

        Vector3 desiredPosition = targetPos + (targetRot * chaseOffset);

        // Smooth position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * chaseSmoothSpeed);

        // Look at point slightly in front of the car
        Vector3 lookPoint = targetPos + (targetRot * Vector3.forward * lookAheadDistance) + Vector3.up * 1.0f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookPoint - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed);
    }

    private void UpdateTopDownView()
    {
        Vector3 targetPos = target.position;
        Vector3 desiredPosition = targetPos + topDownOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * chaseSmoothSpeed);

        Quaternion desiredRotation = Quaternion.Euler(topDownPitchAngle, target.eulerAngles.y * 0.2f, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed);
    }
}
