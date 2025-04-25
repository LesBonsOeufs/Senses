using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Root
{
    public class WindowControl : MonoBehaviour, 
        IPointerDownHandler, IPointerExitHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Transform toForward;
        [SerializeField] private TextLineHighlighter textLineHighlighter;
        [SerializeField] private Transform basePoint;
        [SerializeField] private Transform topPoint;
        [SerializeField, ReadOnly] private Vector2 input;

        private Controllable controllable;

        private void Start()
        {
            controllable = FindAnyObjectByType<Controllable>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Assuming basePoint in overlay UI
            textLineHighlighter.UpdateHighlight(basePoint.position, eventData.position);
            RefreshInput(eventData.position);
            controllable.input = input;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            toForward.SetAsLastSibling();
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            textLineHighlighter.ClearHighlight();
            controllable.input = Vector2.zero;
        }

        public void OnPointerExit(PointerEventData eventData) => OnPointerUp(eventData);

        private void RefreshInput(Vector2 mousePosition)
        {
            Vector2 lVector = mousePosition - (Vector2)basePoint.position;
            lVector.y = Mathf.Max(lVector.y, 0f);
            float lBotToTopDistance = Vector2.Distance(basePoint.position, topPoint.position);

            input = lVector / lBotToTopDistance;
        }
    }
}