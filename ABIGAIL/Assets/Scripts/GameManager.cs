using UnityEngine;
using Cinemachine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool hasKey = false;

    public GameObject sideViewPlayer;
    public GameObject topDownPlayer;

    public CinemachineVirtualCamera sideViewCamera;
    public CinemachineVirtualCamera topDownCamera;
    private bool isSideViewActive = true;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchPerspective();
        }
    }

    private void SwitchPerspective()
    {
        isSideViewActive = !isSideViewActive;

        if (isSideViewActive)
        {
            sideViewPlayer.SetActive(true);
            sideViewCamera.Priority = topDownCamera.Priority + 1;

            TransferPlayerState(topDownPlayer, sideViewPlayer);

            topDownPlayer.SetActive(false);
        }
        else
        {
            topDownPlayer.SetActive(true);
            topDownCamera.Priority = sideViewCamera.Priority + 1;

            TransferPlayerState(sideViewPlayer, topDownPlayer);

            sideViewPlayer.SetActive(false);
        }
        Debug.Log(isSideViewActive ? "Switched to Side View" : "Switched to Top-Down View");
    }

    private void TransferPlayerState(GameObject fromPlayer, GameObject toPlayer)
    {
        // Rigidbody2D fromRb = fromPlayer.GetComponent<Rigidbody2D>();
        // Rigidbody2D toRb = toPlayer.GetComponent<Rigidbody2D>();
        // if (fromRb != null && toRb != null)
        // {
        //     toRb.velocity = fromRb.velocity;
        // }
        if (!isSideViewActive)
        {
            topDownPlayer.transform.position = GetTopDownPosition();
        }
    }

    private Vector3 GetTopDownPosition()
    {
        return new Vector3(-210, -57, 0);
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
