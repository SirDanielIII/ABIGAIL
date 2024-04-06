#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Abigail
{
    public class SpriteManagerAbigail : MonoBehaviour
    {
        public Vector2 idleColliderOffset = new Vector2(0f, -0.5f);
        public Vector2 idleColliderSize = new Vector2(6f, 20f);
        public Vector2 runColliderOffset = new Vector2(-0.5f, -1.4f);
        public Vector2 runColliderSize = new Vector2(9f, 18.5f);
        public Vector2 jumpColliderOffset = new Vector2(0f, 1.4f);
        public Vector2 jumpColliderSize = new Vector2(11f, 16f);
        public Vector2 crouchColliderOffset = new Vector2(0f, -3f);
        public Vector2 crouchColliderSize = new Vector2(9f, 12f);
        public Vector2 slideColliderOffset;
        public Vector2 slideColliderSize;

        private Movement script; // Player's movement script
        private BoxCollider2D playerCollider;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private void Start()
        {
            script = GetComponent<Movement>();
            playerCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (QuitMenuController.isMenuOpen)
            {
                return;
            }
            // Update Direction
            var direction = script.GetDirection();
            bool grounded = script.IsGrounded();

            // Handle Sprites & Collider Changes
            if (script.GetLevelType() == GlobalEnums.LevelType.SideScroll)
            {
                // Flip directions
                spriteRenderer.flipX = direction switch
                {
                    GlobalEnums.Direction.Left => false,
                    GlobalEnums.Direction.Right => true,
                    GlobalEnums.Direction.TopLeft => false,
                    GlobalEnums.Direction.TopRight => true,
                    GlobalEnums.Direction.BottomLeft => false,
                    GlobalEnums.Direction.BottomRight => true,
                    _ => spriteRenderer.flipX
                };
                // Change Sprites & Collider Sizes
                if (script.PlayerSliding() && grounded)
                {
                    // playerCollider.offset = slideColliderOffset;
                    // playerCollider.size = slideColliderOffset;
                    // animator.Play("Slide_Abigail");
                }
                else if (script.PlayerCrouching() && grounded)
                {
                    // playerCollider.offset = crouchColliderOffset;
                    // playerCollider.size = crouchColliderSize;
                    animator.Play("Crouch_Abigail");
                }
                else if (direction == GlobalEnums.Direction.Falling && !grounded)
                {
                    // Change offsets when we get new sprite
                    // playerCollider.offset = jumpColliderOffset;
                    // playerCollider.size = jumpColliderSize;
                    animator.Play("Jump_Abigail");
                }
                else if (script.PlayerJumping())
                {
                    // playerCollider.offset = jumpColliderOffset;
                    // playerCollider.size = jumpColliderSize;
                    animator.Play("Jump_Abigail");
                }
                else if (direction == GlobalEnums.Direction.Idle && grounded)
                {
                    // playerCollider.offset = idleColliderOffset;
                    // playerCollider.size = idleColliderSize;
                    animator.Play("Idle_Abigail");
                }
                else if (grounded)
                {
                    // playerCollider.offset = runColliderOffset;
                    // playerCollider.size = runColliderSize;
                    animator.Play("Run_Abigail");
                }

                // Debug.Log(playerCollider.size);
            }
            else if (script.GetLevelType() == GlobalEnums.LevelType.TopDown)
            {
            }
        }
    }
}