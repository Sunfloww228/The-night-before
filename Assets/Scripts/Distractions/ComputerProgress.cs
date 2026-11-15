using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class ComputerProgress : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Sprite[] progressFrames;

    private SpriteRenderer monitor;

    private void Awake()
    {
        monitor = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        gameManager.State.Changed += Refresh;
        Refresh(gameManager.State);
    }

    private void OnDisable()
    {
        gameManager.State.Changed -= Refresh;
    }

    private void Refresh(GameState state)
    {
        float fraction = state.Progress / GameState.ResourceMax;
        int frame = Mathf.FloorToInt(fraction * (progressFrames.Length - 1));
        Sprite sprite = progressFrames[frame];
        if (monitor.sprite != sprite)
            monitor.sprite = sprite;
    }
}
