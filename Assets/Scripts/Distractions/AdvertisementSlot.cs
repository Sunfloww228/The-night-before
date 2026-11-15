using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class AdvertisementSlot : MonoBehaviour
{
    [SerializeField] private AdvertismentDistraction advertisement;
    [SerializeField] private InteractionBlocker interactionBlocker;
    [SerializeField] private MouseCameraFollow cameraFollow;
    [SerializeField] private Collider2D monitorCollider;
    [SerializeField] private Camera mainCamera;

    [Header("Индекс слота")]
    [SerializeField] private int slotIndex;

    [Header("Область крестика")]
    [SerializeField] private Collider2D closeArea;

    private SpriteRenderer spriteRenderer;
    private Sprite normalSprite;
    private Sprite activeSprite;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetAdvertisement(Sprite normal, Sprite active)
    {
        normalSprite = normal;
        activeSprite = active;

        spriteRenderer.sprite = normalSprite;
    }

    public void ClearAdvertisement()
    {
        normalSprite = null;
        activeSprite = null;

        spriteRenderer.sprite = null;
    }

    private void Update()
    {
        if (normalSprite == null || cameraFollow == null || monitorCollider == null || closeArea == null)
            return;

        if (interactionBlocker != null &&
            interactionBlocker.IsBlocked)
        {
            spriteRenderer.sprite = normalSprite;
            return;
        }


        if (cameraFollow.FocusedTarget != monitorCollider)
        {
            spriteRenderer.sprite = normalSprite;
            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        Vector2 cursor = mouse.position.ReadValue();

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(cursor);

        bool hovering =mainCamera.pixelRect.Contains(cursor) && closeArea.isActiveAndEnabled && closeArea.OverlapPoint(worldPoint);

        if (hovering)
        {
            spriteRenderer.sprite = activeSprite != null ? activeSprite : normalSprite;

            if (mouse.leftButton.wasPressedThisFrame)
                advertisement.CloseAdvertisement(slotIndex);
        }
        else
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    public bool IsOccupied => normalSprite != null;

}
