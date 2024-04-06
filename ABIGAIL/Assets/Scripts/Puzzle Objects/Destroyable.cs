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
        if (QuitMenuController.isMenuOpen)
        {
            return;
        }
        gameObject.SetActive(false);
        bottlebreak.Play();
    }

    private void OnMouseOver()
    {
        if (QuitMenuController.isMenuOpen)
        {
            return;
        }
        spriteRenderer.material.color = mouseHoverColor;
    }

    private void OnMouseExit()
    {
        if (QuitMenuController.isMenuOpen)
        {
            return;
        }
        spriteRenderer.material.color = previousColor;
    }

}
