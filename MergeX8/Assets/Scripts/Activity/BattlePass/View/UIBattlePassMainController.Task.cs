using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BattlePass
{
    public partial class UIBattlePassMainController
    {
        private List<BattlePassTaskView> _dailyTasks = new List<BattlePassTaskView>();
        private List<BattlePassTaskView> _challengeTask = new List<BattlePassTaskView>();
        private BattlePassTaskView _fixedTask = new BattlePassTaskView();

        public void InitTaskView()
        {
            for (int i = 0; i < BattlePassTaskModel.Instance.battlePassTask.DailyTask.TaskInfos.Count; i++)
            {
                var dailyObj = transform.Find("Root/BottomGroup/Scroll View/Viewport/Content/DailyTask_" + (i + 1)).gameObject;
                _dailyTasks.Add(new BattlePassTaskView());
                _dailyTasks[i].Init(dailyObj, BattlePassTaskView.TaskUIType.Daily);
                _dailyTasks[i].UpdateView(BattlePassTaskModel.Instance.battlePassTask.DailyTask.TaskInfos[i]);
            }

            for (int i = 0; i < BattlePassTaskModel.Instance.battlePassTask.ChallengeTask.TaskInfos.Count; i++)
            {
                var challengeObj = transform.Find("Root/BottomGroup/Scroll View/Viewport/Content/ChallengeTask_" + (i + 1)).gameObject;
                _challengeTask.Add(new BattlePassTaskView());
                _challengeTask[i].Init(challengeObj, BattlePassTaskView.TaskUIType.Challenge);
                _challengeTask[i].UpdateView(BattlePassTaskModel.Instance.battlePassTask.ChallengeTask.TaskInfos[i]);
            }

            var fixedObj = transform.Find("Root/BottomGroup/Scroll View/Viewport/Content/FixedTask").gameObject;
            _fixedTask.Init(fixedObj, BattlePassTaskView.TaskUIType.Fixed);
            _fixedTask.UpdateView(BattlePassTaskModel.Instance.battlePassTask.FixationTask.TaskInfos[0]);

            UpdateTaskUI();

            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Bp_Task, transform.Find("Root/BottomGroup/Scroll View/Viewport/Content") as RectTransform, moveToTarget: null);
            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_REFRESH, BattlePassRefresh);
        }

        private void OnOpenView()
        {
            UIPopupBattlePassRefreshController.ReissueCache(false);
        }

        public void UpdateTaskActivityTime()
        {
            _dailyTasks.ForEach(a => a.UpdateActivityTime());
            _challengeTask.ForEach(a => a.UpdateActivityTime());
            _fixedTask.UpdateActivityTime();
        }

        public void UpdateTaskUI()
        {
            bool isCompleteDailyTask = true;
            _dailyTasks.ForEach(a =>
            {
                a._normalObject.gameObject.SetActive(true);
                a._lockObject.gameObject.SetActive(false);

                if (a._info.IsComplete && a._info.IsShow)
                    a._gameObject.SetActive(false);
                else
                {
                    a._gameObject.SetActive(true);

                    isCompleteDailyTask = false;
                }
            });

            _challengeTask.ForEach(a => { a._gameObject.SetActive(false); });

            if (!isCompleteDailyTask)
            {
                _challengeTask[0]._gameObject.SetActive(true);
                _challengeTask[0]._normalObject.SetActive(false);
                _challengeTask[0]._lockObject.SetActive(true);
            }
            else
            {
                foreach (var view in _challengeTask)
                {
                    if (view._info.IsComplete && view._info.IsShow)
                        continue;

                    view._gameObject.SetActive(true);
                    view._normalObject.SetActive(true);
                    view._lockObject.SetActive(false);
                    break;
                }
            }

            _fixedTask._gameObject.SetActive(true);
            _fixedTask._normalObject.SetActive(true);
            _fixedTask._lockObject.SetActive(false);
        }

        private void BattlePassRefresh(BaseEvent e)
        {
            if (!this)
            {
                EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_REFRESH, BattlePassRefresh);
                return;
            }

            for (int i = 0; i < BattlePassTaskModel.Instance.battlePassTask.DailyTask.TaskInfos.Count; i++)
            {
                _dailyTasks[i].UpdateView(BattlePassTaskModel.Instance.battlePassTask.DailyTask.TaskInfos[i]);
            }

            for (int i = 0; i < BattlePassTaskModel.Instance.battlePassTask.ChallengeTask.TaskInfos.Count; i++)
            {
                _challengeTask[i].UpdateView(BattlePassTaskModel.Instance.battlePassTask.ChallengeTask.TaskInfos[i]);
            }
        }
    }
}