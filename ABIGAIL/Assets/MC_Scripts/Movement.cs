using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    // setting a rigidbody object 
    private Rigidbody2D rb;
    public float speed; // Speed of the character
    public float jumpForce;
    private float moveInput; // Values for the horizontal input movement
    private bool isGrounded;
    private bool isJumping;
    public Transform feet;
    public float checkRadius;
    public LayerMask whatIsGround;

    private float jumpTimeCounter;
    public float jumpTime;
    void Start() { 
        rb = GetComponent<Rigidbody2D>(); // initializing the rigidbody
    }

    void FixedUpdate() {
        moveInput = Input.GetAxisRaw("Horizontal"); // setting the move input to horizontal
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y); // giving that moveinput some force
    }

    void Update() {
        isGrounded = Physics2D.OverlapCircle(feet.position, checkRadius, whatIsGround);

        if (isGrounded == true && Input.GetKeyDown(KeyCode.Space)) {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping == true) {
            if (jumpTimeCounter > 0) {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            } else { 
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space)) { 
            isJumping = false;
        }

    }
}