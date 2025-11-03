using DragonPlus.Config.TMatch;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;
using DG.Tweening;
using Dlugin;
using DragonU3DSDK.Network.API.Protocol;
using Newtonsoft.Json;

namespace DragonPlus.Config.TMatch
{
    public partial class TMatchConfigManager
    {
        public StarChest GetStarChestReward(int curIndex)
        {
            var listCount = StarChestList.Count;
            return StarChestList[curIndex % listCount];
        }
    }
}


namespace TMatch
{
    public class UILobbyMainViewStarChest : UIView
    {
        protected override bool IsChildView => true;

        [ComponentBinder("ButtonStars")] private Button starChestButton;

        [ComponentBinder("ClaimButton")] private Button starChestClaimButton;

        [ComponentBinder("Fill")] private Image fill;

        [ComponentBinder("UIActivityAddItems")]
        private Animator addItems;

        [ComponentBinder("VFX")] private Transform vfx;

        private bool isStarChestFull = false;

        // 当前的奖励配置
        private StarChest curRewardInfo;
        private List<ItemData> rewards;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            var starChestUnlock = TMatchConfigManager.Instance.GlobalList[0].StarChestUnlcok;
            var userLevel = TMatchModel.Instance.GetMainLevel();
            starChestButton.gameObject.SetActive(userLevel >= starChestUnlock);
            starChestButton.onClick.AddListener(OnStarChestButtonClicked);
            starChestClaimButton.onClick.AddListener(OnStarChestButtonClicked);
            UpdateStarChestProgress(false);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);
        }

        public override void OnViewDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);
            base.OnViewDestroy();
        }

        private void UpdateStarChestProgress(bool isAni)
        {
            var totalStars = StorageManager.Instance.GetStorage<StorageTMatch>().StarChest.TotalStars;
            var curIndex = StorageManager.Instance.GetStorage<StorageTMatch>().StarChest.CurIndex;
            curRewardInfo = TMatchConfigManager.Instance.GetStarChestReward(curIndex);
            isStarChestFull = totalStars >= curRewardInfo.star;

            float curProgress = (float)totalStars / (float)curRewardInfo.star;
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
                    starChestClaimButton.gameObject.SetActive(isStarChestFull);
                };
            }
            else
            {
                fill.fillAmount = Mathf.Lerp(0.1f, 0.9f, curProgress);
                ;
                starChestClaimButton.gameObject.SetActive(isStarChestFull);
            }

        }

        private void OnStarChestButtonClicked()
        {
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStarChestClickTm);
            if (!isStarChestFull)
            {
                UIViewSystem.Instance.Open<UIStarChestPopup>();
            }
            else
            {
                ClaimStarChestRewards();
            }
            // UpdateStarChestProgress(true);
        }

        private void ClaimStarChestRewards()
        {
            var itemDatas = GenerateStarChestReward();
            rewards = itemDatas;
            UIGetRewardParam param = new UIGetRewardParam();
            param.itemChangeReasonArgs = new DragonPlus.GameBIManager.ItemChangeReasonArgs { reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.StartBoxTm };
            param.itemDatas = itemDatas;
            param.closeAction = StarChesClaimCallBack;
            UIViewSystem.Instance.Open<UIGetStarsRewardView>(param);
        }

        private void StarChesClaimCallBack()
        {
            var itemIdConnect = "";
            var itemCntConnect = "";
            for (var i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];
                itemIdConnect += reward.id.ToString();
                itemCntConnect += reward.cnt.ToString();
                if (i != rewards.Count - 1)
                {
                    itemIdConnect += ",";
                    itemCntConnect += ",";
                }
            }
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStarChestGetTm, data1:itemIdConnect,data2:itemCntConnect);
            StorageManager.Instance.GetStorage<StorageTMatch>().StarChest.TotalStars -= curRewardInfo.star;
            StorageManager.Instance.GetStorage<StorageTMatch>().StarChest.CurIndex += 1;

            UpdateStarChestProgress(false);
        }

        // 根据配置的值来生成奖励
        private List<ItemData> GenerateStarChestReward()
        {
            var allRewards = new List<ItemData>();
            var randomIndex1 = CommonUtils.GetRandomWeightResult(curRewardInfo.randomRewardPro1.ToList());
            if (randomIndex1 < curRewardInfo.randomRewardID1.Length)
            {
                allRewards.Add(new ItemData() { id = curRewardInfo.randomRewardID1[randomIndex1], cnt = curRewardInfo.randomRewardCnt1[randomIndex1] });
            }

            var randomIndex2 = CommonUtils.GetRandomWeightResult(curRewardInfo.randomRewardPro2.ToList());
            if (randomIndex2 < curRewardInfo.randomRewardID2.Length)
            {
                allRewards.Add(new ItemData() { id = curRewardInfo.randomRewardID2[randomIndex2], cnt = curRewardInfo.randomRewardCnt2[randomIndex2] });
            }

            var randomIndex3 = CommonUtils.GetRandomWeightResult(curRewardInfo.randomRewardPro3.ToList());
            if (randomIndex3 < curRewardInfo.randomRewardID3.Length)
            {
                allRewards.Add(new ItemData() { id = curRewardInfo.randomRewardID3[randomIndex3], cnt = curRewardInfo.randomRewardCnt3[randomIndex3] });
            }

            return allRewards;
        }

        private async void OnTMatchResultExecute(BaseEvent evt)
        {
            TMatchResultExecuteEvent realEvt = evt as TMatchResultExecuteEvent;
            if (realEvt.ExecuteType != TMatchResultExecuteType.Last) return;
            if (!realEvt.LevelData.win) return;
            var cnt = StarCurrencyModel.Instance.GetStarCnt(realEvt.LevelData.layoutCfg.levelTimes, realEvt.LevelData.LastTimes);
            StorageManager.Instance.GetStorage<StorageTMatch>().StarChest.TotalStars += cnt;
            addItems.gameObject.SetActive(false);
            addItems.gameObject.SetActive(true);
            addItems.transform.Find("Root/NumberText").GetComponent<TextMeshProUGUI>().SetText($"+{cnt}");

            addItems.Play("appear01");
            addItems.Update(0);
            await Task.Delay((int)(addItems.GetCurrentAnimatorClipInfo(0)[0].clip.length * 1000.0f));
            UpdateStarChestProgress(true);
        }

    }
}