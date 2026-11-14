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
}
