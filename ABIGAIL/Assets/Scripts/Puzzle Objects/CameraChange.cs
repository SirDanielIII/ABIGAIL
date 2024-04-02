using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abigail;

public class CameraChange : MonoBehaviour
{
    public Camera churchCam;
    public Camera unsolvedCam;
    public Camera solvedCam;
    public GameObject lamplight_object;
    private LampLight lamplight_script;
    public GameObject playerObject; // Renamed to avoid conflict with the Player GameObject

    private Movement movement; // Declared movement script reference

    // Start is called before the first frame update
    void Start()
    {
        lamplight_script = lamplight_object.GetComponent<LampLight>();
        churchCam.enabled = true;
        unsolvedCam.enabled = false;
        solvedCam.enabled = false;

        // Get reference to the Movement script
        movement = playerObject.GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            if (churchCam.enabled)
            {
                // Disable Movement script
                movement.enabled = false;

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
                // Enable Movement script
                movement.enabled = true;
                churchCam.enabled = true;
                unsolvedCam.enabled = false;
                solvedCam.enabled = false;
            }
        }
    }
}
