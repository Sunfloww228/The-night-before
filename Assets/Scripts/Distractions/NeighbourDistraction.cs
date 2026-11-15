using UnityEngine;
using System.Collections;

public class NeighbourDistraction : DistractionBase
{
    [SerializeField] private BlackPanelController blackout;
    [SerializeField] private GameObject neighbourIcon;
    [SerializeField] private AudioSource neighbourAudio;
    [SerializeField] private AudioSource knockingAudio;

    [SerializeField] private float minAppearTime = 20f;
    [SerializeField] private float maxAppearTime = 50f;
    [SerializeField] private float waitTime = 20f;
    [SerializeField] private float normalAudioInterval = 5f;

    [Header("эффекты")]
    [SerializeField] private float focusLoss = 2f;
    [SerializeField] private float stressPenalty = 3f;

    [Header("Время бить батарею соседу")]
    [SerializeField] private float walkingTimeCost = 5f;

    private float appearTimer;
    private float waitTimer;
    private float soundTimer;

    private bool neighbourWaiting;
    private bool walking;

    private Coroutine walkingRoutine;
    private IEnumerator Start()
    {
        ResetNeighbour();
        yield return new WaitUntil(() => CanActivate());
        Activate();

    }

    private void ResetNeighbour() 
    {
        appearTimer = Random.Range(minAppearTime, maxAppearTime);

        waitTimer = 0;
        soundTimer = 0;

        neighbourWaiting = false;
        walking = false;

        if (neighbourIcon != null)
            neighbourIcon.SetActive(false);
        if (neighbourAudio != null)
            neighbourAudio.Stop();

        if (knockingAudio != null)
            knockingAudio.Stop();

        if (blackout != null)
            blackout.End(this);

        if (Game != null)
            Game.SetWorkBlocked(this, false);

    }

    public override void Tick(float dt)
    {
        if (walking)
            return;

        if (!neighbourWaiting)
        {
            appearTimer -= dt;

            if (appearTimer <= 0f)
                Arrive();

            return;
        }

        Game.ApplyResourceChanges(
            -focusLoss * dt,
            -stressPenalty * dt
        );

        if (!Game.IsPlaying)
            return;

        waitTimer += dt;
        soundTimer -= dt;

        if (soundTimer <= 0f)
        {
            PlayNoise();
            soundTimer = Mathf.Max(0.1f, normalAudioInterval);
        }

        if (waitTimer >= waitTime)
            StopNoise();
    }

    private void Arrive()
    {
        neighbourWaiting = true;
        waitTimer = 0f;
        soundTimer = Mathf.Max(0.1f, normalAudioInterval);

        if (neighbourIcon != null)
            neighbourIcon.SetActive(true);

        PlayNoise();

        Debug.Log("Соседи начали шуметь!");
    }

    private void PlayNoise()
    {
        if (neighbourAudio != null &&
            neighbourAudio.clip != null &&
            !neighbourAudio.isPlaying)
        {
            neighbourAudio.Play();
        }
    }

    private void StopNoise()
    {
        if (!neighbourWaiting)
            return;

        neighbourWaiting = false;

        if (neighbourIcon != null)
            neighbourIcon.SetActive(false);

        if (neighbourAudio != null)
            neighbourAudio.Stop();

        Debug.Log("Соседи перестали шуметь.");

        FinishIfPossible();
    }

    public void Interact()
    {
        if (!neighbourWaiting ||
            walking ||
            Game == null ||
            !Game.IsPlaying)
            return;

        //if (player == null ||
        //    batteryPoint == null ||
        //    blackout == null)
        //{
        //    Debug.LogError(
        //        "Не назначены Player, BatteryPoint или BlackoutController!",
        //        this
        //    );
        //    return;
        //}

        if (!blackout.TryBegin(this))
            return;

        neighbourWaiting = false;
        walking = true;

        if (neighbourIcon != null)
            neighbourIcon.SetActive(false);

        if (neighbourAudio != null)
            neighbourAudio.Stop();

        Game.SetWorkBlocked(this, true);

        walkingRoutine = StartCoroutine(WalkingSequence());
    }

    private IEnumerator WalkingSequence()
    {
        float startTime = Time.time;

        float knockDuration = 0.5f;

        if (knockingAudio != null &&
            knockingAudio.clip != null)
        {
            knockDuration = knockingAudio.clip.length;
        }

        // Персонаж идёт к батарее.
        // Когда доходит — звучит стук.
        //// Затем персонаж возвращается.
        //yield return player.GoToAndReturn(
        //    batteryPoint,
        //    knockDuration,
        //    PlayKnocking
        //);

        float elapsed = Time.time - startTime;
        float remaining = walkingTimeCost - elapsed;

        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);

        if (Game == null || !Game.IsPlaying)
            yield break;

        walking = false;
        walkingRoutine = null;

        blackout.End(this);
        Game.SetWorkBlocked(this, false);

        Debug.Log("Игрок вернулся. Соседи затихли.");

        FinishIfPossible();
    }

    private void PlayKnocking()
    {
        if (knockingAudio != null &&
            knockingAudio.clip != null)
        {
            knockingAudio.Play();
        }
    }

    private void FinishIfPossible()
    {
        if (Game == null || !Game.IsPlaying)
            return;

        ResetNeighbour();

        Debug.Log(
            $"Соседи снова зашумят через {appearTimer:F1} секунд"
        );
    }

    private void StopWalking()
    {
        if (walkingRoutine != null)
        {
            StopCoroutine(walkingRoutine);
            walkingRoutine = null;
        }

        //if (player != null)
        //    player.ResetPosition();
    }
    public override void Resolve()
    {
        // Останавливаем поход к батарее
        StopWalking();

        ResetNeighbour();

    }
    public bool IsNeighbourWaiting => neighbourWaiting;
    public bool IsWalking => walking;
}

