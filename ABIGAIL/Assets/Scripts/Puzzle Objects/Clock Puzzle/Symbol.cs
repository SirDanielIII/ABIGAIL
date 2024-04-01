using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{

    public int symbolNumber;
    public Slot slot;
    private Transform slotTransform;

    void Start()
    {
        slotTransform = slot.GetComponent<Slot>().GetPos();
        Debug.Log("Symbol " + symbolNumber + " Position: " + slotTransform.position);
        this.transform.position = slotTransform.position;
    }

    int GetSymbolNumber()
    {
        return symbolNumber;
    }

}
