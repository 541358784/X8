using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptScreen : MonoBehaviour
{
    private const int orgScreenHeight = 1366;
    private const int orgScreenWidth = 768;
    public Camera sceneCameras;

    void Start()
    {
        float standardAspect = orgScreenWidth / (float)orgScreenHeight;
        float deviceAspect = Screen.width / (float)Screen.height;
        if (deviceAspect < standardAspect)
        {
            float temp = standardAspect / deviceAspect;
            float size = sceneCameras.orthographicSize;
            sceneCameras.orthographicSize = size * temp;
        }
    }
}
