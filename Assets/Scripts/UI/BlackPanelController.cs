using UnityEngine;

public class BlackPanelController : MonoBehaviour
{
    [SerializeField] private GameObject blackoutPanel;
    [SerializeField] private GameManager gameManager;

    private MonoBehaviour currentOwner;

    public bool IsBusy => currentOwner != null;

    private void Awake()
    {
        if (blackoutPanel != null)
            blackoutPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (gameManager != null)
            gameManager.SessionRestarting += ResetBlackout;
    }

    private void OnDisable()
    {
        if (gameManager != null)
            gameManager.SessionRestarting -= ResetBlackout;
    }

    public bool TryBegin(MonoBehaviour owner)
    {
        if (owner == null || IsBusy)
            return false;

        if (gameManager != null && !gameManager.IsPlaying)
            return false;

        currentOwner = owner;

        if (blackoutPanel != null)
            blackoutPanel.SetActive(true);

        return true;
    }

    public void End(MonoBehaviour owner)
    {
        if (currentOwner != owner)
            return;

        currentOwner = null;

        if (blackoutPanel != null)
            blackoutPanel.SetActive(false);
    }

    public void ResetBlackout()
    {
        currentOwner = null;

        if (blackoutPanel != null)
            blackoutPanel.SetActive(false);
    }
}
