using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupGiftBagSendThreeController:UIWindowController
{
    private static string coolTimeKey = "UIPopupGiftBagSendThree";
    public static bool CanShowUI()
    {
        if (!GiftBagSendThreeModel.Instance.IsOpened())
            return false;
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;
        if (Open())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
    public static UIPopupGiftBagSendThreeController Instance;
    public static UIPopupGiftBagSendThreeController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance =
            UIManager.Instance.OpenUI(UINameConst.UIPopupGiftBagSendThree) as UIPopupGiftBagSendThreeController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        
    }

    private StorageGiftBagSendThree Storage => GiftBagSendThreeModel.Instance.StorageGiftBagSendThree;
    private List<GiftBagSendThreeResource> RewardConfig;
    private Button ClosetBtn;
    private LocalizeTextMeshProUGUI TimeText;
    private List<GiftNode> GiftNodeList = new List<GiftNode>();
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        RewardConfig = GiftBagSendThreeModel.Instance.GetGiftBagSendThreeResources();
        var i = 0;
        foreach (var config in RewardConfig)
        {
            if (i == 0)
            {
                if (config.ConsumeAmount > 0)
                {
                    GetItem<LocalizeTextMeshProUGUI>("Root/Vip/Text").SetText(VipStoreModel.Instance.GetVipScoreString(config.ConsumeAmount));   
                }
                else
                {
                    GetItem<Transform>("Root/Vip").gameObject.SetActive(false);
                }
            }
            
            i++;
            var node = transform.Find("Root/Gift" + i);
            if (node == null)
                break;
            var giftNode = node.gameObject.AddComponent<GiftNode>();
            giftNode.Init(config);
            GiftNodeList.Add(giftNode);
        }

        ClosetBtn = GetItem<Button>("Root/ButtonClose");
        ClosetBtn.onClick.AddListener(()=>AnimCloseWindow());
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTimeText",0,1);
    }

    public void UpdateTimeText()
    {
        TimeText.SetText(GiftBagSendThreeModel.Instance.GetActivityLeftTimeString());
        if (GiftBagSendThreeModel.Instance.GetActivityLeftTime() <= 0)
        {
            AnimCloseWindow();
            CancelInvoke("UpdateTimeText");
        }
    }

    public void OnBuy()
    {
        foreach (var gift in GiftNodeList)
        {
            gift.UpdateState();
        }
    }
    public class GiftNode : MonoBehaviour
    {
        private GiftBagSendThreeResource Config;
        private Button BuyBtn;
        private Text BuyBtnText;
        private Button CollectBtn;
        private Button FinishBtn;
        private Button LockBtn;
        private Transform DefaultItem;
        private Button _rvRebornButton;
        private LocalizeTextMeshProUGUI _rvText;
        private LocalizeTextMeshProUGUI _rvTextGrey;
        private List<CommonRewardItem> ItemList = new List<CommonRewardItem>();
        private StorageGiftBagSendThree Storage => GiftBagSendThreeModel.Instance.StorageGiftBagSendThree;
        public void Init(GiftBagSendThreeResource config)
        {
            Config = config;
            BuyBtn = transform.Find("ButtonBuy")?.GetComponent<Button>();
            if (BuyBtn)
            {
                BuyBtn.onClick.AddListener(() =>
                {
                    StoreModel.Instance.Purchase(Config.ConsumeAmount);
                });
                BuyBtnText = BuyBtn.transform.Find("Text").GetComponent<Text>();
                if (Config.ConsumeType == 2)
                {
                    BuyBtnText.text = StoreModel.Instance.GetPrice(Config.ConsumeAmount);
                }
            }
            _rvRebornButton = transform.Find("ButtonRv")?.GetComponent<Button>();
            if (_rvRebornButton)
            {
                _rvRebornButton.gameObject.SetActive(true);
                _rvRebornButton.onClick.RemoveAllListeners();
                UIAdRewardButton rvBtn = null;
                rvBtn = UIAdRewardButton.Create(ADConstDefine.R_GIFT_BAG_SEND_3, UIAdRewardButton.ButtonStyle.Disable, _rvRebornButton.gameObject,
                    (s) =>
                    {
                        if (s)
                        {
                            DestroyImmediate(rvBtn);
                            GiftBagSendThreeModel.Instance.OnRvSuccess(Config);
                        }
                    }, false, null, () =>
                    {
                    });
            }

            _rvText = transform.Find("ButtonRv/Text")?.GetComponent<LocalizeTextMeshProUGUI>();
            _rvTextGrey = transform.Find("ButtonRv/GreyText")?.GetComponent<LocalizeTextMeshProUGUI>();
            CollectBtn = transform.Find("Button")?.GetComponent<Button>();
            if (CollectBtn)
            {
                CollectBtn.onClick.AddListener(() =>
                {
                    GiftBagSendThreeModel.Instance.GiftBagSendThreeGetReward(Config);
                    UpdateState();
                });
            }
            FinishBtn = transform.Find("ButtonFinish")?.GetComponent<Button>();
            LockBtn = transform.Find("ButtonLock")?.GetComponent<Button>();
            UpdateState();
            DefaultItem = transform.Find("ItemGroup/Item");
            DefaultItem.gameObject.SetActive(false);
            var rewards = CommonUtils.FormatReward(config.RewardID, config.Amount);
            foreach (var reward in rewards)
            {
                var rewardItem = Instantiate(DefaultItem, DefaultItem.parent).gameObject.AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                rewardItem.Init(reward);
                ItemList.Add(rewardItem);
            }
        }

        public void UpdateState()
        {
            if (Storage.CollectState.Contains(Config.Id))
            {
                FinishBtn?.gameObject.SetActive(true);
                LockBtn?.gameObject.SetActive(false);
                CollectBtn?.gameObject.SetActive(false);
                BuyBtn?.gameObject.SetActive(false);
                _rvRebornButton?.gameObject.SetActive(false);
            }
            else if (GiftBagSendThreeModel.Instance.CanCollect(Config))
            {
                CollectBtn?.gameObject.SetActive(true);
                FinishBtn?.gameObject.SetActive(false);
                LockBtn?.gameObject.SetActive(false);
                BuyBtn?.gameObject.SetActive(false);
                _rvRebornButton?.gameObject.SetActive(false);
            }
            else if (Config.ConsumeType == 2)
            {
                CollectBtn?.gameObject.SetActive(false);
                FinishBtn?.gameObject.SetActive(false);
                LockBtn?.gameObject.SetActive(false);
                if (Config.ConsumeAmount > 0)
                {
                    BuyBtn?.gameObject.SetActive(true);
                    _rvRebornButton?.gameObject.SetActive(false);   
                }
                else
                {
                    BuyBtn?.gameObject.SetActive(false);
                    _rvRebornButton?.gameObject.SetActive(true);
                    _rvText?.SetText(Storage.RvTimes+"/"+-Config.ConsumeAmount);
                    _rvTextGrey?.SetText(Storage.RvTimes+"/"+-Config.ConsumeAmount);
                }
            }
            else
            {
                LockBtn?.gameObject.SetActive(true);
                BuyBtn?.gameObject.SetActive(false);
                CollectBtn?.gameObject.SetActive(false);
                FinishBtn?.gameObject.SetActive(false);
                _rvRebornButton?.gameObject.SetActive(false);
            }
        }
    }
}