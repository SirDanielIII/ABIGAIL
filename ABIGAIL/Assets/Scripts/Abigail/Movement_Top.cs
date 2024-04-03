using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Movement_Top : MonoBehaviour
{
    public float speed = 22.0f;
    public Rigidbody2D rb;
    Vector2 movement; 
    private Animator animator;
    public float rotationSpeed = 720f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        bool isRunning = movement.magnitude > 0;
        animator.SetBool("IsRunning", isRunning);
        
        if (isRunning)
        {
            UpdateDirection();
        }
        Vector2 normalizedMovement = movement.normalized;
        rb.MovePosition(rb.position + normalizedMovement * (speed * Time.fixedDeltaTime));
    }

    void UpdateDirection()
    {
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
