//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月19日 星期四
//describe    :   
//-----------------------------------

using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using OutsideGuide;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    public class TM_BPMainViewOpenData : UIWindowData
    {
        /// <summary>
        /// 是否是任务链弹出的
        /// </summary>
        public bool TaskPop;

        /// <summary>
        /// 是否要显示解锁动画
        /// </summary>
        public bool ShowUnlockAnim;
    }

    /// <summary>
    /// 战令主界面
    /// </summary>
    public class TM_BPMainView : UIWindowController
    {
        #region open ui

        /// <summary>
        /// 预制体路径
        /// </summary>
        private const string PREFAB_PATH = "Prefabs/Activity/TMatch/TMBP/TM_BPMainView";

        /// <summary>
        /// 打开
        /// </summary>
        public static void Open(TM_BPMainViewOpenData openData = null)
        {
            TMBPModel.Instance.JoinActivity();
            UIManager.Instance.OpenWindow<TM_BPMainView>(PREFAB_PATH, openData);
        }

        #endregion

        /// <summary>
        /// 经验条
        /// </summary>
        public Transform ExpTrans;

        /// <summary>
        /// 购买
        /// </summary>
        public Button btnBuy;

        /// <summary>
        /// Rect
        /// </summary>
        private RectTransform rectTransform;

        /// <summary>
        /// 倒计时
        /// </summary>
        private LocalizeTextMeshProUGUI txtCountDown;

        /// <summary>
        /// 组件
        /// </summary>
        private TM_BpComExp expCom;

        /// <summary>
        /// 滑动列表
        /// </summary>
        public TM_BPComMain comMain;

        /// <summary>
        /// 计时器ß
        /// </summary>
        private Timer timer;

        /// <summary>
        /// 剩余时间
        /// </summary>
        private ulong remainTime;

        /// <summary>
        /// 是否是任务链弹出的
        /// </summary>
        private bool isTaskPop;

        public override void PrivateAwake()
        {
            rectTransform = transform.GetComponent<RectTransform>();

            SetClickListener("Root/CloseButton", () => CloseWindowWithinUIMgr());
            SetClickListener("Root/RuleButton", ClickRule);

            btnBuy = GetItem<Button>("Root/ButtonBuy");
            SetClickListener(btnBuy, ClickBuy);

            txtCountDown = GetItem<LocalizeTextMeshProUGUI>("Root/CountDown/Time");

            ExpTrans = GetItem("Root/TM_BPComExp").transform;
            expCom = ExpTrans.gameObject.GetOrCreateComponent<TM_BpComExp>();
            comMain = GetItem("Root/TM_BPComMain").GetOrCreateComponent<TM_BPComMain>();

            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BattlePassOnBuy, OnBuyBp);
            EventDispatcher.Instance.AddEventListener(EventEnum.TM_CreateRewardItemAnim, OnCreateRewardItemAnim);
        }

        protected override void OnOpenWindow(UIWindowData data)
        {
            base.OnOpenWindow(data);
            
            bool inGuide = !DecoGuideManager.Instance.GetGuideState(10143);
            bool playUnlockAnim = false;
            if (data != null && data is TM_BPMainViewOpenData openData)
            {
                isTaskPop = openData.TaskPop;
                playUnlockAnim = openData.ShowUnlockAnim;
            }

            // GameBIManager.SendGameEventEx(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmBpEntermain,
            //     TMBPModel.Instance.GetCurLevel(), TMBPModel.Instance.HaveBuy() ? 1 : 0);
            
            expCom.Init();
            comMain.Init(inGuide, playUnlockAnim);

            UpdateBuyBtnShow();

            timer = Timer.Register(1, OnTimer, null, true);
            OnTimer();

            if (inGuide)
            {
                TMBPModel.Instance.StartBpViewGuide();
            }
        }

        /// <summary>
        /// 计时器
        /// </summary>
        private void OnTimer()
        {
            if (TMBPModel.Instance.IsOpened(false))
            {
                remainTime = TMBPModel.Instance.GetActivityLeftTime();
            }
            else if (TMBPModel.Instance.IsInReward(false))
            {
                remainTime = TMBPModel.Instance.GetActivityRewardLeftTime();
            }
            else
            {
                remainTime = 0;
            }
            
            if (remainTime == 0)
                CloseWindowWithinUIMgr();
            else
                txtCountDown.SetText(CommonUtils.FormatLongToTimeStr((long)remainTime));
        }

        /// <summary>
        /// 更新购买按钮显示
        /// </summary>
        private void UpdateBuyBtnShow()
        {
            btnBuy.gameObject.SetActive(TMBPModel.Instance.ShowBPBuy());
        }

        /// <summary>
        /// 点击购买
        /// </summary>
        private void ClickBuy()
        {
            TM_BPBuyView.Open();
        }

        /// <summary>
        /// 点击规则按钮
        /// </summary>
        private void ClickRule()
        {
            TM_BPHelpView.Open();
        }

        /// <summary>
        /// 购买bp事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnBuyBp(BaseEvent obj)
        {
            UpdateBuyBtnShow();
        }

        /// <summary>
        /// 创建奖励
        /// </summary>
        private void OnCreateRewardItemAnim(BaseEvent eventInfo)
        {
            if (eventInfo is TM_BPCreateRewardItemAnimEvent eventData)
            {
                TM_RewardAnimItem.Create(eventData, rectTransform);
            }
        }
        
        /// <summary>
        /// 返回按钮
        /// </summary>
        protected override void OnBackButtonCallBack()
        {
            base.OnBackButtonCallBack();
            if (DecoGuideManager.Instance.IsRunning)
                return;
            CloseWindowWithinUIMgr();
        }

        protected override void OnCloseWindow(bool destroy = true)
        {
            base.OnCloseWindow(destroy);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_BattlePassOnBuy, OnBuyBp);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_CreateRewardItemAnim, OnCreateRewardItemAnim);
            Timer.Cancel(timer);
            if (isTaskPop)
            {
                LobbyTaskSystem.Instance.FinishCurrentTask();
            }
            
            TMBPModel.Instance.SetLastShowLevel();
        }
    }
}