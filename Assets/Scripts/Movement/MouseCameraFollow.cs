using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class MouseCameraFollow : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private SpriteRenderer background;

    [Header("Normal Movement")]
    [SerializeField, Min(0f)] private float smoothSpeed = 5f;

    [Header("Zoom")]
    [SerializeField] private Collider2D[] zoomTargets = new Collider2D[0];
    [SerializeField, Range(0.25f, 1f)] private float hoverSizeMultiplier = 0.65f;
    [SerializeField, Min(0.1f)] private float zoomInSpeed = 12f;
    [SerializeField, Min(0.1f)] private float zoomOutSpeed = 8f;

    private Camera cam;
    private float normalSize;
    private Collider2D focusedTarget;
    private SpriteRenderer focusedSprite;
    private Collider2D releasedTarget;
    private bool returningFromFocus;
    private Vector2 previousCursor;

    public void ReleaseFocus(Collider2D target)
    {
        if (target == null || focusedTarget != target)
            return;

        releasedTarget = focusedTarget;
        returningFromFocus = true;
        focusedTarget = null;
        focusedSprite = null;
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
        normalSize = cam.orthographicSize;
    }

    private void LateUpdate()
    {
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
        Vector2 normalPosition = center + Vector2.Scale(normalRange, offset);

        Vector2 worldPoint = (Vector2)cam.ScreenToWorldPoint(cursor);
        bool pointerInside = mouse != null && viewport.Contains(cursor);
        bool cursorMoved = !cursor.Equals(previousCursor);
        previousCursor = cursor;
        if (mouse != null && mouse.rightButton.wasPressedThisFrame)
        {
            ReleaseFocus(focusedTarget);
        }
        else if (focusedTarget == null || !focusedTarget.isActiveAndEnabled)
        {
            focusedTarget = null;
            // Rearm hover after the pointer leaves the released object at normal zoom.
            if (releasedTarget != null &&
                (!pointerInside || !IsPointerOver(releasedTarget, worldPoint)))
                releasedTarget = null;

            if (!returningFromFocus && pointerInside)
            {
                SetFocus(FindTarget(worldPoint));
            }
        }
        else if (pointerInside && cursorMoved)
        {
            // Keep the current focus over empty space, but allow another visible target.
            Collider2D nextTarget = FindTarget(worldPoint);
            if (nextTarget != null && nextTarget != focusedTarget)
                SetFocus(nextTarget);
        }

        bool focusing = focusedTarget != null;

        float targetSize = focusing ? normalSize * hoverSizeMultiplier : normalSize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize,
            Blend(focusing ? zoomInSpeed : zoomOutSpeed));
        if (Mathf.Abs(cam.orthographicSize - targetSize) < 0.001f)
        {
            cam.orthographicSize = targetSize;
            if (!focusing)
                returningFromFocus = false;
        }

        Vector2 extents = new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize);
        Vector3 position = transform.position;
        if (focusing)
        {
            Vector2 centerOfTarget = focusedSprite != null
                ? focusedSprite.bounds.center
                : focusedTarget.bounds.center;
            Vector2 next = Vector2.Lerp(position, centerOfTarget, Blend(zoomInSpeed));
            position.x = next.x;
            position.y = next.y;
        }
        else
        {
            Vector2 next = Vector2.Lerp(position, normalPosition, Blend(smoothSpeed));
            Vector2 range = GetPanRange(bounds, extents);
            position.x = Mathf.Clamp(next.x, center.x - range.x, center.x + range.x);
            position.y = Mathf.Clamp(next.y, center.y - range.y, center.y + range.y);
        }

        transform.position = position;
    }

    private void SetFocus(Collider2D target)
    {
        focusedTarget = target;
        focusedSprite = target != null ? target.GetComponentInParent<SpriteRenderer>() : null;
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
}
