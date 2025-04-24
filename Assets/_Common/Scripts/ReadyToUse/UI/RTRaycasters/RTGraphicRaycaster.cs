using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;
using NaughtyAttributes;
using System.Linq;

[RequireComponent(typeof(RawImage))]
public class RTGraphicRaycaster : BaseRaycaster
{
    #region Copied from GraphicRaycaster.cs

    protected const int kNoEventMaskSet = -1;

    /// <summary>
    /// Type of raycasters to check against to check for canvas blocking elements.
    /// </summary>
    public enum BlockingObjects
    {
        /// <summary>
        /// Perform no raycasts.
        /// </summary>
        None = 0,
        /// <summary>
        /// Perform a 2D raycast check to check for blocking 2D elements
        /// </summary>
        TwoD = 1,
        /// <summary>
        /// Perform a 3D raycast check to check for blocking 3D elements
        /// </summary>
        ThreeD = 2,
        /// <summary>
        /// Perform a 2D and a 3D raycasts to check for blocking 2D and 3D elements.
        /// </summary>
        All = 3,
    }

    /// <summary>
    /// Priority of the raycaster based upon sort order.
    /// </summary>
    /// <returns>
    /// The sortOrder priority.
    /// </returns>
    public override int sortOrderPriority
    {
        get
        {
            // We need to return the sorting order here as distance will all be 0 for overlay.
            if (sourceCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return sourceCanvas.sortingOrder;

            return base.sortOrderPriority;
        }
    }

    /// <summary>
    /// Priority of the raycaster based upon render order.
    /// </summary>
    /// <returns>
    /// The renderOrder priority.
    /// </returns>
    public override int renderOrderPriority
    {
        get
        {
            // We need to return the sorting order here as distance will all be 0 for overlay.
            if (sourceCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return sourceCanvas.rootCanvas.renderOrder;

            return base.renderOrderPriority;
        }
    }

    [FormerlySerializedAs("ignoreReversedGraphics")]
    [SerializeField]
    private bool m_IgnoreReversedGraphics = true;
    [FormerlySerializedAs("blockingObjects")]
    [SerializeField]
    private BlockingObjects m_BlockingObjects = BlockingObjects.None;

    /// <summary>
    /// Whether Graphics facing away from the raycaster are checked for raycasts.
    /// </summary>
    public bool ignoreReversedGraphics { get { return m_IgnoreReversedGraphics; } set { m_IgnoreReversedGraphics = value; } }

    /// <summary>
    /// The type of objects that are checked to determine if they block graphic raycasts.
    /// </summary>
    public BlockingObjects blockingObjects { get { return m_BlockingObjects; } set { m_BlockingObjects = value; } }

    [SerializeField]
    protected LayerMask m_BlockingMask = kNoEventMaskSet;

    /// <summary>
    /// The type of objects specified through LayerMask that are checked to determine if they block graphic raycasts.
    /// </summary>
    public LayerMask blockingMask { get { return m_BlockingMask; } set { m_BlockingMask = value; } }

    [NonSerialized] private List<Graphic> m_RaycastResults = new List<Graphic>();

    public override Camera eventCamera => sourceCamera;

    /// <summary>
    /// Perform a raycast into the screen and collect all graphics underneath it.
    /// </summary>
    [NonSerialized] static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();
    private static void Raycast(Camera eventCamera, Vector2 pointerPosition, IList<Graphic> foundGraphics, List<Graphic> results)
    {
        // Necessary for the event system
        int totalCount = foundGraphics.Count;
        for (int i = 0; i < totalCount; ++i)
        {
            Graphic graphic = foundGraphics[i];

            // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
            if (!graphic.raycastTarget || graphic.canvasRenderer.cull || graphic.depth == -1)
                continue;

            if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera, graphic.raycastPadding))
                continue;

            if (eventCamera != null && eventCamera.WorldToScreenPoint(graphic.rectTransform.position).z > eventCamera.farClipPlane)
                continue;

            if (graphic.Raycast(pointerPosition, eventCamera))
            {
                s_SortedGraphics.Add(graphic);
            }
        }

        s_SortedGraphics.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));
        totalCount = s_SortedGraphics.Count;
        for (int i = 0; i < totalCount; ++i)
            results.Add(s_SortedGraphics[i]);

        s_SortedGraphics.Clear();
    }

    #endregion

    [Tooltip("Source camera of the render texture"), SerializeField, ReadOnly]
    private Camera sourceCamera;
    [Tooltip("Canvas to read from"), SerializeField, ReadOnly]
    private Canvas sourceCanvas;

    private RawImage rawImage;

    protected override void Start()
    {
        base.Start();
        rawImage = GetComponent<RawImage>();

        if (rawImage.texture is RenderTexture lRT)
        {
            sourceCamera = lRT.FindSourceCamera();
            sourceCanvas = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(canvas => canvas.worldCamera == sourceCamera).First();
        }
        else
            Debug.LogError("No render texture assigned to the RawImage!");
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (rawImage.texture == null)
            return;

        RectTransform lRectTransform = rawImage.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(lRectTransform, eventData.position, 
            eventData.pressEventCamera, out Vector2 lLocalPoint))
            return;
        
        // Normalize local point to [0,1] UV
        Vector2 lUv = rawImage.rectTransform.LocalToViewportPoint(lLocalPoint);

        if (lUv.x < 0 || lUv.x > 1 || lUv.y < 0 || lUv.y > 1)
            return;

        IList<Graphic> lCanvasGraphics = GraphicRegistry.GetRaycastableGraphicsForCanvas(sourceCanvas);

        if (lCanvasGraphics == null || lCanvasGraphics.Count == 0)
            return;

        Camera lCurrentEventCamera = eventCamera;
        float lHitDistance = float.MaxValue;
        Ray lRay = new Ray();

        if (lCurrentEventCamera != null)
            lRay = lCurrentEventCamera.ViewportPointToRay(lUv);

        m_RaycastResults.Clear();
        Raycast(lCurrentEventCamera, lCurrentEventCamera.ViewportToScreenPoint(lUv), lCanvasGraphics, m_RaycastResults);

        int lTotalCount = m_RaycastResults.Count;

        for (int index = 0; index < lTotalCount; index++)
        {
            GameObject lGo = m_RaycastResults[index].gameObject;
            bool lAppendGraphic = true;

            if (ignoreReversedGraphics)
            {
                if (lCurrentEventCamera == null)
                {
                    // If we dont have a camera we know that we should always be facing forward
                    Vector3 lDir = lGo.transform.rotation * Vector3.forward;
                    lAppendGraphic = Vector3.Dot(Vector3.forward, lDir) > 0;
                }
                else
                {
                    // If we have a camera compare the direction against the cameras forward.
                    Vector3 lCameraForward = lCurrentEventCamera.transform.rotation * Vector3.forward * lCurrentEventCamera.nearClipPlane;
                    lAppendGraphic = Vector3.Dot(lGo.transform.position - lCurrentEventCamera.transform.position - lCameraForward, lGo.transform.forward) >= 0;
                }
            }

            if (lAppendGraphic)
            {
                float lDistance;
                Transform lTransform = lGo.transform;
                Vector3 lForward = lTransform.forward;

                if (lCurrentEventCamera == null || sourceCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    lDistance = 0;
                else
                {
                    // http://geomalgorithms.com/a06-_intersect-2.html
                    lDistance = Vector3.Dot(lForward, lTransform.position - lRay.origin) / Vector3.Dot(lForward, lRay.direction);

                    // Check to see if the go is behind the camera.
                    if (lDistance < 0)
                        continue;
                }

                if (lDistance >= lHitDistance)
                    continue;

                var lCastResult = new RaycastResult
                {
                    gameObject = lGo,
                    module = this,
                    distance = lDistance,
                    screenPosition = eventData.position,
                    index = resultAppendList.Count,
                    depth = m_RaycastResults[index].depth,
                    sortingLayer = sourceCanvas.sortingLayerID,
                    sortingOrder = sourceCanvas.sortingOrder,
                    worldPosition = lRay.origin + lRay.direction * lDistance,
                    worldNormal = -lForward
                };
                resultAppendList.Add(lCastResult);
            }
        }
    }
}