using UnityEngine;
using UnityEngine.UI;

public sealed class MenuSettings : MonoBehaviour
{
    private const string VolumeKey = "MasterVolume";
    private const string SensitivityKey = "MouseSensitivity";

    private const float DefaultVolume = 1f;
    private const float DefaultSensitivity = 1f;
    private const float MinSensitivity = 0.5f;
    private const float MaxSensitivity = 2.5f;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;

    public static float MouseSensitivity { get; private set; } = 1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadSettings()
    {
        AudioListener.volume = Mathf.Clamp01(
            PlayerPrefs.GetFloat(VolumeKey, DefaultVolume)
        );

        MouseSensitivity = Mathf.Clamp(
            PlayerPrefs.GetFloat(SensitivityKey, DefaultSensitivity),
            MinSensitivity,
            MaxSensitivity
        );
    }

    private void OnEnable()
    {
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(AudioListener.volume);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        if (sensitivitySlider != null)
        {
            sensitivitySlider.SetValueWithoutNotify(MouseSensitivity);
            sensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        }
    }

    private void OnDisable()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(SetVolume);

        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.RemoveListener(SetMouseSensitivity);
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

        Debug.Log(
            $"Настройки сохранены: " +
            $"громкость = {AudioListener.volume}, " +
            $"чувствительность = {MouseSensitivity}"
        );
    }
}
