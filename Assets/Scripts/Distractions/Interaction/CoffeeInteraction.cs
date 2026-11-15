using UnityEngine;
using UnityEngine.InputSystem;

public class CoffeeInteraction : MonoBehaviour
{
    [SerializeField] private InteractionBlocker interactionBlocker;
    [SerializeField] private Camera interactionCamera;
    [SerializeField] private CoffeeDistraction coffee;
    [SerializeField] private Color hoverTint = new Color(0.75f, 0.9f, 1f);

    private Collider2D hitArea;
    private SpriteRenderer cupSprite;
    private MouseCameraFollow cameraFollow;
    private Color originalTint;
    private bool panelWasOpen;

    private void Awake()
    {
        hitArea = GetComponent<Collider2D>();
        cupSprite = GetComponent<SpriteRenderer>();
        originalTint = cupSprite.color;
        if (interactionCamera != null)
            cameraFollow = interactionCamera.GetComponent<MouseCameraFollow>();
    }

    private void Update()
    {
        if (coffee == null || interactionCamera == null)
            return;

        Mouse mouse = Mouse.current;

        bool cancel = mouse != null &&
            mouse.rightButton.wasPressedThisFrame;

        if (coffee.IsMachineOpen && cancel)
            coffee.CloseMachine();

        if (panelWasOpen && !coffee.IsMachineOpen &&
            cameraFollow != null)
        {
            cameraFollow.ReleaseFocus(hitArea);
        }

        panelWasOpen = coffee.IsMachineOpen;

        if (interactionBlocker != null && interactionBlocker.IsBlocked)
        {
            cupSprite.color = originalTint;
            return;
        }

        bool hovering = false;

        if (mouse != null && !coffee.IsMachineOpen && !panelWasOpen && !cancel)
        {
            Vector2 cursor = mouse.position.ReadValue();

            Vector2 worldPoint =interactionCamera.ScreenToWorldPoint(cursor);

            Bounds bounds = cupSprite.bounds;

            bool overCollider = hitArea.isActiveAndEnabled && hitArea.OverlapPoint(worldPoint);

            bool overSprite = cupSprite.enabled && cupSprite.sprite != null && bounds.Contains
                (new Vector3(
                    worldPoint.x,
                    worldPoint.y,
                    bounds.center.z
                ));

            hovering = interactionCamera.pixelRect.Contains(cursor) && (overCollider || overSprite);

            if (hovering && mouse.leftButton.wasPressedThisFrame)
            {
                Debug.Log("Cup clicked!");
                coffee.Interact();
                Debug.Log("Machine open: " + coffee.IsMachineOpen);
            }
        }

        panelWasOpen = coffee.IsMachineOpen;

        cupSprite.color = hovering && !panelWasOpen ? originalTint * hoverTint : originalTint;
    }

    private void OnDisable()
    {
        if (cupSprite != null)
            cupSprite.color = originalTint;

        panelWasOpen = false;
    }
}
