using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Root
{
    public abstract class WindowPullBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, 
        IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public WindowOpenCloseAnim windowPrefab;
        [SerializeField, Tooltip("The camera rendering this object")] private Camera renderingCamera;
        [SerializeField] private Image linePrefab;

        private WindowOpenCloseAnim sourceWindow;
        private RectTransform sourceWindowRectTransform;

        private WindowOpenCloseAnim window;
        private Image line;

        private bool isDragged = false;

        private Vector2 PositionOnSourceWindow()
        {
            Vector2 lPos = sourceWindowRectTransform.ViewportToLocalPoint(
                renderingCamera.WorldToViewportPoint(transform.position));

            return lPos;
        }

        private WindowOpenCloseAnim GetCurrentSourceWindow()
        {
            WindowOpenCloseAnim[] lWindows = FindObjectsByType<WindowOpenCloseAnim>(FindObjectsSortMode.None);

            foreach (WindowOpenCloseAnim lWindow in lWindows)
            {
                RenderTexture lRenderTexture = lWindow.GetComponentInChildren<RawImage>().texture as RenderTexture;

                if (lRenderTexture != null && lRenderTexture == renderingCamera.targetTexture)
                    return lWindow;
            }

            return null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (windowPrefab == null || isDragged || window != null)
                return;

            In();
        }

        public abstract void In();

        public void OnPointerExit(PointerEventData eventData)
        {
            if (windowPrefab == null || isDragged || window != null)
                return;

            Out();
        }

        public abstract void Out();

        public void OnPointerDown(PointerEventData eventData)
        {
            if (windowPrefab == null || window != null)
                return;

            InteractOn();
        }

        public abstract void InteractOn();

        public void OnPointerUp(PointerEventData eventData)
        {
            if (windowPrefab == null || window != null)
                return;

            InteractOff();
        }

        public abstract void InteractOff();

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (windowPrefab == null || window != null)
            {
                eventData.pointerDrag = null;
                return;
            }

            if (sourceWindow != null)
                sourceWindow.OnStartOut -= SourceWindow_OnStartOut;

            //Get source window from renderingCamera's renderTexture
            sourceWindow = GetCurrentSourceWindow();
            sourceWindowRectTransform = sourceWindow.GetComponent<RectTransform>();
            sourceWindow.OnStartOut += SourceWindow_OnStartOut;

            isDragged = true;
            line = Instantiate(linePrefab, sourceWindowRectTransform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(sourceWindowRectTransform, eventData.position,
            eventData.pressEventCamera, out Vector2 lLocalPos);
            line.rectTransform.Line(PositionOnSourceWindow(), lLocalPos, 24f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragged = false;
            RectTransform lSourceWindowCanvasRectTransform = sourceWindowRectTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

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

        private void SourceWindow_OnStartOut(float duration)
        {
            window.Out();
            window = null;
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
                line.rectTransform.Line(PositionOnSourceWindow(), sourceWindowRectTransform.InverseTransformPoint(window.transform.position), 24f);
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