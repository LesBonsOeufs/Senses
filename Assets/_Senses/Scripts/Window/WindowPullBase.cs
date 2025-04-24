using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Root
{
    public abstract class WindowPullBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, 
        IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Camera renderingCamera;
        [SerializeField] private RectTransform sourceWindow;
        [SerializeField] private WindowOpenCloseAnim windowPrefab;
        [SerializeField] private Image linePrefab;

        private WindowOpenCloseAnim window;
        private Image line;

        private bool isDragged = false;

        private Vector2 PositionOnSourceWindow()
        {
            Vector2 lPos = sourceWindow.ViewportToLocalPoint(
                renderingCamera.WorldToViewportPoint(transform.position));

            return lPos;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isDragged || window != null)
                return;

            In();
        }

        public abstract void In();

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isDragged || window != null)
                return;

            Out();
        }

        public abstract void Out();

        public void OnPointerDown(PointerEventData eventData)
        {
            if (window != null)
                return;

            InteractOn();
        }

        public abstract void InteractOn();

        public void OnPointerUp(PointerEventData eventData)
        {
            if (window != null)
                return;

            InteractOff();
        }

        public abstract void InteractOff();

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (window != null)
            {
                eventData.pointerDrag = null;
                return;
            }

            isDragged = true;
            line = Instantiate(linePrefab, sourceWindow);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(sourceWindow, eventData.position,
            eventData.pressEventCamera, out Vector2 lLocalPos);
            line.rectTransform.Line(PositionOnSourceWindow(), lLocalPos, 24f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragged = false;
            RectTransform lSourceWindowCanvasRectTransform = sourceWindow.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(lSourceWindowCanvasRectTransform, eventData.position,
            eventData.pressEventCamera, out Vector2 lCanvasPos);

            MakeWindow(lSourceWindowCanvasRectTransform, lCanvasPos);
        }

        protected virtual void MakeWindow(Transform parent, Vector2 position)
        {
            window = Instantiate(windowPrefab, parent);
            window.transform.localPosition = position;
            window.OnStartOut += Window_OnStartOut;
        }

        protected virtual void Window_OnStartOut(float duration)
        {
            line.DOFade(0f, duration)
                .OnComplete
                (() =>
                {
                    Destroy(line.gameObject);
                });
        }

        private void LateUpdate()
        {
            if (window != null && line != null)
            {
                line.rectTransform.Line(PositionOnSourceWindow(), sourceWindow.InverseTransformPoint(window.transform.position), 24f);
                Vector2 lViewportPoint = renderingCamera.WorldToViewportPoint(transform.position);

                if (lViewportPoint.x > 1f || lViewportPoint.x < 0f || lViewportPoint.y > 1f || lViewportPoint.y < 0f)
                {
                    window.Out();
                    window = null;
                }
            }
        }
    }
}