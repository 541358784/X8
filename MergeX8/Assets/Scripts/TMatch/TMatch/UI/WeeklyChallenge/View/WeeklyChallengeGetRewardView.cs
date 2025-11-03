using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class WeeklyChallengeGetRewardViewParam : UIViewParam
    {
        public WeeklyChallengeItemViewParam curItem;
        public WeeklyChallengeItemViewParam nextItem;
        public Transform src;
        public Action claim;
        public Action finish;
    }

    [AssetAddress("TMatch/Prefabs/UICommonWeeklyChallegeReward")]
    public class WeeklyChallengeGetRewardView : UIView
    {
        // protected override bool IsChildView => true;

        [ComponentBinder("")] private Animator animator;

        [ComponentBinder("Root/RewardGroup/Root")]
        private Transform root;

        [ComponentBinder("RewardItem")] private Transform rewardItem;
        [ComponentBinder("ContinueButton")] private Button continueButton;

        private WeeklyChallengeGetRewardViewParam derivedParam;
        private WeeklyChallengeItemView rewardView;

        public override async void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            derivedParam = param as WeeklyChallengeGetRewardViewParam;
            rewardView = AddChildView<WeeklyChallengeItemView>(rewardItem.gameObject, derivedParam.curItem);
            FlyEffectManager.Instance.Fly(derivedParam.src, root, rewardView.gameObject, null, null, null, 1, 0.5f, scaleFrom: 1, scaleTo: 1.81f, needInstantiate: false);
            continueButton.onClick.AddListener(CliamOnClick);

            continueButton.interactable = false;
            transform.GetComponent<Image>().DOColor(new Color(0.0f, 0.0f, 0.0f, 172.0f / 255.0f), 0.1f);
            await Task.Delay(500);
            animator.Play("appear");
            continueButton.interactable = true;
        }

        private async void CliamOnClick()
        {
            continueButton.interactable = false;
            // bi
            List<ItemData> biRewards = new List<ItemData>();
            biRewards.Add(derivedParam.curItem.data);

            
            var itemIdConnect = "";
            var itemCntConnect = "";
            for (var i = 0; i < biRewards.Count; i++)
            {
                var reward = biRewards[i];
                itemIdConnect += reward.id.ToString();
                itemCntConnect += reward.cnt.ToString();
                if (i != biRewards.Count - 1)
                {
                    itemIdConnect += ",";
                    itemCntConnect += ",";
                }
            }
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventChallengeChestGetTm,
                data1: itemIdConnect,
                data2: itemCntConnect,
                data3:StorageManager.Instance.GetStorage<StorageTMatch>().WeeklyChallenge.CurLevel.ToString());

            derivedParam.claim?.Invoke();
            if (derivedParam.nextItem != null)
            {
                rewardItem.transform.DOScale(0.0f, 0.2f);
                await Task.Delay(200);
                rewardView.Refresh(derivedParam.nextItem);
                rewardItem.transform.DOScale(1.81f, 0.2f);
                animator.Play("switch");
                await Task.Delay(500);
                transform.GetComponent<Image>().DOColor(new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.1f);
                FlyEffectManager.Instance.Fly(root, derivedParam.src, rewardView.gameObject, null, null, null, 1, 0.5f, scaleFrom: 1.81f, scaleTo: 1f, needInstantiate: false);
                await Task.Delay(500);
                derivedParam.finish?.Invoke();
                rewardItem.transform.DOKill();
                UIViewSystem.Instance.Close<WeeklyChallengeGetRewardView>();
            }
            else
            {
                derivedParam.finish?.Invoke();
                UIViewSystem.Instance.Close<WeeklyChallengeGetRewardView>();
            }
        }
    }
}