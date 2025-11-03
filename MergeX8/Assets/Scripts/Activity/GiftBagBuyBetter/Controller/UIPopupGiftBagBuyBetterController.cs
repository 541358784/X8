using System;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine.UI;

public partial class UIPopupGiftBagBuyBetterController:UIWindowController
{
    private Button CloseBtn;
    private List<LevelGroup> LevelGroups = new List<LevelGroup>();
    private LocalizeTextMeshProUGUI TimeText;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        for (var i = 1; i <= 3; i++)
        {
            var levelGroup = transform.Find("Root/Gift"+i).gameObject.AddComponent<LevelGroup>();
            LevelGroups.Add(levelGroup);
        }
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.GIFT_BAG_BUY_BETTER_PURCHASE_REFRESH, PurchseRefresh);
        EventDispatcher.Instance.AddEventListener(EventEnum.REWARD_POPUP, RewardPopup);
        EventDispatcher.Instance.AddEventListener(EventEnum.NOTICE_POPUP, RewardPopup);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GIFT_BAG_BUY_BETTER_PURCHASE_REFRESH, PurchseRefresh);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.REWARD_POPUP, RewardPopup);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NOTICE_POPUP, RewardPopup);
    }

    public void UpdateTime()
    {
        TimeText.SetText(GiftBagBuyBetterModel.Instance.GetActivityLeftTimeString());
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var levelConfigs = GiftBagBuyBetterModel.Instance.GetGiftBagBuyBetterResources();
        for (var i = 0; i < LevelGroups.Count; i++)
        {
            if (i < levelConfigs.Count)
            {
                LevelGroups[i].SetConfig(levelConfigs[i],i,this);
                LevelGroups[i].UpdateUI();
            }
        }
    }

    private LevelGroup _curGiftBagData;
    private void PurchseRefresh(BaseEvent e)
    {
        if (_curGiftBagData == null)
            return;

        if (e == null || e.datas == null || e.datas.Length < 1)
            return;

        int index = (int) e.datas[0];

        if (_curGiftBagData.Index != index)
            return;
        _curGiftBagData.PerformComplete();
        if (LevelGroups.Count > _curGiftBagData.Index + 1)
        {
            LevelGroups[_curGiftBagData.Index + 1].PerformUnlock();
        }
        else
        {
            OnClickCloseBtn();
        }
    }

    public void OnClickCloseBtn()
    {
        if (isClose)
            return;
        isClose = true;
        AnimCloseWindow();
    }

    private bool isClose = false;
    public void GetLevelReward(LevelGroup level)
    {
        if (isClose)
            return;
        _curGiftBagData = level;
        GiftBagBuyBetterModel.Instance.GiftBagBuyBetterGetReward(level.Index);
    }
    
    private void RewardPopup(BaseEvent baseEvent)
    {
        // CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
    }

    public static UIPopupGiftBagBuyBetterController Open()
    {
        if (!GiftBagBuyBetterModel.Instance.IsOpened())
            return null;
        return UIManager.Instance.OpenUI(UINameConst.UIPopupGiftBagBuyBetter) as UIPopupGiftBagBuyBetterController;
    }
    private static string coolTimeKey = "UIPopupGiftBagBuyBetter";
    public static bool CanShowUI()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;

        if (Open())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
}