using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject sideViewPlayer;
    public GameObject topDownPlayer;

    public CinemachineVirtualCamera sideViewCamera;
    public CinemachineVirtualCamera topDownCamera;
    public bool isSideViewActive = true;
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
        topDownPlayer.SetActive(false);
        sideViewPlayer.SetActive(true);
        sideViewCamera.enabled = true;
        topDownCamera.enabled = false;
    }

    public void SwitchPerspective(GameObject currentZone)
    {
        sideViewCamera.enabled = false;
        topDownCamera.enabled = false;
        sideViewPlayer.SetActive(false);
        topDownPlayer.SetActive(false);
        foreach (var mapping in transitionMappings)
        {
            if ((isSideViewActive && mapping.sideViewEntry == currentZone) ||
                (!isSideViewActive && mapping.topDownEntry == currentZone))
            {
                var targetZone = isSideViewActive ? mapping.topDownEntry : mapping.sideViewEntry;
                var targetPosition = targetZone.transform.position;

                MovePlayerToTargetPosition(targetPosition);

                // Toggle the perspective
                isSideViewActive = !isSideViewActive;
                break;
            }
        }
        StartCoroutine(EnableCamerasAfterDelay(0.1f));
    }

    public IEnumerator EnableCamerasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        sideViewCamera.enabled = true;
        topDownCamera.enabled = true;
    }

    private void MovePlayerToTargetPosition(Vector3 targetPosition)
    {
        // Switch TO side view
        if (!isSideViewActive)
        {
            sideViewPlayer.SetActive(true); 
            topDownPlayer.SetActive(false); 
            
            RaycastHit2D hit = Physics2D.Raycast(targetPosition, Vector2.down, Mathf.Infinity, groundLayerMask);
            if (hit.collider != null)
            {
                sideViewPlayer.transform.position = new Vector3(targetPosition.x, hit.point.y + playerHeightOffset, sideViewPlayer.transform.position.z);
}           sideViewCamera.Priority = 11;
            topDownCamera.Priority = 0;
        }
        // Switch TO top-down view
        else
        {
            topDownPlayer.SetActive(true);
            sideViewPlayer.SetActive(false);

            topDownPlayer.transform.position = targetPosition; 
            topDownCamera.Priority = 11; 
            sideViewCamera.Priority = 0; 
        }
    }
}
