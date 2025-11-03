using System.Collections;
using System.Collections.Generic;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using StoryMovie;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIEasterEndController : UIWindowController
{
    private Button _buttonClose;
    private Button _buttonShow;
    private LocalizeTextMeshProUGUI _descText;
    private StorageEaster _storageEaster;

    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonShow = GetItem<Button>("Root/Button");
        _buttonShow.onClick.AddListener(OnBtnClose);
        _descText = GetItem<LocalizeTextMeshProUGUI>("Root/BGGroup/Text");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        _storageEaster = (StorageEaster) objs[0];

    }


    private void OnBtnClose()
    {
        _storageEaster.IsShowEndView = true;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEndData,_storageEaster.CurIndex.ToString());
        AnimCloseWindow(() =>
        {
            var rewards = EasterModel.Instance.GetCanClaimRewards(_storageEaster);
            if (rewards != null && rewards.Count>0)
            {
                CoroutineManager.Instance.StartCoroutine(AddReward(rewards));
            }

            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                MergeManager.Instance.CheckEasterReplaceItem(MergeBoardEnum.Main);

           
        });
    }

    private IEnumerator AddReward(List<EasterReward> rewards)
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
        foreach (var _reward in rewards)
        {
            for (int i = 0; i < _reward.RewardId.Count; i++)
            {
                if (UserData.Instance.IsResource(_reward.RewardId[i]))
                {
                    UserData.Instance.AddRes(_reward.RewardId[i], _reward.RewardNum[i],
                        new GameBIManager.ItemChangeReasonArgs()
                        {
                            reason = BiEventCooking.Types.ItemChangeReason.EasterGet
                        }, false);
                    FlyGameObjectManager.Instance.FlyCurrency(
                        CurrencyGroupManager.Instance.GetCurrencyUseController(),
                        (UserData.ResourceId) _reward.RewardId[i], _reward.RewardNum[i], Vector2.zero, 0.8f,
                        true, true, 0.15f,
                        () =>
                        {
                        });
                }
                else
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(_reward.RewardId[i]);
                    if (itemConfig != null)
                    {
                        var mergeItem = MergeManager.Instance.GetEmptyItem();
                        mergeItem.Id = _reward.RewardId[i];
                        mergeItem.State = 1;
                        MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType
                                .MergeChangeReasonEasterReward,
                            isChange = true,
                        });
                        FlyGameObjectManager.Instance.FlyObject(_reward.RewardId[i], Vector2.zero, endPos,
                            1.2f, 2.0f, 1f,
                            () =>
                            {
                                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                            });
                    }
                    else
                    {
                        DecoManager.Instance.UnlockDecoBuilding(_reward.RewardId[i]);
                        DecoManager.Instance.ApplyItem(_reward.RewardId);
                    }
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
}