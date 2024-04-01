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
            if (LevelManager.Instance.hasKey)
            {
                OpenGate();
                noKeyUIElement.SetActive(false); 
                respawnPad.SetActive(false);
            }
            else
            {
                noKeyUIElement.SetActive(true);
                respawnPad.SetActive(true); 
            }
        }
        else
        {
            noKeyUIElement.SetActive(false); 
            respawnPad.SetActive(false); 
            if (gateOpened && distanceX > openDistance)
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
