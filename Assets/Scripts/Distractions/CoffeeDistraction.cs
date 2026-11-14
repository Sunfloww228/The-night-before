using System.Collections.Generic;
using UnityEngine;

public sealed class CoffeeDistraction : DistractionBase
{
    [SerializeField] private GameObject machinePanel;
    [SerializeField] private GameObject fullCup;
    [SerializeField, Min(0)] private float brewTime = 2f;
    [SerializeField, Min(0)] private float coolTime = 15f;
    [SerializeField, Min(0)] private float crashDelay = 20f;
    [SerializeField, Min(0)] private float hotReward = 15f;
    [SerializeField, Min(0)] private float coolReward = 5f;
    [SerializeField, Min(0)] private float crashCost = 10f;

    private readonly List<float> crashDeadlines = new List<float>();
    private bool machineOpen;
    private bool groundsAdded;
    private bool holding;
    private bool cupReady;
    private float brewElapsed;
    private float cooledAt;

    public override void Activate()
    {
        if (!CanActivate() || cupReady || machineOpen)
            return;

        machineOpen = true;
        groundsAdded = false;
        base.Activate();
        if (machinePanel != null)
            machinePanel.SetActive(true);
    }

    public void AddGrounds()
    {
        if (machineOpen)
            groundsAdded = true;
    }

    public void BeginBrew()
    {
        if (!machineOpen || !groundsAdded || holding)
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

    public void CloseMachine()
    {
        machineOpen = false;
        EndBrew();
        if (machinePanel != null)
            machinePanel.SetActive(false);

        ResolveIfIdle();
    }

    public void Drink()
    {
        if (!cupReady || !Game.IsPlaying)
            return;

        bool hot = Game.TimeLeft > cooledAt;
        cupReady = false;
        if (fullCup != null)
            fullCup.SetActive(false);
        if (hot)
            crashDeadlines.Add(Game.TimeLeft - crashDelay);

        Game.ApplyResourceChanges(hot ? hotReward : coolReward, 0f);
        ResolveIfIdle();
    }

    public override void Tick(float dt)
    {
        for (int i = crashDeadlines.Count - 1; i >= 0; i--)
        {
            if (Game.TimeLeft > crashDeadlines[i])
                continue;

            crashDeadlines.RemoveAt(i);
            Game.ApplyResourceChanges(-crashCost, 0f);
            if (!IsActive)
                return;
        }

        if (holding)
        {
            brewElapsed += dt;
            if (brewElapsed >= brewTime)
            {
                cupReady = true;
                cooledAt = Game.TimeLeft - coolTime;
                CloseMachine();
                if (fullCup != null)
                    fullCup.SetActive(true);
            }
        }

        ResolveIfIdle();
    }

    public override void Resolve()
    {
        machineOpen = false;
        cupReady = false;
        crashDeadlines.Clear();
        EndBrew();
        if (machinePanel != null)
            machinePanel.SetActive(false);
        if (fullCup != null)
            fullCup.SetActive(false);

        base.Resolve();
    }

    private void ResolveIfIdle()
    {
        if (IsActive && !machineOpen && !cupReady && crashDeadlines.Count == 0)
            Resolve();
    }
}
