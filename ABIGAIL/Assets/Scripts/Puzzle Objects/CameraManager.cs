using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera leftCamera;
    public Camera rightCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera.enabled = true;
        leftCamera.enabled = false;
        rightCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera.enabled == true)
        {
            if(Input.GetAxis("Horizontal") < 0)
            {
                mainCamera.enabled = false;
                leftCamera.enabled = true;
            }
            else if(Input.GetAxis("Horizontal") > 0)
            {
                mainCamera.enabled = false;
                rightCamera.enabled = true;
            }
        }
        else
        {
            if (Input.GetKeyDown("r"))
            {
                mainCamera.enabled = true;
                leftCamera.enabled = false;
                rightCamera.enabled = false;
            }
        }
    }
}
