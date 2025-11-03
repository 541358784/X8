using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_BlindBoxTheme : Aux_ItemBase
{
    public static Aux_BlindBoxTheme Instance;
    private BlingBoxThemeRedPoint RedPoint;
    private Transform FreeTag;
    private StorageBlindBox Theme => BlindBoxModel.Instance.GetStorage(ThemeId);
    private int ThemeId;

    public void SetTheme(int theme)
    {
        ThemeId = theme;
        RedPoint.Init(Theme,true,true);
    }
    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        InvokeRepeating("UpdateUI", 0, 1);
        RedPoint = transform.Find("RedPoint").gameObject.AddComponent<BlingBoxThemeRedPoint>();
        FreeTag = transform.Find("FreeTag");
        FreeTag.gameObject.SetActive(false);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(BlindBoxModel.Instance.IsUnlock);
        if (!gameObject.activeSelf)
            return;
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        Theme.OpenThemeView();
    }
    private void OnDestroy()
    {
    }
}
