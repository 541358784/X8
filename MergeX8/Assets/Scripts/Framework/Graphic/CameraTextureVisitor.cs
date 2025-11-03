using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;

public class CameraTextureVisitor : MonoBehaviour
{
    public RenderTexture RT;

    private bool _copyRTOnce = false;

    public bool CopyRTOnce
    {
        get
        {
            var ret = _copyRTOnce;
            if (_copyRTOnce)
            {
                _copyRTOnce = false;
            }

            return ret;
        }
        set { _copyRTOnce = value; }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (CopyRTOnce && RT)
        {
            Graphics.Blit(src, RT);
            DebugUtil.Log($"Camera[{name}]:Blit a render texture");
        }

        Graphics.Blit(src, dest);
    }
}