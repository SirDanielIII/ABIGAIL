using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    public Camera churchCam;
    public Camera unsolvedCam;
    public Camera solvedCam;
    public GameObject lamplight_object;
    private LampLight lamplight_script;
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        lamplight_script = lamplight_object.GetComponent<LampLight>();
        churchCam.enabled = true;
        unsolvedCam.enabled = false;
        solvedCam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            if (churchCam.enabled)
            {
                Player.SetActive(false);

                if (lamplight_script.isSolved)
                {
                    churchCam.enabled = false;
                    solvedCam.enabled = true;
                    unsolvedCam.enabled = false;
                }
                else
                {
                    churchCam.enabled = false;
                    solvedCam.enabled = false;
                    unsolvedCam.enabled = true;
                }
            }
            else
            {
                Player.SetActive(true);
                churchCam.enabled = true;
                unsolvedCam.enabled = false;
                solvedCam.enabled = false;
            }
        }
    }
}
