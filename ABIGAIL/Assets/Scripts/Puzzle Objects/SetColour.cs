using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColour : MonoBehaviour
{
    public GameObject[] bottles;
    private float redTotal;
    private float greenTotal;
    private float blueTotal;
    private int count = 0;
    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

        foreach (GameObject bottle in bottles)
        {
            HiddenColour hiddenColour = bottle.GetComponent<HiddenColour>();
            if (hiddenColour != null) // Make sure HiddenColour component exists
            {
                redTotal += hiddenColour.hiddenColor.r;
                greenTotal += hiddenColour.hiddenColor.g;
                blueTotal += hiddenColour.hiddenColor.b;
                count++;
            }
        }

        if (count > 0) // Avoid division by zero
        {
            float redAverage = redTotal / count;
            float greenAverage = greenTotal / count;
            float blueAverage = blueTotal / count;
            rend.material.color = new Color(redAverage, greenAverage, blueAverage);
        }
        else
        {
            Debug.LogError("No HiddenColour components found on bottles.");
        }
    }
}
