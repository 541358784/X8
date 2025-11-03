using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Dlugin;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Network.API.Protocol;
using Newtonsoft.Json;

namespace DragonPlus.Config.TMatch
{
    public partial class TMatchConfigManager
    {
        public LevelChest GetLevelChestReward(int curIndex) 
        {
            var listCount = LevelChestList.Count;
            return LevelChestList[curIndex % listCount];
        }
    }
}


namespace TMatch
{
    public class UILobbyMainViewLevelChest : UIView
    {
        protected override bool IsChildView => true;

        [ComponentBinder("ButtonGradeBox")] private Button _levelChestButton;

        [ComponentBinder("ClaimButton")] private Button _levelChestClaimButton;

        [ComponentBinder("Fill")] private Image fill;

        [ComponentBinder("VFX")] private Transform vfx;

        private bool _isLevelChestFull = false;

        // 当前的奖励配置
        private LevelChest _curLevelRewardInfo;
        private List<ItemData> _rewards;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            var levelChestUnlock = TMatchConfigManager.Instance.GlobalList[0].LevelChestUnlock;
            var userLevel = TMatchModel.Instance.GetMainLevel();
            _levelChestButton.gameObject.SetActive(userLevel >= levelChestUnlock);

            _levelChestButton.onClick.AddListener(OnLevelChestButtonClicked);
            _levelChestClaimButton.onClick.AddListener(OnLevelChestButtonClicked);
            UpdateLevelChestProgress(false);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);
        }

        public override void OnViewDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);
            base.OnViewDestroy();
        }

        private async void UpdateLevelChestProgress(bool isAni)
        {
            var totalLevel = StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.TotalLevel;
            var curIndex = StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.CurIndex;
            _curLevelRewardInfo = TMatchConfigManager.Instance.GetLevelChestReward(curIndex);
            _isLevelChestFull = totalLevel >= _curLevelRewardInfo.level;

            float curProgress = (float)totalLevel / (float)_curLevelRewardInfo.level;
            curProgress = curProgress > 1.0f ? 1.0f : curProgress;
            if (isAni)
            {
                DOTween.Kill(fill);
                fill.DOFillAmount(Mathf.Lerp(0.1f, 0.9f, curProgress), 0.2f);
                vfx.gameObject.SetActive(true);
                DOTween.Kill(vfx);
                vfx.DOLocalRotate(new Vector3(0.0f, 0.0f, Mathf.Lerp(120.0f, -120.0f, curProgress)), 0.2f).onComplete += () =>
                {
                    vfx.gameObject.SetActive(false);
                    _levelChestClaimButton.gameObject.SetActive(_isLevelChestFull);
                };
            }
            else
            {
                fill.fillAmount = Mathf.Lerp(0.1f, 0.9f, curProgress);
                ;
                _levelChestClaimButton.gameObject.SetActive(_isLevelChestFull);
            }

        }

        private void OnLevelChestButtonClicked()
        {
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventLevelChestClickTm);
            if (!_isLevelChestFull)
            {
                UIViewSystem.Instance.Open<UILevelChestPopup>();
            }
            else
            {
                ClaimLevelChestRewards();
            }
        }

        private void ClaimLevelChestRewards()
        {
            var itemDatas = GenerateLevelChestReward();
            _rewards = itemDatas;
            UIGetRewardParam param = new UIGetRewardParam();
            param.itemChangeReasonArgs = new DragonPlus.GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.LevelBoxTm);
            param.itemDatas = itemDatas;
            param.closeAction = LevelChesClaimCallBack;
            UIViewSystem.Instance.Open<UIGetGradeRewardView>(param);
        }

        private void LevelChesClaimCallBack()
        {
            var itemIdConnect = "";
            var itemCntConnect = "";
            for (var i = 0; i < _rewards.Count; i++)
            {
                var reward = _rewards[i];
                itemIdConnect += reward.id.ToString();
                itemCntConnect += reward.cnt.ToString();
                if (i != _rewards.Count - 1)
                {
                    itemIdConnect += ",";
                    itemCntConnect += ",";
                }
            }
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventLevelChestGetTm, data1: itemIdConnect,data2:itemCntConnect);
            StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.TotalLevel -= _curLevelRewardInfo.level;
            StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.LastShowedLevel = 0;
            StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.CurIndex += 1;

            UpdateLevelChestProgress(false);
        }

        // 根据配置的值来生成奖励
        private List<ItemData> GenerateLevelChestReward()
        {
            var allRewards = new List<ItemData>();
            var randomIndex1 = CommonUtils.GetRandomWeightResult(_curLevelRewardInfo.randomRewardPro1.ToList());
            if (randomIndex1 < _curLevelRewardInfo.randomRewardID1.Length)
            {
                allRewards.Add(new ItemData() { id = _curLevelRewardInfo.randomRewardID1[randomIndex1], cnt = _curLevelRewardInfo.randomRewardCnt1[randomIndex1] });
            }

            var randomIndex2 = CommonUtils.GetRandomWeightResult(_curLevelRewardInfo.randomRewardPro2.ToList());
            if (randomIndex2 < _curLevelRewardInfo.randomRewardID2.Length)
            {
                allRewards.Add(new ItemData() { id = _curLevelRewardInfo.randomRewardID2[randomIndex2], cnt = _curLevelRewardInfo.randomRewardCnt2[randomIndex2] });
            }

            var randomIndex3 = CommonUtils.GetRandomWeightResult(_curLevelRewardInfo.randomRewardPro3.ToList());
            if (randomIndex3 < _curLevelRewardInfo.randomRewardID3.Length)
            {
                allRewards.Add(new ItemData() { id = _curLevelRewardInfo.randomRewardID3[randomIndex3], cnt = _curLevelRewardInfo.randomRewardCnt3[randomIndex3] });
            }

            return allRewards;
        }

        private async void OnTMatchResultExecute(BaseEvent evt)
        {
            TMatchResultExecuteEvent realEvt = evt as TMatchResultExecuteEvent;
            if (realEvt.ExecuteType != TMatchResultExecuteType.Last) return;
            if (!realEvt.LevelData.win) return;

            StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.TotalLevel += 1;
            UpdateLevelChestProgress(true);
        }
    }
}