using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class PhoneDistraction : DistractionBase, IBeginDragHandler, IEndDragHandler, IScrollHandler
{
    [SerializeField] private Image frame;
    [SerializeField] private Image display;
    [SerializeField] private Sprite[] memes;
    [SerializeField] private Sprite motivationalCard;

    [SerializeField, Min(1)] private int minMemes = 2;
    [SerializeField, Min(1)] private int maxMemes = 5;
    [SerializeField, Min(0)] private float swipeCooldown = 1f;
    [SerializeField, Min(1)] private float swipeDistance = 60f;
    [SerializeField, Min(0)] private float watchTime = 2f;
    [SerializeField, Min(0)] private float stuckAfter = 8f;
    [SerializeField, Min(0)] private float focusPerSecond = 1f;
    [SerializeField, Min(0)] private float reward = 10f;

    private int remainingMemes;
    private float elapsed;
    private float cardElapsed;

    public override void Activate()
    {
        if (IsActive || !CanActivate())
            return;

        remainingMemes = Random.Range(minMemes, Mathf.Max(minMemes, maxMemes) + 1);
        elapsed = 0f;
        if (frame == null)
            BuildPanel();
        ShowCard();
        base.Activate();
        Game.SetWorkBlocked(this, true);

    }

    public override void Tick(float dt)
    {
        if (remainingMemes == 0)
            dt = Mathf.Min(dt, Mathf.Max(0f, watchTime - cardElapsed));

        float normalTime = Mathf.Min(dt, Mathf.Max(0f, stuckAfter - elapsed));
        elapsed += dt;
        cardElapsed += dt;
        Game.ApplyResourceChanges(-focusPerSecond * (normalTime + 2f * (dt - normalTime)), 0f);

        if (IsActive && remainingMemes == 0 && cardElapsed >= watchTime)
        {
            Resolve();
            Game.ApplyResourceChanges(reward, 0f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnScroll(PointerEventData eventData)
    {
        if (eventData.scrollDelta.y < 0f)
            Swipe();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Vector2.Distance(eventData.pressPosition, eventData.position) >= swipeDistance)
            Swipe();
    }

    public void Swipe()
    {
        if (!IsActive || remainingMemes == 0 || cardElapsed < swipeCooldown)
            return;

        remainingMemes--;
        ShowCard();
    }

    public override void Resolve()
    {
        if (Game != null)
            Game.SetWorkBlocked(this, false);

        base.Resolve();
    }

    private void ShowCard()
    {
        cardElapsed = 0f;
        bool helpful = remainingMemes == 0;
        if (frame != null)
            frame.color = helpful ? Color.green : Color.red;

        if (display != null)
        {
            display.sprite = helpful ? motivationalCard : PickMeme();
            display.enabled = display.sprite != null;
        }
    }

    private Sprite PickMeme()
    {
        return memes != null && memes.Length > 0 ? memes[Random.Range(0, memes.Length)] : null;
    }

    private void BuildPanel()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;
        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 1f;
        gameObject.AddComponent<GraphicRaycaster>();

        Image shade = MakeImage("Backdrop", transform, new Color(0f, 0f, 0f, 0.72f));
        Stretch(shade.rectTransform, 0f);
        Image body = MakeImage("Phone", shade.transform, new Color(0.08f, 0.08f, 0.1f));
        body.rectTransform.sizeDelta = new Vector2(328f, 576f);
        frame = MakeImage("Card Frame", body.transform, Color.red);
        frame.rectTransform.sizeDelta = new Vector2(300f, 520f);
        display = MakeImage("Meme", frame.transform, Color.white);
        Stretch(display.rectTransform, 6f);
        display.preserveAspect = true;
        display.raycastTarget = false;
    }

    private static Image MakeImage(string objectName, Transform parent, Color color)
    {
        GameObject child = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        child.transform.SetParent(parent, false);
        Image image = child.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static void Stretch(RectTransform rect, float padding)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.one * padding;
        rect.offsetMax = -Vector2.one * padding;
    }
}
