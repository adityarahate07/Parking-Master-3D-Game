using UnityEngine;

/// <summary>
/// Marks an object as a collision hazard (Cone, Barrier, Parked Car, Wall).
/// Notifies CollisionManager with debounced hit detection.
/// </summary>
public class Obstacle : MonoBehaviour
{
    public enum HazardType { TrafficCone, Barrier, ParkedCar, Wall }
    public HazardType hazardType = HazardType.TrafficCone;

    private float lastHitTime = -1f;
    private const float HIT_COOLDOWN = 1.0f;

    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject other)
    {
        if (Time.time - lastHitTime < HIT_COOLDOWN) return;

        CarController playerCar = other.GetComponentInParent<CarController>();
        if (playerCar != null)
        {
            lastHitTime = Time.time;
            CollisionManager.Instance?.RegisterCollision(gameObject, hazardType.ToString());
        }
    }
}
