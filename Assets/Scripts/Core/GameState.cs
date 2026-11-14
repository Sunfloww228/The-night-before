using System;
using UnityEngine;

public enum GameStatus
{
    Idle,
    Running,
    Won,
    LostFocus,
    DeadlineExpired
}

public sealed class GameState
{
    public const float ResourceMax = 100f;

    public float Focus { get; private set; }
    public float Stress { get; private set; }
    public float Progress { get; private set; }
    public float RemainingTime { get; private set; }
    public GameStatus Status { get; private set; }

    public event Action<GameState> Changed;

    internal void Set(float focus, float stress, float progress, float remainingTime, GameStatus status)
    {
        focus = Mathf.Clamp(focus, 0f, ResourceMax);
        stress = Mathf.Clamp(stress, 0f, ResourceMax);
        progress = Mathf.Clamp(progress, 0f, ResourceMax);
        remainingTime = Mathf.Max(0f, remainingTime);

        if (Focus == focus && Stress == stress && Progress == progress &&
            RemainingTime == remainingTime && Status == status)
            return;

        Focus = focus;
        Stress = stress;
        Progress = progress;
        RemainingTime = remainingTime;
        Status = status;
        Changed?.Invoke(this);
    }
}
