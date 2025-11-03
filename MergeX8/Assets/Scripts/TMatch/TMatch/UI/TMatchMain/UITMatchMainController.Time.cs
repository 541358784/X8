using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DragonPlus.Config.TMatch;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using OutsideGuide;


namespace TMatch
{
    public partial class UITMatchMainController
    {
        private bool destoryTime;

        private bool timeEnable;
        private bool frozen;
        private Dictionary<string, int> pause = new Dictionary<string, int>();

        private TextMeshProUGUI timeText;
        private TweenerCore<float, float, FloatOptions> tween;

        private void InitTime()
        {
            timeEnable = true;
            frozen = false;

            timeText = transform.Find("Root/TOPGroup/TimeGroup/TimeText").GetComponent<TextMeshProUGUI>();
            RefreshTimeText((int)TMatchSystem.LevelController.LevelData.LastTimes);

            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_TIME_PAUSE, OnTimePauseEvent);
        }

        private void DestoryTime()
        {
            destoryTime = true;
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TIME_PAUSE, OnTimePauseEvent);
        }

        private void OnTimeFinishEvent()
        {
            timeEnable = false;
        }

        private void UpdateTime(float deltaTime)
        {
            if (timeEnable && !TMatchSystem.LevelController.LevelStateData.pause && !frozen && pause.Count == 0 && !DecoGuideManager.Instance.IsRunning)
            {
                TMatchSystem.LevelController.LevelData.LastTimes -= deltaTime;
                if (TMatchSystem.LevelController.LevelData.LastTimes <= 0) TMatchSystem.LevelController.LevelData.LastTimes = 0;
                if (tween == null)
                {
                    int tempValue = (int)TMatchSystem.LevelController.LevelData.LastTimes;
                    RefreshTimeText(tempValue);

                    if (TMatchSystem.LevelController.LevelData.LastTimes < TMatchConfigManager.Instance.GlobalList[0].MatchLevelWarningTime)
                    {
                        if (timeText.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("normal"))
                        {
                            timeText.GetComponent<Animator>().Play("time_finish");
                        }
                    }
                    else
                    {
                        if (!timeText.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("normal"))
                        {
                            timeText.GetComponent<Animator>().Play("normal");
                        }
                    }
                }

                if (TMatchSystem.LevelController.LevelData.LastTimes <= 0)
                {
                    timeEnable = false;
                    EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_TIMEOUT);
                }
            }
        }

        private void RefreshTimeText(int value)
        {
            timeText.SetText(DragonU3DSDK.Utils.GetTimeString("%mm:%ss", value));
        }

        private void OnTimePauseEvent(BaseEvent evt)
        {
            TMatchGameTimePauseEvent drivedParam = evt as TMatchGameTimePauseEvent;
            int cnt = 0;
            pause.TryGetValue(drivedParam.name, out cnt);
            if (drivedParam.pause) cnt++;
            else cnt--;
            pause.Remove(drivedParam.name);
            if (cnt > 0) pause.Add(drivedParam.name, cnt);
        }

        public async void FrozenTime(float times)
        {
            frozen = true;
            Transform tra = transform.Find("Root/VFX_Frozen");
            tra.gameObject.SetActive(true);
            tra.GetComponent<Animator>().Play("frozen");
            while (!destoryTime)
            {
                bool tempEnable = timeEnable && !TMatchSystem.LevelController.LevelStateData.pause && pause.Count == 0;
                tra.GetComponent<Animator>().enabled = tempEnable;
                if (tempEnable) times -= Time.deltaTime;
                if (times > 0) await Task.Yield();
                else break;
            }

            if (destoryTime) return;
            tra.GetComponent<Animator>().Play("normal");
            tra.gameObject.SetActive(false);
            frozen = false;
        }

        public bool IsFrozen()
        {
            return frozen;
        }

        public void AddTime(float times)
        {
            float tempValue = TMatchSystem.LevelController.LevelData.LastTimes;
            TMatchSystem.LevelController.LevelData.LastTimes += times;
            timeText.GetComponent<Animator>().Play("addtime");
            tween?.Kill(true);
            tween = DOTween.To(() => tempValue,
                value => RefreshTimeText((int)value),
                TMatchSystem.LevelController.LevelData.LastTimes,
                2.0f);
            tween.onComplete += () =>
            {
                tween = null;
                timeText.GetComponent<Animator>().Play("normal");
                RefreshTimeText((int)TMatchSystem.LevelController.LevelData.LastTimes);
            };
            timeEnable = true;
        }
    }
}