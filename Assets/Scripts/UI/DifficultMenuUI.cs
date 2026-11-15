using TMPro;
using UnityEngine;

public class DifficultMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject difficultyPanel;

    public void OpenDifficulties() 
    {
        difficultyPanel.SetActive(true);
    }
    public void Back()
    {
        difficultyPanel.SetActive(false);
    }

}
