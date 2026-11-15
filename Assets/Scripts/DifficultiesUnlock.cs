using UnityEngine;

public class DifficultiesUnlock : MonoBehaviour
{
    private const string Key = "DifficultyUnlocked";

    public static bool IsUnlocked =>
        PlayerPrefs.GetInt(Key, 0) == 1;

    public static void Unlock()
    {
        if (IsUnlocked)
            return;

        PlayerPrefs.SetInt(Key, 1);
        PlayerPrefs.Save();
    }
}
