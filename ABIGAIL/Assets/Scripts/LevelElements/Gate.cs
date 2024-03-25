using UnityEngine;

public class GateController : MonoBehaviour
{
    private Animator animator; 
    public Transform playerTransform; 
    public float openDistance = 7f; 
    private bool gateOpened = false;
    

    private void Start()
    {
        animator = GetComponent<Animator>(); 
    }

    private void Update()
    {
        float distanceX = Mathf.Abs(transform.position.x - playerTransform.position.x);
        Debug.Log(distanceX);
        Debug.Log(GameManager.instance.hasKey);

        // Check if the gate has not been opened, the player is horizontally close enough, and the player has the key
        if (!gateOpened && distanceX <= openDistance && GameManager.instance.hasKey)
        {
            if (!gateOpened)
            {
                OpenGate();
            }
        }
        else if (gateOpened && distanceX > openDistance)
        {
            if (gateOpened)
            {
                CloseGate();
            }
        }
    }


    private void OpenGate()
    {
        animator.SetTrigger("GateOpen");
        gateOpened = true;
    }

    private void CloseGate()
    {
        animator.SetTrigger("GateClose");
        gateOpened = false;
    }
}
