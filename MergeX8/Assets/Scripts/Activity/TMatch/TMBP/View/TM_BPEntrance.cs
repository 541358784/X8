//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月19日 星期四
//describe    :   
//-----------------------------------

using System.Collections;
using Activity;
using DG.Tweening;
using OutsideGuide;
using UnityEngine;

namespace TMatch
{
    /// <summary>
    /// Tm战令入口
    /// </summary>
    public class TM_BPEntrance : UIEntranceBase<TMBPModel>
    {
        /// <summary>
        /// model
        /// </summary>
        protected override TMBPModel Model => TMBPModel.Instance;

        /// <summary>
        /// 活动Id
        /// </summary>
        // protected override string ActivityId => Model.Instance.ActivityId;
        

        /// <summary>
        /// 剩余时间
        /// </summary>
        private ulong remainTime;

        /// <summary>
        /// 领取
        /// </summary>
        private GameObject claimObj;

        /// <summary>
        /// 已经飞了
        /// </summary>
        private bool haveFly;

        protected override void Awake()
        {
            _useProgress = true;

            base.Awake();
            claimObj = transform.Find("Root/Claim").gameObject;

            UpdateShow(Model.GetLastGameWinExp() > 0);

            EventDispatcher.Instance.AddEventListener(EventEnum.TM_RewardUpdateStatus, UpdateInfo);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecuteEvt);
        }

        private void OnTMatchResultExecuteEvt(BaseEvent obj)
        {
            if (haveFly) return;
            haveFly = true;
            StartCoroutine(FlyExp());
        }

        /// <summary>
        /// 播放飞经验
        /// </summary>
        /// <returns></returns>
        private IEnumerator FlyExp()
        {
            yield return new WaitForSeconds(0.8f);
            Model.FlyExp(UILobbyMainViewLevelButton.GetTopView().position, gameObject, OnFlyExpEnd);
        }

        /// <summary>
        /// 非动画完成
        /// </summary>
        private void OnFlyExpEnd()
        {
            float nowValue = Model.GetProgress();

            float finalNowValue = nowValue;
            if (nowValue < Model.EntranceLastSliderValue)
            {
                finalNowValue = 1;
            }

            var tweenEnterAnimation = _progressValue.DOValue(finalNowValue, 1f);
            tweenEnterAnimation.onComplete = () =>
            {
                if (gameObject != null)
                {
                    UpdateShow(false);
                    if (!DecoGuideManager.Instance.IsRunning)
                    {
                        if (Model.GetCurLevel() > Model.Data.LevelViewed)
                        {
                            TM_BPMainView.Open();
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="evt"></param>
        private void UpdateInfo(BaseEvent evt = null)
        {
            UpdateShow(false);
        }

        private void UpdateShow(bool showLast)
        {
            float sliderValue = Model.GetProgress();
            bool canClaim = Model.HaveRewardClaim();
            //是否显示上一次的
            if (showLast)
            {
                sliderValue = Model.EntranceLastSliderValue;
                canClaim = Model.EntranceLastCanCalim;
            }

            _progressValue.value = sliderValue;
            claimObj.SetActive(canClaim);
            _countDownTxt.gameObject.SetActive(!canClaim);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_RewardUpdateStatus, UpdateInfo);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecuteEvt);
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        protected override void UpdateTime()
        {
            base.UpdateTime();
            
            if (Model.IsOpened(false))
            {
                remainTime = Model.GetActivityLeftTime();
            }
            else if (Model.IsInReward(false))
            {
                remainTime = Model.GetActivityRewardLeftTime();
            }
            else
            {
                remainTime = 0;
            }

            if (remainTime == 0)
                _countDownTxt.SetTerm("UI_event_valentine_end");
            else
                _countDownTxt.SetText(CommonUtils.FormatLongToTimeStr((long)remainTime));
        }


        protected override void OnClick()
        {
            base.OnClick();

            TM_BPMainView.Open();
        }
    }
}