using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RTPhysicsRaycaster : RTRaycasterBase
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

    protected override void RaycastFromRTUV(Vector2 uv, PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        Ray lRay = sourceCamera.ViewportPointToRay(new Vector3(uv.x, uv.y, 0));

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