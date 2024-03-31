using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.XR;

namespace Abigail
{
    public enum LevelType
    {
        SideScroll
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Idle
    }

    public class Movement : MonoBehaviour
    {
        // Ground Check
        public Transform playerFeet;
        public float checkRadius = 0.1f;
        public LayerMask groundLayer;
        public Direction facingDirection = Direction.Right;

        // Movement Values
        public float movementSpeed = 8f; // Speed of the character (normal)
        public float movementSpeedAcceleration = 5; // How fast / slow the player accelerates 
        public float sprintSpeed = 16f; // Speed of the character (when sprinting)
        public float staminaTotal = 100f;
        public float staminaSprintRate = 25f;
        public float staminaJumpCost = 25f;
        public float staminaSprintJumpRate = 30f;
        public float staminaRecoveryDelay = 1.5f; // The recovery delay after sprinting
        public float staminaRecoveryRate = 15f; // How fast stamina recharges

        public float sprintJumpBoost = 1.15f; // How much extra jump there is when sprinting
        public float jumpInitialPower = 8f; // Speed of jump (normal)
        public float jumpHoldApplyAfter = 0.06f;
        public float jumpHoldPower = 11f; // Speed of jump (hold)
        public float jumpHoldTime = 0.2f; // Jump duration
        public float crouchSlowdownMultiplier = 0.5f; // How much slower the player moves when crouching
        public Vector2 colliderSizeStanding = new Vector2(1f, 1);
        public Vector2 colliderSizeCrouching = new Vector2(1f, 0.7f);
        public Vector2 colliderSizeSliding = new Vector2(1f, 0.3f);

        // The Player
        private Rigidbody2D rb;
        private Vector2 movement;
        private bool isJumping = false;
        private bool doJump = false;
        private BoxCollider2D playerCollider;
        private SpriteRenderer playerRenderer;
        private float stamina;
        private bool isSprinting = false;
        private bool isSliding = false;
        private bool isCrouching = false;
        public bool isKnockedBack = false;
        public bool isInQuicksand = false;
        public float quicksandSpeedFactor = 0.01f;
        private float speed;
        public float speedMultiplier = 1f;
        private float quicksandSinkRate = -0.35f;
        private float quicksandJumpPower = 1.2f;
        private float quicksandJumpHoldTime = 0.05f;


        // Level Modifiers
        public LevelType levelType;
        public int sideScrollSceneIndex = -1;
        public int topDownSceneIndex = -1;

        // Timers
        private float jumpTimeStart;
        private float sprintTimeEnd;
        public float knockbackCooldown = 0f;
        private float lastKnockbackTime = -10f;
        private IEnumerator quicksandDamageCoroutine;
        private Health playerHealth;

        void Start()
        {
            // Get the player's attributes
            rb = GetComponent<Rigidbody2D>();
            playerCollider = GetComponent<BoxCollider2D>();
            playerRenderer = GetComponent<SpriteRenderer>();
            playerHealth = GetComponent<Health>();

            // Set attributes
            playerCollider.size = colliderSizeStanding;

            // Set variables
            stamina = staminaTotal;
            speed = movementSpeed;
        }

        void FixedUpdate()
        {
            HandleMovement(); 
            if (isInQuicksand && !isJumping)
            {
                SinkInQuicksand();
            }
        }

        void Update()
        {
            HandlePlatformingInput();
        }

        public void HandleQuicksand(bool isInQuicksand)
        {
            this.isInQuicksand = isInQuicksand;
            if (isInQuicksand)
            {
                speed = movementSpeed * quicksandSpeedFactor;
                rb.gravityScale = 0;
                if (quicksandDamageCoroutine == null)
                {
                    quicksandDamageCoroutine = ApplyQuicksandDamage();
                    StartCoroutine(quicksandDamageCoroutine);
                }
                
            }
            else
            {
                speed = movementSpeed;
                rb.gravityScale = 3.5f;
                if (quicksandDamageCoroutine != null)
                {
                    StopCoroutine(quicksandDamageCoroutine);
                    quicksandDamageCoroutine = null;
                }
            }
        }

        private IEnumerator ApplyQuicksandDamage()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                }
            }
        }
        private void SinkInQuicksand()
        {
            // Manually apply a sinking effect by moving the player down at a constant rate
            rb.velocity = new Vector2(rb.velocity.x, quicksandSinkRate);
        }


        private void HandleMovement()
        {
            if (!isKnockedBack)
            {
                float speed = isSprinting ? sprintSpeed : movementSpeed;
                if (isCrouching)
                {
                    speed *= crouchSlowdownMultiplier;
                }

                rb.velocity = new Vector2(movement.x * speed * speedMultiplier, rb.velocity.y);
            }
            HandleJumping();
        }

        private void HandleJumping()
        {
            if (doJump)
            {
                Jump();
            }

            if (isJumping)
            {
                ContinueJump();
            } 
        }

        private void Jump()
        {
            if (isInQuicksand)
            {
                rb.velocity = new Vector2(rb.velocity.x, quicksandJumpPower);
                rb.gravityScale = 0;
                StartCoroutine(ResetJumpAfterDelay(quicksandJumpHoldTime));
            }
            else
            {
                float multiplier = 1.0f;
                // If stamina is less than the full amount of stamina  
                if (stamina < (isSprinting ? staminaSprintJumpRate : staminaJumpCost))
                {
                    multiplier = stamina / (isSprinting ? staminaSprintJumpRate : staminaJumpCost);
                }

                // Subtract from the stamina
                stamina -= isSprinting ? staminaSprintJumpRate : staminaJumpCost;
                // Clamp the value to ensure it doesn't go below zero
                stamina = Mathf.Clamp(stamina, 0f, Mathf.Infinity);
                // Do the jump
                rb.velocity = isSprinting
                    ? new Vector2(rb.velocity.x, jumpInitialPower + sprintJumpBoost * multiplier)
                    : new Vector2(rb.velocity.x, jumpInitialPower); 
            }
            doJump = false;
        }

        private void ContinueJump()
        {
            float elapsedTime = CalculateElaspedTime(jumpTimeStart);
            if (!isInQuicksand && isJumping)
            {
                if (elapsedTime <= jumpHoldTime)
                {
                    // Calculate the normalized jump hold duration
                    float normalizedHoldTime = Mathf.Clamp01(elapsedTime / jumpHoldTime);
                    // Calculate the jump force based on the normalized hold time
                    float jumpForce = Mathf.Lerp(jumpInitialPower, jumpHoldPower, normalizedHoldTime);
                    // Apply the jump force
                    rb.velocity = isSprinting
                        ? new Vector2(rb.velocity.x, jumpForce * sprintJumpBoost)
                        : new Vector2(rb.velocity.x, jumpForce);
                }
                else
                {
                    isJumping = false;
                }

            }
        }

        private IEnumerator ResetJumpAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            isJumping = false;

            // Reset gravity if no longer in quicksand
            if (!isInQuicksand)
            {
                rb.gravityScale = 3.5f;
            }
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(playerFeet.position, checkRadius, groundLayer);
        }

        void HandlePlatformingInput()
        {
            if (isKnockedBack) return;
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
            if (movement is { x: 0, y: 0 })
            {
                facingDirection = Direction.Idle;
            }
            else
                facingDirection = movement.x switch
                {
                    < 0 when movement.y < 0 => Direction.BottomLeft,
                    < 0 when movement.y > 0 => Direction.TopLeft,
                    < 0 => Direction.Left,
                    > 0 => movement.y switch
                    {
                        < 0 => Direction.BottomRight,
                        > 0 => Direction.TopRight,
                        _ => Direction.Right
                    },
                    _ => movement.y < 0 ? Direction.Down : Direction.Up
                };

            var grounded = IsGrounded();
            // Restore Stamina
            // If not sprinting, and enough time has passed since the last sprint, start the recovery
            if (!isSprinting && CalculateElaspedTime(sprintTimeEnd) >= staminaRecoveryDelay)
            {
                stamina += staminaRecoveryRate * Time.deltaTime;
                // Clamp the value to ensure it doesn't go above the max value
                stamina = Mathf.Clamp(stamina, 0f, staminaTotal);
            }

            // Allow sprint if there's enough stamina
            if (Input.GetKey(KeyCode.LeftShift) && !isSprinting && grounded && !isCrouching && stamina >= 0)
            {
                isSprinting = true;
            }

            // Handle stamina decrease for sprinting
            if (isSprinting)
            {
                stamina -= staminaSprintRate * Time.deltaTime;
                // Clamp the value to ensure it doesn't go below zero
                stamina = Mathf.Clamp(stamina, 0f, Mathf.Infinity);
                // Check for no more stamina or key release
                if (stamina == 0 || Input.GetKeyUp(KeyCode.LeftShift))
                {
                    isSprinting = false;
                    sprintTimeEnd = Time.realtimeSinceStartup; // Save time when sprint was stopped
                }
            }

            // Jump Movement
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (grounded || isInQuicksand)
                {
                    isJumping = true; // Indicate that we're jumping
                    doJump = true; // Tell code in FixedUpdate to jump
                    jumpTimeStart = Time.realtimeSinceStartup; // Save start time of jump
                    isCrouching = false;
                    isSliding = false;
                }
                else if (isJumping)
                {
                    if (isInQuicksand)
                    {
                        if (CalculateElaspedTime(jumpTimeStart) >= quicksandJumpHoldTime)
                        {
                            isJumping = false; // Stop jump after a certain amount of elapsed time
                        }
                    }
                    if (CalculateElaspedTime(jumpTimeStart) >= jumpHoldTime)
                    {
                        isJumping = false; // Stop jump after a certain amount of elapsed time
                    }

                    isCrouching = false;
                    isSliding = false;
                }
            }

            // Crouch Movement S 
            if (Input.GetKeyDown(KeyCode.S))
            {
                isCrouching = true;
                playerCollider.size = colliderSizeCrouching;
            }

        }


        public bool CanBeKnockedBack()
        {
            return Time.time - lastKnockbackTime >= knockbackCooldown;
        }

        public void ApplyKnockback(Vector2 knockbackVelocity, float duration)
        {
            if (!CanBeKnockedBack()) return;
            lastKnockbackTime = Time.time;
            isKnockedBack = true;
            rb.velocity = knockbackVelocity;
            StartCoroutine(ResetKnockbackState(duration));
        }

        private IEnumerator ResetKnockbackState(float duration)
        {
            yield return new WaitForSeconds(duration);
            isKnockedBack = false;
        }

        private float CalculateElaspedTime(float current)
        {
            return Time.realtimeSinceStartup - current;
        }

        private List<string> GetLogs()
        {
            var messages = new List<string>
            {
                "Current View: " + levelType,
                "Stamina Left: " + stamina,
                "Is Crouching: " + isCrouching,
                "Is Jumping: " + isJumping,
                "Is Sliding: " + isSliding,
                "Is Sprinting: " + isSprinting,
                "Time Since Last Sprint: " + CalculateElaspedTime(sprintTimeEnd) + "s",
                "Time Since Last Jump: " + CalculateElaspedTime(jumpTimeStart) + "s",
                "Movement: " + movement,
                "Velocity: " + rb.velocity,
                "Direction: " + facingDirection
            };
            return messages;
        }

        private void OutputLogsToConsole()
        {
        //    var messages = GetLogs();
        //    foreach (string i in messages)
        //    {
        //        Debug.Log(i);
        //    }
        }
    }
}