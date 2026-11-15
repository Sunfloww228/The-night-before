using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class PhoneDistraction : DistractionBase
{
    [SerializeField] private DifficultyManager difficultyManager;

    [SerializeField] private GameObject telephonePanel;
    [SerializeField] private Image frame;
    [SerializeField] private Image display;
    [SerializeField] private Sprite[] memes;
    [SerializeField] private Sprite motivationalCard;
    [SerializeField] private InteractionBlocker interactionBlocker;

    [SerializeField, Min(1)] private int minMemes = 2;
    [SerializeField, Min(1)] private int maxMemes = 5;
    [SerializeField, Min(0)] private float swipeCooldown = 1f;
    [SerializeField, Min(0)] private float watchTime = 2f;
    [SerializeField, Min(0)] private float stuckAfter = 8f;
    [SerializeField, Min(0)] private float focusPerSecond = 1f;
    [SerializeField, Min(0)] private float stressReward = 20f;

    private int remainingMemes;
    private float elapsed;
    private float cardElapsed;

    private void ApplyDifficulty()
    {
        if (difficultyManager == null ||
            difficultyManager.Phone == null)
            return;

        var settings = difficultyManager.Phone;

        minMemes = settings.minMemes;
        maxMemes = settings.maxMemes;
        swipeCooldown = settings.swipeCooldown;
        watchTime = settings.watchTime;
        stuckAfter = settings.stuckAfter;

        focusPerSecond = settings.focusPerSecond;
        stressReward = settings.stressReward;
    }

    public override void Activate()
    {
        if (IsActive || !CanActivate())
            return;

        if (interactionBlocker != null && interactionBlocker.IsBlocked)
            return;
        ApplyDifficulty();
        remainingMemes = Random.Range(minMemes, Mathf.Max(minMemes, maxMemes) + 1);
        elapsed = 0f;
        ShowCard();
        base.Activate();
        if (telephonePanel != null)
            telephonePanel.SetActive(true);
        if (interactionBlocker != null)
            interactionBlocker.Block(this);
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
            Game.ApplyResourceChanges(0f, -stressReward);
        }
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
        if (interactionBlocker != null)
            interactionBlocker.Unblock(this);
        if (Game != null)
            Game.SetWorkBlocked(this, false);

        if (telephonePanel != null)
            telephonePanel.SetActive(false);

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
