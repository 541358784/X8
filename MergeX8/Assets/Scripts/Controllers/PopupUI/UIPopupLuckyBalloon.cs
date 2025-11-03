using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupLuckyBalloonController : UIWindowController
{
    private Transform _rewardItem = null;

    private bool isAdPlay = false;

    private Animator _animator;
    private List<ResData> _rewardList;
    private Action<bool> callBack;
    private ConsumeExtend consumeExtend;
    private Button buttonAds;
    private Transform buttonLoading;
    private Button buttonBuy;
    private Button buttonBuyNew;
    private Text _priceText;
    private string str = "none";
    private BalloonPack _balloonPack;
    private bool IsPay = false;
    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();
        _rewardItem = transform.Find("Root/MidGroup/RewardGroup/Rewards/Reward1");
        buttonLoading = transform.Find("Root/ButtonGroup/ADSLoadingButton");
        _rewardItem.gameObject.SetActive(false);
        buttonAds = GetItem<Button>("Root/ButtonGroup/PlayButton");
        buttonBuy = GetItem<Button>("Root/ButtonGroup/ButtonBuy");
        buttonBuyNew = GetItem<Button>("Root/ButtonGroup/ButtonBuyNew");
        _priceText=GetItem<Text>("Root/ButtonGroup/ButtonBuyNew/Text");
        consumeExtend = AdConfigHandle.Instance.GetConsumeExtendID(AdConfigHandle.Instance.GetLuckBalloonData());
        buttonLoading.gameObject.SetActive(false);
        Button buttonClose = GetItem<Button>("Root/CloseButton");
        buttonClose.onClick.AddListener(() =>
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBalloonClose, str);

            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
            {
                callBack?.Invoke(false);
                CloseWindowWithinUIMgr(true);
            }));
        });
        
    }



    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs != null && objs.Length > 0)
        {
            _rewardList = objs[0] as List<ResData>;
            callBack = (Action<bool>) objs[1];
            IsPay = (bool) objs[2];
            _balloonPack = (BalloonPack) objs[3];
        }

        if (_rewardList != null)
        {
            for (int i = 0; i < _rewardList.Count; i++)
            {
                var item = Instantiate(_rewardItem, _rewardItem.parent);
                item.gameObject.SetActive(true);
                if (UserData.Instance.IsResource(_rewardList[i].id))
                {
                    item.transform.Find("Icon").GetComponent<Image>().sprite =
                        UserData.GetResourceIcon(_rewardList[i].id,UserData.ResourceSubType.Reward);
                }
                else
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(_rewardList[i].id);
                    item.transform.Find("Icon").GetComponent<Image>().sprite =
                        MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                }
        
                
                item.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>()
                    .SetText(_rewardList[i].count.ToString());
            }
        }

        if (IsPay)
        {
            InitWithPay();
        }
        else
        {
            AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip,AdLocalOperate.Open);
            InitWithRv();
        }
        
    }
    
    protected override void OnCloseWindow(bool destroy = false)
    {
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip,AdLocalOperate.Close,AdLocalSkipScene.LuckBalloon);

        base.OnCloseWindow(destroy);
    }


    private void InitWithPay()
    {
        buttonBuyNew.gameObject.SetActive(true);
        buttonAds.gameObject.SetActive(false);
        buttonBuy.gameObject.SetActive(false);
        buttonBuyNew.onClick.AddListener(() =>
        {
            var  storageHome = StorageManager.Instance.GetStorage<StorageHome>();
            storageHome.Balloon.ShopID = _balloonPack.Id;
            StoreModel.Instance.Purchase(_balloonPack.ShopItem);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBalloonClick, str,
                _balloonPack.Id.ToString());
        });
        _priceText.text = StoreModel.Instance.GetPrice(_balloonPack.ShopItem);
        
        transform.Find("Root/ButtonGroup/ButtonBuyNew/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(_balloonPack.ShopItem));

        str = _balloonPack.Id.ToString();
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBalloonPop, str);
    }

    private void InitWithRv()
    {
        buttonBuyNew.gameObject.SetActive(false);
        UIAdRewardButton.Create(ADConstDefine.RV_BALLOON, UIAdRewardButton.ButtonStyle.Hide, buttonAds.gameObject,
            (s) =>
            {
                if (s)
                {
                    AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip,AdLocalOperate.Operate);
                    callBack?.Invoke(true);
                    CloseWindowWithinUIMgr(true);
                    AddReward();
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBalloonGain, str,
                        "rv");
                }
            }, true, () =>
            {
                if (consumeExtend == null || !ConsumeExtendManager.Instance.CanShowConsumeExtend(consumeExtend.Id))
                    buttonLoading.gameObject.SetActive(!AdLogicManager.Instance.ShouldShowRV(ADConstDefine.RV_BALLOON));
                return true;
            },
            () =>
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBalloonClick, str,
                    "rv");
            });
        if (consumeExtend == null || !ConsumeExtendManager.Instance.CanShowConsumeExtend(consumeExtend.Id) ||
            (!UserData.Instance.CanAford((UserData.ResourceId) consumeExtend.ConsumeType, consumeExtend.ConsumeValue) &&
             AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_BALLOON)))
        {
            buttonBuy.gameObject.SetActive(false);
        }
        else
        {
            Image iconImage = transform.Find("Root/ButtonGroup/ButtonBuy/Diamond").GetComponent<Image>();
            if (iconImage != null)
                iconImage.sprite = UserData.GetResourceIcon(consumeExtend.ConsumeType);

            LocalizeTextMeshProUGUI text = transform.Find("Root/ButtonGroup/ButtonBuy/Text")
                .GetComponent<LocalizeTextMeshProUGUI>();
            if (text != null)
                text.SetText(consumeExtend.ConsumeValue.ToString());

            buttonBuy.onClick.AddListener(() =>
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBalloonClick, str,
                    "diamond");

                switch ((UserData.ResourceId) consumeExtend.ConsumeType)
                {
                    case UserData.ResourceId.Diamond:
                    {
                        int needCount = consumeExtend.ConsumeValue;
                        if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, consumeExtend.ConsumeValue))
                        {
                            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "LuckBalloon",
                                consumeExtend.ConsumeValue.ToString(), "LuckBalloon",true,needCount);
                        }
                        else
                        {
                            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, consumeExtend.ConsumeValue,
                                new GameBIManager.ItemChangeReasonArgs
                                {
                                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.BalloonGems
                                });

                            AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip,AdLocalOperate.Operate);
                            callBack?.Invoke(true);
                            CloseWindowWithinUIMgr(true);
                            AddReward();
                            ConsumeExtendManager.Instance.ADConsumeRecord(consumeExtend.Id);
                            GameBIManager.Instance.SendGameEvent(
                                BiEventAdventureIslandMerge.Types.GameEventType.GameEventBalloonGain, str, "diamond");
                        }

                        break;
                    }
                }
            });
        }

        if (AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_BALLOON) && buttonBuy.IsActive())
        {
            str = "dia&rv";
        }
        else
        {
            if (buttonBuy.IsActive())
            {
                str = "diamond";
            }
            else
            {
                str = "rv";
            }
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBalloonPop, str);
    }
    public void AddReward()
    {
        if (_rewardList != null)
        {
            List<ResData> rewards = new List<ResData>();
            for (int i = 0; i < _rewardList.Count; i++)
            {
                int itemId = _rewardList[i].id;
                ResData resData = new ResData(itemId, _rewardList[i].count);
                rewards.Add(resData);
                TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(resData.id);
                if (mergeItemConfig == null)
                    continue;
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeBalloon,
                    itemAId = mergeItemConfig.id,
                    ItemALevel = mergeItemConfig.level,
                    isChange = true,
                    extras = new Dictionary<string, string>
                    {
                    }
                });
            }

            var _storageLuckBallRecord = StorageManager.Instance.GetStorage<StorageHome>().LuckBall;
            _storageLuckBallRecord.PlayCount++;
            _storageLuckBallRecord.LastPlayTime = CommonUtils.GetTimeStamp();

            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                true, new GameBIManager.ItemChangeReasonArgs(), () => { });
        }
    }

    public static void ShowUI(List<ResData> _rewardList, Action<bool> callBack,bool isPay,BalloonPack balloonPack)
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupLuckyBalloon, _rewardList, callBack,isPay,balloonPack);
    }
    public override void ClickUIMask()
    {
        base.ClickUIMask();

        callBack?.Invoke(false);
    }
    public static void PurchaseSuccess()
    {
        List<ResData> listResData = new List<ResData>();
        var  storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        storageHome.Balloon.PayCount++;
        var balloonPack = AdConfigHandle.Instance.GetBallPack(storageHome.Balloon.ShopID);
        if (balloonPack != null)
        {
            for (int i = 0; i < balloonPack.Content.Count; i++)
            {
                listResData.Add(new ResData(balloonPack.Content[i],balloonPack.Count[i]));
            }
        }

        var reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap;
        CommonRewardManager.Instance.PopCommonReward(listResData,
            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = reason,
            }, () =>
            {
                PayRebateModel.Instance.OnPurchaseAniFinish();
                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                var windows= UIManager.Instance.GetOpenUI<UIPopupLuckyBalloonController>();
                if (windows != null)
                    windows.AnimCloseWindow();
                MergeMainBalloon.Instance.Reset();

            });
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);

        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBalloonGain,
            balloonPack.Id.ToString());
    }
}