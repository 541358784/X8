using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupPayRebateController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;
    private Button _button;
    private LocalizeTextMeshProUGUI _buttonText;

    private Button _buttonClose;
    private PayRebateConfig _payRebateConfig;
    
    private static string coolTimeKey = "PayRebate";
    public override void PrivateAwake()
    {
        _timeText=GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _buttonText=GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text");
        _button=GetItem<Button>("Root/Button");
        _button.onClick.AddListener(OnClaimBtn);
        _buttonClose=GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseBtn);
       
        Init();
    }

    private void Init()
    {
        _payRebateConfig = PayRebateModel.Instance.GetPayRebateConfig();
        if (_payRebateConfig != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (UserData.Instance.IsResource(_payRebateConfig.RewardID[i]))
                {
                    GetItem<Image>("Root/ItemGroup/"+(i+1)+"/Icon").sprite=UserData.GetResourceIcon(_payRebateConfig.RewardID[i]);
                }
                else
                {
                    var itemConfig= GameConfigManager.Instance.GetItemConfig(_payRebateConfig.RewardID[i]);
                    GetItem<Image>("Root/ItemGroup/"+(i+1)+"/Icon").sprite= MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                }
                GetItem<LocalizeTextMeshProUGUI>("Root/ItemGroup/"+(i+1)+"/Text").SetText(_payRebateConfig.RewardNum[i].ToString());
            }
        }

        string key = PayRebateModel.Instance.IsCanClaim() ? "UI_button_claim_text" : "ui_recharge_return_button";
        _buttonText.SetTerm(key);
        UpdateTimeText();
        InvokeRepeating("UpdateTimeText",1,1);
    }

    private void UpdateTimeText()
    {
        _timeText.SetText(PayRebateModel.Instance.GetActivityLeftTimeString());
    }
    private void OnClaimBtn()
    {
        if (PayRebateModel.Instance.IsCanClaim())
        {
            PayRebateModel.Instance.Claim();
            var ret = new List<ResData>();
            for (int i = 0; i < _payRebateConfig.RewardID.Count; i++)
            {
                ret.Add(new ResData(_payRebateConfig.RewardID[i],
                    _payRebateConfig.RewardNum[i]));
                if (!UserData.Instance.IsResource(_payRebateConfig.RewardID[i]))
                {
                    var config = GameConfigManager.Instance.GetItemConfig(_payRebateConfig.RewardID[i]);
                    if (config != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonRechargeReturn,
                            itemAId = config.id,
                            ItemALevel = config.level,
                            isChange = true,
                        }); 
                    }
                  
                }
            }
        
            var reasonArgs =
                new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.RechargeReturn);
            
            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController,
                true, reasonArgs,
                () => { AnimCloseWindow(); });
            
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRebateReward);

        }
        else
        {
            AnimCloseWindow(() =>
            {
                UIStoreController.OpenUI("",ShowArea.gem_shop);
            }); 
        }
    }

    private void OnCloseBtn()
    {
        if (PayRebateModel.Instance.IsCanClaim())
        {
            PayRebateModel.Instance.Claim();
            var ret = new List<ResData>();
            for (int i = 0; i < _payRebateConfig.RewardID.Count; i++)
            {
                ret.Add(new ResData(_payRebateConfig.RewardID[i],
                    _payRebateConfig.RewardNum[i]));
                if (!UserData.Instance.IsResource(_payRebateConfig.RewardID[i]))
                {
                    var config = GameConfigManager.Instance.GetItemConfig(_payRebateConfig.RewardID[i]);
                    if (config != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonRechargeReturn,
                            itemAId = config.id,
                            ItemALevel = config.level,
                            isChange = true,
                        }); 
                    }
                  
                }
            }
        
            var reasonArgs =
                new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.RechargeReturn);
            
            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController,
                true, reasonArgs,
                () => { AnimCloseWindow(); });
            
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRebateReward);

        }
        else
        {
            AnimCloseWindow();
        }
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRebateClose);

    }
    public static bool CanShowUI()
    {
        if (!PayRebateModel.Instance.CanShowUI())
            return false;

        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;

        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
            CommonUtils.GetTimeStamp());

        UIManager.Instance.OpenUI(UINameConst.UIPopupPayRebate);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRebatePop);

        return true;
    }
    private void OnDestroy()
    {
    }

}