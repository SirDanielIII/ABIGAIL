using UnityEngine;

public class GateController : MonoBehaviour
{
    private Animator animator; 
    public Transform playerTransform; 
    public float openDistance = 8f; 
    private bool gateOpened = false;
    public GameObject noKeyUIElement;
    public GameObject respawnPad;
    

    private void Start()
    {
        animator = GetComponent<Animator>(); 
    }

    private void Update()
    {
        float distanceX = Mathf.Abs(transform.position.x - playerTransform.position.x);

        if (!gateOpened && distanceX <= openDistance)
        {
            if (GameManager.Instance.hasKey)
            {
                OpenGate();
                noKeyUIElement.SetActive(false); // Hide the UI element when the player has the key and the gate is close enough to open
                respawnPad.SetActive(false); // Hide the respawn pad when the gate is open
            }
            else
            {
                // Show the UI element when the player is close enough but doesn't have the key
                noKeyUIElement.SetActive(true);
                respawnPad.SetActive(true); // Show the respawn pad when the gate is closed
                Debug.Log("You need a key to open this gate");
            }
        }
        else
        {
            noKeyUIElement.SetActive(false); // Ensure the UI element is hidden in all other cases
            respawnPad.SetActive(false); // Ensure the respawn pad is hidden in all other cases
            if (gateOpened && distanceX > openDistance)
            {
                CloseGate();
            }
        }
    }


    private void OpenGate()
    {
        Debug.Log("Opening gate");
        animator.SetTrigger("GateOpen");
        gateOpened = true;
    }

    private void CloseGate()
    {
        Debug.Log("Closing gate");
        animator.SetTrigger("GateClose");
        gateOpened = false;
    }
}
