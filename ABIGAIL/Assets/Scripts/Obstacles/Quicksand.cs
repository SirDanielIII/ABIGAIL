using UnityEngine;

namespace Abigail
{
    public class Quicksand : MonoBehaviour
    {
        [SerializeField] private float slowMovementFactor = 0.3f;
        [SerializeField] private int damageOnSubmerge = 10;
        [SerializeField] private Transform topMarker; // Assign this to mark the top of the quicksand
        [SerializeField] private Transform bottomMarker; // Assign this to mark the bottom of the quicksand

        private float playerOriginalSpeed;
        private bool playerIsInQuicksand = false;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Abigail"))
            {
                playerIsInQuicksand = true;
                var playerMovement = collision.GetComponent<Movement>();
                if (playerMovement)
                {
                    playerOriginalSpeed = playerMovement.movementSpeed;
                    playerMovement.movementSpeed *= slowMovementFactor;
                    playerMovement.isInQuicksand = true;
                    CalculateJumpAttemptsNeeded(collision.transform.position.y);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (playerIsInQuicksand)
            {
                CalculateJumpAttemptsNeeded(collision.transform.position.y);
                var playerMovement = collision.GetComponent<Movement>();
                if (playerMovement && jumpAttemptsNeeded >= 5)
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

    private void CalculateJumpAttemptsNeeded(float playerYPosition)
    {
        // Calculate submersion percentage such that 0% is not submerged and 100% is fully submerged
        float submersionPercentage = 1-((playerYPosition - bottomMarker.position.y) / (topMarker.position.y - bottomMarker.position.y));
        submersionPercentage = Mathf.Clamp01(submersionPercentage);

        Debug.Log("Quicksand Submersion percentage: " + submersionPercentage);

        // Calculate jumps needed based on submersion percentage
        if(submersionPercentage >= 0.8f && submersionPercentage < 1f) {
            jumpAttemptsNeeded = 4;
        } else if(submersionPercentage >= 0.6f) {
            jumpAttemptsNeeded = 3;
        } else if(submersionPercentage >= 0.4f) {
            jumpAttemptsNeeded = 2;
        } else if(submersionPercentage >= 0.2f) {
            jumpAttemptsNeeded = 1;
        } else {
            jumpAttemptsNeeded = 0;
        }

        Debug.Log("Quicksand Jump attempts needed: " + jumpAttemptsNeeded);
    }



        public int GetJumpAttemptsNeeded()
        {
            return jumpAttemptsNeeded;
        }

        public void DecreaseJumpAttemptsNeeded(Transform playerTransform)
        {
            if (jumpAttemptsNeeded > 0)
            {
                // Decrease jump attempts and move the player up
                jumpAttemptsNeeded--;
                float raiseHeight = (topMarker.position.y - bottomMarker.position.y) * 0.2f; // 20% of the quicksand height
                playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + raiseHeight, playerTransform.position.z);

                // Recalculate the submersion based on new position
                CalculateJumpAttemptsNeeded(playerTransform.position.y);
            }
        }


        private void ResetSinkDepthAndEscape(Transform playerTransform)
        {
            playerIsInQuicksand = false;
            jumpAttemptsNeeded = 0;
            var playerMovement = playerTransform.GetComponent<Movement>();
            if (playerMovement)
            {
                playerMovement.movementSpeed = playerOriginalSpeed;
                playerMovement.isInQuicksand = false;
            }
        }

        private int jumpAttemptsNeeded = 0;
    }
}
