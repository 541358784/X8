using System;
using DragonU3DSDK.Asset;
using SRF;
using UnityEngine;

public class TMWinPrizeLabelCreator:MonoBehaviour
{
    private void Awake()
    {
        InvokeRepeating("TryCreate",0,1);
    }

    private TMWinPrizeLabel Label;
    public void TryCreate()
    {
        if (Label)
        {
            CancelInvoke("TryCreate");
            Destroy(this);
            return;   
        }
        if (TMWinPrizeModel.Instance.IsOpened())
        {
            var asset = ResourcesManager.Instance.LoadResource<GameObject>(
                "Prefabs/Activity/TMatch/TMWinPrize/TMWinPrizeLabel");
            if (asset == null)
                return;
            var obj = GameObject.Instantiate(asset,transform);
            obj.gameObject.SetActive(true);
            Label = obj.AddComponent<TMWinPrizeLabel>();
            CancelInvoke("TryCreate");
            Destroy(this);
        }
    }
}