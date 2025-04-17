using UnityEngine;
using System.Linq;

[RequireComponent(typeof(RectTransform))]
public class ConfinedRectTransform : MonoBehaviour
{
    [SerializeField] private RectTransform confiner;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        //Use first active Overlay Canvas as default
        if (confiner == null)
            confiner = FindObjectsByType<Canvas>(FindObjectsSortMode.None)
                .Where(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay).First().GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        rectTransform.anchoredPosition = ClampPosition(confiner.rect);
    }

    private Vector2 ClampPosition(Rect confinerRect)
    {
        Vector2 lSize = rectTransform.sizeDelta * rectTransform.localScale;
        Vector2 lAnchorFromCenter = rectTransform.anchorMax - Vector2.one * 0.5f;
        Vector2 lConfinerSize = confinerRect.size;

        // Calculate the needed offset from the pivot & the anchors' position
        Vector2 lOffset = lSize * rectTransform.pivot - lConfinerSize * lAnchorFromCenter;

        // Calculate the minimum and maximum allowed positions within the confiner.
        float lMinX = confinerRect.xMin + lOffset.x;
        float lMaxX = confinerRect.xMax + lOffset.x - lSize.x;
        float lMinY = confinerRect.yMin + lOffset.y;
        float lMaxY = confinerRect.yMax + lOffset.y - lSize.y;

        Vector2 lPosition = rectTransform.anchoredPosition;
        // Clamp the position within the calculated boundaries.
        lPosition.x = Mathf.Clamp(lPosition.x, lMinX, lMaxX);
        lPosition.y = Mathf.Clamp(lPosition.y, lMinY, lMaxY);

        return lPosition;
    }
}