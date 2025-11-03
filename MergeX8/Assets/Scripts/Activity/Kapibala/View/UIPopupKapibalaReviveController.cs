using System;
using UnityEngine.UI;
using DragonPlus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gameplay;
using UnityEngine;


namespace TMatch
{
    public class UIPopupKapibalaReviveParam : UIViewParam
    {
        public int failType;
        public Action<bool> handleClick;
    }

    [AssetAddress("Prefabs/Activity/Kapibala/UIPopupKapibalaRevive")]
    public class UIPopupKapibalaReviveController : UIPopup
    {
        private int ReviveBoostItemId => _viewParam.failType == 1 ? 100024 : 100026;
        public override Action EmptyCloseAction => null;
        private UIPopupKapibalaReviveParam _viewParam;
        private Button ReviveBtn;
        private LocalizeTextMeshProUGUI ReviveCountText;
        private Transform DefaultRewardItem;
        private List<CommonRewardItem> GiftBagRewardList = new List<CommonRewardItem>();
        private Button GiftBagBtn;
        private Button CloseBtn;
        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);
            // TMatchDifficulty difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchSystem.LevelController.LevelData.level);
            // CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);
            _viewParam = data as UIPopupKapibalaReviveParam;
            global::EventDispatcher.Instance.AddEvent<EventKapibalaRebornCountChange>(OnRebornCountChange);

            ReviveBtn = transform.Find("Root/Button").GetComponent<Button>();
            ReviveBtn.onClick.AddListener(() =>
            {
                if (KapibalaModel.Instance.Storage.RebornCount <= 0)
                {
                    UIPopupKapibalaGiftBagController.Open(KapibalaModel.Instance.Storage);
                    return;
                }

                (TMatchSystem.LevelController as TMatchKapibalaLevelController).CostProp(
                    (int)UserData.ResourceId.KapibalaReborn, 1);
                KapibalaModel.Instance.AddRebornItem(-1,"UseByFailType"+_viewParam.failType);
                Revive();
            });
            ReviveCountText = transform.Find("Root/Button/Text").GetComponent<LocalizeTextMeshProUGUI>();
            ReviveCountText.SetText("x"+KapibalaModel.Instance.Storage.RebornCount);

            var giftBagConfig = KapibalaModel.Instance.GiftBagConfig.Last();
            var giftBagRewards = global::CommonUtils.FormatReward(giftBagConfig.RewardId, giftBagConfig.RewardNum);
            DefaultRewardItem = transform.Find("Root/Gift/ItemGroup/Item");
            DefaultRewardItem.gameObject.SetActive(false);
            for (var i = 0; i < giftBagRewards.Count; i++)
            {
                var rewardItem = Instantiate(DefaultRewardItem, DefaultRewardItem.parent).gameObject
                    .AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                rewardItem.Init(giftBagRewards[i]);
                GiftBagRewardList.Add(rewardItem);
            }
            // transform.Find("Root/Gift/Text").gameObject.SetActive(false);
            GiftBagBtn = transform.Find("Root/Gift").GetComponent<Button>();
            GiftBagBtn.onClick.AddListener(() =>
            {
                UIPopupKapibalaGiftBagController.Open(KapibalaModel.Instance.Storage);
            });
            CloseBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
            CloseBtn.onClick.AddListener(GiveUp);
        }

        public void Revive()
        {
            UIViewSystem.Instance.Close<UIPopupKapibalaReviveController>();
            EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(ReviveBoostItemId,true));
            _viewParam?.handleClick?.Invoke(true);
        }

        public void GiveUp()
        {
            UIViewSystem.Instance.Close<UIPopupKapibalaReviveController>();
            _viewParam?.handleClick?.Invoke(false);
        }

        public override Task OnViewClose()
        {
            SetAllButtonInteractable(false);
            global::EventDispatcher.Instance.RemoveEvent<EventKapibalaRebornCountChange>(OnRebornCountChange);
            return base.OnViewClose();
        }
        
        private void SetAllButtonInteractable(bool interactable)
        {
            ReviveBtn.interactable = interactable;
            GiftBagBtn.interactable = interactable;
            CloseBtn.interactable = interactable;
        }
        
        private void OnRebornCountChange(EventKapibalaRebornCountChange evt)
        {
            ReviveCountText.SetText("x"+KapibalaModel.Instance.Storage.RebornCount);
        }
    }
}