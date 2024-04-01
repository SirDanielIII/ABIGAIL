using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abigail
{
    public class Movement : MonoBehaviour
    {
        // Ground Check
        public Transform playerFeet;
        public float checkRadius = 0.1f;
        public LayerMask groundLayer;
        public GlobalEnums.Direction facingDirection = GlobalEnums.Direction.Right;
        public bool enableSliding = false;
        public bool enableCrouching = true;

        // Movement Values
        public float movementSpeed = 8f; // Speed of the character (normal)
        public float topDownSpeedMultiplier = 1.8f; // Speed multiplier for top-down view
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
        // public float fallMultiplier = 2.5f; // Modifies the falling speed during jump
        public float crouchSlowdownMultiplier = 0.5f; // How much slower the player moves when crouching
        public float slideSpeedBoostMultiplier = 1.05f; // How much extra boost the player gets when sliding
        public float slideSpeedBoostDuration = 0.05f; // How long the slide boost lasts for
        public float slideSlowdownRate = 0.15f; // How much the slide slows down the player

        // The Player
        private Rigidbody2D rb;
        private Vector2 movement;
        private GlobalEnums.Direction slideDirection;
        private bool isJumping = false;
        private bool doJump = false;
        private float stamina;
        private bool isSprinting = false;
        private bool isSliding = false;
        private bool isCrouching = false;

        public bool isKnockedBack = false;
        public bool isInQuicksand = false;
        public float quicksandSpeedFactor = 0.01f;
        private float quicksandSinkRate = -0.35f;
        private float quicksandJumpPower = 2f;
        private float quicksandJumpHoldTime = 0.05f;

        // Level Modifiers
        public GlobalEnums.LevelType levelType;

        // Timers
        private float jumpTimeStart;
        private float sprintTimeEnd;
        private float slideTimeStart;
        public float knockbackCooldown = 0f;
        private float lastKnockbackTime = -10f;
        private IEnumerator quicksandDamageCoroutine;
        private Health playerHealth;

        void Start()
        {
            // Get the player's attributes
            rb = GetComponent<Rigidbody2D>();
            playerHealth = GetComponent<Health>();
            // Set variables
            stamina = staminaTotal;
        }

        void FixedUpdate()
        {
            HandleMovement();
            if (isInQuicksand && !isJumping)
            {
                // Handle quicksand logic
                SinkInQuicksand();
            }
            // else
            // {
            //     // Handle horizontal movement
            //     if (isKnockedBack) return;
            //     // Allow horizontal movement always, even in quicksand
            //     float speed = isSprinting ? sprintSpeed : movementSpeed;
            //     if (isCrouching && IsGrounded())
            //     {
            //         speed *= crouchSlowdownMultiplier;
            //     }

            //     // Specific handling for level types
            //     if (levelType == GlobalEnums.LevelType.SideScroll)
            //     {
            //         HandleJumping();
            //         // Move Player (Side Scroll)
            //         rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
            //     }
            //     else if (levelType == GlobalEnums.LevelType.TopDown)
            //     {
            //         // Move Player (Top Down)
            //         rb.velocity = new Vector2(movement.x * speed * topDownSpeedMultiplier, movement.y * speed * topDownSpeedMultiplier);
            //     }
            // }
        }

        void Update()
        {
            HandlePlatformingInput();
        }

        public void HandleQuicksand(bool isInQuicksand)
        {
            float speed;
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

            rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
        }

        private IEnumerator ApplyQuicksandDamage()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                Debug.Log("Quicksand Damage");
                Debug.Log("Player Health: " + playerHealth);
                if (playerHealth != null)
                {
                    Debug.Log("Applying Quicksand Damage");
                    playerHealth.TakeDamage(1);
                }
            }
        }

        private void SinkInQuicksand()
        {
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

                rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
            }
            HandleJumping();
        }

        private void HandleJumping()
        {
            // if (isInQuicksand)
            // {
            //     rb.velocity = new Vector2(rb.velocity.x, quicksandJumpPower);
            //     rb.gravityScale = 0;
            //     StartCoroutine(ResetJumpAfterDelay(quicksandJumpHoldTime));
            // }
            // else
            // {
            if (doJump)
            {
                Jump();
            }

            if (isJumping)
            {
                ContinueJump();
            }

                // if (rb.velocity.y < 0) // Player is falling
                // {
                //     // Make Falling Faster
                //     facingDirection = GlobalEnums.Direction.Falling;
                //     rb.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
                // }
            // }
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

        // private void HandleSlide()
        // {
        //     var velocity = rb.velocity;
        //     if (CalculateElaspedTime(slideTimeStart) <= slideSpeedBoostDuration)
        //     {
        //         if (levelType == GlobalEnums.LevelType.SideScroll)
        //         {
        //             velocity = new Vector2(velocity.x * slideSpeedBoostMultiplier, velocity.y);
        //         }
        //         else if (levelType == GlobalEnums.LevelType.TopDown)
        //         {
        //             velocity = new Vector2(velocity.x * slideSpeedBoostMultiplier, movement.y * slideSpeedBoostMultiplier);
        //         }
        //     }
        //     else
        //     {
        //         switch (facingDirection)
        //         {
        //             case GlobalEnums.Direction.Left:
        //                 velocity = new Vector2(
        //                     Math.Clamp(velocity.x + slideSlowdownRate, Mathf.NegativeInfinity, 0F),
        //                     velocity.y);
        //                 break;
        //             case GlobalEnums.Direction.Right:
        //                 velocity = new Vector2(
        //                     Math.Clamp(velocity.x - slideSlowdownRate, 0F, Mathf.Infinity),
        //                     velocity.y);
        //                 break;
        //             case GlobalEnums.Direction.Up:
        //                 velocity = new Vector2(
        //                     velocity.x,
        //                     Math.Clamp(velocity.y - slideSlowdownRate, 0F, Mathf.Infinity));
        //                 break;
        //             case GlobalEnums.Direction.Down:
        //                 velocity = new Vector2(
        //                     velocity.x,
        //                     Math.Clamp(velocity.y - slideSlowdownRate, Mathf.NegativeInfinity, 0));
        //                 break;
        //             case GlobalEnums.Direction.TopLeft:
        //                 velocity = new Vector2(
        //                     Math.Clamp(velocity.x + slideSlowdownRate, Mathf.NegativeInfinity, 0F),
        //                     Math.Clamp(velocity.y - slideSlowdownRate, 0F, Mathf.Infinity));
        //                 break;
        //             case GlobalEnums.Direction.TopRight:
        //                 velocity = new Vector2(
        //                     Math.Clamp(velocity.x - slideSlowdownRate, 0F, Mathf.Infinity),
        //                     Math.Clamp(velocity.y - slideSlowdownRate, 0F, Mathf.Infinity));
        //                 break;
        //             case GlobalEnums.Direction.BottomLeft:
        //                 velocity = new Vector2(
        //                     Math.Clamp(velocity.x + slideSlowdownRate, Mathf.NegativeInfinity, 0F),
        //                     Math.Clamp(velocity.y - slideSlowdownRate, Mathf.NegativeInfinity, 0));
        //                 break;
        //             case GlobalEnums.Direction.BottomRight:
        //                 velocity = new Vector2(
        //                     Math.Clamp(velocity.x - slideSlowdownRate, 0F, Mathf.Infinity),
        //                     Math.Clamp(velocity.y - slideSlowdownRate, Mathf.NegativeInfinity, 0));
        //                 break;
        //             case GlobalEnums.Direction.Idle:
        //                 // Stop slide if not moving anymore
        //                 isSliding = false;
        //                 break;
        //         }
        //     }

        //     rb.velocity = velocity;
        // }

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

        public bool IsGrounded()
        {
            BoxCollider2D feetCollider = playerFeet.GetComponent<BoxCollider2D>();
            Vector2 boxSize = new Vector2(0.8f, 0.1f);
            Vector2 adjustedOffset = new Vector2(-0.05f, feetCollider.offset.y + 1.03f);

            Vector2 boxCenter = (Vector2)playerFeet.position + adjustedOffset + Vector2.down * boxSize.y / 2;

            Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, groundLayer);
            return hit != null;        
        }


        void HandlePlatformingInput()
        {
            if (isKnockedBack) return;

            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
            var grounded = IsGrounded();

            if (movement is { x: 0, y: 0 } || (movement.x == 0 && grounded))
            {
                facingDirection = GlobalEnums.Direction.Idle;
            }
            else
                facingDirection = movement.x switch
                {
                    < 0 when movement.y < 0 => GlobalEnums.Direction.BottomLeft,
                    < 0 when movement.y > 0 => GlobalEnums.Direction.TopLeft,
                    < 0 => GlobalEnums.Direction.Left,
                    > 0 => movement.y switch
                    {
                        < 0 => GlobalEnums.Direction.BottomRight,
                        > 0 => GlobalEnums.Direction.TopRight,
                        _ => GlobalEnums.Direction.Right
                    },
                    _ => movement.y < 0 ? GlobalEnums.Direction.Down : GlobalEnums.Direction.Up
                };

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

            // Crouch Movement S 
            // if (levelType == GlobalEnums.LevelType.SideScroll && Input.GetKeyDown(KeyCode.S) && enableCrouching)
            // {
            //     isCrouching = true;
            // }

            // Slide
            // if (Input.GetKeyDown(KeyCode.LeftControl) && !isSliding && grounded && !isCrouching && stamina >= staminaSlideCost && enableSliding)
            // {
            //     stamina = Mathf.Clamp(stamina - staminaSlideCost, 0f, Mathf.Infinity);
            //     isSliding = true;
            //     slideTimeStart = Time.realtimeSinceStartup;
            // }

            // Jump Movement
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (grounded || isInQuicksand)
                {
                    isJumping = true; // Indicate that we're jumping
                    doJump = true; // Tell code in FixedUpdate to jump
                    jumpTimeStart = Time.realtimeSinceStartup; // Save start time of jump
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
                    else if (CalculateElaspedTime(jumpTimeStart) >= jumpHoldTime)
                    {
                        isJumping = false; // Stop jump after a certain amount of elapsed time
                    }

                    isSliding = false;
                }
            }

            // Release Crouch
            if ((levelType == GlobalEnums.LevelType.SideScroll && Input.GetKeyUp(KeyCode.S) && enableCrouching))
            {
                isCrouching = false;
            }

            // Release Sprint
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isSprinting = false;
            }

            // Release Slide
            if (Input.GetKeyUp(KeyCode.LeftControl) && enableSliding)
            {
                isSliding = false;
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
                // "Current View: " + levelType,
                // "Stamina Left: " + stamina,
                // "Is Crouching: " + isCrouching,
                // "Is Jumping: " + isJumping,
                // "Is Sliding: " + isSliding,
                // "Is Sprinting: " + isSprinting,
                // "Time Since Last Sprint: " + CalculateElaspedTime(sprintTimeEnd) + "s",
                // "Time Since Last Jump: " + CalculateElaspedTime(jumpTimeStart) + "s",
                // "Slowing Down From Slide: " + (CalculateElaspedTime(slideTimeStart) >= slideSpeedBoostDuration),
                // "Movement: " + movement,
                // "Velocity: " + rb.velocity,
                // "Direction: " + facingDirection,
                // "Grounded: " + IsGrounded(),
            };
            return messages;
        }

        private void OutputLogsToConsole()
        {
            var messages = GetLogs();
            foreach (string i in messages)
            {
                Debug.Log(i);
            }
        }

        public GlobalEnums.Direction GetDirection()
        {
            return facingDirection;
        }

        public GlobalEnums.LevelType GetLevelType()
        {
            return levelType;
        }

        public bool PlayerSprinting()
        {
            return isSprinting;
        }

        public bool PlayerCrouching()
        {
            return isCrouching;
        }

        public bool PlayerJumping()
        {
            return isJumping;
        }

        public bool PlayerSliding()
        {
            return isSliding;
        }
    }
}