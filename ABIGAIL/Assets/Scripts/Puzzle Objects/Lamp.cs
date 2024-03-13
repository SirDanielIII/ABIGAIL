using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    public GameObject spotlight_object;
    private Spotlight spotlight_script;

    // Start is called before the first frame update
    void Start()
    {
        spotlight_script = spotlight_object.GetComponent<Spotlight>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spotlight_script.isConnected == true)
        {
            if (Input.GetKeyDown("z"))
            {
                transform.eulerAngles += Vector3.forward * 10;
            }
                
            if (Input.GetKeyDown("x"))
            {
                transform.eulerAngles += Vector3.forward * -10;
            }
                
        }
    }
}
