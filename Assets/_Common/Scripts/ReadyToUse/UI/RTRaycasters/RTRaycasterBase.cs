using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public abstract class RTRaycasterBase : BaseRaycaster
{
    [Tooltip("Source camera of the render texture"), SerializeField, ReadOnly]
    protected Camera sourceCamera;

    private RawImage rawImage;

    [SerializeField, ReadOnly] private GraphicRaycaster graphicRaycaster;

    //Used for not directly disabling raycaster during Raycast method (throws an error)
    private bool shouldGraphicRaycasterBeEnabled = true;

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

    public sealed override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
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

        RaycastFromRTUV(lUv, eventData, resultAppendList);
    }

    protected abstract void RaycastFromRTUV(Vector2 uv, PointerEventData eventData, List<RaycastResult> resultAppendList);

    private void LateUpdate()
    {
        // Must disable used graphic raycaster to avoid unwinnable conflicts (this raycaster would be blocked)
        graphicRaycaster.enabled = shouldGraphicRaycasterBeEnabled;
    }
}