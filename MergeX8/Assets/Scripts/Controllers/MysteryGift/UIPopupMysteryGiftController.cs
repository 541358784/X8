using System.Collections.Generic;
using ABTest;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Manager;
using SomeWhere;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMysteryGiftController : UIWindow
{
    private Image _headIcon;
    private Button _rvBtn;
    private Button buttonBuy;
    private Button _closeBtn;
    private GameObject portraitSpineObj = null;
    private ConsumeExtend consumeExtend;
    private List<ResData> bs;
    private string str = "none";
    private bool isPay = false;

    private Transform _showGroup;
    private Transform _showGroupNew;

    private Button _buttonBuyNew;
    private Text _text;
    private List<Transform> _newGroupItems;
    private MysteryPack _mysteryPack;
    public override void PrivateAwake()
    {
        _rvBtn = transform.Find("Root/ButtonGroup/PlayButton").GetComponent<Button>();
        buttonBuy = transform.Find("Root/ButtonGroup/ButtonBuy").GetComponent<Button>();
        _closeBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
        _closeBtn.onClick.AddListener(() =>
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMisterygiftClose, str);
            CloseWindowWithinUIMgr(true);
        });
        
        _showGroup=  transform.Find("Root/ShowGroup");
        _showGroupNew=  transform.Find("Root/ShowGroupNew");
        _buttonBuyNew = transform.Find("Root/ButtonGroup/ButtonBuyNew").GetComponent<Button>();
        _text = transform.Find("Root/ButtonGroup/ButtonBuyNew/Text").GetComponent<Text>();
        _newGroupItems = new List<Transform>();
        for (int i = 1; i < 5; i++)
        {
           var item= transform.Find("Root/ShowGroupNew/Item"+i);
           _newGroupItems.Add(item);
        }
 
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs != null && objs.Length > 0)
        {
            isPay = (bool) objs[0];
        }
        _showGroup.gameObject.SetActive(!isPay);
        _rvBtn.gameObject.SetActive(!isPay);
        buttonBuy.gameObject.SetActive(!isPay);
        _showGroupNew.gameObject.SetActive(isPay);
        _buttonBuyNew.gameObject.SetActive(isPay);
        var packs= AdConfigHandle.Instance.GetMysteryGiftPack();
        _mysteryPack = packs.RandomPickOne();
        if (isPay && _mysteryPack!=null)
        {
            str =_mysteryPack.Id.ToString();
            _text.text = StoreModel.Instance.GetPrice(_mysteryPack.ShopItem);
            _buttonBuyNew.onClick.AddListener(() =>
            {
               var  storageGame = StorageManager.Instance.GetStorage<StorageGame>();
                storageGame.MySteryGiftShopID = _mysteryPack.Id;
                StoreModel.Instance.Purchase(_mysteryPack.ShopItem);
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMisterygiftClick, str);
            });
            for (int i = 0; i < _newGroupItems.Count; i++)
            {
                if (i < _mysteryPack.Content.Count)
                {
                    var item = _newGroupItems[i];
                    InitRewardItem(item, _mysteryPack.Content[i], _mysteryPack.Count[i]);
                }
                else
                {
                    _newGroupItems[i].gameObject.SetActive(false);
                }
                
            }
        }
        else
        {
            bs = GetRandomReward();
            UIAdRewardButton.Create(ADConstDefine.Rv_TASK_BONUS, UIAdRewardButton.ButtonStyle.Hide, _rvBtn.gameObject,
                (s) =>
                {
                    if (s)
                    {
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMisterygiftGain,
                            str, "rv");

                        AddReward();
                    }
                }, true, null, () =>
                {
                    GameBIManager.Instance.SendGameEvent(
                        BiEventAdventureIslandMerge.Types.GameEventType.GameEventMisterygiftClick, str,
                        "rv");
                    CloseWindowWithinUIMgr(true);
                });

            var tex = transform.Find("Root/ShowGroup/Item3/Text").GetComponent<LocalizeTextMeshProUGUI>();
            foreach (var resData in bs)
            {
                if (resData.id == (int) UserData.ResourceId.Energy)
                    tex.SetText("x" + resData.count);
            }

            Init();
            if (AdSubSystem.Instance.CanPlayRV(ADConstDefine.Rv_TASK_BONUS) && buttonBuy.IsActive())
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
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMisterygiftPop, str);
    }

    public void Init()
    {
        consumeExtend = AdConfigHandle.Instance.GetConsumeExtendID(AdConfigHandle.Instance.GetMysteryGiftConsumeData());
        if (consumeExtend == null || !ConsumeExtendManager.Instance.CanShowConsumeExtend(consumeExtend.Id)
                                  || !UserData.Instance.CanAford((UserData.ResourceId) consumeExtend.ConsumeType,
                                      consumeExtend.ConsumeValue) &&
                                  AdSubSystem.Instance.CanPlayRV(ADConstDefine.Rv_TASK_BONUS))
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
                switch ((UserData.ResourceId) consumeExtend.ConsumeType)
                {
                    case UserData.ResourceId.Coin:
                    {
                        if (!UserData.Instance.CanAford(UserData.ResourceId.Coin, consumeExtend.ConsumeValue))
                        {
                            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Coin, "MysteryGift",
                                consumeExtend.ConsumeValue.ToString(), "MysteryGift");
                        }
                        else
                        {
                            UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, consumeExtend.ConsumeValue,
                                new GameBIManager.ItemChangeReasonArgs
                                {
                                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MysteryGiftBuy
                                });

                            CloseWindowWithinUIMgr(true);
                            AddReward();
                            GameBIManager.Instance.SendGameEvent(
                                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMisterygiftGain, str,
                                "diamond");

                            ConsumeExtendManager.Instance.ADConsumeRecord(consumeExtend.Id);
                        }

                        break;
                    }
                    case UserData.ResourceId.Diamond:
                    {
                        int needCount = consumeExtend.ConsumeValue;
                        if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, consumeExtend.ConsumeValue))
                        {
                            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "MysteryGift",
                                consumeExtend.ConsumeValue.ToString(), "MysteryGift", true, needCount);
                        }
                        else
                        {
                            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, consumeExtend.ConsumeValue,
                                new GameBIManager.ItemChangeReasonArgs
                                {
                                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MysteryGiftBuy
                                });

                            CloseWindowWithinUIMgr(true);
                            GameBIManager.Instance.SendGameEvent(
                                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMisterygiftGain, str,
                                "diamond");
                            AddReward();
                            ConsumeExtendManager.Instance.ADConsumeRecord(consumeExtend.Id);
                        }

                        break;
                    }
                }

                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMisterygiftClick, str,
                    "diamond");
            });
        }
    }
    private void InitRewardItem(Transform rewardItem, int rewardId,int rewardCount)
    {
        var rewardImage = rewardItem.Find("Icon").GetComponent<Image>();
        if(rewardImage == null)
            return;
            
        var tipsBtn = rewardItem.Find("TipsBtn")?.GetComponent<Button>();
        if (UserData.Instance.IsResource(rewardId))
        {
            rewardImage.sprite = UserData.GetResourceIcon(rewardId,UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig= GameConfigManager.Instance.GetItemConfig(rewardId);
            if(itemConfig != null)
                rewardImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            if (tipsBtn != null)
            {
                tipsBtn.gameObject.SetActive(true);
                tipsBtn.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemConfig, null, false,true);
                });
            }
              
        }

        var  text=rewardItem.Find("Text")?.GetComponent<LocalizeTextMeshProUGUI>();
        if(text!=null)
            text.SetText("x"+rewardCount);

    }
    private void AddReward()
    {
        foreach (var resData in bs)
        {
            TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(resData.id);
            if (mergeItemConfig == null)
                continue;
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeTaskBonus,
                itemAId = mergeItemConfig.id,
                ItemALevel = mergeItemConfig.level,
                isChange = true,
                extras = new Dictionary<string, string>
                {
                }
            });
        }

        CommonRewardManager.Instance.PopCommonReward(bs, CurrencyGroupManager.Instance.GetCurrencyUseController(), true,
            new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Adv,
                data2 = ADConstDefine.Rv_TASK_BONUS
            }, () => { });
    }


    public List<ResData> GetRandomReward()
    {
        List<ResData> tempList = new List<ResData>();
        var configs = AdConfigHandle.Instance.GetMysteryGifts();
        var starNum = UserData.Instance.GetTotalDecoCoin();
        var mysteryGift = configs.Find(a => { return a.StarMin <= starNum && starNum <= a.StarMax; });
        if (mysteryGift != null)
        {
            int index = CommonUtils.RandomIndexByWeight(new List<int>(mysteryGift.Weight1));
            ResData resData1 = new ResData(mysteryGift.Reward1[index], mysteryGift.RewardNum1[index]);
            tempList.Add(resData1);
            index = CommonUtils.RandomIndexByWeight(new List<int>(mysteryGift.Weight2));
            ResData resData2 = new ResData(mysteryGift.Reward2[index], mysteryGift.RewardNum2[index]);
            tempList.Add(resData2);
            index = CommonUtils.RandomIndexByWeight(new List<int>(mysteryGift.Weight3));
            ResData resData3 = new ResData(mysteryGift.Reward3[0], mysteryGift.RewardNum3[index]);
            tempList.Add(resData3);
            index = CommonUtils.RandomIndexByWeight(new List<int>(mysteryGift.Weight4));
            ResData resData4 = new ResData(mysteryGift.Reward4[index], mysteryGift.RewardNum4[index]);
            tempList.Add(resData4);
        }

        return tempList;
    }

    public static bool CanShowUI()
    {
        if (NewIceBreakGiftBagModel.Instance.IsOpenAB())
            return false;
        if (ABTestManager.Instance.IsOpenADTest())
            return false;
        
        if (!FunctionsSwitchManager.Instance.FunctionOn(FunctionsSwitchManager.FuncType.RvGift))
            return false;

        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CustomerGift))
            return false;

        if (GuideSubSystem.Instance.IsShowingGuide())
            return false;
        var common = AdConfigHandle.Instance.GetCommon();

        StorageGame storageGame = StorageManager.Instance.GetStorage<StorageGame>();
        var packs = AdConfigHandle.Instance.GetMysteryGiftPack();
        if (packs != null && packs.Count > 0)
        {
            var pack = packs.RandomPickOne();
            if (!CommonUtils.IsSameDayWithToday((ulong) storageGame.MySteryLastShowTime))
            {
                storageGame.MySteryPayCount = 0;
                storageGame.MySteryPayShowCount = 0;
            }
            if (storageGame.MySteryPayShowCount <= pack.Max_times && storageGame.MySteryPayCount <= pack.Pay_times)
            {
            }
            else
            {
                if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.Rv_TASK_BONUS) &&
                    !ConsumeExtendManager.Instance.CanShowConsumeExtend(common.MysteryGiftConsume))
                    return false;
            }
        }
        else
        {
            if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.Rv_TASK_BONUS) &&
                !ConsumeExtendManager.Instance.CanShowConsumeExtend(common.MysteryGiftConsume))
                return false;
        }

        if (_minCount == 0 && _maxCount == 0)
        {
            string str = GlobalConfigManager.Instance.GetGlobal_Config_Number_Value("Task_Bonus_Display");
            string[] arr = str.Split(',');

            _minCount = arr[0].ToInt();
            _maxCount = arr[1].ToInt();
        }

        if (storageGame.MysteryGiftShowLimitCount == 0)
            storageGame.MysteryGiftShowLimitCount = Random.Range(_minCount, _maxCount + 1);

        if (Time.realtimeSinceStartup - MysteryGiftShowTime >
            GlobalConfigManager.Instance.GetNumValue("MysteryGiftDisplayTime") ||
            storageGame.MysteryGiftCompTaskCount >= storageGame.MysteryGiftShowLimitCount)
        {
            storageGame.MysteryGiftCompTaskCount = 0;
            MysteryGiftShowTime = (int) Time.realtimeSinceStartup;
            storageGame.MysteryGiftShowLimitCount = Random.Range(_minCount, _maxCount + 1);
            bool isPayShow = false;
            if (!CommonUtils.IsSameDayWithToday((ulong) storageGame.MySteryLastShowTime))
            {
                storageGame.MySteryPayCount = 0;
                storageGame.MySteryPayShowCount = 0;
            }
            bool canShowRvOrExtend = true;
            if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.Rv_TASK_BONUS) &&
                !ConsumeExtendManager.Instance.CanShowConsumeExtend(common.MysteryGiftConsume))
                canShowRvOrExtend= false;
            if (packs != null && packs.Count > 0)
            {
                var pack = packs.RandomPickOne();
                if (!canShowRvOrExtend||storageGame.MysteryGiftShowCount % (pack.Interval + 1) == 0 &&
                    storageGame.MySteryPayShowCount <= pack.Max_times && storageGame.MySteryPayCount <= pack.Pay_times)
                {
                    isPayShow = true;
                    storageGame.MySteryPayShowCount++;
                }
            }

            storageGame.MysteryGiftShowCount++;
            storageGame.MySteryLastShowTime = (long) APIManager.Instance.GetServerTime();
            UIManager.Instance.OpenUI(UINameConst.UIPopupMysteryGift, isPayShow);
            return true;
        }
        return false;
    }


    private static int _minCount = 0;
    private static int _maxCount = 0;
    public static int MysteryGiftShowTime;

    // public static void TryShowMysteryGift(int headIndex)
    // {
    //     StorageGame storageGame = StorageManager.Instance.GetStorage<StorageGame>();
    //
    //     if (_minCount == 0 && _maxCount == 0)
    //     {
    //         string str = GlobalConfigManager.Instance.GetGlobal_Config_Number_Value("Task_Bonus_Display");
    //         string[] arr = str.Split(',');
    //
    //         _minCount = arr[0].ToInt();
    //         _maxCount = arr[1].ToInt();
    //     }
    //
    //     if (storageGame.MysteryGiftShowLimitCount == 0)
    //         storageGame.MysteryGiftShowLimitCount = Random.Range(_minCount, _maxCount + 1);
    //
    //     if (Time.realtimeSinceStartup - MysteryGiftShowTime >
    //         GlobalConfigManager.Instance.GetNumValue("MysteryGiftDisplayTime") ||
    //         storageGame.MysteryGiftCompTaskCount >= storageGame.MysteryGiftShowLimitCount)
    //     {
    //         storageGame.MysteryGiftCompTaskCount = 0;
    //         MysteryGiftShowTime = (int) Time.realtimeSinceStartup;
    //         storageGame.MysteryGiftShowLimitCount = Random.Range(_minCount, _maxCount + 1);
    //         bool isPayShow = false;
    //         if (!CommonUtils.IsSameDayWithToday((ulong) storageGame.MySteryLastShowTime))
    //         {
    //             storageGame.MySteryPayCount = 0;
    //             storageGame.MySteryPayShowCount = 0;
    //         }
    //         var common = AdConfigHandle.Instance.GetCommon();
    //         bool canShowRvOrExtend = true;
    //         if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.Rv_TASK_BONUS) &&
    //             !ConsumeExtendManager.Instance.CanShowConsumeExtend(common.MysteryGiftConsume))
    //             canShowRvOrExtend= false;
    //
    //         var packs = AdConfigHandle.Instance.GetMysteryGiftPack();
    //         if (packs != null && packs.Count > 0)
    //         {
    //             var pack = packs.RandomPickOne();
    //             if (!canShowRvOrExtend||storageGame.MysteryGiftShowCount % (pack.Interval + 1) == 0 &&
    //                 storageGame.MySteryPayShowCount <= pack.Max_times && storageGame.MySteryPayCount <= pack.Pay_times)
    //             {
    //                 isPayShow = true;
    //                 storageGame.MySteryPayShowCount++;
    //             }
    //         }
    //
    //         storageGame.MysteryGiftShowCount++;
    //         storageGame.MySteryLastShowTime = (long) APIManager.Instance.GetServerTime();
    //         UIManager.Instance.OpenUI(UINameConst.UIPopupMysteryGift, isPayShow);
    //     }
    // }

    public static void PurchaseSuccess()
    {
        List<ResData> listResData = new List<ResData>();
        var  storageGame = StorageManager.Instance.GetStorage<StorageGame>();
        storageGame.MySteryPayCount++;
        var mysteryPack = AdConfigHandle.Instance.GetMysteryGiftPack(storageGame.MySteryGiftShopID);
        if (mysteryPack != null)
        {
            for (int i = 0; i < mysteryPack.Content.Count; i++)
            {
                listResData.Add(new ResData(mysteryPack.Content[i],mysteryPack.Count[i]));
            }
            
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMisterygiftGain, mysteryPack.Id.ToString());
        }
        var reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap;
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);

        CommonRewardManager.Instance.PopCommonReward(listResData,
            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = reason,
            }, () =>
            {
                PayRebateModel.Instance.OnPurchaseAniFinish();
                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                var windows= UIManager.Instance.GetOpenUI<UIPopupMysteryGiftController>();
                if (windows != null)
                    windows.AnimCloseWindow();
            });
    }
}