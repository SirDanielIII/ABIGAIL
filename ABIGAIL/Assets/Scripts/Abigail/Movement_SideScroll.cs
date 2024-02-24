
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;


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

        // Timers
        private float jumpTimeStart;

        public bool platformingOn;

        // Public strings so we can change the scene tranfer in unity for changing levels.
        public string platforming_scene = "Platforming";
        public string topDown_scene = "TopDown";

        void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // initializing the rigidbody


            // Partial if statement for current state of the perspective
            if (SceneManager.GetActiveScene().name == platforming_scene)
            {
                platformingOn = true;
            } else if (SceneManager.GetActiveScene().name == topDown_scene)
            {
                platformingOn = false;
            }
        }

        void FixedUpdate()
        {
            MovePlayer();
        }

        void Update()
        {
            
            /* Note for DANIEL ZHUO: if possible try to link the scene changes entirely to the active scene rather 
            than to the platformingOn boolean. Just to make it more fluid. I would do it myself but I spent 20 minutes setting up
            the current system (which I know is dumb) to ima just lay it onto you.
            */
            HandlePerspectiveInput();
            if (platformingOn) {
                HandlePlatformingInput();
            } else {
                HandleTopdownInput();
            }
        }

        private bool isGrounded()
        {
            return Physics2D.OverlapCircle(playerFeet.position, checkRadius, groundLayer);
        }

        void MovePlayer()
        {
            if (SceneManager.GetActiveScene().name == platforming_scene)
            {
                rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
            } else if (SceneManager.GetActiveScene().name == topDown_scene) 
            { 
                rb.velocity = new Vector2(movement.x * movementSpeed, movement.y * movementSpeed);
            }
        }

        void HandlePerspectiveInput() 
        { 
            if (Input.GetKeyDown(KeyCode.R))
            {
                
                platformingOn = !platformingOn;

                if (platformingOn == true) { 
                    SceneManager.LoadScene(platforming_scene);
                } else { 
                    SceneManager.LoadScene(topDown_scene);
                }

            }
        }

        void HandleTopdownInput() 
        { 
            Debug.Log("Player is in topdown view");

            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
        }
        void HandlePlatformingInput()
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
    }
}