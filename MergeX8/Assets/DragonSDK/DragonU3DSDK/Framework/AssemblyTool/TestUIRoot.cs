#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEngine.U2D;
using DragonU3DSDK;
using DragonU3DSDK.Asset;

public class TestUIRoot : MonoBehaviour
{
    void Awake()
    {
        SpriteAtlasManager.atlasRequested += OnLoadAtlas;
    }

    void OnDestroy()
    {
        SpriteAtlasManager.atlasRequested -= OnLoadAtlas;
    }

    private void OnLoadAtlas(string atlasName, Action<SpriteAtlas> act)
    {
        if (atlasName.Contains(".SD") || atlasName.Contains("/Sd/"))
        {
            return;
        }

        DebugUtil.LogEnable = false;
        SpriteAtlas sa = ResourcesManager.Instance.LoadSpriteAtlas(string.Format("{0}/{1}", atlasName, atlasName));
        DebugUtil.LogEnable = true;
        if (sa == null)
        {
            try
            {
                sa = ResourcesManager.Instance.LoadSpriteAtlasVariant(atlasName);
            }
            catch
            {

            }
        }
        if (sa == null)
        {
            DebugUtil.LogWarning("加载[" + atlasName + "]图集失败");
        }

        act(sa);
    }
}

#endif