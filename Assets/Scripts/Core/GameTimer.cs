using UnityEngine;

public static class GameTimer
{
    public static float GetStep(float remainingTime, float deltaTime)
    {
        return Mathf.Clamp(deltaTime, 0f, remainingTime);
    }
}
