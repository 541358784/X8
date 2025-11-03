using Activity.GardenTreasure.Model;
using Activity.TreasureHuntModel;
using DragonPlus;
using DragonPlus.Config.ButterflyWorkShop;
using DragonPlus.Config.GardenTreasure;
using DragonPlus.Config.TreasureHunt;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
public class UIPopupGardenTreasureGiftController : UIWindowController
{
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _timeText;

    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnBtnCLose);
        for (int i = 1; i <= 3; i++)
        {
           var item=  transform.Find("Root/Gift" + i).gameObject.AddComponent<GardenTreasureGiftItem>();
           item.Init(GardenTreasureConfigManager.Instance.GardenTreasurePackageConfigList[i-1]);
        }
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("RefreshTime",0,1);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);

        string source = "1";
        if (objs != null && objs.Length >= 1)
            source = (string)objs[0];
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGardenTreasurePackage,  source);
    }

    public void RefreshTime()
    {
        _timeText.SetText(GardenTreasureModel.Instance.GetActivityLeftTimeString());
    }
    private void OnBtnCLose()
    {
        AnimCloseWindow();
    }

}
