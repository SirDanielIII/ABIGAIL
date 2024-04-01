using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinuteHand : MonoBehaviour
{
    public Slot slot;

    void Start()
    {
        Vector3 dir = (slot.GetComponent<Slot>().GetPos().position - this.transform.position).normalized;
        this.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir);
    }

}
