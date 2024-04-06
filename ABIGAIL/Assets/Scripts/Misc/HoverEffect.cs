using UnityEngine;
using UnityEngine.EventSystems; // Required for event handling

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite normalSprite; 
    public Sprite hoverSprite;
    private SpriteRenderer spriteRenderer;
    private UnityEngine.UI.Image uiImage;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiImage = GetComponent<UnityEngine.UI.Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Change to hover sprite
        if (spriteRenderer != null) spriteRenderer.sprite = hoverSprite;
        if (uiImage != null) uiImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Change back to normal sprite
        if (spriteRenderer != null) spriteRenderer.sprite = normalSprite;
        if (uiImage != null) uiImage.sprite = normalSprite;
    }
}
