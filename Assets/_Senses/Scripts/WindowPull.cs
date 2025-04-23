using DG.Tweening;
using System.Linq;
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


        //test
        [SerializeField] private RawImage refInUI;
        [SerializeField] private RenderTexture rtTest;
        [SerializeField] private Transform windowPrefab;
        [SerializeField] private RectTransform linePrefab;
        private Transform window;
        private RectTransform line;
        private Vector2 startDragPos;
        private Canvas canvas;
        //

        private Outline outline;

        void Start()
        {
            //Quick & dirty for overlay canvas parent
            canvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None)
                .Where(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay).First();

            Outline lOutline = GetComponent<Outline>();
            outline = lOutline == null ? gameObject.AddComponent<Outline>() : lOutline;
            outline.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            outline.enabled = true;
            TweenOutline(hoverWidth);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TweenOutline(0f, true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            TweenOutline(interactWidth);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TweenOutline(hoverWidth);
        }

        private void TweenOutline(float finalWidth, bool disable = false)
        {
            outline.DOKill();

            DOVirtual.Float(outline.OutlineWidth, finalWidth, tweenDuration,
                width => outline.OutlineWidth = width).SetTarget(outline)
                .OnComplete(() =>
                {
                    if (disable)
                        outline.enabled = false;
                });
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (window != null)
            {
                eventData.pointerDrag = null;
                return;
            }

            line = Instantiate(linePrefab, canvas.transform);

            Camera lCamera = FindObjectsByType<Camera>(FindObjectsSortMode.None)
                .Where(camera => camera.name == "CameraViewDoor").First();

            //test
            startDragPos = refInUI.rectTransform.ViewportToLocalPoint(
                lCamera.WorldToViewportPoint(transform.position));

            startDragPos = refInUI.rectTransform.TransformPoint(startDragPos);
            startDragPos = canvas.GetComponent<RectTransform>().InverseTransformPoint(startDragPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position,
            eventData.pressEventCamera, out Vector2 lCanvasPos);

            line.Line(startDragPos, lCanvasPos, 24f);

            if (window == null && Vector2.Distance(startDragPos, lCanvasPos) > 100f)
            {
                window = Instantiate(windowPrefab, canvas.transform);
                window.GetComponentInChildren<RawImage>().texture = rtTest;
            }
            else if (window != null)
                window.localPosition = lCanvasPos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Destroy(line.gameObject);
        }
    }
}