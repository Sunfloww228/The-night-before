using UnityEngine;
using UnityEngine.EventSystems;

public class PhoneSwipeMemes : MonoBehaviour,
    IBeginDragHandler,
    IEndDragHandler,
    IScrollHandler
{
    [SerializeField] private PhoneDistraction phone;
    [SerializeField] private float swipeDistance = 60f;

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnScroll(PointerEventData eventData)
    {
        if (phone == null)
            return;

        if (eventData.scrollDelta.y < 0f)
        {
            phone.Swipe();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (phone == null)
            return;

        float distance = Vector2.Distance(
            eventData.pressPosition,
            eventData.position
        );

        if (distance >= swipeDistance)
        {
            phone.Swipe();
        }
    }
}
