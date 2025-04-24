using UnityEngine;

public static class Extension_RenderTexture
{
    public static Camera FindSourceCamera(this RenderTexture renderTexture)
    {
        Camera[] lCameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Camera lCamera in lCameras)
        {
            if (renderTexture == lCamera.targetTexture)
                return lCamera;
        }

        return null;
    }
}