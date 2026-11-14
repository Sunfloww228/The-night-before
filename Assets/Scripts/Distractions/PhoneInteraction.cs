using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public sealed class PhoneInteraction : MonoBehaviour
{
    [SerializeField] private Camera interactionCamera;
    [SerializeField] private PhoneDistraction panel;
    [SerializeField] private Color hoverTint = new Color(0.75f, 0.9f, 1f);

    private Collider2D hitArea;
    private SpriteRenderer phoneSprite;
    private MouseCameraFollow cameraFollow;
    private Color originalTint;
    private bool panelWasOpen;

    private void Awake()
    {
        hitArea = GetComponent<Collider2D>();
        phoneSprite = GetComponent<SpriteRenderer>();
        originalTint = phoneSprite.color;
        if (interactionCamera != null)
            cameraFollow = interactionCamera.GetComponent<MouseCameraFollow>();
    }

    private void Update()
    {
        if (panel == null || interactionCamera == null)
            return;

        Mouse mouse = Mouse.current;
        bool cancel = mouse != null && mouse.rightButton.wasPressedThisFrame;
        if (panel.IsActive && cancel)
            panel.Resolve();

        if (panelWasOpen && !panel.IsActive && cameraFollow != null)
            cameraFollow.ReleaseFocus(hitArea);

        bool hovering = false;
        if (mouse != null && !panel.IsActive && !panelWasOpen && !cancel)
        {
            Vector2 cursor = mouse.position.ReadValue();
            hovering = hitArea.isActiveAndEnabled && interactionCamera.pixelRect.Contains(cursor) &&
                hitArea.OverlapPoint(interactionCamera.ScreenToWorldPoint(cursor));
            if (hovering && mouse.leftButton.wasPressedThisFrame)
                panel.Activate();
        }

        panelWasOpen = panel.IsActive;
        phoneSprite.color = hovering && !panelWasOpen ? originalTint * hoverTint : originalTint;
    }

    private void OnDisable()
    {
        phoneSprite.color = originalTint;
        if (panel != null && panel.IsActive)
            panel.Resolve();
        panelWasOpen = false;
    }
}
