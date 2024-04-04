using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    public GameObject spotlight_object;
    private Spotlight spotlight_script;
    public float RotationAmount;
    private readonly float delayBetweenInputs = 0.05f;
    private float t;
    public AudioSource lampRotate;

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
            if (Input.GetKey("z") && t <= 0)
            {
                lampRotate.Play();
                transform.eulerAngles += Vector3.forward * RotationAmount;
                t = delayBetweenInputs;
            }
                
            if (Input.GetKey("x") && t <= 0)
            {
                lampRotate.Play();
                transform.eulerAngles += Vector3.forward * -RotationAmount;
                t = delayBetweenInputs;
            }
            t -= Time.deltaTime;

        }
    }
}
