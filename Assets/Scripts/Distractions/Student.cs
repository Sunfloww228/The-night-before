using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Student : MonoBehaviour
{
    [SerializeField] private SpriteRenderer studentSprite;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Animator animator;

    [Header("Названия состояний Animator")]
    [SerializeField] private string sittingState = "Sitting";
    [SerializeField] private string courierState = "Courier";
    [SerializeField] private string neighbourState = "Neighbour";

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (gameManager != null)
            gameManager.SessionRestarting += PlaySitting;
    }

    private void Start()
    {
        PlaySitting();
    }

    private void OnDisable()
    {
        if (gameManager != null)
            gameManager.SessionRestarting -= PlaySitting;
    }

    public void SetVisible(bool visible)
    {
        if (studentSprite != null)
            studentSprite.enabled = visible;
    }

    public void PlaySitting()
    {
        PlayAnimation(sittingState);
    }

    public void PlayCourier()
    {
        PlayAnimation(courierState);
    }

    public void PlayNeighbour()
    {
        PlayAnimation(neighbourState);
    }

    private void PlayAnimation(string stateName)
    {
        if (animator == null || string.IsNullOrEmpty(stateName))
            return;

        animator.CrossFadeInFixedTime(stateName, 0.1f);
    }
}
