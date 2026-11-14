using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Slider focusSlider;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Slider stressSlider;

    [SerializeField] private TMP_Text  timerText;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;

    private void OnEnable()
    {
        gameManager.State.Changed += UpdateUI;
        gameManager.SessionEnded += ShowResult;
        UpdateUI(gameManager.State);

        resultPanel.SetActive(
            gameManager.State.Status != GameStatus.Running
            && gameManager.State.Status != GameStatus.Idle
        );
    }
    private void OnDisable()
    {
        gameManager.State.Changed -= UpdateUI;
        gameManager.SessionEnded -= ShowResult;
    }

    void UpdateUI(GameState state)
    {
        focusSlider.value = state.Focus;
        progressSlider.value = state.Progress;
        stressSlider.value = state.Stress;

        int seconds = Mathf.CeilToInt(state.RemainingTime);
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;

        timerText.text = $"{minutes:00}:{remainingSeconds:00}";
    }

    private void ShowResult(GameStatus status) 
    {
        resultPanel.SetActive(true);

        switch (status)
        {
            case GameStatus.Won:
                resultText.text = " ”–—Œ¬¿ﬂ √Œ“Œ¬¿!";
                break;

            case GameStatus.LostFocus:
                resultText.text = "“€ œŒ“≈–ﬂÀ  ŒÕ÷≈Õ“–¿÷»ﬁ!";
                break;

            case GameStatus.DeadlineExpired:
                resultText.text = "¬–≈Ãﬂ ¬€ÿÀŒ!";
                break;
        }
    }
    public void Restart()
    {
        resultPanel.SetActive(false);
        gameManager.RestartGame();
    }
}
