using UnityEngine;

public sealed class CoffeeGroundsAnimationEvents : MonoBehaviour
{
    [SerializeField] private CoffeeDistraction coffee;

    public void OnGroundsAnimationFinished()
    {
        coffee.FinishAddingGrounds();
    }
}