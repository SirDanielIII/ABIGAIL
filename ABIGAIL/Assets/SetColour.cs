using UnityEngine;

public class SetColour : MonoBehaviour
{
    public GameObject[] bottles;
    private HiddenColour hiddenColour;

    void Start()
    {
        // Variables to store cumulative color values
        float red = 0;
        float green = 0;
        float blue = 0;

        // Loop through each bottle
        foreach (GameObject bottle in bottles)
        {
            hiddenColour = bottle.GetComponent<HiddenColour>();
            red += hiddenColour.hiddenColor.r;
            green += hiddenColour.hiddenColor.g;
            blue += hiddenColour.hiddenColor.b;
        }

        // Calculate the average color values
        red /= bottles.Length;
        green /= bottles.Length;
        blue /= bottles.Length;

        // Set the color of the current GameObject's renderer
        GetComponent<Renderer>().material.color = new Color(red, green, blue);
    }

    void Update()
    {

    }
}
