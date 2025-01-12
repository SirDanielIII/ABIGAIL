using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineColour : MonoBehaviour
{
    public GameObject[] bottles;
    private HiddenColour hiddenColour;
    private float red;
    private float green;
    private float blue;
    public SpriteRenderer spriteRenderer;
    private int destroyed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Reset color values
        red = 0f;
        green = 0f;
        blue = 0f;

        foreach (GameObject bottle in bottles)
        {
            if (bottle.activeSelf)
            {
                hiddenColour = bottle.GetComponent<HiddenColour>();
                red += hiddenColour.hiddenColor.r;
                green += hiddenColour.hiddenColor.g;
                blue += hiddenColour.hiddenColor.b;
            }
            else if (!bottle.activeSelf)
            {
                destroyed++;
            }
        }
        if (destroyed < 3)
        {
            red = red / (3 - destroyed);
            green = green / (3 - destroyed);
            blue = blue / (3 - destroyed);
            spriteRenderer.material.color = new Color(red, green, blue);
        }
        else
        {
            gameObject.SetActive(false);
        }
        destroyed = 0;
    }

}
