//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月20日 星期五
//describe    :   
//-----------------------------------

using System.Collections;
using Dlugin;
using DragonPlus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus.Config.TMBP;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Network.API.Protocol;

namespace TMatch
{
    public enum RouteStatus
    {
        Locked = 1, // 锁定
        Unlock = 2, // 解锁
        Available = 3, // 可领取
        AvailableAd = 4, // 可广告领取
        Claimed = 5, // 已领取
    }
    /// <summary>
    /// 
    /// </summary>
    public class TM_BPComItem : MonoBehaviour
    {
        #region ui mem

        /// <summary>
        /// 图标
        /// </summary>
        private Image imgIcon;

        /// <summary>
        /// 宝箱
        /// </summary>
        private GameObject iconChest;

        /// <summary>
        /// 无限体力 其他
        /// </summary>
        private GameObject limitEnergy, limitOther;

        /// <summary>
        /// 数量
        /// </summary>
        private LocalizeTextMeshProUGUI txtCount;

        /// <summary>
        /// 按钮
        /// </summary>
        private Button btn;

        /// <summary>
        /// 动画状态机
        /// </summary>
        private Animator statusAnim;

        #endregion

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool haveInit;

        /// <summary>
        /// 基础配置
        /// </summary>
        private Base cfg;

        /// <summary>
        /// 奖励配置
        /// </summary>
        private Rewards awardCfg;

        /// <summary>
        /// 类型
        /// </summary>
        private TM_BpType _bpType;

        /// <summary>
        /// 道具id num
        /// </summary>
        private int itemId, itemNum;
        
        /// <summary>
        /// 初始化组件
        /// </summary>
        public void InitComponent()
        {
            if (haveInit) return;

            statusAnim = transform.Find("Root/Status").GetComponent<Animator>();

            imgIcon = transform.Find("Root/Mask/IconItem").GetComponent<Image>();
            txtCount = transform.Find("Root/Count").GetComponent<LocalizeTextMeshProUGUI>();

            iconChest = transform.Find("Root/Mask/IconChest").gameObject;
            limitEnergy = transform.Find("Root/Mask/IconItem/Limit").gameObject;
            limitOther = transform.Find("Root/Mask/IconItem/LimitGroup").gameObject;

            btn = GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);

            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BattlePassOnClaimReward, OnClaimReward);

            haveInit = true;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(Base cfg, TM_BpType bpType)
        {
            InitComponent();

            this.cfg = cfg;
            _bpType = bpType;
            awardCfg = TMBPModel.Instance.GetRewardsConfig(cfg, bpType);
            switch ((TM_BpAwardType)awardCfg.awardType)
            {
                case TM_BpAwardType.Item:
                    itemId = awardCfg.itemIds[0];
                    itemNum = awardCfg.itemCounts[0];
                    imgIcon.sprite = ItemModel.Instance.GetItemSprite(awardCfg.itemIds[0]);
                    txtCount.SetText(CommonUtils.GetItemText(awardCfg.itemIds[0], awardCfg.itemCounts[0]));
                    
                    ItemConfig itemCfg = ItemModel.Instance.GetConfigById(awardCfg.itemIds[0]);
                    limitEnergy.SetActive((ItemType)itemCfg.type == ItemType.TMEnergyInfinity);
                    limitOther.SetActive((ItemType)itemCfg.type == ItemType.TMLightingInfinity || (ItemType)itemCfg.type == ItemType.TMClockInfinity);
                    break;
                case TM_BpAwardType.Chest:
                    txtCount.SetText("");
                    break;
            }

            iconChest.SetActive((TM_BpAwardType)awardCfg.awardType == TM_BpAwardType.Chest);
            imgIcon.gameObject.SetActive((TM_BpAwardType)awardCfg.awardType == TM_BpAwardType.Item);

            Play(TMBPModel.Instance.GetBpRewardStatus(cfg.id, bpType).ToString());
        }

        /// <summary>
        /// 领奖事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnClaimReward(BaseEvent obj)
        {
            TM_BPClaimRewardEvent eventInfo = obj as TM_BPClaimRewardEvent;
            if (eventInfo != null && !eventInfo.IsLoopReward && eventInfo.Id == cfg.id &&
                _bpType == eventInfo.BpType)
            {
                statusAnim.Play("ToClaimed");
                if ((TM_BpAwardType)awardCfg.awardType == TM_BpAwardType.Item)
                {
                    EventDispatcher.Instance.DispatchEvent(
                        new TM_BPCreateRewardItemAnimEvent(transform, itemId, itemNum,_bpType));
                }
            }
        }

        /// <summary>
        /// 购买
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnStatusChangeToGolden()
        {
            TM_BPRewardStatus statusOld = TMBPModel.Instance.GetBpRewardStatus(cfg.id, _bpType, true, true);
            TM_BPRewardStatus statusNew = TMBPModel.Instance.GetBpRewardStatus(cfg.id, _bpType, false, true);
            if (statusOld == statusNew)
                yield break;

            // 如果是从锁定到可领取状态，则中间还需要插入一个播放unlock的动画
            if (statusOld == TM_BPRewardStatus.Locked && statusNew == TM_BPRewardStatus.Available)
            {
                Play("To" + RouteStatus.Unlock);
                yield return new WaitForSeconds(0.15f);
                Play("To" + statusNew);
                yield return new WaitForSeconds(0.15f);
                yield break;
            }

            Play("To" + statusNew);
            yield return new WaitForSeconds(0.3f);
        }

        /// <summary>
        /// 等级变化
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnStatusChangeByLevel()
        {
            TM_BPRewardStatus statusOld = TMBPModel.Instance.GetBpRewardStatus(cfg.id, _bpType, false, true);
            TM_BPRewardStatus statusNew = TMBPModel.Instance.GetBpRewardStatus(cfg.id, _bpType, false, false);
            if (statusOld == statusNew)
                yield break;

            Play("To" + statusNew);
            yield return new WaitForSeconds(0.3f);
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="s"></param>
        protected void Play(string s)
        {
            statusAnim.Play(s);
        }

        /// <summary>
        /// 点击
        /// </summary>
        public void OnClick()
        {
            AudioManager.Instance.PlayBtnTap();
            TM_BPRewardStatus rewardStatus = TMBPModel.Instance.GetBpRewardStatus(cfg.id, _bpType);
            string tip = "";

            switch (rewardStatus)
            {
                case TM_BPRewardStatus.Locked:
                    tip = LocalizationManager.Instance.GetLocalizedString(TMBPModel.Instance.GetCurLevel() >= cfg.id
                        ? "UI_battlepass_tips02_text" // 购买黄金门票后可以获得奖励
                        : "UI_battlepass_tips06_text"); // 购买黄金门票，并且提升等级，可以获得奖励
                    break;
                case TM_BPRewardStatus.Unlock:
                    tip = LocalizationManager.Instance.GetLocalizedString(
                        "UI_battlepass_tips04_text"); // 完成任务，提升等级以获得奖励
                    break;
                case TM_BPRewardStatus.Available:
                    TMBPModel.Instance.ClaimRewards(cfg.id, _bpType);
                    break;
                case TM_BPRewardStatus.Claimed:
                    tip = LocalizationManager.Instance.GetLocalizedString(
                        "UI_battlepass_tips08_text"); // 该奖励已领取
                    break;
            }

            if (!string.IsNullOrEmpty(tip))
            {
                if (awardCfg.awardType == (int)TM_BpAwardType.Chest)
                {
                    UiTipMainBox.ShowTip(tip, awardCfg.itemIds.ToList(), awardCfg.itemCounts.ToList(), transform,
                        UiTipMainBox.TipBoxItemDir.DownRight, new Vector2(30, 0));
                }
                else
                {
                    TM_BPTipBox.Open(transform, tip, new Vector2(0, 80));
                }
            }
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_BattlePassOnClaimReward, OnClaimReward);
        }
    }
}