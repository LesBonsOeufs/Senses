using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Outline = QuickOutline.Outline;

namespace Root
{
    public class WindowPull : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
        , IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private float hoverWidth = 5f;
        [SerializeField] private float interactWidth = 10f;
        [SerializeField] private float tweenDuration = 0.2f;

        [SerializeField] private Camera renderingCamera;
        [SerializeField] private RectTransform sourceWindow;
        [SerializeField] private WindowOpenCloseAnim windowPrefab;
        [SerializeField] private Image linePrefab;

        private Outline outline;
        private WindowOpenCloseAnim window;
        private Image line;

        private bool isDragged = false;

        void Start()
        {
            Outline lOutline = GetComponent<Outline>();
            outline = lOutline == null ? gameObject.AddComponent<Outline>() : lOutline;
            outline.enabled = false;
        }

        private Vector2 PositionOnSourceWindow()
        {
            Vector2 lPos = sourceWindow.ViewportToLocalPoint(
                renderingCamera.WorldToViewportPoint(transform.position));

            return lPos;
        }

        private Tweener TweenOutline(float finalWidth)
        {
            outline.DOKill();

            if (finalWidth != 0f)
                outline.enabled = true;

            return DOVirtual.Float(outline.OutlineWidth, finalWidth, tweenDuration,
                width => outline.OutlineWidth = width).SetTarget(outline)
                .OnComplete(() =>
                {
                    if (finalWidth == 0f)
                        outline.enabled = false;
                });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isDragged || window != null)
                return;

            TweenOutline(hoverWidth);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isDragged || window != null)
                return;

            TweenOutline(0f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (window != null)
                return;

            TweenOutline(interactWidth);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (window != null)
                return;

            TweenOutline(hoverWidth);
        }

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

            window = Instantiate(windowPrefab, lSourceWindowCanvasRectTransform);
            window.transform.localPosition = lCanvasPos;
            window.OnStartOut += Window_OnStartOut;

            outline.DOKill();
            outline.OutlineWidth = hoverWidth;
            DOVirtual.Float(outline.OutlineWidth, interactWidth, tweenDuration * 2f, width => outline.OutlineWidth = width)
                .SetTarget(outline)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void Window_OnStartOut(float duration)
        {
            TweenOutline(0f);
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