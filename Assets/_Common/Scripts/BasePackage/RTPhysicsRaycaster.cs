using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using NaughtyAttributes;

[RequireComponent(typeof(Camera))]
public class RTPhysicsRaycaster : PhysicsRaycaster
{
    [Tooltip("UI RawImage showing the RenderTexture")]
    public RawImage targetRawImage;
    [SerializeField, ReadOnly] private GraphicRaycaster graphicRaycaster;

    //Used for not directly disabling raycaster during Raycast method (throws an error)
    private bool shouldGraphicRaycasterBeEnabled = true;

    protected override void Start()
    {
        base.Start();
        graphicRaycaster = targetRawImage.GetComponentInParent<GraphicRaycaster>();
    }

    private void LateUpdate()
    {
        // Must disable used graphic raycaster to avoid unwinnable conflicts (this raycaster would be blocked)
        graphicRaycaster.enabled = shouldGraphicRaycasterBeEnabled;
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (targetRawImage == null || targetRawImage.texture == null)
            return;

        RectTransform lRectTransform = targetRawImage.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(lRectTransform, eventData.position, 
            eventData.pressEventCamera, out Vector2 lLocalPoint))
            return;

        // Normalize local point to [0,1] UV
        Rect lRect = lRectTransform.rect;
        Vector2 lUv = new Vector2(
            (lLocalPoint.x - lRect.x) / lRect.width,
            (lLocalPoint.y - lRect.y) / lRect.height
            );

        if (lUv.x < 0 || lUv.x > 1 || lUv.y < 0 || lUv.y > 1)
        {
            shouldGraphicRaycasterBeEnabled = true;
            return;
        }

        shouldGraphicRaycasterBeEnabled = false;

        Ray lRay = eventCamera.ViewportPointToRay(new Vector3(lUv.x, lUv.y, 0));

        // Use Physics.RaycastAll to get all hits
        RaycastHit[] lHits = Physics.RaycastAll(lRay, eventCamera.farClipPlane, finalEventMask);

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