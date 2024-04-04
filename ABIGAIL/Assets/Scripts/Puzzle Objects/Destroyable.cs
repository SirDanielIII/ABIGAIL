using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public Color mouseHoverColor = Color.green;
    public Color previousColor;
    public SpriteRenderer spriteRenderer;
    public AudioSource bottlebreak;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousColor = spriteRenderer.material.color;
    }

    private void OnMouseDown()
    {
        gameObject.SetActive(false);
        bottlebreak.Play();
    }

    private void OnMouseOver()
    {
        spriteRenderer.material.color = mouseHoverColor;
    }

    private void OnMouseExit()
    {
        spriteRenderer.material.color = previousColor;
    }

}
