using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;

public class MergeInformationTipsController : UIWindow
{
    private GameObject packageItem;
    private Transform packageRoot;
    bool isInit = false;

    public override void PrivateAwake()
    {
        packageItem = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/MergePackageUnit");
        packageRoot = this.transform.Find("Root/ContentGroup/Scroll View/Viewport/Content");
        BindClick("Root/ContentGroup/Button", (go) =>
        {
            //AudioManager.Instance.PlaySound(HotelSound.sfx_ui_Common_ClickButton);
            //CloseWindowWithinUIMgrNew(true, true);
            CloseWindowWithinUIMgr(true);
        });

        EventDispatcher.Instance.DispatchEvent(EventEnum.REWARD_POPUP);
    }

    public void SetTipsInfo(int id)
    {
        if (isInit)
            return;
        InitRoot(id);
        isInit = true;
    }

    private void InitRoot(int id)
    {
        // var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        // TableMergeLine line =  GameConfigManager.Instance.GetMergeLine(itemConfig.re_line);
        // if (line == null)
        //     return;
        // for (int i = 0; i < line.output.Length; i++)
        // {
        //     var item = Instantiate(packageItem, packageRoot);
        //     var script = item.gameObject.AddComponent<MergePackageUnit>();
        //     script.SetItemInfomation(GameConfigManager.Instance.GetItemConfig(line.output[i]), -1, MergePackageUnitType.infoTips);
        // }
    }
}