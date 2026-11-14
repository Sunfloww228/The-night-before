using UnityEngine;

public abstract class DistractionBase : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    protected GameManager Game;

    public bool IsActive { get; private set; }

    public void Initialize(GameManager game)
    {
        Unsubscribe();
        Game = game;
        if (Game != null)
        {
            Game.SessionRestarting += ResetDistraction;
            Game.SessionEnded += OnSessionEnded;
        }
    }

    protected bool CanActivate()
    {
        if (Game == null)
            Initialize(gameManager);

        return enabled && Game != null && Game.IsPlaying;
    }

    public virtual void Activate()
    {
        IsActive = true;
        gameObject.SetActive(true);
    }

    public virtual void Tick(float dt) { }

    public virtual void Resolve()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (IsActive && Game != null && Game.IsPlaying)
            Tick(GameTimer.GetStep(Game.TimeLeft, Time.deltaTime));
    }

    protected virtual void OnDisable() => ResetDistraction();

    protected virtual void OnDestroy() => Unsubscribe();

    private void OnSessionEnded(GameStatus status) => ResetDistraction();

    private void ResetDistraction()
    {
        if (IsActive)
            Resolve();
    }

    private void Unsubscribe()
    {
        if (Game == null)
            return;

        Game.SessionRestarting -= ResetDistraction;
        Game.SessionEnded -= OnSessionEnded;
    }
}
