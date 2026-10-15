using UnityEngine;

/// <summary>
/// Intermediate-level mobile car controller for Parking Master 3D.
/// Features stable physics (lowered center-of-mass, zero flipping),
/// smooth mobile steering, acceleration, brake, and reverse gear.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Vehicle Configuration")]
    public float maxSpeed = 14f;
    public float maxReverseSpeed = 7f;
    public float acceleration = 10f;
    public float reverseAcceleration = 7f;
    public float brakeForce = 20f;
    public float maxSteerAngle = 38f;
    public float steerSpeed = 4.5f;
    public float naturalDeceleration = 5f;

    [Header("Components")]
    public Rigidbody rb;
    public WheelVisualizer wheelVisualizer;
    public CarLights carLights;

    // Inputs
    [Header("Current Inputs")]
    [Range(-1f, 1f)] public float steerInput = 0f;
    [Range(-1f, 1f)] public float throttleInput = 0f; // >0 accelerate, <0 reverse
    public bool isHandbraking = false;

    // State
    public float CurrentSpeed { get; private set; } // signed forward speed in m/s
    public float SpeedMagnitude => Mathf.Abs(CurrentSpeed);
    public bool IsReversing => CurrentSpeed < -0.1f || (throttleInput < -0.1f && CurrentSpeed < 0.2f);
    public bool IsBraking => (throttleInput < -0.05f && CurrentSpeed > 0.5f) ||
                             (throttleInput > 0.05f && CurrentSpeed < -0.5f) ||
                             isHandbraking;

    private float currentSteerAngle = 0f;
    private bool controlsEnabled = true;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (wheelVisualizer == null) wheelVisualizer = GetComponent<WheelVisualizer>();
        if (carLights == null) carLights = GetComponent<CarLights>();

        ConfigureRigidbody();
    }

    private void ConfigureRigidbody()
    {
        if (rb != null)
        {
            rb.mass = 1200f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 3.5f;
            rb.centerOfMass = new Vector3(0, -0.45f, 0.1f); // Lowered center of mass prevents roll-over
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            // Freeze X and Z rotation to completely guarantee no flipping
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    public void SetStats(CarStats stats)
    {
        if (stats == null) return;
        maxSpeed = stats.maxSpeed;
        acceleration = stats.acceleration;
        maxSteerAngle = stats.steerAngle;
        brakeForce = stats.brakeForce;
    }

    public void SetInput(float steer, float throttle, bool brake)
    {
        if (!controlsEnabled)
        {
            steerInput = 0f;
            throttleInput = 0f;
            isHandbraking = true;
            return;
        }

        steerInput = Mathf.Clamp(steer, -1f, 1f);
        throttleInput = Mathf.Clamp(throttle, -1f, 1f);
        isHandbraking = brake;
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
        if (!enabled)
        {
            steerInput = 0f;
            throttleInput = 0f;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        // Calculate signed forward speed
        CurrentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        // Apply Steering
        ApplySteering();

        // Apply Acceleration / Deceleration / Brake
        ApplyDriveForces();

        // Counteract lateral sliding (tire traction)
        ApplyLateralFriction();
    }

    private void ApplySteering()
    {
        float targetAngle = steerInput * maxSteerAngle;
        currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, targetAngle, maxSteerAngle * steerSpeed * Time.fixedDeltaTime);

        // Turn vehicle only if moving
        if (Mathf.Abs(CurrentSpeed) > 0.05f)
        {
            // Reverse steering direction when driving backwards
            float directionSign = CurrentSpeed >= 0 ? 1f : -1f;
            // Scale turn rate by speed to feel natural
            float speedFactor = Mathf.Clamp01(Mathf.Abs(CurrentSpeed) / 4f);
            float turnAmount = (currentSteerAngle / maxSteerAngle) * 55f * speedFactor * directionSign * Time.fixedDeltaTime;

            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    private void ApplyDriveForces()
    {
        if (isHandbraking)
        {
            // Strong handbrake
            Vector3 brakeVelocity = Vector3.MoveTowards(rb.linearVelocity, Vector3.zero, brakeForce * 1.5f * Time.fixedDeltaTime);
            rb.linearVelocity = brakeVelocity;
            return;
        }

        if (throttleInput > 0.05f)
        {
            // Driving Forward or Braking if moving backwards
            if (CurrentSpeed < -0.2f)
            {
                // Active braking while in reverse
                rb.AddForce(transform.forward * brakeForce * 1000f * Time.fixedDeltaTime, ForceMode.Force);
            }
            else if (CurrentSpeed < maxSpeed)
            {
                // Accelerate forward
                rb.AddForce(transform.forward * throttleInput * acceleration * 1000f * Time.fixedDeltaTime, ForceMode.Force);
            }
        }
        else if (throttleInput < -0.05f)
        {
            // Reverse or Braking if moving forwards
            if (CurrentSpeed > 0.2f)
            {
                // Active braking while in drive
                rb.AddForce(-transform.forward * brakeForce * 1000f * Time.fixedDeltaTime, ForceMode.Force);
            }
            else if (CurrentSpeed > -maxReverseSpeed)
            {
                // Accelerate reverse
                rb.AddForce(transform.forward * throttleInput * reverseAcceleration * 1000f * Time.fixedDeltaTime, ForceMode.Force);
            }
        }
        else
        {
            // Natural coasting deceleration
            if (Mathf.Abs(CurrentSpeed) > 0.05f)
            {
                float slowDown = Mathf.Sign(CurrentSpeed) * naturalDeceleration * 1000f * Time.fixedDeltaTime;
                rb.AddForce(-transform.forward * slowDown, ForceMode.Force);
            }
            else
            {
                // Completely stop jitter at low speeds
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }

    private void ApplyLateralFriction()
    {
        // Cancel out sideways drift so the car grips the asphalt cleanly
        Vector3 lateralVelocity = transform.right * Vector3.Dot(rb.linearVelocity, transform.right);
        rb.linearVelocity -= lateralVelocity * 0.85f;
    }

    private void Update()
    {
        // Visual Wheel update
        if (wheelVisualizer != null)
        {
            wheelVisualizer.UpdateWheels(CurrentSpeed, currentSteerAngle);
        }

        // Lighting update
        if (carLights != null)
        {
            carLights.SetBrakeLights(IsBraking);
        }
    }
}
