using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenu : MonoBehaviour
{
    private const string VolumeKey = "MasterVolume";
    private static readonly Color Background = new Color(0.07f, 0.08f, 0.11f);
    private static readonly Color ButtonColor = new Color(0.20f, 0.28f, 0.42f);

    private Font font;
    private GameObject mainPanel;
    private GameObject settingsPanel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadVolume()
    {
        AudioListener.volume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, 1f));
    }

    private void Awake()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;
        gameObject.AddComponent<GraphicRaycaster>();

        if (EventSystem.current == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        Image backdrop = gameObject.GetComponent<Image>();
        if (backdrop == null)
            backdrop = gameObject.AddComponent<Image>();
        backdrop.color = Background;

        mainPanel = Element("Main", transform, Vector2.zero, Vector2.zero).gameObject;
        settingsPanel = Element("Settings", transform, Vector2.zero, Vector2.zero).gameObject;
        Label(mainPanel.transform, "The Night Before", 105f, 44);
        AddButton(mainPanel.transform, "Начать", 15f, () => SceneManager.LoadScene("Game"));
        AddButton(mainPanel.transform, "Настройки", -55f, () => ShowSettings(true));

        Label(settingsPanel.transform, "Настройки", 105f, 38);
        Text volume = Label(settingsPanel.transform, VolumeText(AudioListener.volume), 42f, 24);
        AddVolumeSlider(settingsPanel.transform, volume);
        AddButton(settingsPanel.transform, "Назад", -65f, () => ShowSettings(false));
        ShowSettings(false);
    }

    private void ShowSettings(bool show)
    {
        mainPanel.SetActive(!show);
        settingsPanel.SetActive(show);
    }

    private void AddVolumeSlider(Transform parent, Text label)
    {
        RectTransform rect = Element("Volume", parent, Vector2.zero, new Vector2(280f, 36f));
        rect.gameObject.AddComponent<Image>().color = ButtonColor;
        RectTransform handleArea = Element("Handle Area", rect, Vector2.zero, new Vector2(256f, 36f));
        RectTransform handle = Element("Handle", handleArea, Vector2.zero, new Vector2(20f, 32f));
        Image handleImage = handle.gameObject.AddComponent<Image>();
        handleImage.color = Color.white;

        Slider slider = rect.gameObject.AddComponent<Slider>();
        slider.handleRect = handle;
        slider.targetGraphic = handleImage;
        slider.SetValueWithoutNotify(AudioListener.volume);
        slider.onValueChanged.AddListener(value =>
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat(VolumeKey, value);
            PlayerPrefs.Save();
            label.text = VolumeText(value);
        });
    }

    private void AddButton(Transform parent, string title, float y, UnityEngine.Events.UnityAction onClick)
    {
        RectTransform rect = Element(title, parent, new Vector2(0f, y), new Vector2(280f, 54f));
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = ButtonColor;
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);
        Label(rect, title, 0f, 24);
    }

    private Text Label(Transform parent, string title, float y, int size)
    {
        RectTransform rect = Element(title, parent, new Vector2(0f, y), new Vector2(480f, 62f));
        Text text = rect.gameObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        text.text = title;
        return text;
    }

    private static RectTransform Element(string title, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject child = new GameObject(title, typeof(RectTransform));
        RectTransform rect = (RectTransform)child.transform;
        rect.SetParent(parent, false);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static string VolumeText(float value) => $"Громкость: {Mathf.RoundToInt(value * 100f)}%";
}
