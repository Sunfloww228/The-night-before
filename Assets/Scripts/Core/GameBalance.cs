using UnityEngine;

[CreateAssetMenu(menuName = "The Night Before/Game Balance")]
public sealed class GameBalance : ScriptableObject
{
    [SerializeField, Range(1f, 120f)] private float durationSeconds = 120f;
    [SerializeField, Range(1f, 100f)] private float startingFocus = 70f;
    [SerializeField, Range(0f, 100f)] private float startingStress;
    [SerializeField, Min(0f)] private float workStressPerSecond = 1f;
    [SerializeField, Range(0f, 100f)] private float stressDamageThreshold = 70f;
    [SerializeField, Min(0f)] private float stressDamageFactor = 0.2f;

    [Header("Work speed")]
    [SerializeField, Range(0f, 100f)] private float flowThreshold = 80f;
    [SerializeField, Range(0f, 100f)] private float normalThreshold = 50f;
    [SerializeField, Range(0f, 100f)] private float anxietyThreshold = 20f;
    [SerializeField, Min(0f)] private float flowMultiplier = 2f;
    [SerializeField, Min(0f)] private float normalMultiplier = 1f;
    [SerializeField, Min(0f)] private float anxietyMultiplier = 0.5f;
    [SerializeField, Min(0f)] private float panicMultiplier = 0.1f;

    public float DurationSeconds => durationSeconds;
    public float StartingFocus => startingFocus;
    public float StartingStress => startingStress;
    public float BaseProgressPerSecond => GameState.ResourceMax / durationSeconds;
    public float WorkStressPerSecond => workStressPerSecond;

    public float GetWorkMultiplier(float focus)
    {
        if (focus >= flowThreshold)
            return flowMultiplier;
        if (focus >= normalThreshold)
            return normalMultiplier;
        if (focus >= anxietyThreshold)
            return anxietyMultiplier;
        return panicMultiplier;
    }

    public float GetStressDamagePerSecond(float stress)
    {
        return Mathf.Max(0f, stress - stressDamageThreshold) * stressDamageFactor;
    }

    private void OnValidate()
    {
        normalThreshold = Mathf.Min(normalThreshold, flowThreshold);
        anxietyThreshold = Mathf.Min(anxietyThreshold, normalThreshold);
    }
}
