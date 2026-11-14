using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CoffeeDistraction : DistractionBase
{
    [Header("UI")]
    [SerializeField] private GameObject coffeeGroundsPanel;
    [SerializeField] private GameObject coffeeMakingPanel;
    [SerializeField] private Button takeCupButton;

    [Header("Cup")]
    [SerializeField] private Image machineCupImage;
    [SerializeField] private Sprite machineEmptyCupSprite;
    [SerializeField] private Sprite machineFullCupSprite;

    [SerializeField] private SpriteRenderer tableCupRenderer;
    [SerializeField] private Sprite tableEmptyCupSprite;
    [SerializeField] private Sprite tableFullCupSprite;

    [Header("Time")]
    [SerializeField, Min(0)] private float brewTime = 2f;
    [SerializeField, Min(0)] private float coolTime = 15f;
    [SerializeField, Min(0)] private float crashDelay = 20f;

    [Header("Effects")]
    [SerializeField, Min(0)] private float hotReward = 15f;
    [SerializeField, Min(0)] private float coolReward = 5f;
    [SerializeField, Min(0)] private float crashFocusCost = 10f;

    //[SerializeField] private Animator groundsAnimator;


    private readonly List<float> crashDeadlines = new List<float>();
    private bool machineOpen;
    private bool groundsAdded;
    private bool holding;
    private bool cupReady;
    private float brewElapsed;
    private float cooledAt;
    private bool cupOnTable;

    //private bool groundsAnimating;

    public void Interact()
    {
        if (!CanActivate())
            return;

        if (machineOpen)
            return;

        if (cupOnTable) 
        {
            Debug.Log("Пьем!");
            Drink(); 
        } 

        else
        {
            Debug.Log("Завариваем кофе");
            Activate();
        }

    }


    public override void Activate()
    {
        if (!CanActivate() || cupReady || cupOnTable || machineOpen)
            return;

        machineOpen = true;
        groundsAdded = false;
        holding = false;
        brewElapsed = 0f;

        if (!IsActive)
            base.Activate();
        if (coffeeGroundsPanel != null)
            coffeeGroundsPanel.SetActive(true);
        if (coffeeMakingPanel != null)
            coffeeMakingPanel.SetActive(false);
        if (takeCupButton != null)
            takeCupButton.interactable = false;
        if (machineCupImage != null)
            machineCupImage.sprite = machineEmptyCupSprite;
    }


    public void AddGrounds()  //мешочек с кофе
    {
        if (!machineOpen || groundsAdded)
            return;
        groundsAdded = true;
        //анимация        насыпания

        //groundsAnimating = true;
        //groundsAnimator.SetTrigger("PourGrounds");
        coffeeGroundsPanel.SetActive(false);
        coffeeMakingPanel.SetActive(true);
    }
    //public void FinishAddingGrounds()
    //{
    //    if (!machineOpen || !groundsAnimating)
    //        return;

    //    groundsAnimating = false;
    //    groundsAdded = true;

    //    coffeeGroundsPanel.SetActive(false);
    //    coffeeMakingPanel.SetActive(true);
    //}

    public void BeginBrew()    //кнопка включить машину
    {
        if (!machineOpen || !groundsAdded || holding || cupReady)
            return;

        holding = true;
        brewElapsed = 0f;
        Game.SetWorkBlocked(this, true);
    }

    public void EndBrew()
    {
        holding = false;
        brewElapsed = 0f;
        if (Game != null)
            Game.SetWorkBlocked(this, false);
    }

    public void TakeCup()
    {
        if (!machineOpen || !cupReady)
            return;

        cupReady = false;
        cupOnTable = true;
        machineOpen = false;

        if (coffeeMakingPanel != null)
            coffeeMakingPanel.SetActive(false);

        if (coffeeGroundsPanel != null)
            coffeeGroundsPanel.SetActive(false);

        UpdateTableCup();

        ResolveIfIdle();
    }

    public void CloseMachine()
    {
        Debug.LogWarning("CloseMachine вызван\n" + System.Environment.StackTrace);
        machineOpen = false;
        groundsAdded = false;
        cupReady = false;

        EndBrew();
        if (coffeeMakingPanel != null)
            coffeeMakingPanel.SetActive(false);
        if (coffeeGroundsPanel != null)
            coffeeGroundsPanel.SetActive(false);

        if (takeCupButton != null)
            takeCupButton.interactable = false;
        ResolveIfIdle();
    }

    public void Drink()
    {
        if (!cupOnTable || Game == null || !Game.IsPlaying)
            return;

        bool hot = Game.TimeLeft > cooledAt;

        cupOnTable = false;
        UpdateTableCup();

        if (hot)
            crashDeadlines.Add(Game.TimeLeft - crashDelay);

        Game.ApplyResourceChanges(
            hot ? hotReward : coolReward,
            0f
        );

        ResolveIfIdle();
    }

    public override void Tick(float dt)
    {
        for (int i = crashDeadlines.Count - 1; i >= 0; i--)
        {
            if (Game.TimeLeft > crashDeadlines[i])
                continue;

            crashDeadlines.RemoveAt(i);
            Game.ApplyResourceChanges(-crashFocusCost, 0f);
            if (!IsActive)
                return;
        }

        if (holding)
        {
            brewElapsed += dt;
            if (brewElapsed >= brewTime)
            {
                EndBrew();
                cupReady = true;
                cooledAt = Game.TimeLeft - coolTime;

                if (machineCupImage != null)
                    machineCupImage.sprite = machineFullCupSprite;

                if (takeCupButton != null)
                    takeCupButton.interactable = true;
                Debug.Log("Кофе готов! Можно забрать чашку.");
            }
        }

        ResolveIfIdle();
    }

    public override void Resolve()
    {
        Debug.LogWarning("Resolve вызван\n" + System.Environment.StackTrace);
        machineOpen = false;
        cupReady = false;
        cupOnTable = false;
        crashDeadlines.Clear();
        EndBrew();
        if (coffeeMakingPanel != null)
            coffeeMakingPanel.SetActive(false);
        if (coffeeGroundsPanel != null)
            coffeeGroundsPanel.SetActive(false);
        if (takeCupButton != null)
            takeCupButton.interactable = false;
        UpdateTableCup();
        base.Resolve();
    }

    private void ResolveIfIdle()
    {
        if (IsActive && !machineOpen && !cupReady && !cupOnTable && crashDeadlines.Count == 0)
            Resolve();
    }
    private void UpdateTableCup()
    {
        if (tableCupRenderer == null)
            return;

        tableCupRenderer.sprite = cupOnTable
            ? tableFullCupSprite
            : tableEmptyCupSprite;
    }

    public bool IsMachineOpen => machineOpen;
    public bool IsCupFull => cupOnTable;
}
