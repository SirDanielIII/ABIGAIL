using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Abigail
{
    public class Movement : MonoBehaviour
    {
        // Movement Values
        public float movementSpeed; // Speed of the character
        public float jumpInitialPower; // Speed of jump (normal)
        public float jumpHoldApplyAfter;
        public float jumpHoldPower; // Speed of jump (hold)
        public float jumpHoldTime;

        // Ground Check
        public Transform playerFeet;
        public float checkRadius;
        public LayerMask groundLayer;

        // The Player
        private Rigidbody2D rb;
        private Vector2 movement;
        private bool isFacingRight;
        private bool isJumping;
        private BoxCollider2D playerCollider;
        private Vector2 standingColliderSize;
        private Vector2 crouchingColliderSize;
        private bool isCrouching = false;

        // Timers
        private float jumpTimeStart;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // initializing the rigidbody
            playerCollider = GetComponent<BoxCollider2D>(); // Get the player's collider
            standingColliderSize = playerCollider.size; // Store the original size
            crouchingColliderSize = new Vector2(playerCollider.size.x, playerCollider.size.y / 2); // Define a smaller size for crouching
        }

        void FixedUpdate()
        {
            MovePlayer();
        }

        void Update()
        {
            HandleInput();
        }

        private bool isGrounded()
        {
            return Physics2D.OverlapCircle(playerFeet.position, checkRadius, groundLayer);
        }

        void MovePlayer()
        {
            rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
        }


        void HandleInput()
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");

            // Right Movement
            if (movement.x > 0f)
            {
                rb.velocity = new Vector2(movement.x * movementSpeed * Time.fixedDeltaTime, rb.velocity.y);
            }

            // Left Movement
            if (movement.x < 0f)
            {
                rb.velocity = new Vector2(movement.x * movementSpeed * Time.fixedDeltaTime, rb.velocity.y);
            }

            // Jump Movement
            if (Input.GetKey(KeyCode.Space))
            {
                if (isGrounded())
                {
                    isJumping = true; // Indicate that we're jumping
                    jumpTimeStart = Time.realtimeSinceStartup; // Save start time of jump
                    rb.velocity = new Vector2(rb.velocity.x, jumpInitialPower); // Actually do the jump
                }

                if (isJumping)
                {
                    float elapsedTime = Time.realtimeSinceStartup - jumpTimeStart; // Get current time - previous time

                    // Let's you jump higher if you hold down the key
                    if (elapsedTime < jumpHoldTime)
                    {
                        // Thanks Chat-GPT
                        // Calculate the normalized jump hold duration
                        float normalizedHoldTime = Mathf.Clamp01(elapsedTime / jumpHoldTime);
                        // Calculate the jump force based on the normalized hold time
                        float jumpForce = Mathf.Lerp(jumpInitialPower, jumpHoldPower, normalizedHoldTime);

                        // Apply the jump force
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                        
//                      // Old Jump Code
//                        if (elapsedTime >= jumpHoldApplyAfter)
//                        {
//                            rb.velocity = new Vector2(rb.velocity.x, jumpHoldPower);
//                        }
                        Debug.Log("Jump Elapsed Time: " + Math.Round(elapsedTime, 2) + " sec");
                    }
                    else if (elapsedTime >= jumpHoldTime)
                    {
                        isJumping = false;
                        Debug.Log("Jump: Finished Timer");
                    }
                }
            }

            // Reset jumping
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
                Debug.Log("Jump: Let Go");
            }

            // Crouch Movement S or DownArrow
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) 
            {
                Crouch(true);
            }
            else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                Crouch(false);
            }


            // Flip sprite image
            Flip();
        }


        private void Flip()
        {
            if (isFacingRight && movement.x < 0f || !isFacingRight && movement.x > 0f)
            {
                isFacingRight = !isFacingRight;
                Vector2 localScale = transform.localScale;
                localScale.x *= -1f;
                if (transform != null) transform.localScale = localScale;
            }
        }
        void Crouch(bool isCrouching)
        {
            this.isCrouching = isCrouching;
            playerCollider.size = isCrouching ? crouchingColliderSize : standingColliderSize;
        }
    }
}