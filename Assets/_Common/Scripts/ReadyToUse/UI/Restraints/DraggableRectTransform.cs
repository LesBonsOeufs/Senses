using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DraggableRectTransform : MonoBehaviour, IDragHandler
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

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}