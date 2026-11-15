using UnityEngine;
using UnityEngine.InputSystem;

public class NeighbourInteraction : MonoBehaviour
{
    [SerializeField] private NeighbourDistraction neighbour;
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

    // Update is called once per frame
    void Update()
    {
        if (neighbour == null ||
            mainCamera == null ||
            objectSprite == null ||
            hitArea == null)
            return;

        if (gameManager != null && !gameManager.IsPlaying)
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
        neighbour.Interact();
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
