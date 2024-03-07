using System.Collections;
using System.Collections.Generic;
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

    public class Movement : MonoBehaviour
    {
        // Ground Check
        public Transform playerFeet;
        public float checkRadius = 0.1f;
        public LayerMask groundLayer;

        // Movement Values
        public float movementSpeed = 11f; // Speed of the character (normal)
        public float sprintSpeed = 16f; // Speed of the character (when sprinting)
        public float staminaTotal = 100f;
        public float staminaSprintRate = 25f;
        public float staminaJumpRate = 25f;
        public float staminaSprintJumpRate = 30f;
        public float staminaRecoveryDelay = 1.5f; // The recovery delay after sprinting
        public float staminaRecoveryRate = 15f; // How fast stamina recharges

        [FormerlySerializedAs("sprintJumpMultiplier")]
        public float sprintJumpBoost = 1.15f; // How much extra jump there is when sprinting

        public float jumpInitialPower = 8f; // Speed of jump (normal)
        public float jumpHoldApplyAfter = 0.06f;
        public float jumpHoldPower = 11f; // Speed of jump (hold)
        public float jumpHoldTime = 0.2f; // Jump duration
        public float slideSpeedBoostMultiplier = 1.05f; // How much extra boost the player gets when sliding
        public float slideSpeedBoostDuration = 0.15f; // How long the slide boost lasts for
        public float slideHoldTime = 0.3f; // How long the slide lasts for

        // The Player
        private Rigidbody2D rb;
        private Vector2 movement;
        private bool isFacingRight = true;
        private bool isJumping = false;
        private bool doJump = false;
        private BoxCollider2D playerCollider;
        private Vector2 standingColliderSize;
        private Vector2 crouchingColliderSize;
        private float stamina;
        private bool isSprinting = false;
        private bool isSliding = false;
        private bool isCrouching = false;
        public bool isKnockedBack = false;


        // Level Modifiers
        public LevelType levelType;
        [FormerlySerializedAs("sceneSideScroll")] public int sideScrollSceneIndex = -1;
        [FormerlySerializedAs("sceneTopDown")] public int topDownSceneIndex = -1;

        // Timers
        private float jumpTimeStart;
        private float sprintTimeEnd;
        private float slideTimeStart;
        public float knockbackCooldown = 2f;
        private float lastKnockbackTime = -10f;

        

        void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // initializing the rigidbody
            playerCollider = GetComponent<BoxCollider2D>(); // Get the player's collider
            standingColliderSize = playerCollider.size; // Store the original size
            crouchingColliderSize =
                new Vector2(playerCollider.size.x, playerCollider.size.y / 2); // Define a smaller size for crouching
            stamina = staminaTotal;
        }

        // Where da player movesssss
        void FixedUpdate()
        {
            if (levelType == LevelType.SideScroll)
            {
                if (isKnockedBack) // Skip normal movement if knocked back
                    return;
                // Basic left / right strafing movement
                rb.velocity = isSprinting ? new Vector2(movement.x * sprintSpeed, rb.velocity.y) : new Vector2(movement.x * movementSpeed, rb.velocity.y);
                // Jump
                if (doJump)
                {
                    doJump = false; // Toggle off variable from Update()
                    float multiplier = 1.0f;
                    // If stamina is less than the full amount of stamina  
                    if (stamina < (isSprinting ? staminaSprintJumpRate : staminaJumpRate))
                    {
                        multiplier = stamina / (isSprinting ? staminaSprintJumpRate : staminaJumpRate); // Get percentage for multiplier
                    }

                    // Subtract from the stamina
                    stamina -= isSprinting ? staminaSprintJumpRate : staminaJumpRate;
                    // Clamp the value to ensure it doesn't go below zero
                    stamina = Mathf.Clamp(stamina, 0f, Mathf.Infinity);
                    // Do the jump
                    rb.velocity = isSprinting
                        ? new Vector2(rb.velocity.x, jumpInitialPower + sprintJumpBoost * multiplier)
                        : new Vector2(rb.velocity.x, jumpInitialPower);
                }

                if (isJumping)
                {
                    float elapsedTime = CalculateElaspedTime(jumpTimeStart);
                    // Let's you jump higher if you hold down the key
                    if (elapsedTime <= jumpHoldTime)
                    {
                        // Thanks Chat-GPT
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
            }
            else if (levelType == LevelType.TopDown)
            {
                rb.velocity = isSprinting
                    ? new Vector2(movement.x * sprintSpeed, movement.y * sprintSpeed)
                    : new Vector2(movement.x * movementSpeed, movement.y * movementSpeed);
            }
        }

        void Update()
        {
            if (levelType == LevelType.SideScroll)
            {
                HandlePlatformingInput();
            }
            else if (levelType == LevelType.TopDown)
            {
                HandleTopdownInput();
            }
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(playerFeet.position, checkRadius, groundLayer);
        }

        void HandleTopdownInput()
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(sideScrollSceneIndex);
            }
        }

        void HandlePlatformingInput()
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");

            // Restore Stamina
            // If not sprinting, and enough time has passed since the last sprint, start the recovery
            if (!isSprinting && CalculateElaspedTime(sprintTimeEnd) >= staminaRecoveryDelay)
            {
                stamina += staminaRecoveryRate * Time.deltaTime;
                // Clamp the value to ensure it doesn't go above the max value
                stamina = Mathf.Clamp(stamina, 0f, staminaTotal);
            }

            // Allow sprint if there's enough stamina
            if (Input.GetKey(KeyCode.LeftShift) && !isSprinting && IsGrounded() && stamina >= 0)
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
            if (Input.GetKey(KeyCode.Space))
            {
                if (IsGrounded())
                {
                    isJumping = true; // Indicate that we're jumping
                    doJump = true; // Tell code in FixedUpdate to jump
                    jumpTimeStart = Time.realtimeSinceStartup; // Save start time of jump
                }

                if (isJumping)
                {
                    if (CalculateElaspedTime(jumpTimeStart) >= jumpHoldTime)
                    {
                        isJumping = false; // Stop jump after a certain amount of elapsed time
                    }
                }
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

            // Release Crouch
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                isCrouching = false;
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
            
            // Flip sprite image
            Flip();

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(topDownSceneIndex);
            }

            OutputLogsToConsole();
        }

        private void Flip()
        {
            if (isFacingRight && movement.x < 0f || !isFacingRight && movement.x > 0f)
            {
                isFacingRight = !isFacingRight;
                var transform1 = transform;
                Vector2 localScale = transform1.localScale;
                localScale.x *= -1f;
                transform1.localScale = localScale;
            }
        }

        private void Crouch(bool crouching)
        {
            isCrouching = crouching;
            playerCollider.size = crouching ? crouchingColliderSize : standingColliderSize;
        }
        public bool CanBeKnockedBack() {
            return Time.time - lastKnockbackTime >= knockbackCooldown;
        }

        public void ApplyKnockback(Vector2 knockbackVelocity, float duration) {
            if (!CanBeKnockedBack()) return;

            lastKnockbackTime = Time.time;
            isKnockedBack = true;
            rb.velocity = knockbackVelocity; // Apply knockback velocity directly
            StartCoroutine(ResetKnockbackState(duration));
        }

        private IEnumerator ResetKnockbackState(float duration) {
            yield return new WaitForSeconds(duration);
            isKnockedBack = false;
            // Consider adding logic here to momentarily increase the player's collision layer to ignore the tumbleweed
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