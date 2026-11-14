using UnityEngine;
using UnityEngine.EventSystems;



public class CoffeeBrewButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] private CoffeeDistraction coffee;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (coffee != null)
            coffee.BeginBrew();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (coffee != null)
            coffee.EndBrew();
    }

    private void OnDisable()
    {
        if (coffee != null)
            coffee.EndBrew();
    }
}