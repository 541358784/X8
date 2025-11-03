using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class CollectCoinGateView : UIView
    {
        [ComponentBinder("")] private Button gateButton;
        [ComponentBinder("Fill")] private Image fill;
        [ComponentBinder("VFX")] private Transform vfx;
        [ComponentBinder("TipsBG")] private Transform tipsBG;
        [ComponentBinder("TimeText")] private LocalizeTextMeshProUGUI timeText;
        [ComponentBinder("ClaimButton")] private Button claimButton;

        [ComponentBinder("UIActivityAddItems")]
        private Animator addItems;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            OnActivityUpdateEvt(null);
            gateButton.onClick.AddListener(GateButtonOnClick);
            claimButton.onClick.AddListener(ClaimButtonOnClick);
            EventDispatcher.Instance.AddEventListener(EventEnum.LobbyRefreshShow, OnLobbyRefreshShow);
            EventDispatcher.Instance.AddEventListener(EventEnum.ACTIVITY_UPDATE, OnActivityUpdateEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);
        }

        public override void OnViewDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.LobbyRefreshShow, OnLobbyRefreshShow);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.ACTIVITY_UPDATE, OnActivityUpdateEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);
            base.OnViewDestroy();
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
            if (!gameObject.activeSelf) return;
            // if (CollectCoinActivityModel.Instance.CanClaimReward()) return;
            // timeText.SetText(CollectCoinActivityModel.Instance.GetActivityLeftTimeString());
        }

        private void Refresh(bool anim)
        {
            // for (int i = 1; i <= 3; i++)
            // {
            //     transform.Find($"Icon{i}").gameObject.SetActive(CollectCoinActivityModel.Instance.GetStorageActivityData().Round == i);
            // }
            // bool canClaimReward = CollectCoinActivityModel.Instance.CanClaimReward();
            // gateButton.interactable = !canClaimReward;
            // tipsBG.gameObject.SetActive(!canClaimReward);
            // float progress = CollectCoinActivityModel.Instance.GetCurRoundProgress();
            // if (anim)
            // {
            //     DOTween.Kill(fill);
            //     fill.DOFillAmount(Mathf.Lerp(0.1f, 0.9f, progress), 0.2f);
            //     vfx.gameObject.SetActive(true);
            //     DOTween.Kill(vfx);
            //     vfx.DOLocalRotate(new Vector3(0.0f, 0.0f, Mathf.Lerp(120.0f, -120.0f, progress)), 0.2f).onComplete += () =>
            //     {
            //         vfx.gameObject.SetActive(false);
            //         claimButton.gameObject.SetActive(canClaimReward);
            //     };
            // }
            // else
            // {
            //     fill.fillAmount = Mathf.Lerp(0.1f, 0.9f, progress);
            //     vfx.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Lerp(120.0f, -120.0f, progress)));
            //     claimButton.gameObject.SetActive(canClaimReward);
            // }
        }

        private void GateButtonOnClick()
        {
            // DragonPlus.GameBIManager.SendGameEvent(BiEventMatchFrenzy.Types.GameEventType.GameEventCollectionChestClick, data1:"coin");
            // UIViewSystem.Instance.Open<CollectCoinView>();
        }

        private void ClaimButtonOnClick()
        {
            // DragonPlus.GameBIManager.SendGameEvent(BiEventMatchFrenzy.Types.GameEventType.GameEventCollectionChestClick, data1:"coin");
            // UIGetCoinCollectionRewardParam param = new UIGetCoinCollectionRewardParam();
            // param.itemChangeReasonArgs =
            //     new DragonPlus.GameBIManager.ItemChangeReasonArgs
            //     {
            //         reason = BiEventMatchFrenzy.Types.ItemChangeReason.CollectionActivite,
            //         data1 = "coin"
            //     };
            // param.level = CollectCoinActivityModel.Instance.GetStorageActivityData().Round;
            // param.itemDatas = CollectCoinActivityModel.Instance.GetCurRoundRewards();
            // param.fly = false;
            // param.closeAction = () =>
            // {
            //     DragonPlus.GameBIManager.SendGameEvent(BiEventMatchFrenzy.Types.GameEventType.GameEventCollectionChestGet, 
            //         data1: JsonConvert.SerializeObject(param.itemDatas), 
            //         data2: "coin");
            //     foreach (var p in param.itemDatas)
            //     {
            //         DragonPlus.Config.Game.Item cfg = TMatch.GameConfigManager.Instance.GetItem(p.id);
            //         EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId)cfg.ResourceId));
            //     }
            //     CollectCoinActivityModel.Instance.GetStorageActivityData().Round++;
            //     OnActivityUpdateEvt(null);
            //     if (gameObject.activeSelf)
            //     {
            //         UIViewSystem.Instance.Open<CollectCoinView>();
            //     }
            // };
            // UIViewSystem.Instance.Open<UIGetCoinCollectionRewardView>(param);
        }

        private async void OnLobbyRefreshShow(BaseEvent evt)
        {
            if (!gameObject.activeSelf) OnActivityUpdateEvt(null);
        }

        private void OnActivityUpdateEvt(BaseEvent evt)
        {
            // gameObject.SetActive(CollectCoinActivityModel.Instance.IsActivityOpened());
            if (gameObject.activeSelf) Refresh(false);
        }

        private async void OnTMatchResultExecute(BaseEvent evt)
        {
            // TMatchResultExecuteEvent realEvt = evt as TMatchResultExecuteEvent;
            // if (realEvt.ExecuteType != TMatchResultExecuteType.Last) return;
            // if (!realEvt.LevelData.win) return;
            // if (!CollectCoinActivityModel.Instance.IsActivityOpened() && !CollectCoinActivityModel.Instance.IsActivityInReward()) return;
            // if(CollectCoinActivityModel.Instance.configManager.BaseList.Count == 0) return;
            // DragonPlus.Config.Game.Item cfg = TMatch.GameConfigManager.Instance.GetItem(CollectCoinActivityModel.Instance.configManager.BaseList[0].ItemID);
            // int cnt = CollectCnt(realEvt.LevelData.tripleItems);
            // if (cnt == 0) return;
            //
            // //极端情况：开始后活动已结束但还在奖励领取展示事件范围内，但此时回来加上局内收集的数量达到了领取条件
            // {
            //     if (!CollectCoinActivityModel.Instance.IsActivityOpened())
            //     {
            //         if (CollectCoinActivityModel.Instance.IsActivityInReward() && CollectCoinActivityModel.Instance.CanClaimReward(-cnt))
            //         {
            //             gameObject.SetActive(true);
            //             Refresh(false);
            //         }
            //         else
            //         {
            //             return;
            //         }
            //     }
            // }
            // addItems.gameObject.SetActive(false);
            // addItems.gameObject.SetActive(true);
            // addItems.transform.Find("Root/NumberText").GetComponent<TextMeshProUGUI>().SetText($"+{cnt}");
            // addItems.transform.Find("Root/Icon").GetComponent<Image>().sprite = ResourcesManager.Instance.GetSpriteVariant(cfg.Atlas, cfg.Icon);
            // addItems.Play("appear01");
            // addItems.Update(0);
            // int delaytime = 0;
            // if (addItems.GetCurrentAnimatorClipInfo(0) != null && addItems.GetCurrentAnimatorClipInfo(0).Length > 0)
            // {
            //     delaytime = (int)(addItems.GetCurrentAnimatorClipInfo(0)[0].clip.length * 1000.0f);
            // }
            // await Task.Delay(delaytime);
            // CollectCoinActivityModel.Instance.GetStorageActivityData().CollectNum += cnt;
            // GameBIManager.Instance.LevelInfo.CollectionCount += (uint)cnt;
            // Refresh(true);
        }

        public static int CollectCnt(Dictionary<int, int> tripleItems)
        {
            // if (!CollectCoinActivityModel.Instance.IsActivityOpened() && !CollectCoinActivityModel.Instance.IsActivityInReward()) return 0;
            // DragonPlus.Config.Game.Item cfg = TMatch.GameConfigManager.Instance.GetItem(CollectCoinActivityModel.Instance.configManager.BaseList[0].ItemID);
            // int cnt = 0;
            // foreach (var p in tripleItems)
            // {
            //     if (p.Key == cfg.MatchItemId)
            //     {
            //         cnt += p.Value;
            //     }
            // }
            // return cnt;
            return 0;
        }
    }
}