using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StressVignette : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Volume volume;

    [Header("Настройки виньетки")]
    [SerializeField, Range(0f, 100f)]
    private float startStress = 0f;

    [SerializeField, Range(0f, 1f)]
    private float maxIntensity = 0.6f;

    [SerializeField, Min(0f)]
    private float smoothSpeed = 2f;

    private Vignette vignette;

    private void Awake()
    {
        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGet(out vignette);
        }

        if (vignette != null)
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0f;
        }
    }

    private void Update()
    {
        if (gameManager == null || vignette == null)
            return;

        float stress = gameManager.State.Stress;

        float intensity = Mathf.InverseLerp(
            startStress,
            GameState.ResourceMax,
            stress
        );

        float targetIntensity = intensity * maxIntensity;

        vignette.intensity.value = Mathf.MoveTowards(
            vignette.intensity.value,
            targetIntensity,
            smoothSpeed * Time.deltaTime
        );
    }

}
