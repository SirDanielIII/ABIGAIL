using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBottles : MonoBehaviour
{
    public GameObject[] bottles;
    private Destroyable destroyable;

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (GameObject bottle in bottles)
            {
                destroyable = bottle.GetComponent<Destroyable>();

                if (!bottle.activeSelf)
                {
                    bottle.SetActive(true);
                    destroyable.spriteRenderer.material.color = destroyable.previousColor;
                }
            }
        }
    }
}
