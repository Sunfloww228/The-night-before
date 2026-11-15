using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("Настройки баланса")]
    [SerializeField] private DifficultySettings settings;

    [Header("Уровни сложности (1–3)")]
    [SerializeField, Range(1, 3)] private int coffeeLevel = 1;
    [SerializeField, Range(1, 3)] private int phoneLevel = 1;
    [SerializeField, Range(1, 3)] private int courierLevel = 1;
    [SerializeField, Range(1, 3)] private int advertisementLevel = 1;
    [SerializeField, Range(1, 3)] private int neighbourLevel = 1;

    private const int DefaultLevel = 1;
    void Awake()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {

        if (!DifficultiesUnlock.IsUnlocked)
        {
            coffeeLevel = DefaultLevel;
            phoneLevel = DefaultLevel;
            courierLevel = DefaultLevel;
            advertisementLevel = DefaultLevel;
            neighbourLevel = DefaultLevel;

            return;
        }

        coffeeLevel = LoadLevel("Coffee");
        phoneLevel = LoadLevel("Phone");
        courierLevel = LoadLevel("Courier");
        advertisementLevel = LoadLevel("Advertisement");
        neighbourLevel = LoadLevel("Neighbour");
    }

    private int LoadLevel(string name)
    {
        int savedLevel = PlayerPrefs.GetInt(
            "Difficulty_" + name,
            DefaultLevel
        );

        return Mathf.Clamp(savedLevel, 1, 3);
    }
    private int SaveLevel(string name, int level)
    {
        level = Mathf.Clamp(level, 1, 3);

        PlayerPrefs.SetInt("Difficulty_" + name, level);
        PlayerPrefs.Save();

        return level;
    }

    private int GetIndex(int level)
    {
        return Mathf.Clamp(level, 1, 3) - 1;
    }

    public DifficultySettings.Coffee Coffee =>
        settings != null &&
        settings.coffee != null &&
        settings.coffee.Length > GetIndex(coffeeLevel)
            ? settings.coffee[GetIndex(coffeeLevel)]
            : null;

    public DifficultySettings.Phone Phone =>
        settings != null &&
        settings.phone != null &&
        settings.phone.Length > GetIndex(phoneLevel)
            ? settings.phone[GetIndex(phoneLevel)]
            : null;

    public DifficultySettings.Courier Courier =>
        settings != null &&
        settings.courier != null &&
        settings.courier.Length > GetIndex(courierLevel)
            ? settings.courier[GetIndex(courierLevel)]
            : null;

    public DifficultySettings.Advertisement Advertisement =>
        settings != null &&
        settings.advertisement != null &&
        settings.advertisement.Length > GetIndex(advertisementLevel)
            ? settings.advertisement[GetIndex(advertisementLevel)]
            : null;

    public DifficultySettings.Neighbour Neighbour =>
        settings != null &&
        settings.neighbour != null &&
        settings.neighbour.Length > GetIndex(neighbourLevel)
            ? settings.neighbour[GetIndex(neighbourLevel)]
            : null;

    public int CoffeeLevel => coffeeLevel;
    public int PhoneLevel => phoneLevel;
    public int CourierLevel => courierLevel;
    public int AdvertisementLevel => advertisementLevel;
    public int NeighbourLevel => neighbourLevel;

    public bool CanChangeDifficulty =>
        DifficultiesUnlock.IsUnlocked;

    public void SetCoffeeLevel(int level)
    {
        if (!CanChangeDifficulty) return;
        coffeeLevel = SaveLevel("Coffee", level);
    }

    public void SetPhoneLevel(int level)
    {
        if (!CanChangeDifficulty) return;
        phoneLevel = SaveLevel("Phone", level);
    }

    public void SetCourierLevel(int level)
    {
        if (!CanChangeDifficulty) return;
        courierLevel = SaveLevel("Courier", level);
    }

    public void SetAdvertisementLevel(int level)
    {
        if (!CanChangeDifficulty) return;
        advertisementLevel = SaveLevel("Advertisement", level);
    }

    public void SetNeighbourLevel(int level)
    {
        if (!CanChangeDifficulty) return;
        neighbourLevel = SaveLevel("Neighbour", level);
    }

    public void SetAllLevels(int level)
    {
        if (!CanChangeDifficulty) return;

        level = Mathf.Clamp(level, 1, 3);

        coffeeLevel = SaveLevel("Coffee", level);
        phoneLevel = SaveLevel("Phone", level);
        courierLevel = SaveLevel("Courier", level);
        advertisementLevel = SaveLevel("Advertisement", level);
        neighbourLevel = SaveLevel("Neighbour", level);
    }

    public void SetCoffeeFromDropdown(int index) => SetCoffeeLevel(index + 1);

    public void SetPhoneFromDropdown(int index) => SetPhoneLevel(index + 1);

    public void SetCourierFromDropdown(int index) => SetCourierLevel(index + 1);

    public void SetAdvertisementFromDropdown(int index) => SetAdvertisementLevel(index + 1);

    public void SetNeighbourFromDropdown(int index) => SetNeighbourLevel(index + 1);
}
