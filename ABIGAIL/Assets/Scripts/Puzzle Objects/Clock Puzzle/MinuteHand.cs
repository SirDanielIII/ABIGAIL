using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinuteHand : MonoBehaviour
{
    public int slotNumber;
    public Transform hand_script;

    void Rotate()
    {
        hand_script = gameObject.GetComponent<Transform>();
        Vector3 pivotPoint = new Vector3(90, -5, 0);
        Quaternion rot = Quaternion.Euler(0, 0, 30*(12-slotNumber));
        hand_script.transform.position = rot * (hand_script.transform.position - pivotPoint) + pivotPoint;
	    hand_script.transform.rotation = rot * hand_script.transform.rotation;
    }

}
