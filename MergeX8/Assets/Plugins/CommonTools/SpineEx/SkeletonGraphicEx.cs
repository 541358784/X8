using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEditor;

#if CK_CLIENT
using DragonU3DSDK;
using DragonU3DSDK.Asset;
#endif

public class SkeletonGraphicEx : SkeletonGraphic
{
    public string dataGuid;
    public string matGuid;
    public string projDataPath;
    public string projMatPath;
    public string dataPath;
    public string matPath;
    public bool immediate = true; 

    public void StartExecute()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            skeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(projDataPath);
            if (!string.IsNullOrEmpty(projMatPath))
                material = AssetDatabase.LoadAssetAtPath<Material>(projMatPath);
        }
#elif CK_CLIENT
       if (Application.isPlaying &&  ResourcesManager.Instance)
        {
            skeletonDataAsset = ResourcesManager.Instance.LoadResource<SkeletonDataAsset>(dataPath);
            if (!string.IsNullOrEmpty(matPath))
                material = ResourcesManager.Instance.LoadResource<Material>(matPath);            
        }
#endif

        base.Awake();        
    }

    protected override void Awake()
    {        
        if (string.IsNullOrEmpty(dataPath))
        {
#if CK_CLIENT
            DebugUtil.LogError("SkeletonGraphicEx, data path empty!");
#endif
            return;
        }

        if (!immediate)
            return;

        StartExecute();       
    }

    protected override void OnDestroy()
    {
        if (!Application.isPlaying)
            return;

        skeletonDataAsset = null;
        material = null;

#if CK_CLIENT
        if (Application.isPlaying && ResourcesManager.Instance)
        {
            ResourcesManager.Instance.ReleaseRes(dataPath);
            if (!string.IsNullOrEmpty(matPath))
                ResourcesManager.Instance.ReleaseRes(matPath);            
        }
#endif
        base.OnDestroy();        
    }
}
