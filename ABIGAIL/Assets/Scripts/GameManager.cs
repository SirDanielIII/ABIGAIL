using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool hasKey = false;

    public GameObject sideViewPlayer;
    public GameObject topDownPlayer;

    public CinemachineVirtualCamera sideViewCamera;
    public CinemachineVirtualCamera topDownCamera;
    private bool isSideViewActive = true;
    public List<TransitionMapping> transitionMappings;
    private float playerHeightOffset = 0.5f;
    public LayerMask groundLayerMask;

    public bool isSideView()
    {
        return isSideViewActive; 
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        hasKey = false;
    }

    IEnumerator DelayCameraSwitch(CinemachineVirtualCamera newActiveCamera, CinemachineVirtualCamera oldActiveCamera, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        newActiveCamera.Priority = 11;
        oldActiveCamera.Priority = 0;
    }
    public void SwitchPerspective(GameObject currentZone)
    {
        sideViewPlayer.SetActive(false);
        topDownPlayer.SetActive(false);
        foreach (var mapping in transitionMappings)
        {
            // Check which mapping entry the player is currently in
            if ((isSideViewActive && mapping.sideViewEntry == currentZone) ||
                (!isSideViewActive && mapping.topDownEntry == currentZone))
            {
                // Calculate the target position based on the current perspective
                var targetZone = isSideViewActive ? mapping.topDownEntry : mapping.sideViewEntry;
                var targetPosition = targetZone.transform.position;

                // Assuming you have a method to move the player
                MovePlayerToTargetPosition(targetPosition);

                // Toggle the perspective
                isSideViewActive = !isSideViewActive;
                break;
            }
        }
    }

    private void MovePlayerToTargetPosition(Vector3 targetPosition)
    {
        // Switch TO side view (since we toggle isSideViewActive *after* this method)
        if (!isSideViewActive)
        {
            // Correcting this block to now properly handle moving to side view
            sideViewPlayer.SetActive(true); // Ensure the player is active
            topDownPlayer.SetActive(false); // Deactivate the top-down player
            
            RaycastHit2D hit = Physics2D.Raycast(targetPosition, Vector2.down, Mathf.Infinity, groundLayerMask);
            if (hit.collider != null)
            {
                // Place the player just above the ground hit point
                sideViewPlayer.transform.position = new Vector3(targetPosition.x, hit.point.y + playerHeightOffset, sideViewPlayer.transform.position.z);
}           sideViewCamera.Priority = 11; // Ensure side view camera has higher priority
            topDownCamera.Priority = 0; // Lower priority for top-down camera
        }
        // Switch TO top-down view
        else
        {
            // Correcting this block to now properly handle moving to top-down view
            topDownPlayer.SetActive(true); // Ensure the top-down player is active
            sideViewPlayer.SetActive(false); // Deactivate the side-view player

            topDownPlayer.transform.position = targetPosition; // Move the top-down player
            topDownCamera.Priority = 11; // Ensure top-down camera has higher priority
            sideViewCamera.Priority = 0; // Lower priority for side view camera
        }
    }

    



    public void CollectKey()
    {
        Debug.Log("Key collected!");
        hasKey = true;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateKeyIndicator(hasKey);
        }    
    }
}
