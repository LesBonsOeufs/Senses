using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using NaughtyAttributes;

[RequireComponent(typeof(RawImage))]
public class RTPhysicsRaycaster : BaseRaycaster
{
    #region Copied from PhysicsRaycaster.cs

    /// <summary>
    /// Const to use for clarity when no event mask is set
    /// </summary>
    protected const int kNoEventMaskSet = -1;

    /// <summary>
    /// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
    /// </summary>
    [SerializeField]
    protected LayerMask m_EventMask = kNoEventMaskSet;

    /// <summary>
    /// Event mask used to determine which objects will receive events.
    /// </summary>
    public int finalEventMask
    {
        get { return (eventCamera != null) ? eventCamera.cullingMask & m_EventMask : kNoEventMaskSet; }
    }

    /// <summary>
    /// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
    /// </summary>
    public LayerMask eventMask
    {
        get { return m_EventMask; }
        set { m_EventMask = value; }
    }

    protected Camera m_EventCamera;

    #endregion

    [Tooltip("Source camera of the render texture"), SerializeField, ReadOnly]
    private Camera sourceCamera;
    [SerializeField, ReadOnly] private GraphicRaycaster graphicRaycaster;

    private RawImage rawImage;
    //Used for not directly disabling raycaster during Raycast method (throws an error)
    private bool shouldGraphicRaycasterBeEnabled = true;

    public override Camera eventCamera
    {
        get
        {
            if (m_EventCamera == null)
            {
                Canvas lCanvas = GetComponentInParent<Canvas>();

                switch (lCanvas.renderMode)
                {
                    case RenderMode.ScreenSpaceOverlay:
                        m_EventCamera = null;
                        break;
                    case RenderMode.ScreenSpaceCamera:
                        m_EventCamera = lCanvas.worldCamera;
                        break;
                    case RenderMode.WorldSpace:

                        if (lCanvas.worldCamera != null)
                            m_EventCamera = lCanvas.worldCamera;
                        else
                            m_EventCamera = Camera.main;
                        break;
                }
            }

            return m_EventCamera;
        }
    }

    protected override void Start()
    {
        base.Start();
        rawImage = GetComponent<RawImage>();

        if (rawImage.texture is RenderTexture lRT)
        {
            sourceCamera = lRT.FindSourceCamera();
            graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        }
        else
            Debug.LogError("No render texture assigned to the RawImage!");
    }

    private void LateUpdate()
    {
        // Must disable used graphic raycaster to avoid unwinnable conflicts (this raycaster would be blocked)
        graphicRaycaster.enabled = shouldGraphicRaycasterBeEnabled;
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
        {
            shouldGraphicRaycasterBeEnabled = true;
            return;
        }

        shouldGraphicRaycasterBeEnabled = false;

        Ray lRay = sourceCamera.ViewportPointToRay(new Vector3(lUv.x, lUv.y, 0));

        // Use Physics.RaycastAll to get all hits
        RaycastHit[] lHits = Physics.RaycastAll(lRay, sourceCamera.farClipPlane, finalEventMask);

        if (lHits.Length > 0)
        {
            foreach (var hit in lHits)
            {
                RaycastResult result = new RaycastResult
                {
                    gameObject = hit.collider.gameObject,
                    module = this,
                    distance = hit.distance,
                    worldPosition = hit.point,
                    worldNormal = hit.normal,
                    screenPosition = eventData.position
                };
                resultAppendList.Add(result);
            }
        }
    }
}