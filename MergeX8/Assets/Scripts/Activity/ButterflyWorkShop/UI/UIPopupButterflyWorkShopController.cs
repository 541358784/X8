using Activity.TreasureHuntModel;
using DragonPlus;
using DragonPlus.Config.ButterflyWorkShop;
using DragonPlus.Config.TreasureHunt;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
public class UIPopupButterflyWorkShopController : UIWindowController
{
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _timeText;

    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnBtnCLose);
        for (int i = 1; i <= 3; i++)
        {
           var item=  transform.Find("Root/Gift" + i).gameObject.AddComponent<ButterflyWorkShopGiftItem>();
           item.Init(ButterflyWorkShopModel.Instance.ButterflyWorkShopPackageConfigList[i-1]);
        }
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("RefreshTime",0,1);

    }
    public void RefreshTime()
    {
        _timeText.SetText(ButterflyWorkShopModel.Instance.GetActivityLeftTimeString());
    }
    private void OnBtnCLose()
    {
        AnimCloseWindow();
    }

}
