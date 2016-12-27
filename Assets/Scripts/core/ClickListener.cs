using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public UnityEvent onClick;

    private bool _isPointerDown;

    public void OnDrag(PointerEventData eventData)
    {
        _isPointerDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isPointerDown && onClick != null)
        {
            _isPointerDown = false;
            onClick.Invoke();
        }
    }
}