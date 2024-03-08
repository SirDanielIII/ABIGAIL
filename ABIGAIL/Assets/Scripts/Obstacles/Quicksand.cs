using UnityEngine;

namespace Abigail
{
    public class Quicksand : MonoBehaviour
    {
        [SerializeField] private float slowMovementFactor = 0.3f;
        [SerializeField] private int damageOnSubmerge = 10;
        [SerializeField] private Transform topMarker; // Assign this to mark the top of the quicksand
        [SerializeField] private Transform bottomMarker; // Assign this to mark the bottom of the quicksand
        private Transform playerTransform;
        private float playerOriginalSpeed;
        private bool playerIsInQuicksand = false;

        void Update() 
        {
            if (playerIsInQuicksand && playerTransform != null) 
            {
                BoxCollider2D playerCollider = playerTransform.GetComponent<BoxCollider2D>();
                if (playerCollider != null) 
                {
                    float playerBottom = playerCollider.bounds.min.y; // Get the player's bottom position from collider bounds

                    // Check if player's bottom position is above the top marker or if no jump attempts are needed
                    if (playerBottom > topMarker.position.y)
                    {
                        ResetSinkDepthAndEscape(playerTransform);
                        playerIsInQuicksand = false;
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Abigail"))
            {
                playerIsInQuicksand = true;
                playerTransform = collision.transform;
                var playerMovement = collision.GetComponent<Movement>();
                if (playerMovement)
                {
                    playerOriginalSpeed = playerMovement.movementSpeed;
                    playerMovement.movementSpeed *= slowMovementFactor;
                    playerMovement.isInQuicksand = true;
                }
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (playerIsInQuicksand)
            {
                BoxCollider2D playerCollider = collision.transform.GetComponent<BoxCollider2D>();
                float playerBottom = playerCollider.bounds.min.y;
                float playerTop = playerCollider.bounds.max.y;
                float playerHeight = playerTop - playerBottom;

                if (playerBottom < topMarker.position.y - playerHeight)
                {
                    var health = collision.GetComponent<Health>();
                    if (health) health.TakeDamage(damageOnSubmerge);
                    ResetSinkDepthAndEscape(collision.transform);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Abigail"))
            {
                ResetSinkDepthAndEscape(collision.transform);
            }
        }

        private void ResetSinkDepthAndEscape(Transform playerTransform)
        {
            playerIsInQuicksand = false;
            var playerMovement = playerTransform.GetComponent<Movement>();
            if (playerMovement)
            {
                playerMovement.movementSpeed = playerOriginalSpeed;
                playerMovement.isInQuicksand = false;
            }
            this.playerTransform = null;
        }
    }
}
