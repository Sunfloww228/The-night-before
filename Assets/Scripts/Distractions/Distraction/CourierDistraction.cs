using UnityEngine;
using System.Collections;

public class CourierDistraction : DistractionBase
{
    [SerializeField] private GameObject bellIcon;
    [SerializeField] private AudioSource bellAudio;

    [SerializeField] private float minAppearTime = 20f;
    [SerializeField] private float maxAppearTime = 50f;
    [SerializeField] private float waitTime = 20f;

    [SerializeField] private float normalRingInterval = 5f;
    [SerializeField] private float urgentRingInterval = 2f;
    [SerializeField] private float urgentAfter = 10f;

    [SerializeField] private Student student;

    [SerializeField] private GameObject pizzaObject;

    [Header("эффекты")]
    [SerializeField] private float ringingFocusLoss = 3f;
    [SerializeField] private float pizzaFocusReward = 25f;
    [SerializeField] private float pizzaStressReduction = 25f;
    [SerializeField] private float missedCourierFocusPenalty = 20f;
    [SerializeField] private float missedCourierStressPenalty = 20f;

    [Header("сходить за пиццей время")]
    [SerializeField] private float deliveryTimeCost = 5f;
    [SerializeField] private BlackPanelController blackout;
    [SerializeField] private InteractionBlocker interactionBlocker;

    private float appearTimer;
    private float waitTimer;
    private float ringTimer;

    private bool courierWaiting;
    private bool courierFinished;
    private bool pizzaReady;
    private bool delivering;

    private Coroutine deliveryRoutine;
    private IEnumerator Start()
    {
        ResetCourier();
        yield return new WaitUntil(() => CanActivate());
        Activate();

    }

    private void ResetCourier() 
    {
        appearTimer = Random.Range(minAppearTime, maxAppearTime);

        waitTimer = 0;
        ringTimer = 0;

        courierWaiting = false;
        courierFinished = false;
        pizzaReady = false;
        delivering = false;

        if (bellIcon != null)
            bellIcon.SetActive(false);

        if (pizzaObject != null)
            pizzaObject.SetActive(false);

        if (bellAudio != null)
            bellAudio.Stop();

        if (blackout != null)
            blackout.End(this);

        if (interactionBlocker != null)
            interactionBlocker.Unblock(this);

        if (Game != null)
            Game.SetWorkBlocked(this, false);
    }

    public override void Tick(float dt)
    {
        if (courierFinished || delivering)
            return;

        if (!courierWaiting)
        {
            appearTimer -= dt;

            if (appearTimer <= 0f)
                Arrive();

            return;
        }

        Game.ApplyResourceChanges(-ringingFocusLoss * dt, 0f);

        if (!Game.IsPlaying)
            return;

        waitTimer += dt;
        ringTimer -= dt;

        if (ringTimer <= 0f)
        {
            Ring();

            ringTimer = waitTimer >= urgentAfter
                ? urgentRingInterval
                : normalRingInterval;
        }

        if (waitTimer >= waitTime)
            Leave();
    }

    private void Arrive()
    {
        courierWaiting = true;
        waitTimer = 0f;
        ringTimer = 0f;

        if (bellIcon != null) 
        {
            bellIcon.SetActive(true);
        }

        Ring();
        ringTimer = normalRingInterval;
        Debug.Log("Курьер пришёл!");
    }

    private void Ring() 
    {
        if (bellAudio == null || bellAudio.clip == null)
            return;
        Debug.Log(
           $"AudioSource enabled: {bellAudio.enabled}, " +
           $"Active: {bellAudio.gameObject.activeInHierarchy}, " +
           $"Volume: {bellAudio.volume}, " +
           $"Clip: {bellAudio.clip.name}"
        );
        Debug.Log($"Master Volume: {AudioListener.volume}");
        Debug.Log($"Audio Paused: {AudioListener.pause}");

        bellAudio.Play();

        Debug.Log($"Is Playing: {bellAudio.isPlaying}");
    }

    private void Leave() 
    {
        courierWaiting = false;
        courierFinished = true;

        if (bellIcon != null) 
        {
            bellIcon.SetActive(false);
        }
        if (bellAudio != null)
            bellAudio.Stop();

        Game.ApplyResourceChanges(-missedCourierFocusPenalty, missedCourierStressPenalty);

        FinishIfPossible();
        Debug.Log("Курьер ушёл.");
    }

    public void AnswerDoor() 
    {
        if (!courierWaiting || delivering || Game == null || !Game.IsPlaying)
            return;

        if (interactionBlocker == null || interactionBlocker.IsBlocked || blackout == null || !blackout.TryBegin(this))
            return;

        interactionBlocker.Block(this);

        if (student != null)
            student.PlayCourier();

        courierWaiting = false;
        delivering = true;

        if (bellIcon != null) 
        {
            bellIcon.SetActive(false);
        }
        if (bellAudio != null)
        {
            bellAudio.Stop();
        }

        Game.SetWorkBlocked(this, true);


        deliveryRoutine = StartCoroutine(DeliverySequence());
    }

    private IEnumerator DeliverySequence()
    {
        if (student != null)
            student.PlayCourier();

        yield return new WaitForSeconds(deliveryTimeCost);

        if (Game == null || !Game.IsPlaying)
            yield break;

        if (student != null)
            student.PlaySitting();

        pizzaReady = true;
        delivering = false;
        courierFinished = true;
        deliveryRoutine = null;

        if (pizzaObject != null)
            pizzaObject.SetActive(true);
        if (blackout != null)
            blackout.End(this);
        if (interactionBlocker != null)
            interactionBlocker.Unblock(this);
        Game.SetWorkBlocked(this, false);
        Debug.Log("Игрок получил пиццу!");
    }

    public void EatPizza() 
    {
        if (!pizzaReady)
            return;

        pizzaReady=false;

        if (pizzaObject != null) 
        {
            pizzaObject.SetActive(false);
        }

        Game.ApplyResourceChanges(pizzaFocusReward, -pizzaStressReduction);
        Debug.Log("Игрок съел пиццу!");
        FinishIfPossible();
    }

    public override void Resolve()
    {
        if (deliveryRoutine != null)
        {
            StopCoroutine(deliveryRoutine);
            deliveryRoutine = null;
        }

        //if (player != null)
        //    player.ResetPosition();

        courierWaiting = false;
        delivering = false;
        pizzaReady = false;

        if (bellIcon != null)
            bellIcon.SetActive(false);

        if (pizzaObject != null)
            pizzaObject.SetActive(false);

        if (blackout != null)
            blackout.End(this);

        if (interactionBlocker != null)
            interactionBlocker.Unblock(this);

        if (Game != null)
            Game.SetWorkBlocked(this, false);

        ResetCourier();
    }

    private void FinishIfPossible()
    {
        if (!courierFinished ||
            courierWaiting ||
            delivering ||
            pizzaReady)
            return;

        ResetCourier();

        Debug.Log(
            $"Следующий курьер через {appearTimer:F1} секунд"
        );
    }

    public bool IsCourierWaiting => courierWaiting;
    public bool IsPizzaReady => pizzaReady;

}
