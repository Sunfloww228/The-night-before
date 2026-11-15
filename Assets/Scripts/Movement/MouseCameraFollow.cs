using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class MouseCameraFollow : MonoBehaviour
{
    [SerializeField] private InteractionBlocker interactionBlocker;
    [SerializeField] private BlackPanelController blackout;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Student student;

    [Header("Background")]
    [SerializeField] private SpriteRenderer background;

    [Header("Normal Movement")]
    [SerializeField, Min(0f)] private float smoothSpeed = 5f;

    [Header("Zoom")]
    [SerializeField] private Collider2D[] zoomTargets = new Collider2D[0];
    [SerializeField, Range(0.25f, 1f)] private float hoverSizeMultiplier = 0.65f;
    [SerializeField, Min(0.1f)] private float zoomInSpeed = 12f;
    [SerializeField, Min(0.1f)] private float zoomOutSpeed = 8f;

    [Header("Hover Delay")]
    [SerializeField, Range(0.5f, 3f)]
    private float focusDelay = 1.5f;

    private Camera cam;
    private float normalSize;
    private Collider2D focusedTarget;
    private SpriteRenderer focusedSprite;
    private Collider2D releasedTarget;
    private Vector2 previousCursor;

    private Collider2D hoveredTarget;
    private float hoverTimer;

    public void ReleaseFocus(Collider2D target)
    {
        if (target == null)
            return;

        if (focusedTarget == target)
        {
            releasedTarget = target;
            focusedTarget = null;
            focusedSprite = null;
        }

        if (hoveredTarget == target)
        {
            hoveredTarget = null;
            hoverTimer = 0f;
        }
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Bounds bounds = background.bounds;

        float backgroundHeight = bounds.size.y;
        float backgroundWidth = bounds.size.x;

        float sizeByHeight = backgroundHeight / 1.5f;
        float sizeByWidth = backgroundWidth / (1.5f * cam.aspect);

        normalSize = Mathf.Min(sizeByHeight, sizeByWidth) * 0.7f;
        cam.orthographicSize = normalSize;
    }

    private void LateUpdate()
    {
        if (gameManager == null || !gameManager.IsPlaying)
            return;

        if (background == null || !cam.orthographic)
            return;

        Mouse mouse = Mouse.current;
        Rect viewport = cam.pixelRect;
        if (viewport.width <= 0f || viewport.height <= 0f)
            return;

        Vector2 cursor = mouse != null ? mouse.position.ReadValue() : viewport.center;
        Vector2 offset = new Vector2(
            Mathf.Clamp01((cursor.x - viewport.xMin) / viewport.width) * 2f - 1f,
            Mathf.Clamp01((cursor.y - viewport.yMin) / viewport.height) * 2f - 1f);

        Bounds bounds = background.bounds;
        Vector2 center = bounds.center;
        Vector2 normalExtents = new Vector2(normalSize * cam.aspect, normalSize);
        Vector2 normalRange = GetPanRange(bounds, normalExtents);
        Vector2 panOffset = offset * MenuSettings.MouseSensitivity;
        panOffset.x = Mathf.Clamp(panOffset.x, -1f, 1f);
        panOffset.y = Mathf.Clamp(panOffset.y, -1f, 1f);
        Vector2 normalPosition = center + Vector2.Scale(normalRange, panOffset);

        Vector2 worldPoint = (Vector2)cam.ScreenToWorldPoint(cursor);
        bool pointerInside = mouse != null && viewport.Contains(cursor);
        if (releasedTarget != null && (!pointerInside || !IsPointerOver(releasedTarget, worldPoint)))
        {
            releasedTarget = null;
        }
        if (focusedTarget != null &&
            !focusedTarget.isActiveAndEnabled)
        {
            ReleaseFocus(focusedTarget);
        }

        if (blackout != null && blackout.IsBusy)
        {
            if (focusedTarget != null)
                ReleaseFocus(focusedTarget);

            hoveredTarget = null;
            hoverTimer = 0f;
        }
        else if (interactionBlocker != null && interactionBlocker.IsBlocked)
        {
            hoveredTarget = null;
            hoverTimer = 0f;
        }
        else if (mouse != null && mouse.rightButton.wasPressedThisFrame)
        {
            ReleaseFocus(focusedTarget);

            hoveredTarget = null;
            hoverTimer = 0f;
        }
        else
        {
            UpdateHoverFocus(
                worldPoint,
                pointerInside,
                Time.unscaledDeltaTime
            );
        }

        bool focusing = focusedTarget != null;

        float targetSize = focusing ? normalSize * hoverSizeMultiplier : normalSize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize,
            Blend(focusing ? zoomInSpeed : zoomOutSpeed));

        Vector2 extents = new Vector2(
            cam.orthographicSize * cam.aspect,
            cam.orthographicSize
        );

        Vector3 position = transform.position;

        if (focusing)
        {
            Vector2 centerOfTarget = focusedSprite != null
                ? focusedSprite.bounds.center
                : focusedTarget.bounds.center;

            Vector2 next = Vector2.Lerp(
                position,
                centerOfTarget,
                Blend(zoomInSpeed)
            );

            position.x = next.x;
            position.y = next.y;
        }
        else
        {
            Vector2 next = Vector2.Lerp(
                position,
                normalPosition,
                Blend(smoothSpeed)
            );

            position.x = next.x;
            position.y = next.y;
        }

        Vector2 range = GetPanRange(bounds, extents);

        position.x = Mathf.Clamp(
            position.x,
            center.x - range.x,
            center.x + range.x
        );

        position.y = Mathf.Clamp(
            position.y,
            center.y - range.y,
            center.y + range.y
        );

        transform.position = position;

        if (student != null)
        {
            student.SetVisible(focusedTarget == null);
        }
    }

    private void SetFocus(Collider2D target)
    {
        focusedTarget = target;
        focusedSprite = target != null ? target.GetComponentInParent<SpriteRenderer>() : null;


        Debug.Log(
        "Camera focus: " +
        (target != null ? target.name : "NONE")
    );
    }

    private Collider2D FindTarget(Vector2 point)
    {
        foreach (Collider2D target in zoomTargets)
        {
            if (target != null && target != releasedTarget && IsPointerOver(target, point))
                return target;
        }

        return null;
    }

    private void UpdateHoverFocus(Vector2 worldPoint, bool pointerInside, float dt)
    {
        Collider2D target = pointerInside
            ? FindTarget(worldPoint)
            : null;

        if (target != hoveredTarget)
        {
            hoveredTarget = target;
            hoverTimer = 0f;
        }

        if (target == null)
            return;

        if (focusedTarget == target)
            return;

        hoverTimer += dt;

        if (hoverTimer >= focusDelay)
        {
            SetFocus(target);

            hoveredTarget = null;
            hoverTimer = 0f;
        }
    }

    private static bool IsPointerOver(Collider2D target, Vector2 point)
    {
        if (!target.isActiveAndEnabled)
            return false;

        if (target.OverlapPoint(point))
            return true;

        // Include the visible sprite when its collider is smaller or tilted.
        SpriteRenderer sprite = target.GetComponent<SpriteRenderer>();
        if (sprite == null || !sprite.enabled || sprite.sprite == null)
            return false;

        Bounds bounds = sprite.bounds;
        return bounds.Contains(new Vector3(point.x, point.y, bounds.center.z));
    }

    private static Vector2 GetPanRange(Bounds bounds, Vector2 cameraExtents)
    {
        return Vector2.Max(Vector2.zero, (Vector2)bounds.extents - cameraExtents);
    }

    private static float Blend(float speed) => 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime);

    public Collider2D FocusedTarget => focusedTarget;
}
