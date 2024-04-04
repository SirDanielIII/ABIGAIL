using UnityEngine;

public class BackgroundFollower : MonoBehaviour
{
    public Transform cameraTransform; // Assign this in the Inspector with your main camera
    private Vector3 offset; // Maintain an offset if needed
    private float originalZ;

    void Start()
    {
        originalZ = transform.position.z; // Keep the background's original Z position
        if (cameraTransform != null)
        {
            // Calculate the offset from the camera, if you want the background to be offset from the camera's position.
            // If you want the background to directly follow without any offset, you can set offset to Vector3.zero
            offset = transform.position - cameraTransform.position;
            offset.z = 0; // Ignore Z offset to keep the background at its original Z position
        }
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // Follow the camera's position, applying the offset, but maintain the original Z position.
            Vector3 newPosition = cameraTransform.position + offset;
            newPosition.z = originalZ;
            transform.position = newPosition;
        }
    }
}
