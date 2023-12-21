using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [SerializeField] public LineRenderer lineRenderer;
    [SerializeField, Min(3)] private int lineSegments = 60;
    [SerializeField, Min(1)] private float timeOfTheFlight = 5;

    public Color startColor = Color.red;
    public Color endColor = Color.blue;
    public float transparency = 1f;
    public GameObject targetObject;
    private Material lineMaterial;
    private void Start()
    {
        lineMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        
    }

    public void ShowTrajectoryLine(Vector3 startPoint, Vector3 startVelocity, float tension)
    {
        float timeStep = timeOfTheFlight / lineSegments;
        Vector3[] lineRendererPoints = CalculateTrajectoryLine(startPoint, startVelocity, timeStep);

        // Check for collisions with the target object
        int collisionIndex = CheckCollisionIndex(lineRendererPoints, targetObject);

        // Set positions based on the collision index
        lineRenderer.positionCount = collisionIndex + 1;
        lineRenderer.SetPositions(lineRendererPoints);

        Color lineColor = new Color(startColor.r, startColor.g, startColor.b, transparency / tension);
        lineMaterial.SetColor("_Color", lineColor);
        lineRenderer.material = lineMaterial;
    }

    private Vector3[] CalculateTrajectoryLine(Vector3 startPoint, Vector3 startVelocity, float timeStep)
    {
        Vector3[] lineRendererPoints = new Vector3[lineSegments];
        lineRendererPoints[0] = startPoint;

        for (int i = 1; i < lineSegments; i++)
        {
            float timeOffset = timeStep * i;

            Vector3 progressBeforeGravity = startVelocity * timeOffset;
            Vector3 gravityOffset = Vector3.up * -0.5f * Physics.gravity.y * timeOffset * timeOffset;
            Vector3 newPosition = startPoint + progressBeforeGravity - gravityOffset;

            lineRendererPoints[i] = newPosition;
        }

        return lineRendererPoints;
    }

    private int CheckCollisionIndex(Vector3[] lineRendererPoints, GameObject targetObject)
    {
        for (int i = 0; i < lineRendererPoints.Length; i++)
        {
            // Check if the current trajectory point is below or at the same height as the target object
            if (lineRendererPoints[i].y <= targetObject.transform.position.y)
            {
                Debug.Log("Collision detected with the object!");
                return i;
            }
        }
        return lineRendererPoints.Length - 1; // Return the last index if no collision is detected
    }
}
