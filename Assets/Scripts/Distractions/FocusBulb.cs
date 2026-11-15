using UnityEngine;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class FocusBulb : MonoBehaviour
{
    private const float FlameFramesPerSecond = 10f;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private Sprite[] focusLevels;
    [SerializeField] private Sprite[] flameFrames;

    private SpriteRenderer bulb;
    private Vector3 headPosition;
    private Vector3 headScale;
    private bool showInHead = true;
    private bool atMaximumFocus;
    private float flameStart;

    private void Awake()
    {
        bulb = GetComponent<SpriteRenderer>();
        headPosition = transform.localPosition;
        headScale = transform.localScale;
    }

    private void OnEnable()
    {
        gameManager.State.Changed += Refresh;
        Refresh(gameManager.State);
    }

    private void OnDisable()
    {
        gameManager.State.Changed -= Refresh;
        atMaximumFocus = false;
    }

    private void Refresh(GameState state)
    {
        if (state.Focus >= GameState.ResourceMax)
        {
            if (!atMaximumFocus)
            {
                flameStart = Time.time;
                bulb.sprite = flameFrames[0];
                atMaximumFocus = true;
            }

            return;
        }

        atMaximumFocus = false;
        float fraction = state.Focus / GameState.ResourceMax;
        int level = Mathf.FloorToInt(fraction * focusLevels.Length);
        bulb.sprite = focusLevels[level];
    }

    public void SetShowInHead(bool show)
    {
        showInHead = show;
    }

    private void LateUpdate()
    {
        Vector3 headWorldPosition = transform.parent.TransformPoint(headPosition);
        Vector3 headViewportPosition = sceneCamera.WorldToViewportPoint(headWorldPosition);
        if (showInHead && headViewportPosition.x > 0f && headViewportPosition.x < 1f &&
            headViewportPosition.y > 0f && headViewportPosition.y < 1f && headViewportPosition.z > 0f)
        {
            transform.localPosition = headPosition;
            transform.localScale = headScale;
            return;
        }

        // Reserve room for the flame and keep the bulb readable at any camera zoom.
        headViewportPosition.x = 0.09f;
        headViewportPosition.y = 0.62f;
        transform.position = sceneCamera.ViewportToWorldPoint(headViewportPosition);

        const float bulbHeightInViewport = 0.15f;
        const float bulbSpriteHeight = 0.74f;
        float scale = 2f * sceneCamera.orthographicSize * bulbHeightInViewport / bulbSpriteHeight;
        Vector3 parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(scale / parentScale.x, scale / parentScale.y, headScale.z);
    }

    private void Update()
    {
        if (!atMaximumFocus)
            return;

        int frame = Mathf.FloorToInt((Time.time - flameStart) * FlameFramesPerSecond);
        bulb.sprite = flameFrames[frame % flameFrames.Length];
    }
}
