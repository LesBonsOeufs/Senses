using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DraggableRectTransform : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform rectTransform;
    private Vector2 lastMousePosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 lDeltaDrag = eventData.position - lastMousePosition;
        rectTransform.anchoredPosition += lDeltaDrag;
        lastMousePosition = eventData.position;
    }
}