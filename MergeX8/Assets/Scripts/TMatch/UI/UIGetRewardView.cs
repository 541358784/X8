using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class UIGetRewardParam : UIViewParam
    {
        public List<ItemData> itemDatas; //item
        public bool autoAddReward = true; //是否自动加上奖励
        public bool fly = true; //是否飞

        public Action closeAction; //界面关闭时的回调

        // bi changeReason 一般来说必传参数
        public DragonPlus.GameBIManager.ItemChangeReasonArgs itemChangeReasonArgs;
    }

    [AssetAddress("TMatch/Prefabs/UICommonReward")]
    public class UIGetRewardView : UIView
    {
        [ComponentBinder("")] protected Animator animator;
        [ComponentBinder("ClaimButton")] protected Button claimButton;
        [ComponentBinder("ContinueButton")] public Button continueButton;

        [ComponentBinder("Root/RewardGroup/Root")]
        protected Transform rewardRoot;

        public UIGetRewardParam paramData;
        protected List<UIItemView> itemViews = new List<UIItemView>();
        public virtual bool hasBox => false;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            if (hasBox)
            {
                AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Reward);
            }

            paramData = param as UIGetRewardParam;
            List<GameObject> gameObjects = new List<GameObject>();
            for (int i = 1; i <= 5; i++) rewardRoot.Find($"UICommonItem{i}").gameObject.SetActive(false);
            if (paramData.itemDatas.Count == 1)
            {
                GameObject rewardItem3 = rewardRoot.Find("UICommonItem3").gameObject;
                rewardItem3.SetActive(true);
                gameObjects.Add(rewardItem3);
            }
            else if (paramData.itemDatas.Count == 2)
            {
                GameObject rewardItem2 = rewardRoot.Find("UICommonItem2").gameObject;
                rewardItem2.SetActive(true);
                gameObjects.Add(rewardItem2);
                GameObject rewardItem4 = rewardRoot.Find("UICommonItem4").gameObject;
                rewardItem4.SetActive(true);
                gameObjects.Add(rewardItem4);
            }
            else
            {
                GameObject rewardItem1 = rewardRoot.Find("UICommonItem1").gameObject;
                rewardItem1.SetActive(true);
                gameObjects.Add(rewardItem1);
                GameObject rewardItem3 = rewardRoot.Find("UICommonItem3").gameObject;
                rewardItem3.SetActive(true);
                gameObjects.Add(rewardItem3);
                GameObject rewardItem5 = rewardRoot.Find("UICommonItem5").gameObject;
                rewardItem5.SetActive(true);
                gameObjects.Add(rewardItem5);
            }

            for (int i = 0; i < paramData.itemDatas.Count; i++)
            {
                var itemView = AddChildView<UIItemView>(gameObjects[i], new ItemViewParam() { data = paramData.itemDatas[i] });
                itemViews.Add(itemView);
            }

            if (null != claimButton) claimButton.onClick.AddListener(ClaimOnClick);
            if (null != continueButton) continueButton.onClick.AddListener(ContinueOnClick);
        }

        protected void ClaimOnClick()
        {
            claimButton.interactable = false;
            animator.Play("claim");
            AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Reward_open);
        }

        protected void ContinueOnClick()
        {
            if (paramData.fly)
            {
                // AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Reward_fly);
                foreach (var itemView in itemViews)
                {
                    itemView.Fly();
                }
            }

            UIViewSystem.Instance.Close(GetType());
        }

        protected virtual int RewardAddType => 0;
        public override Task OnViewClose()
        {
            if (paramData.autoAddReward)
            {
                foreach (var itemData in paramData.itemDatas)
                {
                    ItemModel.Instance.Add(itemData.id, itemData.cnt, paramData.itemChangeReasonArgs,addType:RewardAddType);
                }
                // CommonUtils.AddRewards(paramData.itemDatas, paramData.itemChangeReasonArgs);
                // if (!paramData.fly)
                // {
                //     foreach (var p in paramData.itemDatas)
                //     {
                //         DragonPlus.Config.Game.Item cfg = TMatch.GameConfigManager.Instance.GetItem(p.id);
                //         if (cfg.ResourceId != 0) EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId)cfg.ResourceId));
                //         EventDispatcher.Instance.DispatchEvent(new GameItemChangeEvent(cfg.ItemId));
                //     }
                // }
            }

            paramData.closeAction?.Invoke();
            return base.OnViewClose();
        }
    }
}