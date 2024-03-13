using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Abigail
{
    public enum LevelType
    {
        SideScroll,
        TopDown
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
        public float movementSpeed = 11f; // Speed of the character (normal)
        public float movementSpeedAcceleration = 5; // How fast / slow the player accelerates 
        public float sprintSpeed = 16f; // Speed of the character (when sprinting)
        public float staminaTotal = 100f;
        public float staminaSprintRate = 25f;
        public float staminaJumpCost = 25f;
        public float staminaSprintJumpRate = 30f;
        public float staminaSlideCost = 8;
        public float staminaRecoveryDelay = 1.5f; // The recovery delay after sprinting
        public float staminaRecoveryRate = 15f; // How fast stamina recharges

        public float sprintJumpBoost = 1.15f; // How much extra jump there is when sprinting
        public float jumpInitialPower = 8f; // Speed of jump (normal)
        public float jumpHoldApplyAfter = 0.06f;
        public float jumpHoldPower = 11f; // Speed of jump (hold)
        public float jumpHoldTime = 0.2f; // Jump duration
        public float quicksandJumpHoldTime = 0.05f; // Jump duration in quicksand
        public float crouchSlowdownMultiplier = 0.5f; // How much slower the player moves when crouching
        public float slideSpeedBoostMultiplier = 1.05f; // How much extra boost the player gets when sliding
        public float slideSpeedBoostDuration = 0.05f; // How long the slide boost lasts for
        public float slideSlowdownRate = 0.15f; // How much the slide slows down the player
        public Vector2 colliderSizeStanding = new Vector2(1f, 1);
        public Vector2 colliderSizeCrouching = new Vector2(1f, 0.7f);
        public Vector2 colliderSizeSliding = new Vector2(1f, 0.3f);

        // The Player
        private Rigidbody2D rb;
        private Vector2 movement;
        private Direction slideDirection;
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
        private float sinkingSpeed = -0.05f;

        // Level Modifiers
        public LevelType levelType;
        public int sideScrollSceneIndex = -1;
        public int topDownSceneIndex = -1;

        // Timers
        private float jumpTimeStart;
        private float sprintTimeEnd;
        private float slideTimeStart;
        public float knockbackCooldown = 1f;
        private float lastKnockbackTime = -10f;

        // Script References
        private Quicksand quicksand;


        void Start()
        {
            // Get the player's attributes
            rb = GetComponent<Rigidbody2D>();
            playerCollider = GetComponent<BoxCollider2D>();
            playerRenderer = GetComponent<SpriteRenderer>();

            // Set attributes
            playerCollider.size = colliderSizeStanding;

            // Set variables
            stamina = staminaTotal;
        }

        void FixedUpdate()
        {
            if (isSliding)
            {
                HandleSlide();
                HandleJumping();
            }
            else
            {
                // Handle horizontal movement
                if (!isKnockedBack)
                {
                    // Allow horizontal movement always, even in quicksand
                    float speed = isSprinting ? sprintSpeed : movementSpeed;
                    if (isCrouching)
                    {
                        speed *= crouchSlowdownMultiplier;
                    }

                    rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
                }

                // Specific handling for side-scrolling level type
                if (levelType == LevelType.SideScroll)
                {
                    HandleJumping();
                }
                else if (levelType == LevelType.TopDown)
                {
                    // Apply top-down movement logic if necessary
                    float speed = isSprinting ? sprintSpeed : movementSpeed;
                    if (isCrouching)
                    {
                        speed *= crouchSlowdownMultiplier;
                    }

                    rb.velocity = new Vector2(movement.x * speed, movement.y * speed);
                }
            }
        }

        private void HandleJumping()
        {
            if (isInQuicksand)
            {
                HandleQuicksandJump();
            }
            else
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
        }

        private void HandleQuicksandJump()
        {
            if (!doJump && !isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, sinkingSpeed);
            }

            if (doJump)
            {
                QuicksandJump();
                doJump = false;
            }

            if (isJumping)
            {
                ContinueQuicksandJump();
            }
        }

        private void Jump()
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
            // Set this variable back to false so we don't jump a second time
            doJump = false;
        }

        private void ContinueJump()
        {
            float elapsedTime = CalculateElaspedTime(jumpTimeStart);
            // Let's you jump higher if you hold down the key
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
        }

        private void QuicksandJump()
        {
            float multiplier = 0.3f;
            rb.velocity = new Vector2(rb.velocity.x, jumpInitialPower * multiplier);
        }

        private void ContinueQuicksandJump()
        {
            float elapsedTime = CalculateElaspedTime(jumpTimeStart);
            // Let's you jump higher if you hold down the key
            if (elapsedTime <= quicksandJumpHoldTime)
            {
                // Calculate the normalized jump hold duration
                float normalizedHoldTime = Mathf.Clamp01(elapsedTime / quicksandJumpHoldTime);
                // Calculate the jump force based on the normalized hold time
                float jumpForce = Mathf.Lerp(jumpInitialPower * 0.2f, 0, normalizedHoldTime);
                // Apply the jump force
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        private void HandleSlide()
        {
            var velocity = rb.velocity;
            if (CalculateElaspedTime(slideTimeStart) <= slideSpeedBoostDuration)
            {
                if (levelType == LevelType.SideScroll)
                {
                    velocity = new Vector2(velocity.x * slideSpeedBoostMultiplier, velocity.y);
                }
                else if (levelType == LevelType.TopDown)
                {
                    velocity = new Vector2(velocity.x * slideSpeedBoostMultiplier, movement.y * slideSpeedBoostMultiplier);
                }
            }
            else
            {
                switch (facingDirection)
                {
                    case Direction.Left:
                        velocity = new Vector2(
                            Math.Clamp(velocity.x + slideSlowdownRate, Mathf.NegativeInfinity, 0F),
                            velocity.y);
                        break;
                    case Direction.Right:
                        velocity = new Vector2(
                            Math.Clamp(velocity.x - slideSlowdownRate, 0F, Mathf.Infinity),
                            velocity.y);
                        break;
                    case Direction.Up:
                        velocity = new Vector2(
                            velocity.x,
                            Math.Clamp(velocity.y - slideSlowdownRate, 0F, Mathf.Infinity));
                        break;
                    case Direction.Down:
                        velocity = new Vector2(
                            velocity.x,
                            Math.Clamp(velocity.y - slideSlowdownRate, Mathf.NegativeInfinity, 0));
                        break;
                    case Direction.TopLeft:
                        velocity = new Vector2(
                            Math.Clamp(velocity.x + slideSlowdownRate, Mathf.NegativeInfinity, 0F),
                            Math.Clamp(velocity.y - slideSlowdownRate, 0F, Mathf.Infinity));
                        break;
                    case Direction.TopRight:
                        velocity = new Vector2(
                            Math.Clamp(velocity.x - slideSlowdownRate, 0F, Mathf.Infinity),
                            Math.Clamp(velocity.y - slideSlowdownRate, 0F, Mathf.Infinity));
                        break;
                    case Direction.BottomLeft:
                        velocity = new Vector2(
                            Math.Clamp(velocity.x + slideSlowdownRate, Mathf.NegativeInfinity, 0F),
                            Math.Clamp(velocity.y - slideSlowdownRate, Mathf.NegativeInfinity, 0));
                        break;
                    case Direction.BottomRight:
                        velocity = new Vector2(
                            Math.Clamp(velocity.x - slideSlowdownRate, 0F, Mathf.Infinity),
                            Math.Clamp(velocity.y - slideSlowdownRate, Mathf.NegativeInfinity, 0));
                        break;
                    case Direction.Idle:
                        // Stop slide if not moving anymore
                        isSliding = false;
                        playerCollider.size = colliderSizeStanding;
                        break;
                }
            }
            // Debug.Log(velocity);
            rb.velocity = velocity;
        }

        void Update()
        {
            HandlePlatformingInput();
            OutputLogsToConsole();
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(playerFeet.position, checkRadius, groundLayer);
        }

        void HandlePlatformingInput()
        {
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
            if (Input.GetKey(KeyCode.Space) && levelType == LevelType.SideScroll)
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
                    if (CalculateElaspedTime(jumpTimeStart) >= jumpHoldTime)
                    {
                        isJumping = false; // Stop jump after a certain amount of elapsed time
                    }

                    isCrouching = false;
                    isSliding = false;
                }
            }

            // Crouch Movement S 
            if (levelType == LevelType.SideScroll && Input.GetKeyDown(KeyCode.S))
            {
                isCrouching = true;
                playerCollider.size = colliderSizeCrouching;
            }

            // Slide
            if (Input.GetKeyDown(KeyCode.LeftControl) && !isSliding && grounded && !isCrouching && stamina >= staminaSlideCost)
            {
                stamina = Mathf.Clamp(stamina - staminaSlideCost, 0f, Mathf.Infinity);
                isSliding = true;
                slideTimeStart = Time.realtimeSinceStartup;
                playerCollider.size = colliderSizeSliding;
            }

            // Release Crouch
            if (levelType == LevelType.SideScroll && Input.GetKeyUp(KeyCode.S))
            {
                isCrouching = false;
                playerCollider.size = colliderSizeStanding;
            }

            // Release Jump
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
            }

            // Release Sprint
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isSprinting = false;
            }

            // Release Slide
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                isSliding = false;
                playerCollider.size = colliderSizeStanding;
            }

            // Perspective Change
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (levelType == LevelType.SideScroll)
                {
                    SceneManager.LoadScene(topDownSceneIndex);
                }
                else if (levelType == LevelType.TopDown)
                {
                    SceneManager.LoadScene(sideScrollSceneIndex);
                }
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
            rb.velocity = knockbackVelocity; // Apply knockback velocity directly
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

        // void TryJumpOutOfQuicksand()
        // {
        //     Debug.Log("Attempting to jump out of quicksand");
        //     Debug.Log($"Velocity before jump quicksand: {rb.velocity}");
        //     rb.velocity = new Vector2(rb.velocity.x, jumpInitialPower * 0.5f);
        //     Debug.Log($"Velocity after jump quicksand: {rb.velocity}");
        //     doJump = false;
        // }

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
                "Slowing Down From Slide: " + (CalculateElaspedTime(slideTimeStart) >= slideSpeedBoostDuration),
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