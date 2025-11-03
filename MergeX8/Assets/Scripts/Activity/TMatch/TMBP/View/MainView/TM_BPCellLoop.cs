//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月20日 星期五
//describe    :   
//-----------------------------------

using System.Collections;
using DragonPlus;
using SomeWhere;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    /// <summary>
    /// 循环奖励
    /// </summary>
    public class TM_BPCellLoop : TM_BPCellBase
    {
        /// <summary>
        /// 锁
        /// </summary>
        private GameObject lockObj;

        /// <summary>
        /// 领取按钮
        /// </summary>
        private Button btnClaim;

        /// <summary>
        /// 自身按钮
        /// </summary>
        private Button btn;
        
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitComponent()
        {
            lockObj = transform.Find("Locked").gameObject;
            
            btnClaim = transform.Find("MinGroup/BtnClaim").GetComponent<Button>();
            btnClaim.onClick.RemoveAllListeners();
            btnClaim.onClick.AddListener(OnClickClaim);
            
            btn = GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
            
            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BattlePassOnClaimReward, OnClaim);
        }
        
        /// <summary>
        /// 领取奖励
        /// </summary>
        /// <param name="obj"></param>
        private void OnClaim(BaseEvent obj)
        {
            if (!this)
                EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_BattlePassOnClaimReward, OnClaim);
            if (obj is TM_BPClaimRewardEvent eventInfo)
            {
                if (eventInfo.IsLoopReward)
                {
                    UpdateInfo();
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            InitComponent();

            UpdateInfo();
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        private void UpdateInfo()
        {
            lockObj.SetActive(!TMBPModel.Instance.HaveBuy());
            btnClaim.gameObject.SetActive(TMBPModel.Instance.IsLoopRewardsEnable());
        }

        /// <summary>
        /// 点击领取
        /// </summary>
        /// <returns></returns>
        private void OnClickClaim()
        {
            TMBPModel.Instance.ClaimLoopRewards();
        }

        /// <summary>
        /// 点击本身
        /// </summary>
        private void OnClick()
        {
            string key = TMBPModel.Instance.HaveBuy() ? "UI_TM_bp_CycleReward_Note_unable" : "UI_TM_bp_CycleReward_Note_unable";
            string tip = LocalizationManager.Instance.GetLocalizedString(key);
            TM_BPTipBox.Open(transform, tip, new Vector2(0, 200));
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_BattlePassOnClaimReward, OnClaim);
        }

        public override IEnumerator OnStatusChangeToGolden()
        {
            UpdateInfo();
            yield break;
        }

        public override IEnumerator OnStatusChangeByLevel()
        {
            UpdateInfo();
            yield break;
        }
    }
}