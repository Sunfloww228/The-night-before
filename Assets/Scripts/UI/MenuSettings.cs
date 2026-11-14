using UnityEngine;
using UnityEngine.UI;

public sealed class MenuSettings : MonoBehaviour
{
    private const string VolumeKey = "MasterVolume";
    private const string SensitivityKey = "MouseSensitivity";

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;

    public static float MouseSensitivity { get; private set; } = 1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadSettings()
    {
        AudioListener.volume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, 1f));
        MouseSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat(SensitivityKey, 1f), 0.5f, 2f);
    }

    private void OnEnable()
    {
        if (volumeSlider != null)
            volumeSlider.SetValueWithoutNotify(AudioListener.volume);
        if (sensitivitySlider != null)
            sensitivitySlider.SetValueWithoutNotify(MouseSensitivity);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
    }

    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = Mathf.Clamp(value, 0.5f, 2.5f);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(VolumeKey, AudioListener.volume);
        PlayerPrefs.SetFloat(SensitivityKey, MouseSensitivity);
        PlayerPrefs.Save();

        Debug.Log("Настройки сохранены!");
    }
}
