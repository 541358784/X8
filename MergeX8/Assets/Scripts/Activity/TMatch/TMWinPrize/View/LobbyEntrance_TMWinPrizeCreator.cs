using System;
using DragonU3DSDK.Asset;
using UnityEngine;

public class LobbyEntrance_TMWinPrizeCreator:MonoBehaviour
{
    public void Init()
    {
        InvokeRepeating("TryCreate",0,1);   
    }
    
    private LobbyEntrance_TMWinPrize Entrance;
    public void TryCreate()
    {
        if (Entrance)
        {
            CancelInvoke("TryCreate");
            Destroy(this);
            return;   
        }
        if (TMWinPrizeModel.Instance.IsOpened())
        {
            var asset = ResourcesManager.Instance.LoadResource<GameObject>(
                "Prefabs/Activity/TMatch/TMWinPrize/LobbyEntrance_TMWinPrize");
            if (asset == null)
                return;
            var obj = GameObject.Instantiate(asset,transform);
            obj.gameObject.SetActive(true);
            Entrance = obj.AddComponent<LobbyEntrance_TMWinPrize>();
            Entrance.Init();
            CancelInvoke("TryCreate");
            Destroy(this);
        }
    }
}