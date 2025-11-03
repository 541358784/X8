
using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupClimbTreeEndController: UIWindowController
{
    private Button _buttonClose;
    private Button _buttonCollect;
    private Transform rewardItem;
    
    public override void PrivateAwake()
    {
        _buttonCollect = GetItem<Button>("Root/Button");
        _buttonCollect.onClick.AddListener(OnBtnCollect);
        
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonClose.onClick.AddListener(OnBtnCollect);
        _buttonClose.gameObject.SetActive(false);
        
        rewardItem = transform.Find("Root/Scroll View/Viewport/Content/Item");
        rewardItem.gameObject.SetActive(false);
        

    }
    protected override void OnOpenWindow(params object[] objs)
    {
        InitUI();
    }

    public void InitUI()
    {
        var resDatas = ClimbTreeModel.Instance.GetUnCollectRewards();
        foreach (var res in resDatas)
        {
           var item =Instantiate(rewardItem, rewardItem.parent);
           item.gameObject.SetActive(true);
           InitRewardItem(item,res.id,res.count);
        }
    }

    private void OnBtnCollect()
    {
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonkeyReward};
        var unCollectLevels = ClimbTreeModel.Instance.GetUnCollectLevels();
        var unCollectRewards = ClimbTreeModel.Instance.GetUnCollectRewards();
        CommonRewardManager.Instance.PopCommonReward(unCollectRewards, CurrencyGroupManager.Instance.currencyController,
            true, reasonArgs, () =>
            {
                AnimCloseWindow(() =>
                {
                    for (var i = 0; i < unCollectLevels.Count; i++)
                    {
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonkeyReward,unCollectLevels[i].ToString());   
                    }
                    for (int i = 0; i < unCollectRewards.Count; i++)
                    {
                        var reward = unCollectRewards[i];
                        if (!UserData.Instance.IsResource(reward.id))
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            {
                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMonkeyReward,
                                itemAId = reward.id,
                                isChange = true,
                            });
                        }
                        // UserData.Instance.AddRes(reward.id, reward.count,
                        //     new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonkeyReward}, true);
                    }
                    ClimbTreeModel.Instance.CleanUnCollectRewardsList();
                }); 
            });
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

        var  text=rewardItem.Find("Num")?.GetComponent<LocalizeTextMeshProUGUI>();
        if(text!=null)
            text.SetText("x"+rewardCount);

    }
    public static bool CanShowUI()
    {
        if (ClimbTreeModel.Instance.GetUnCollectRewards().Count > 0)
        {
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonkeyReward};
            var unCollectLevels = ClimbTreeModel.Instance.GetUnCollectLevels();
            var unCollectRewards = ClimbTreeModel.Instance.GetUnCollectRewards();
            CommonRewardManager.Instance.PopActivityUnCollectReward(unCollectRewards, reasonArgs, null, () =>
            {
                for (var i = 0; i < unCollectLevels.Count; i++)
                {
                    GameBIManager.Instance.SendGameEvent(
                        BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonkeyReward,
                        unCollectLevels[i].ToString());
                }
                for (int i = 0; i < unCollectRewards.Count; i++)
                {
                    var reward = unCollectRewards[i];
                    if (!UserData.Instance.IsResource(reward.id))
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType
                                .MergeChangeReasonMonkeyReward,
                            itemAId = reward.id,
                            isChange = true,
                        });
                    }
                }
                ClimbTreeModel.Instance.CleanUnCollectRewardsList();
            });
            // UIManager.Instance.OpenUI(UINameConst.UIClimbTreeUnSelect);
            return true;
        }
        return false;
    }

}
