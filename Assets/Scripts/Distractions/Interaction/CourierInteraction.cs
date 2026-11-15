using UnityEngine;
using UnityEngine.InputSystem;

public enum InteractionType 
{
    Doorbell,
    Pizza
}

public class CourierInteraction : MonoBehaviour
{
    [SerializeField] private InteractionBlocker interactionBlocker;
    [SerializeField] private CourierDistraction courier;
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameManager gameManager;

    [Header("Hover")]
    [SerializeField]
    private Color hoverTint =
        new Color(0.75f, 0.9f, 1f);

    private Collider2D hitArea;
    private SpriteRenderer objectSprite;
    private MouseCameraFollow cameraFollow;

    private Color originalTint;
    private void Awake()
    {
        hitArea = GetComponent<Collider2D>();
        objectSprite = GetComponent<SpriteRenderer>();

        if (objectSprite != null)
            originalTint = objectSprite.color;

        if (mainCamera != null)
        {
            cameraFollow =
                mainCamera.GetComponent<MouseCameraFollow>();
        }
    }
    private void Update()
    {
        if (courier == null ||
            mainCamera == null ||
            objectSprite == null ||
            hitArea == null)
            return;

        if (gameManager == null || !gameManager.IsPlaying || (interactionBlocker != null && interactionBlocker.IsBlocked))
        {
            ResetHover();

            if (cameraFollow != null &&
                cameraFollow.FocusedTarget == hitArea)
            {
                cameraFollow.ReleaseFocus(hitArea);
            }

            return;
        }

        bool canInteract = interactionType switch
        {
            InteractionType.Doorbell =>
                courier.IsCourierWaiting,

            InteractionType.Pizza =>
                courier.IsPizzaReady,

            _ => false
        };

        if (!canInteract)
        {
            ResetHover();
            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            ResetHover();
            return;
        }

        Vector2 cursor = mouse.position.ReadValue();

        Vector2 worldPoint =
            mainCamera.ScreenToWorldPoint(cursor);

        Bounds bounds = objectSprite.bounds;

        bool overCollider =
            hitArea.isActiveAndEnabled &&
            hitArea.OverlapPoint(worldPoint);

        bool overSprite =
            objectSprite.enabled &&
            objectSprite.sprite != null &&
            bounds.Contains(new Vector3(
                worldPoint.x,
                worldPoint.y,
                bounds.center.z
            ));

        bool hovering =
            mainCamera.pixelRect.Contains(cursor) &&
            (overCollider || overSprite);

        if (hovering &&
            mouse.leftButton.wasPressedThisFrame)
        {
            Interact();
        }

        objectSprite.color = hovering
            ? originalTint * hoverTint
            : originalTint;
    }
    private void Interact()
    {
        switch (interactionType)
        {
            case InteractionType.Doorbell:
                Debug.Log("Игрок нажал на звонок!");

                courier.AnswerDoor();
                break;

            case InteractionType.Pizza:
                Debug.Log("Игрок нажал на пиццу!");

                courier.EatPizza();
                break;
        }

        if (cameraFollow != null)
            cameraFollow.ReleaseFocus(hitArea);

        ResetHover();
    }

    private void ResetHover()
    {
        if (objectSprite != null)
            objectSprite.color = originalTint;
    }

    private void OnDisable()
    {
        ResetHover();

        if (cameraFollow != null && hitArea != null)
            cameraFollow.ReleaseFocus(hitArea);
    }
}
