using UnityEngine;

public class Butte : MonoBehaviour
{
    public Vector2 size = new Vector2(1, 1);

    // This method is called when the script is loaded or a value is changed in the Inspector
    void OnValidate()
    {
        Resize();
    }

    void Start()
    {
        Resize();
    }

    void Resize()
    {
        // Apply scaling based on the size variable
        transform.localScale = new Vector3(size.x, size.y, transform.localScale.z);
    }
}
