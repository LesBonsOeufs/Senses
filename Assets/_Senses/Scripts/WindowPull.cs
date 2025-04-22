using DG.Tweening;
using QuickOutline;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Root
{
    public class WindowPull : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
        , IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private float hoverWidth = 5f;
        [SerializeField] private float interactWidth = 10f;
        [SerializeField] private float tweenDuration = 0.2f;

        //test
        [SerializeField] private RectTransform linePrefab;
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
            Debug.Log("enter");
            outline.enabled = true;
            TweenOutline(hoverWidth);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("exit");
            TweenOutline(0f, true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("down");
            TweenOutline(interactWidth);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("up");
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
            Debug.Log("beginDrag");
            line = Instantiate(linePrefab, canvas.transform);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position,
            eventData.pressEventCamera, out Vector2 lCanvasPos);
            startDragPos = lCanvasPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("drag");
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position,
            eventData.pressEventCamera, out Vector2 lCanvasPos);

            line.Line(startDragPos, lCanvasPos, 24f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("endDrag");
            Destroy(line.gameObject);
        }
    }
}