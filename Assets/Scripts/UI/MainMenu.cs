using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject difficultyPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
    public void OpenDifficulties()
    {
        difficultyPanel.SetActive(true);
        mainPanel.SetActive(false);
    }
    public void Back()
    {
        difficultyPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
