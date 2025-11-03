using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupKeepPetBagController:UIWindowController
{
    public static UIPopupKeepPetBagController Open(StorageKeepPet storage)
    {
        if (storage.GetCurState().Enum != KeepPetStateEnum.SearchFinish)
            return null;
        return UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetBag, storage) as UIPopupKeepPetBagController;
    }

    private Transform DefaultItem;
    private List<SearchTaskRewardItem> ItemList;
    private List<SearchTaskRewardItem> SelectList;
    private Button ReceiveBtn;
    private Button BuyBtn;
    private LocalizeTextMeshProUGUI PriceText;
    private LocalizeTextMeshProUGUI TitleText;
    private LocalizeTextMeshProUGUI BtnText;
    private Button CloseBtn;
    private Transform TreasureMapTip;
    public override void PrivateAwake()
    {
        DefaultItem = transform.Find("Root/Scroll View/Viewport/Content/Item");
        DefaultItem.gameObject.SetActive(false);
        ReceiveBtn = GetItem<Button>("Root/ButtonGroup/ReceiveButton");
        ReceiveBtn.onClick.AddListener(OnClickReceiveBtn);
        BuyBtn = GetItem<Button>("Root/ButtonGroup/BuyButton");
        BuyBtn.onClick.AddListener(OnClickBuyBtn);
        PriceText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonGroup/BuyButton/Root/NumText");
        TitleText = GetItem<LocalizeTextMeshProUGUI>("Root/BgGroup/Text");
        BtnText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonGroup/ReceiveButton/Root/GreyText");
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        TreasureMapTip = transform.Find("Root/TreasureMap");
    }

    private StorageKeepPet Storage;
    private KeepPetSearchTaskConfig TaskConfig;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageKeepPet;
        TaskConfig = KeepPetModel.Instance.SearchTaskConfig.Find(a => a.Id == Storage.SearchTaskId);
        ItemList = new List<SearchTaskRewardItem>();
        foreach (var pair in Storage.SearchTaskRewardList)
        {
            var item = Instantiate(DefaultItem, DefaultItem.parent);
            item.gameObject.SetActive(true);
            var rewardItem = item.gameObject.AddComponent<SearchTaskRewardItem>();
            rewardItem.Init(this, new ResData(pair.Id, pair.Count));
            ItemList.Add(rewardItem);
        }
        SelectList = new List<SearchTaskRewardItem>();
        UpdateViewState();
        TreasureMapTip.gameObject.SetActive(Storage.GetTreasureMap);
        if (Storage.GetTreasureMap)
            UITreasureMapRewardController.Open();
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetSearchFinish2, "");
    }
    public void UpdateViewState()
    {
        PriceText.SetText(KeepPetModel.Instance.GlobalConfig.SearchTaskPickCountPrice.ToString());
        var chooseFull = SelectList.Count == TaskConfig.PickCount + Storage.SearchTaskExtraSelectRewardCount;
        if (chooseFull)
            TitleText.SetTerm("ui_dog_search_reward_desc2");
        else
        {
            TitleText.SetTerm("ui_dog_search_reward_desc");
            TitleText.SetTermFormats((TaskConfig.PickCount + Storage.SearchTaskExtraSelectRewardCount - SelectList.Count).ToString());
        }
        BtnText.SetTermFormats(SelectList.Count.ToString(),TaskConfig.PickCount + Storage.SearchTaskExtraSelectRewardCount);
        BuyBtn.gameObject.SetActive(TaskConfig.PickCount + Storage.SearchTaskExtraSelectRewardCount < TaskConfig.RewardCount);
        ReceiveBtn.interactable = !Received && SelectList.Count == TaskConfig.PickCount + Storage.SearchTaskExtraSelectRewardCount;
        if (ReceiveBtn.interactable)
        {
            // if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetSearchFinish3))
            // {
            //     List<Transform> topLayer = new List<Transform>();
            //     topLayer.Add(ReceiveBtn.transform);
            //     GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetSearchFinish3, ReceiveBtn.transform as RectTransform,
            //         topLayer: topLayer);
            //     GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetSearchFinish3, "");
            // }
        }
    }

    private bool Received = false;
    public void OnClickReceiveBtn()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetSearchFinish3);
        if (Received)
            return;
        if (SelectList.Count != TaskConfig.PickCount + Storage.SearchTaskExtraSelectRewardCount)
            return;
        ReceiveBtn.interactable = false;
        Received = true;
        Storage.CollectSearchTaskReward = true;
        var oldValue = Storage.Exp;
        Storage.Exp += TaskConfig.Exp;
        var newValue = Storage.Exp;
        KeepPetModel.Instance.UpdateDailyTaskOnExpChange(oldValue,newValue);
        KeepPetModel.Instance.CheckLevelUpBi(oldValue, newValue);
        Action performAddExp=()=>EventDispatcher.Instance.SendEventImmediately(new EventKeepPetExpChange(oldValue, newValue));
        var getTreasureMap = Storage.GetTreasureMap;
        var allRewardsBI = "";
        foreach (var resData in Storage.SearchTaskRewardList)
        {
            allRewardsBI += resData.Id + ",";
        }
        KeepPetModel.Instance.AwakeDog();
        KeepPetModel.Instance.CheckStateChange();
        var rewards = new List<ResData>();
        var rewardsBI = "";
        foreach (var item in SelectList)
        {
            rewards.Add(item.Reward);
            rewardsBI += item.Reward.id + ",";
        }
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetSearchTaskGet);
        UserData.Instance.AddRes(rewards, reason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.GetCurrencyUseController(),
            false, reason,animEndCall:()=>AnimCloseWindow(() =>
            {
                if (getTreasureMap)
                    UIPopupTreasureMapController.Open(performAddExp);
                else
                {
                    performAddExp();
                }
                var mainUI =
                    UIManager.Instance.GetOpenedUIByPath<UIKeepPetMainController>(UINameConst.UIKeepPetMain);
                if (mainUI)
                    mainUI.CheckDailyTaskEntrance2Guide();
            }));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetSearchGet,rewardsBI,allRewardsBI);
    }

    public void OnClickBuyBtn()
    {
        if (Received)
            return;
        if (TaskConfig.PickCount + Storage.SearchTaskExtraSelectRewardCount >= TaskConfig.RewardCount)
            return;
        var price = KeepPetModel.Instance.GlobalConfig.SearchTaskPickCountPrice;
        if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, price))
        {
            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "KeepPetBuySearchTaskRewardPickCount",
                "", "KeepPetBuySearchTaskRewardPickCount",true,price);
            return;
        }
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetUse);
        UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond,price,reason);
        Storage.SearchTaskExtraSelectRewardCount++;
        UpdateViewState();
    }

    public void OnClickRewardItem(SearchTaskRewardItem rewardItem)
    {
        if (SelectList.Contains(rewardItem))
        {
            SelectList.Remove(rewardItem);
            rewardItem.SetSelectState(false);
            UpdateViewState();
        }
        else if (SelectList.Count < TaskConfig.PickCount + Storage.SearchTaskExtraSelectRewardCount)
        {
            SelectList.Add(rewardItem);
            rewardItem.SetSelectState(true);
            UpdateViewState();
        }
    }

    public class SearchTaskRewardItem : MonoBehaviour
    {
        private CommonRewardItem RewardItem;
        public ResData Reward;
        public UIPopupKeepPetBagController Controller;
        public Button Btn;
        public bool IsSelect=false;
        private Transform Select1;
        private Transform Select2;

        private void Awake()
        {
            Select1 = transform.Find("SelectBG");
            Select2 = transform.Find("Select");
            Btn = gameObject.GetComponent<Button>();
            Btn.onClick.AddListener(()=>Controller.OnClickRewardItem(this));
        }

        public void Init(UIPopupKeepPetBagController controller,ResData reward)
        {
            Controller = controller;
            Reward = reward;
            RewardItem = gameObject.AddComponent<CommonRewardItem>();
            RewardItem.Init(Reward);
            ShieldButtonOnClick shieldButton = gameObject.GetComponent<ShieldButtonOnClick>();
            if (shieldButton)
                shieldButton.isUse = false;
        }

        public void SetSelectState(bool isSelect)
        {
            IsSelect = isSelect;
            Select1.gameObject.SetActive(IsSelect);
            Select2.gameObject.SetActive(IsSelect);
        }
    }
}