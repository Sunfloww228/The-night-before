using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    [SerializeField] private GameBalance balance;
    [SerializeField] private bool startAutomatically = true;

    [SerializeField] private MouseCameraFollow cameraFollow;
    [SerializeField] private Collider2D monitor;

    private bool workBlocked;
    private readonly HashSet<MonoBehaviour> workBlockers = new HashSet<MonoBehaviour>();

    public event Action SessionRestarting;

    public GameState State { get; } = new GameState();
    public float TimeLeft => State.RemainingTime;
    public bool IsPlaying => State.Status == GameStatus.Running;

    public event Action<GameStatus> SessionEnded;

    public event Action OnTimeChanged;
    public event Action<bool> OnGameFinished;


    private void Start()
    {
        AudioListener.volume = 1f;

        if (startAutomatically && State.Status == GameStatus.Idle)
            StartSession();
    }

    public void StartSession()
    {
        if (State.Status == GameStatus.Running)
            return;

        RestartGame();
    }

    public void RestartGame()
    {
        if (balance == null)
        {
            Debug.LogError("для начала вставь гейм баланс.", this);
            return;
        }

        SessionRestarting?.Invoke();
        workBlocked = false;
        workBlockers.Clear();
        State.Set(balance.StartingFocus, balance.StartingStress, 0f,
            balance.DurationSeconds, GameStatus.Running);
        OnTimeChanged?.Invoke();
    }

    public void SetWorkBlocked(bool blocked)
    {
        if (State.Status == GameStatus.Running)
            workBlocked = blocked;
    }

    public void ApplyResourceChanges(float focusDelta, float stressDelta)
    {
        if (State.Status != GameStatus.Running)
            return;

        Commit(State.Focus + focusDelta, State.Stress + stressDelta,
            State.Progress, State.RemainingTime);
    }

    public void SetWorkBlocked(MonoBehaviour source, bool blocked)
    {
        if (blocked && IsPlaying)
            workBlockers.Add(source);
        else
            workBlockers.Remove(source);
    }

    private void Update()
    {
        if (State.Status != GameStatus.Running)
            return;

        float step = GameTimer.GetStep(State.RemainingTime, Time.deltaTime);
        if (step <= 0f)
            return;

        float stress = State.Stress;
        float progress = State.Progress;

        bool isLookingAtMonitor = cameraFollow != null && monitor != null && cameraFollow.FocusedTarget == monitor;

        if (isLookingAtMonitor && !workBlocked && workBlockers.Count == 0 && stress < GameState.ResourceMax)
        {
            progress += balance.BaseProgressPerSecond * balance.GetWorkMultiplier(State.Focus) * step;
            stress = Mathf.Min(GameState.ResourceMax, stress + balance.WorkStressPerSecond * step);
        }

        float focus = State.Focus - balance.GetStressDamagePerSecond(stress) * step;
        Commit(focus, stress, progress, State.RemainingTime - step);
    }

    public void CheckGameOver()
    {
        if (IsPlaying)
            Commit(State.Focus, State.Stress, State.Progress, TimeLeft);
    }

    private void Commit(float focus, float stress, float progress, float remainingTime)
    {
        GameStatus status = GameStatus.Running;
        if (focus <= 0f)
            status = GameStatus.LostFocus;
        else if (progress >= GameState.ResourceMax)
            status = GameStatus.Won;
        else if (remainingTime <= 0f)
            status = GameStatus.DeadlineExpired;

        bool timeChanged = TimeLeft != remainingTime;
        State.Set(focus, stress, progress, remainingTime, status);
        if (timeChanged)
            OnTimeChanged?.Invoke();

        if (status != GameStatus.Running)
        {
            DifficultiesUnlock.Unlock();

            SessionEnded?.Invoke(status);
            OnGameFinished?.Invoke(status == GameStatus.Won);
        }
    }
}
