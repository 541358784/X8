using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using SomeWhere;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIBuyDiamondController : UIWindow
{
    private int LifeID = 110;
    private LocalizeTextMeshProUGUI rvCoverNumText;

    private Button rvBtn;
    private LocalizeTextMeshProUGUI rvText;
    private DailyShop _dailyShop;
    private Animator _animator;
    private string biData1;
    private string biData3;

    public override void PrivateAwake()
    {
        BindClick("Root/BGGroup/CloseButton", (go) =>
        {

            OnCloseClick(true);
        });

        rvBtn = this.transform.GetComponent<Button>("Root/ContentGroup/WatchButton");
        Transform contentGroup = this.transform.Find("Root/ContentGroup");
        rvCoverNumText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG/NumText");
        _animator = gameObject.GetComponent<Animator>();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs != null && objs.Length > 0)
        {
            biData1 = (string) objs[0];
            biData3 = (string) objs[1];
            // GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventCoinPop, biData1,TaskModuleManager.Instance.GetCurMaxTaskID().ToString(),biData3);
        }
    }

    void Start()
    {
        var rvAd = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup,
            ADConstDefine.RV_GET_DIAMOND);
        var bs = AdConfigHandle.Instance.GetBonus(rvAd.Bonus);
        int rvGetCount = bs == null && bs.Count <= 0 ? 10 : bs[0].count;
        UIAdRewardButton.Create(ADConstDefine.RV_GET_DIAMOND, UIAdRewardButton.ButtonStyle.Disable, rvBtn.gameObject,
            (s) =>
            {
                if (AdSubSystem.Instance.GetNeedPlayCount(ADConstDefine.RV_GET_DIAMOND) -
                    AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_DIAMOND) > 0 &&
                    AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_DIAMOND) > 0)
                    rvText?.SetTerm(AdSubSystem.Instance.GetRvText(ADConstDefine.RV_GET_DIAMOND));
            }, false, null, () =>
            {
                // GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventCoinRv, biData1,TaskModuleManager.Instance.GetCurMaxTaskID().ToString(),biData3);
                if (AdSubSystem.Instance.GetNeedPlayCount(ADConstDefine.RV_GET_DIAMOND) -
                    AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_DIAMOND) <= 1)
                    OnCloseClick();
            });

        rvText = transform.Find("Root/ContentGroup/WatchButton/Text").GetComponent<LocalizeTextMeshProUGUI>();
        rvText.SetTerm(AdSubSystem.Instance.GetRvText(ADConstDefine.RV_GET_DIAMOND));
        rvCoverNumText.SetText("+" + rvGetCount);
    }

    private void OnCloseClick(bool isJumpStore = false)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            // if (isJumpStore)
            // {
            //     UIStoreController.OpenUI("diamond_lack_flash");
            // }
        }));
    }

    public static bool TryShow(string src, string data3)
    {
        // if (AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_GET_DIAMOND))
        // {
        //     UIManager.Instance.OpenUI(UINameConst.UIBuyDiamond, src, data3);
        //     return true;
        // }

        return false;
    }

    public override void ClickUIMask()
    {
        OnCloseClick(true);
    }
}