using System;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;

public partial class UIPopupMergeInformationController
{
    private AnimGroup AnimNode;
    public void AnimGroupPrivateAwake()
    {
        AnimNode = transform.Find("Root/AnimGroup").gameObject.AddComponent<AnimGroup>();
        AnimNode.gameObject.SetActive(true);
        AnimNode.gameObject.SetActive(false);
    }
    public void IniAnimGroup()
    {
        AnimNode.Init(itemConfig);
    }
    public class AnimGroup:MonoBehaviour
    {
        private Transform Root;

        private void Awake()
        {
            Root = transform.Find("Root");
        }

        public void Init(TableMergeItem config)
        {
            CommonUtils.DestroyAllChildren(Root);
            if (config.showAnimAsset.IsEmptyString())
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                var assetPath = "Prefabs/Merge/" + config.showAnimAsset;
                var asset = ResourcesManager.Instance.LoadResource<GameObject>(assetPath);
                if (asset)
                {
                    var clone = Instantiate(asset, Root);
                    clone.SetActive(true);   
                }
                else
                {
                    Debug.LogError(assetPath+"未找到,确认ItemConfig.id="+config.id+"的showAnimAsset配置");
                }
            }
        }
    }
}