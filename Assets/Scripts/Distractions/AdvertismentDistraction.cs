using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdvertisementSpritePair
{
    public Sprite normal;
    public Sprite active;
}
public class AdvertismentDistraction : DistractionBase
{
    [SerializeField] private AdvertisementSpritePair[] advertisements;
    [SerializeField] private AdvertisementSlot[] adSlots;

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
        if (advertisements == null || advertisements.Length == 0 || adSlots == null || adSlots.Length == 0)
        {
            ScheduleNextAppearance();
            return;
        }

        List<int> availableSlots = new List<int>();

        for (int i = 0; i < adSlots.Length; i++)
        {
            if (adSlots[i] != null &&
                !adSlots[i].IsOccupied)
            {
                availableSlots.Add(i);
            }
        }

        if (availableSlots.Count == 0)
        {
            ScheduleNextAppearance();
            return;
        }

        // Перемешиваем свободные слоты.
        for (int i = availableSlots.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            int temp = availableSlots[i];
            availableSlots[i] = availableSlots[randomIndex];
            availableSlots[randomIndex] = temp;
        }

        // Случайно выбираем количество рекламы.
        int minimum = Mathf.Clamp(
            minAds, 1, availableSlots.Count
        );

        int maximum = Mathf.Clamp(
            maxAds, minimum, availableSlots.Count
        );

        int count = Random.Range(minimum, maximum + 1);

        // Заполняем случайные слоты
        // случайными рекламными изображениями.
        for (int i = 0; i < count; i++)
        {
            int slotIndex = availableSlots[i];

            AdvertisementSpritePair randomAd =
                advertisements[Random.Range(
                    0, advertisements.Length
                )];

            adSlots[slotIndex].SetAdvertisement(
                randomAd.normal,
                randomAd.active
            );

            openAdsCount++;
        }

        Debug.Log($"Появилось реклам: {openAdsCount}");
    }

    public void CloseAdvertisement(int slotIndex)
    {
        if (adSlots == null ||
            slotIndex < 0 ||
            slotIndex >= adSlots.Length)
            return;

        AdvertisementSlot slot = adSlots[slotIndex];

        if (slot == null || !slot.IsOccupied)
            return;

        // Очищаем слот, не удаляя объект.
        slot.ClearAdvertisement();

        openAdsCount--;

        if (openAdsCount <= 0)
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

        if (adSlots != null)
        {
            foreach (AdvertisementSlot slot in adSlots)
            {
                if (slot != null)
                    slot.ClearAdvertisement();
            }
        }

        ScheduleNextAppearance();
    }
    public override void Resolve()
    {
        ResetAdvertisements();
    }

    public bool IsSlotOccupied(int index)
    {
        return adSlots != null && index >= 0 && index < adSlots.Length && adSlots[index] != null &&  adSlots[index].IsOccupied;
    }
}
