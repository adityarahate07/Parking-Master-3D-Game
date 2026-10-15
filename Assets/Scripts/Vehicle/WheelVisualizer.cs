using UnityEngine;

/// <summary>
/// Visually rotates the 4 car wheels according to vehicle forward speed,
/// and turns front wheels left/right according to steering input.
/// </summary>
public class WheelVisualizer : MonoBehaviour
{
    [Header("Wheel Transforms")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    [Header("Settings")]
    public float wheelRadius = 0.35f;

    private float currentSteerAngle = 0f;

    /// <summary>
    /// Updates wheel spin and steering angle.
    /// </summary>
    public void UpdateWheels(float forwardSpeed, float targetSteerAngle)
    {
        // Smooth steer transition
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.deltaTime * 12f);

        // Spin calculation: distance = v * dt, angle = distance / radius (rad) * rad2deg
        float spinAngle = (forwardSpeed / wheelRadius) * Mathf.Rad2Deg * Time.deltaTime;

        // Front wheels: steer yaw + spin pitch
        if (frontLeftWheel != null)
        {
            frontLeftWheel.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);
            frontLeftWheel.Rotate(Vector3.right, spinAngle, Space.Self);
        }
        if (frontRightWheel != null)
        {
            frontRightWheel.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);
            frontRightWheel.Rotate(Vector3.right, spinAngle, Space.Self);
        }

        // Rear wheels: spin only
        if (rearLeftWheel != null)
        {
            rearLeftWheel.Rotate(Vector3.right, spinAngle, Space.Self);
        }
        if (rearRightWheel != null)
        {
            rearRightWheel.Rotate(Vector3.right, spinAngle, Space.Self);
        }
    }
}
