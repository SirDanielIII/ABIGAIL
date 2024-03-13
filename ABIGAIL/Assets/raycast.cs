using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycast : MonoBehaviour
{

    public float hitDistance;
    public bool hit;
    public RaycastHit2D hitData;

    // Update is called once per frame
    void Update()
    {
        FireRay();

    }



    void FireRay()
    {
        Ray2D ray = new Ray2D(transform.position, transform.right);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            hitData = hit;
            hitDistance = hit.distance;
        }

    }
}
