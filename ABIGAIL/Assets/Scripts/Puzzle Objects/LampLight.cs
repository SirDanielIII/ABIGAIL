using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampLight : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask mirrorLayerMask; // Layer mask for mirrors
    public LayerMask stopSurfaceLayerMask; // Layer mask for stop surfaces
    public Material lightMaterial; // Assign your light material here
    public int maxReflections = 10;
    public bool isSolved;

    private void Start()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = lightMaterial;
        isSolved = false;
    }

    private void Update()
    {
        Vector3 direction = transform.right;
        Vector3 startPosition = transform.position;

        ReflectRay(startPosition, direction, 0);
    }

    // Recursive function to handle reflections
    private void ReflectRay(Vector3 startPosition, Vector3 direction, int reflections)
    {
        if (reflections > maxReflections) return; // Limit reflections to prevent infinite loop

        // Perform raycast to find mirrors and windows
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, Mathf.Infinity, mirrorLayerMask | stopSurfaceLayerMask);

        // Draw the ray
        lineRenderer.positionCount = reflections + 1;
        lineRenderer.SetPosition(reflections, startPosition);

        if (hit.collider != null)
        {

            // Check if the ray hits a mirror
            if (hit.collider.CompareTag("Mirror"))
            {
                Vector2 reflectDir = Vector2.Reflect(direction, hit.normal);
                startPosition = hit.point + hit.normal * 0.01f; // Move slightly off the mirror to avoid self-intersections
                direction = reflectDir;

                // Recursive call for further reflections
                ReflectRay(startPosition, direction, reflections + 1);
            }
            // Check if the ray hits a window
            else if (hit.collider.CompareTag("Window"))
            {
                // Calculate the final ray position to extend beyond the collider
                float offsetDistance = 0.3f; // Adjust this value to set the offset distance
                Vector2 finalPosition = hit.point + (Vector2)(direction.normalized * offsetDistance);

                lineRenderer.positionCount = reflections + 2;
                lineRenderer.SetPosition(reflections + 1, finalPosition);

                // Do something when the ray hits a window
                isSolved = true;

            }
            else
            {
                // Check if the ray hits a surface that should stop further reflections
                RaycastHit2D stopSurfaceHit = Physics2D.Raycast(startPosition, direction, Mathf.Infinity, stopSurfaceLayerMask);
                if (stopSurfaceHit.collider != null)
                {
                    // Draw the final ray to the stop surface hit point
                    lineRenderer.positionCount = reflections + 2;
                    lineRenderer.SetPosition(reflections + 1, stopSurfaceHit.point);
                }
                else
                {
                    // Draw the final ray in the original direction
                    lineRenderer.positionCount = reflections + 2;
                    lineRenderer.SetPosition(reflections + 1, startPosition + (Vector3)(direction * 100f));
                }

                isSolved = false;
            }
        }
        else
        {
            // Draw the final ray if nothing is hit
            lineRenderer.positionCount = reflections + 2;
            lineRenderer.SetPosition(reflections + 1, startPosition + (Vector3)(direction * 100f));
        }
    }

}




//{
//    public LineRenderer lineRenderer;
//    public LayerMask ignoreLayer;
//    public Material lightMaterial; // Assign your light material here

//    private void Start()
//    {
//        lineRenderer.positionCount = 2;
//        lineRenderer.useWorldSpace = true;

//        // Assign the light material to the Line Renderer
//        lineRenderer.material = lightMaterial;
//    }

//    private void Update()
//    {
//        Vector3 direction = transform.right;
//        Vector3 startPosition = transform.position;

//        // Keep track of the number of reflections to avoid infinite loops
//        int reflections = 0;

//        while (reflections < 10) // Limit reflections to prevent infinite loop
//        {
//            RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, Mathf.Infinity, ~ignoreLayer);
//            Debug.DrawRay(startPosition, direction * 100f, Color.red);
//            lineRenderer.SetPosition(0, startPosition);

//            if (hit.collider != null && hit.collider.CompareTag("Mirror"))
//            {
//                Vector2 reflectDir = Vector2.Reflect(direction, hit.normal);
//                startPosition = hit.point + hit.normal * 0.01f; // Move slightly off the mirror to avoid self-intersections
//                direction = reflectDir;

//                // Draw the reflected ray
//                Debug.DrawRay(startPosition, direction * 100f, Color.red);
//                lineRenderer.SetPosition(1, startPosition + direction * 100f);

//                reflections++;
//            }
//            else
//            {
//                // If the ray doesn't hit a mirror, continue in the same direction
//                startPosition = startPosition + direction * 100f;
//                lineRenderer.SetPosition(1, startPosition);
//                break;
//            }
//        }
//    }
//}


