using UnityEngine;

/// <summary>
/// Patrols smoothly back and forth between two world coordinates.
/// Used for moving obstacles in Level 7 and Level 10.
/// </summary>
public class MovingObstacle : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float moveSpeed = 2.0f;

    private float pingPongT = 0f;

    public void Setup(Vector3 a, Vector3 b, float speed)
    {
        pointA = a;
        pointB = b;
        moveSpeed = speed;
        transform.position = a;
    }

    private void Update()
    {
        pingPongT += Time.deltaTime * (moveSpeed / Mathf.Max(0.1f, Vector3.Distance(pointA, pointB)));
        float t = Mathf.PingPong(pingPongT, 1.0f);
        transform.position = Vector3.Lerp(pointA, pointB, t);

        // Turn towards moving direction
        Vector3 dir = (Mathf.Sin(pingPongT * Mathf.PI) > 0) ? (pointB - pointA) : (pointA - pointB);
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
