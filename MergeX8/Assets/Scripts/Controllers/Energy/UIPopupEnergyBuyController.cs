
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupEnergyBuyController : UIWindowController
{
    private Button _closeBtn;

    private LocalizeTextMeshProUGUI _timeText;
    private List<EnergyBuyItem> _items;

    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnBtnClose);
        _items = new List<EnergyBuyItem>();
        for (int i = 1; i < 4; i++)
        {
            var tran=transform.Find("Root/Gift" + i);
            var item=tran.gameObject.AddComponent<EnergyBuyItem>();
            _items.Add(item);
        }
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTimeText", 0, 1);
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var packages = GlobalConfigManager.Instance.GetEnergyPackages();
        for (int i = 0; i < packages.Count; i++)
        {
            _items[i].Init(packages[i]);
        }
    }
    public void UpdateTimeText()
    {
        _timeText.SetText(EnergyPackageModel.Instance.GetGetPackageLeftTimeString());
    }


    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

  

}