using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Movement_Top : MonoBehaviour
{
    public float speed = 22.0f;
    public Rigidbody2D rb;
    Vector2 movement; 

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        Vector2 normalizedMovement = movement.normalized;
        rb.MovePosition(rb.position + normalizedMovement * (speed * Time.fixedDeltaTime));
    }
}
