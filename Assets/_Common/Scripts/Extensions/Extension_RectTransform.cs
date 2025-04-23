using UnityEngine;
using UnityEngine.UI;

public static class Extension_RectTransform
{
    /// <summary>
    /// Uses anchored positions
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public static void Line(this RectTransform rectTransform, Vector2 start, Vector2 end, float thickness = 20f)
    {
        Vector2 lStartToEnd = end - start;
        float lAngle = Vector2.SignedAngle(Vector2.up, lStartToEnd);

        rectTransform.anchoredPosition = start + lStartToEnd * 0.5f;
        rectTransform.rotation = Quaternion.Euler(0f, 0f, lAngle);
        rectTransform.sizeDelta = new Vector2(thickness, lStartToEnd.magnitude);
    }

    public static Vector2 LocalToViewportPoint(this RectTransform rectTransform, Vector2 localPosition)
    {
        Rect lRect = rectTransform.rect;

        return new Vector2(
            (localPosition.x - lRect.x) / lRect.width,
            (localPosition.y - lRect.y) / lRect.height
            );
    }

    public static Vector2 ViewportToLocalPoint(this RectTransform rectTransform, Vector2 viewportPosition)
    {
        Rect lRect = rectTransform.rect;

        return new Vector2(
            viewportPosition.x * lRect.width + lRect.x,
            viewportPosition.y * lRect.height + lRect.y
            );
    }
}