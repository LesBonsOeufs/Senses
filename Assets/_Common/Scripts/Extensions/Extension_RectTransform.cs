using UnityEngine;

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
}