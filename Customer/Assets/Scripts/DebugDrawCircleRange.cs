using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawCircleRange : MonoBehaviour
{
    public int Id => id;
    public float Radius => radius;

    Vector3 center;

    [SerializeField]
    bool shouldDraw = false;

    [Header("Circle")]
    [SerializeField]
    int id = 0;
    [SerializeField]
    float radius = 5f; // Radius of the circle
    [SerializeField]
    Color color = Color.red; // Color of the circle

    [Header("All")]
    [SerializeField]
    int segments = 50; // Number of segments to create the circle
    float duration = 0.0f; // Duration for which the circle is visible

    void OnDrawGizmos()
    {
        if (shouldDraw)
        {
            center = this.transform.position;
            DrawCircle(center, radius, segments, color, duration);
        }
    }

    void DrawCircle(Vector3 center, float radius, int segments, Color color, float duration)
    {
        float angle = 0f;
        float angleStep = 360f / segments;

        Vector3 prevPoint = center + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, 0, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
        angle += angleStep;

        for (int i = 1; i <= segments; i++)
        {
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, 0, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
            Debug.DrawLine(prevPoint, nextPoint, color, duration);
            prevPoint = nextPoint;
            angle += angleStep;
        }
    }
}