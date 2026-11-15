using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class AdvertisementSlot : MonoBehaviour
{
    [Header("Спрайты")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite activeSprite;


    [SerializeField] private AdvertismentDistraction advertisement;
    [SerializeField] private InteractionBlocker interactionBlocker;
    [SerializeField] private Collider2D monitorCollider;
    [SerializeField] private Camera mainCamera;

    [Header("Область крестика")]
    [SerializeField] private BoxCollider2D closeArea;

    [Header("Нажатие")]
    [SerializeField, Min(0f)]
    private float pressDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private MouseCameraFollow cameraFollow;

    private bool isShowing;
    private bool isClosing;
    private Coroutine closeRoutine;

    public bool IsShowing => isShowing;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (mainCamera != null)
            cameraFollow = mainCamera.GetComponent<MouseCameraFollow>();
    }

    public void Show()
    {
        if (normalSprite == null)
            return;

        if (closeRoutine != null)
        {
            StopCoroutine(closeRoutine);
            closeRoutine = null;
        }

        isShowing = true;
        isClosing = false;

        spriteRenderer.sprite = normalSprite;
    }
    public void Hide()
    {
        if (closeRoutine != null)
        {
            StopCoroutine(closeRoutine);
            closeRoutine = null;
        }

        isShowing = false;
        isClosing = false;

        if (spriteRenderer != null)
            spriteRenderer.sprite = null;
    }
    private void Update()
    {
        if (!isShowing || isClosing || mainCamera == null || cameraFollow == null || monitorCollider == null)
            return;

        if (interactionBlocker != null && interactionBlocker.IsBlocked)
            return;

        if (cameraFollow.FocusedTarget != monitorCollider)
            return;

        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        Vector2 cursor = mouse.position.ReadValue();

        if (!mainCamera.pixelRect.Contains(cursor))
            return;

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(cursor);

        bool overClose = closeArea.isActiveAndEnabled && closeArea.OverlapPoint(worldPoint);

        if (overClose && mouse.leftButton.wasPressedThisFrame)
        {
            closeRoutine = StartCoroutine(CloseSequence());
        }
    }

    private IEnumerator CloseSequence()
    {
        isClosing = true;

        if (activeSprite != null)
            spriteRenderer.sprite = activeSprite;

        yield return new WaitForSeconds(pressDuration);

        if (advertisement != null)
            advertisement.CloseAdvertisement(this);

        Hide();
    }

}
