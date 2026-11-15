using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvertismentDistraction : DistractionBase
{
    [SerializeField] private AdvertisementSlot[] advertisements;

    [SerializeField, Min(0f)] private float minAppearTime = 15f;
    [SerializeField, Min(0f)] private float maxAppearTime = 35f;

    [SerializeField, Min(1)] private int minAds = 1;
    [SerializeField, Min(1)] private int maxAds = 3;

    [Header("Штраф")]
    [SerializeField, Min(0f)]
    private float focusLossPerSecond = 3f;

    private float appearTimer;
    private int openAdsCount;
    private IEnumerator Start()
    {
        ResetAdvertisements();

        yield return new WaitUntil(() => CanActivate());

        Activate();
    }

    public override void Tick(float dt)
    {
        if (openAdsCount > 0)
        {
            Game.ApplyResourceChanges(
                -focusLossPerSecond * dt,
                0f
            );

            return;
        }

        appearTimer -= dt;

        if (appearTimer <= 0f)
            ShowAdvertisements();
    }

    private void ShowAdvertisements()
    {
        if (advertisements == null || advertisements.Length == 0)
        {
            ScheduleNextAppearance();
            return;
        }

        List<AdvertisementSlot> available =
            new List<AdvertisementSlot>();

        foreach (AdvertisementSlot ad in advertisements)
        {
            if (ad != null &&
                !ad.IsShowing &&
                !available.Contains(ad))
            {
                available.Add(ad);
            }
        }

        if (available.Count == 0)
        {
            ScheduleNextAppearance();
            return;
        }

        for (int i = available.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            AdvertisementSlot temp = available[i];
            available[i] = available[randomIndex];
            available[randomIndex] = temp;
        }
        int minimum = Mathf.Clamp(
            minAds, 1, available.Count
        );

        int maximum = Mathf.Clamp(
            maxAds, minimum, available.Count
        );

        int count = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < count; i++)
        {
            available[i].Show();

            if (available[i].IsShowing)
                openAdsCount++;
        }

        Debug.Log($"Появилось реклам: {openAdsCount}");
        if (openAdsCount == 0)
            ScheduleNextAppearance();
    }

    public void CloseAdvertisement(AdvertisementSlot ad)
    {
        if (openAdsCount <= 0)
            return;

        openAdsCount--;

        if (openAdsCount == 0)
        {
            openAdsCount = 0;
            ScheduleNextAppearance();

            Debug.Log("Вся реклама закрыта.");
        }
    }

    private void ScheduleNextAppearance()
    {
        appearTimer = Random.Range(
            Mathf.Min(minAppearTime, maxAppearTime),
            Mathf.Max(minAppearTime, maxAppearTime)
        );
    }

    private void ResetAdvertisements()
    {
        openAdsCount = 0;

        if (advertisements != null)
        {
            foreach (AdvertisementSlot slot in advertisements)
            {
                if (slot != null)
                    slot.Hide();
            }
        }

        ScheduleNextAppearance();
    }
    public override void Resolve()
    {
        ResetAdvertisements();
    }
}
