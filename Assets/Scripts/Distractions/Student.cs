using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Student : MonoBehaviour
{
    private const float FrameDuration = 1f / 12f;

    [SerializeField] private SpriteRenderer studentSprite;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Sprite sittingSprite;
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private Sprite[] departureFrames;
    [SerializeField] private Vector2[] departureOffsets;
    [SerializeField, Min(0.01f)] private float sittingWidth = 2.4f;

    private Vector3 sittingPosition;
    private Vector3 originalScale;
    private bool sittingFlipX;
    private Coroutine departure;
    private bool away;
    private bool visible = true;

    private void Awake()
    {
        if (studentSprite == null)
            studentSprite = GetComponent<SpriteRenderer>();

        sittingPosition = studentSprite.transform.localPosition;
        originalScale = studentSprite.transform.localScale;
        sittingFlipX = studentSprite.flipX;
    }

    private void OnEnable()
    {
        if (gameManager != null)
            gameManager.SessionRestarting += PlaySitting;

        PlaySitting();
    }

    private void OnDisable()
    {
        if (gameManager != null)
            gameManager.SessionRestarting -= PlaySitting;

        PlaySitting();
    }

    public void SetVisible(bool visible)
    {
        this.visible = visible;
        studentSprite.enabled = visible && !away;
    }

    public void PlaySitting()
    {
        if (departure != null)
        {
            StopCoroutine(departure);
            departure = null;
        }

        away = false;
        studentSprite.flipX = sittingFlipX;
        studentSprite.transform.localPosition = sittingPosition;
        SetSprite(sittingSprite, sittingWidth);
        SetVisible(visible);
    }

    public void PlayCourier()
    {
        PlayDeparture(toLeft: false);
    }

    public void PlayNeighbour()
    {
        PlayDeparture(toLeft: true);
    }

    private void PlayDeparture(bool toLeft)
    {
        if (departure != null || away || departureFrames.Length == 0)
            return;

        departure = StartCoroutine(DepartureSequence(toLeft));
    }

    private IEnumerator DepartureSequence(bool toLeft)
    {
        studentSprite.flipX = sittingFlipX ^ toLeft;
        for (int i = 0; i < departureFrames.Length; i++)
        {
            Sprite frame = departureFrames[i];
            float width = sittingWidth * frame.rect.width / departureFrames[0].rect.width;
            SetSprite(frame, width);
            Vector2 offset = departureOffsets[i] * sittingWidth;
            if (toLeft)
                offset.x = -offset.x;

            studentSprite.transform.localPosition = sittingPosition + (Vector3)offset;
            yield return new WaitForSeconds(FrameDuration);
        }

        if (sceneCamera != null)
            yield return MovePastScreenEdge(toLeft);

        departure = null;
        away = true;
        SetVisible(visible);
    }

    private IEnumerator MovePastScreenEdge(bool toLeft)
    {
        const float exitDuration = 0.3f;
        Transform spriteTransform = studentSprite.transform;
        Vector3 start = spriteTransform.position;
        Vector3 end = start;
        float depth = sceneCamera.WorldToViewportPoint(start).z;
        float viewportEdge = toLeft ? 0f : 1f;
        float screenEdge = sceneCamera.ViewportToWorldPoint(new Vector3(viewportEdge, 0.5f, depth)).x;
        Bounds bounds = studentSprite.bounds;
        end.x += toLeft
            ? Mathf.Min(0f, screenEdge - bounds.max.x) - 0.01f
            : Mathf.Max(0f, screenEdge - bounds.min.x) + 0.01f;

        for (float elapsed = 0f; elapsed < exitDuration; elapsed += Time.deltaTime)
        {
            spriteTransform.position = Vector3.Lerp(start, end, elapsed / exitDuration);
            yield return null;
        }

        spriteTransform.position = end;
    }

    private void SetSprite(Sprite sprite, float width)
    {
        if (sprite == null)
            return;

        studentSprite.sprite = sprite;
        studentSprite.transform.localScale = originalScale / originalScale.x * (width / sprite.rect.width * sprite.pixelsPerUnit);
    }
}
