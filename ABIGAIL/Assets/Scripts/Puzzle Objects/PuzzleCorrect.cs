using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PuzzleCorrect : MonoBehaviour
{
    public Button button;
    public PuzzleData[] puzzleData;

    // Start is called before the first frame update
    void Start()
    {
        button.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (puzzleData == null)
            return;

        int total = 0;
        foreach (PuzzleData data in puzzleData)
        {
            if (data.colours == null || data.colours.Length != 2)
                continue;

            GameObject firstObject = data.colours[0];
            GameObject secondObject = data.colours[1];

            if (firstObject != null && secondObject != null)
            {
                Renderer firstRenderer = firstObject.GetComponent<Renderer>();
                Renderer secondRenderer = secondObject.GetComponent<Renderer>();

                if (firstRenderer != null && secondRenderer != null &&
                    firstRenderer.material.color == secondRenderer.material.color)
                {
                    total++;
                }
            }
        }

        if (total == puzzleData.Length) // Assuming every pair of colours matches
        {
            button.gameObject.SetActive(true);
        }
        else
        {
            button.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class PuzzleData
{
    public GameObject[] colours;
}
