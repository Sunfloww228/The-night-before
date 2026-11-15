using System.Collections.Generic;
using UnityEngine;
public class InteractionBlocker : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private readonly HashSet<MonoBehaviour> blockers = new HashSet<MonoBehaviour>();

    public bool IsBlocked => (gameManager != null && !gameManager.IsPlaying) || blockers.Count > 0;

    public void Block(MonoBehaviour source) 
    {
        if (source == null) return;
        blockers.Add(source);
    }

    public void Unblock(MonoBehaviour source) 
    {
        if (source == null) return;
        blockers.Remove(source);
    }

    private void OnEnable()
    {
        if (gameManager != null)
            gameManager.SessionRestarting += ResetBlocks;
    }

    private void OnDisable()
    {
        if (gameManager != null)
            gameManager.SessionRestarting -= ResetBlocks;
    }

    private void ResetBlocks()
    {
        blockers.Clear();
    }
}
