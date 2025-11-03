using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectLine.Logic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UICatchFishMainController:UIWindowController
{
    public static UICatchFishMainController Instance;

    public static UICatchFishMainController Open()
    {
        if (Instance)
            return Instance;
        // Instance = UIManager.Instance.OpenUI(UINameConst.UICatchFishMain) as UICatchFishMainController;
        UIManager.Instance.OpenUI(UINameConst.UICatchFishMain);
        return Instance;
    }

    public Button CloseBtn;
    public LocalizeTextMeshProUGUI TimeText;
    public LocalizeTextMeshProUGUI ItemCountText;
    private StorageCatchFish Storage => CatchFishModel.Instance.Storage;
    
    public override void PrivateAwake()
    {
        Instance = this;
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        ItemCountText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        UpdateItemCountText();
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/Top/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0,1);
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        XUtility.DisableShieldBtn(gameObject);
    }
    public void UpdateTime()
    {
        TimeText.SetText(CatchFishModel.Instance.Storage.GetLeftTimeText());
    }

    public void UpdateItemCountText()
    {
        ItemCountText.SetText(Storage.ItemCount.ToString());
    }

}