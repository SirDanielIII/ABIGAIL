using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampLight : MonoBehaviour
{
    public LineRenderer lineRenderer; // Line renderer to visualize the ray
    public LayerMask ignoreLayer; // Layer to ignore for raycasting

    private Vector3 previousEndPosition; // Store the end position of the previous ray

    void Start()
    {
        // Initialize the line renderer
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true; // Set to use world space positions

        // Initialize the previous end position to the starting position of the lamp
        previousEndPosition = transform.position;
    }

    void Update()
    {
        // Get the direction the lamp is pointing
        Vector3 direction = transform.right;

        // Cast a ray from the lamp's position in the direction it's pointing, ignoring the specified layer
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, ~ignoreLayer);

        // Debug draw the raycast
        Debug.DrawRay(transform.position, direction * 100f, Color.red);

        // Set the line renderer start position
        lineRenderer.SetPosition(0, transform.position);

        // Initialize the end position of the Line Renderer
        Vector3 endPosition = transform.position + direction * 100f;

        if (hit.collider != null)
        {
            // If the ray hits a mirror, calculate the reflection
            if (hit.collider.CompareTag("Mirror"))
            {
                Vector2 reflectDir = Vector2.Reflect(direction, hit.normal);

                // Cast a new ray from the hit point in the reflected direction
                RaycastHit2D reflectHit = Physics2D.Raycast(hit.point, reflectDir, Mathf.Infinity, ~ignoreLayer);

                // If the reflected ray hits something, update the end position to the reflection point
                if (reflectHit.collider != null)
                {
                    endPosition = reflectHit.point;
                }
            }
        }

        // Set the line renderer end position
        lineRenderer.SetPosition(1, endPosition);

        // Update the previous end position for the next frame
        previousEndPosition = endPosition;
    }
}





// Update is called once per frame
//void Update()
//{
//    float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
//    Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

//    RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 50f, layersToHit);
//    transform.localScale = new Vector3(50f, transform.localScale.y, 1);
//    if (hit.collider == null)
//    {

//        return;
//    }
//    transform.localScale = new Vector3(hit.distance, transform.localScale.y, 1);
//    Debug.Log(hit.collider.gameObject.name);
//}
