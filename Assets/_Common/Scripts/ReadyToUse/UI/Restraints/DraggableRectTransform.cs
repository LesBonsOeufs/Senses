using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DraggableRectTransform : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private bool appliesOnSelf = true;
    [SerializeField, HideIf(nameof(appliesOnSelf))] private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        if (appliesOnSelf || rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        canvas = rectTransform.GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}