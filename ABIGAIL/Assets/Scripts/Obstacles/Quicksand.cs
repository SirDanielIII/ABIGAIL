using UnityEngine;

namespace Abigail
{
    public class Quicksand : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D abigail)
        {
            if (abigail.CompareTag("Abigail"))
            {
                var movement = abigail.GetComponent<Movement>();
                if (movement != null)
                {
                    movement.HandleQuicksand(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D abigail)
        {
            if (abigail.CompareTag("Abigail"))
            {
                var movement = abigail.GetComponent<Movement>();
                if (movement != null)
                {
                    movement.HandleQuicksand(false);
                }
            }
        }
    }
}
