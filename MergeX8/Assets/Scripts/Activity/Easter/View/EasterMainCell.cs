
using System;
using System.Collections;
using System.Collections.Generic;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class EasterMainCell: MonoBehaviour
{

    private Transform _itemReward;
    private Transform _item1;
    private Transform _item2;
    private Transform _item3;
    private Transform _itemFinish;
    
    private Transform _buildReward;
    private Transform _buildFinish;

    private Button _getButton;
    private LocalizeTextMeshProUGUI _lvText;
    private EasterReward _reward;
    public enum RewardStatus
    {
        None,
        Finish,
        CanClaim,
    }
    private void Awake()
    {
        _itemReward =transform.Find("ItemReward");
        _item1=_itemReward.Find("Item1");
        _item2=_itemReward.Find("Item2");
        _item3=_itemReward.Find("Item3");
        _itemFinish = _itemReward.Find("Finish");

        _buildReward = transform.Find("BuildReward");
        _buildFinish = _buildReward.Find("Finish");

        _lvText = transform.Find("Lv/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _getButton = transform.Find("GetButton").GetComponent<Button>();
        _getButton.onClick.AddListener(OnGetBtn);
    }

    public void HideGetBtn()
    {
        _getButton.interactable = false;
    }
    private IEnumerator AddReward()
    {
         Vector3 endPos = Vector3.zero;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            endPos = MergeMainController.Instance.rewardBtnPos;
        }
        else
        {
            endPos = UIHomeMainController.mainController.MainPlayTransform.position;
        }
        
        for (int i = 0; i < _reward.RewardId.Count; i++)
        {
            if (UserData.Instance.IsResource( _reward.RewardId[i]))
            {
                UserData.Instance.AddRes(_reward.RewardId[i],_reward.RewardNum[i],
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EasterGet
                    }, false);
                FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    (UserData.ResourceId)_reward.RewardId[i], _reward.RewardNum[i], transform.position, 0.8f, true, true, 0.15f,
                    () =>
                    {
                     
                    });
            }

            else
            {
                var itemConfig=GameConfigManager.Instance.GetItemConfig(_reward.RewardId[i]);
                if (itemConfig != null)
                {
                    var mergeItem = MergeManager.Instance.GetEmptyItem();
                    mergeItem.Id = _reward.RewardId[i];
                    mergeItem.State = 1;
                    MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonEasterReward,
                        itemAId =mergeItem.Id,
                        isChange = true,
                    });
                    FlyGameObjectManager.Instance.FlyObject(_reward.RewardId[i], transform.position, endPos, 1.2f, 2.0f, 1f,
                        () => { EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH); });
                }
                else
                {
                    var decoItem = DecoWorld.ItemLib[_reward.RewardId[i]];
                    DecoManager.Instance.UnlockDecoBuilding(_reward.RewardId[i]);
                    if (decoItem._node.IsOwned)
                    {
                        UIEasterMainController controller =
                            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIEasterMain) as UIEasterMainController;
                        if (controller != null)
                        {
                            var tempReawrd=_reward;
                            controller.HideButton();
                            controller.AnimCloseWindow(() =>
                            {
                                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                                {
                                    SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,DecoOperationType.Install, tempReawrd.RewardId);
                                }
                                else
                                {
                                    DecoManager.Instance.InstallItem(tempReawrd.RewardId);
                                }
                            });
                        }
                    }
                    else if(i == 0)
                    {
                        DecoManager.Instance.UnlockDecoBuilding(_reward.RewardId[i]);
                        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                        {
                            DescString = LocalizationManager.Instance.GetLocalizedString("ui_easter_node_lock_tips"),
                        });
                    }
                   
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void OnGetBtn()
    {
        if (EasterModel.Instance.IsClaimed(_reward.Id))
            return;
        AudioManager.Instance.PlaySound(20);
        EasterModel.Instance.Claim(_reward.Id);
        CoroutineManager.Instance.StartCoroutine(AddReward());
     

    }

    private void InitItem(Transform item,int itemID,int ItemCount,EasterReward reward=null)
    {
        
        var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
        if (UserData.Instance.IsResource(itemID) )
        {
            item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID,UserData.ResourceSubType.Reward);
        }
        else
        {
            if (itemConfig != null)
            {
                item.Find("Icon").GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else
            {
                item.Find("Icon").GetComponent<Image>().sprite  = ResourcesManager.Instance.GetSpriteVariant($"EasterAtlas", reward.Image); 
            }
        }

        item.Find("Num").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemCount.ToString());
        item.gameObject.SetActive(true);
        bool isRes = UserData.Instance.IsResource(itemID);
        var infoBtn = item.transform.Find("TipsBtn");
        if (infoBtn != null)
        {
            bool isBox = itemConfig != null && (itemConfig.type == (int) MergeItemType.box ||
                                                itemConfig.type == (int) MergeItemType.choiceChest);
            infoBtn.gameObject.SetActive(!isRes && isBox  || reward!=null  );
            
            if (!isRes)
            {
                var itemButton = CommonUtils.GetComponent<Button>(infoBtn.gameObject);
                if (itemConfig != null)
                {
                    itemButton.onClick.AddListener(() =>
                    {
                        MergeInfoView.Instance.OpenMergeInfo(itemID, null);
                        UIPopupMergeInformationController controller =
                            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                        if (controller != null)
                            controller.SetResourcesActive(false);
                    });
                }
                else
                {
                    itemButton.onClick.AddListener(() =>
                    {
                        UIEasterMainController controller =
                            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIEasterMain) as UIEasterMainController;
                        if (controller == null)
                        {
                            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                            {
                                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,DecoOperationType.Preview ,reward.RewardId);
                            }
                            else
                            {
                                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NODE_PREVIEW,reward.RewardId);
                            }
                        }
                        else
                        {
                            controller.HideButton();
                            controller.AnimCloseWindow(() =>
                            {
                                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                                {
                                    SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,DecoOperationType.Preview ,reward.RewardId);
                                }
                                else
                                {
                                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NODE_PREVIEW,reward.RewardId);
                                }
                          
                            });
                        }
                        
                    });
                }
                
                
            }
        }
    }
    public void Init(EasterReward reward,RewardStatus status)
    {
        _reward = reward;
        _lvText.SetText(reward.Id.ToString());
        
        if (reward.RewardId.Count==1 || reward.IsBuild)
        {
            _item2.gameObject.SetActive(false);
            _item3.gameObject.SetActive(false);
            _buildReward.gameObject.SetActive(false);
            var itemConfig = GameConfigManager.Instance.GetItemConfig( reward.RewardId[0]);
            if (itemConfig != null)
            {
                InitItem(_item1, reward.RewardId[0],reward.RewardNum[0]);
            }
            else
            {
                InitItem(_buildReward, reward.RewardId[0],reward.RewardNum[0],reward);
                _buildReward.gameObject.SetActive(true);
                _itemReward.gameObject.SetActive(false);
            }
        }else if (reward.RewardId.Count == 2)
        {
            _item2.gameObject.SetActive(true);
            _item3.gameObject.SetActive(false);
            _buildReward.gameObject.SetActive(false);
            InitItem(_item1, reward.RewardId[0],reward.RewardNum[0]);       
            InitItem(_item2, reward.RewardId[1],reward.RewardNum[1]);     
        }
        else
        {
            _item2.gameObject.SetActive(true);
            _item3.gameObject.SetActive(true);
            _buildReward.gameObject.SetActive(false);
            InitItem(_item1, reward.RewardId[0],reward.RewardNum[0]);       
            InitItem(_item2, reward.RewardId[1],reward.RewardNum[1]);       
            InitItem(_item3, reward.RewardId[2],reward.RewardNum[2]);       
        }
        _getButton.gameObject.SetActive(false);
        _buildFinish.gameObject.SetActive(false);
        _itemFinish.gameObject.SetActive(false);
        switch (status)
        {
           case RewardStatus.None:
               break;    
           case RewardStatus.Finish:
               _buildFinish.gameObject.SetActive(true);
               _itemFinish.gameObject.SetActive(true);
               break;    
           case RewardStatus.CanClaim:
               _getButton.gameObject.SetActive(true);
               break;
        }
   
    }
}
