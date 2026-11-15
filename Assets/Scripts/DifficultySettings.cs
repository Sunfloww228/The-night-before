using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficulty", menuName = "Game/Difficulty Settings")]
public class DifficultySettings : ScriptableObject
{
    [System.Serializable]
    public class Coffee
    {
        [Header("Время")]
        public float brewTime = 2f;
        public float coolTime = 15f;
        public float crashDelay = 20f;

        [Header("Эффекты")]
        public float hotReward = 20f;
        public float coolReward = 8f;
        public float crashFocusCost = 10f;
    }

    [System.Serializable]
    public class Phone
    {
        [Header("Мемы")]
        public int minMemes = 2;
        public int maxMemes = 4;

        [Header("Время")]
        public float swipeCooldown = 1f;
        public float watchTime = 2f;
        public float stuckAfter = 8f;

        [Header("Эффекты")]
        public float focusPerSecond = 1f;
        public float stressReward = 20f;
    }

    [System.Serializable]
    public class Courier
    {
        [Header("Появление")]
        public float minAppearTime = 20f;
        public float maxAppearTime = 35f;
        public float waitTime = 20f;

        [Header("Звонок")]
        public float normalRingInterval = 5f;
        public float urgentRingInterval = 1.5f;
        public float urgentAfter = 10f;

        [Header("Эффекты")]
        public float ringingFocusLoss = 2.5f;
        public float pizzaFocusReward = 25f;
        public float pizzaStressReduction = 20f;
        public float missedCourierFocusPenalty = 15f;
        public float missedCourierStressPenalty = 15f;
    }

    [System.Serializable]
    public class Advertisement
    {
        [Header("Появление")]
        public float minAppearTime = 25f;
        public float maxAppearTime = 40f;

        [Header("Количество рекламы")]
        public int minAds = 1;
        public int maxAds = 3;

        [Header("Эффекты")]
        public float focusLossPerSecond = 2f;
    }

    [System.Serializable]
    public class Neighbour
    {
        [Header("Появление")]
        public float minAppearTime = 30f;
        public float maxAppearTime = 45f;
        public float waitTime = 15f;

        [Header("Звуки")]
        public float normalAudioInterval = 5f;

        [Header("Эффекты")]
        public float focusLoss = 1f;
        public float stressPenalty = 1f;
    }

    [Header("Кофе: 1 / 2 / 3")]
    public Coffee[] coffee =
    {
        new Coffee
        {
            brewTime = 2f,
            coolTime = 20f,
            crashDelay = 25f,
            hotReward = 25f,
            coolReward = 12f,
            crashFocusCost = 5f
        },
        new Coffee
        {
            brewTime = 2f,
            coolTime = 15f,
            crashDelay = 20f,
            hotReward = 20f,
            coolReward = 8f,
            crashFocusCost = 10f
        },
        new Coffee
        {
            brewTime = 2f,
            coolTime = 10f,
            crashDelay = 15f,
            hotReward = 15f,
            coolReward = 5f,
            crashFocusCost = 15f
        }
    };

    [Header("Телефон: 1 / 2 / 3")]
    public Phone[] phone =
    {
        new Phone
        {
            minMemes = 2,
            maxMemes = 3,
            swipeCooldown = 1f,
            watchTime = 2f,
            stuckAfter = 12f,
            focusPerSecond = 1f,
            stressReward = 25f
        },
        new Phone
        {
            minMemes = 2,
            maxMemes = 4,
            swipeCooldown = 1f,
            watchTime = 2f,
            stuckAfter = 8f,
            focusPerSecond = 1f,
            stressReward = 20f
        },
        new Phone
        {
            minMemes = 3,
            maxMemes = 5,
            swipeCooldown = 1f,
            watchTime = 2f,
            stuckAfter = 6f,
            focusPerSecond = 1f,
            stressReward = 15f
        }
    };

    [Header("Курьер: 1 / 2 / 3")]
    public Courier[] courier =
    {
        new Courier
        {
            minAppearTime = 12f,
            maxAppearTime = 22f,
            waitTime = 25f,
            normalRingInterval = 6f,
            urgentRingInterval = 2.5f,
            urgentAfter = 15f,
            ringingFocusLoss = 1.5f,
            pizzaFocusReward = 35f,
            pizzaStressReduction = 25f,
            missedCourierFocusPenalty = 10f,
            missedCourierStressPenalty = 10f
        },
        new Courier
        {
            minAppearTime = 20f,
            maxAppearTime = 35f,
            waitTime = 20f,
            normalRingInterval = 5f,
            urgentRingInterval = 1.5f,
            urgentAfter = 10f,
            ringingFocusLoss = 2.5f,
            pizzaFocusReward = 25f,
            pizzaStressReduction = 20f,
            missedCourierFocusPenalty = 15f,
            missedCourierStressPenalty = 15f
        },
        new Courier
        {
            minAppearTime = 35f,
            maxAppearTime = 50f,
            waitTime = 15f,
            normalRingInterval = 4f,
            urgentRingInterval = 1f,
            urgentAfter = 7f,
            ringingFocusLoss = 3f,
            pizzaFocusReward = 20f,
            pizzaStressReduction = 15f,
            missedCourierFocusPenalty = 20f,
            missedCourierStressPenalty = 20f
        }
    };

    [Header("Реклама: 1 / 2 / 3")]
    public Advertisement[] advertisement =
    {
        new Advertisement
        {
            minAppearTime = 35f,
            maxAppearTime = 50f,
            minAds = 1,
            maxAds = 2,
            focusLossPerSecond = 1f
        },
        new Advertisement
        {
            minAppearTime = 25f,
            maxAppearTime = 40f,
            minAds = 1,
            maxAds = 3,
            focusLossPerSecond = 2f
        },
        new Advertisement
        {
            minAppearTime = 15f,
            maxAppearTime = 25f,
            minAds = 2,
            maxAds = 3,
            focusLossPerSecond = 3f
        }
    };

    [Header("Соседи: 1 / 2 / 3")]
    public Neighbour[] neighbour =
    {
        new Neighbour
        {
            minAppearTime = 40f,
            maxAppearTime = 60f,
            waitTime = 12f,
            normalAudioInterval = 7f,
            focusLoss = 0.5f,
            stressPenalty = 0.5f
        },
        new Neighbour
        {
            minAppearTime = 30f,
            maxAppearTime = 45f,
            waitTime = 15f,
            normalAudioInterval = 5f,
            focusLoss = 1f,
            stressPenalty = 1f
        },
        new Neighbour
        {
            minAppearTime = 20f,
            maxAppearTime = 30f,
            waitTime = 20f,
            normalAudioInterval = 3f,
            focusLoss = 1.5f,
            stressPenalty = 1.5f
        }
    };
}
