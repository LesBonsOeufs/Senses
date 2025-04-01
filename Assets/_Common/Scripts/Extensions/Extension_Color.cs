using UnityEngine;
using System.Collections.Generic;

public static class Extension_Color
{
    public static Color GenerateDistinctColor(this HashSet<Color> distinctFrom, 
                        float minSaturation = 0f, float maxSaturation = 1f, float minValue = 0f, float maxValue = 1f, 
                        float minHueDifference = 0.2f, int maxAttempts = 100)
    {
        Color lNewColor;
        int lAttempts = 0;

        do
        {
            lNewColor = Random.ColorHSV(0f, 1f, minSaturation, maxSaturation, minValue, maxValue); // Generate random color
            lAttempts++;
        }
        while (!IsColorDistinct(lNewColor, distinctFrom, minHueDifference) && lAttempts < maxAttempts);

        return lNewColor;
    }

    private static bool IsColorDistinct(Color color, HashSet<Color> distinctFrom, float minHueDifference)
    {
        foreach (Color lTestedColor in distinctFrom)
        {
            Color.RGBToHSV(color, out float lHue1, out _, out _);
            Color.RGBToHSV(lTestedColor, out float lHue2, out _, out _);

            if (Mathf.Abs(lHue1 - lHue2) < minHueDifference) // Check if Hue is too close
            {
                return false;
            }
        }
        return true;
    }
}