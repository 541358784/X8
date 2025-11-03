using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BattlePass
{
    public enum BpTaskOpenType
    {
        NewTask,
        Complete,
    }

    public class UIPopupBattlePassRefreshController : UIWindowController
    {
        private GameObject _contentObj;
        private GameObject _dailyItem;
        private GameObject _fixedItem;
        private GameObject _challengeItem;
        private GameObject _refreshObj;
        private GameObject _completeObj;

        private BpTaskOpenType _openType = BpTaskOpenType.Complete;
        private bool isRefresh = false;

        private List<StorageBattlePassTaskInfo> _taskInfos = new List<StorageBattlePassTaskInfo>();

        public override void PrivateAwake()
        {
            _contentObj = transform.Find("Root/Scroll View/Viewport/Content").gameObject;
            _dailyItem = transform.Find("Root/Scroll View/Viewport/Content/DailyTask").gameObject;
            _dailyItem.gameObject.SetActive(false);

            _refreshObj = transform.Find("Root/refresh").gameObject;
            _refreshObj.gameObject.SetActive(false);

            _completeObj = transform.Find("Root/complete").gameObject;
            _completeObj.gameObject.SetActive(false);

            _challengeItem = transform.Find("Root/Scroll View/Viewport/Content/ChallengeTask").gameObject;
            _challengeItem.gameObject.SetActive(false);

            _fixedItem = transform.Find("Root/Scroll View/Viewport/Content/FixedTask").gameObject;
            _fixedItem.gameObject.SetActive(false);

            GetItem<Button>("Root/ButtonClose").onClick.AddListener(() => { ClickUIMask(); });
            GetItem<Button>("Root/Button").onClick.AddListener(() => { ClickUIMask(); });
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _openType = (BpTaskOpenType)objs[0];
            _taskInfos.Clear();
            isRefresh = BattlePassTaskModel.Instance.battlePassTask.RefreshTime != BattlePassTaskModel.Instance.battlePassTask.GetRewardTime;

            _refreshObj.gameObject.SetActive(false);
            _completeObj.gameObject.SetActive(false);

            switch (_openType)
            {
                case BpTaskOpenType.NewTask:
                {
                    _refreshObj.gameObject.SetActive(true);
                    BattlePassTaskModel.Instance.battlePassTask.DailyTask.TaskInfos.ForEach(a => _taskInfos.Add(a));
                    if (_taskInfos != null && _taskInfos.Count >= 3)
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpTaskRefrash, _taskInfos[0].Id.ToString(), _taskInfos[1].Id.ToString(), _taskInfos[2].Id.ToString());
                    break;
                }
                case BpTaskOpenType.Complete:
                {
                    _completeObj.gameObject.SetActive(true);
                    foreach (var kv in BattlePassTaskModel.Instance.battlePassTask.CompleteDatas)
                    {
                        for (int i = 0; i < kv.Value; i++)
                        {
                            _taskInfos.Add(new StorageBattlePassTaskInfo());
                            _taskInfos[_taskInfos.Count - 1].Id = kv.Key;
                            _taskInfos[_taskInfos.Count - 1].IsComplete = true;
                            var config = BattlePassConfigManager.Instance.GetTaskConfig(kv.Key);
                            _taskInfos[_taskInfos.Count - 1].TotalNum = config.number;
                            _taskInfos[_taskInfos.Count - 1].CompleteNum = config.difficulty;
                        }
                    }

                    _taskInfos.Sort((x, y) => { return x.CompleteNum - y.CompleteNum; });
                    break;
                }
            }

            InitView();
        }

        private void InitView()
        {
            foreach (var info in _taskInfos)
            {
                GameObject itemCell = null;
                BattlePassTaskView.TaskUIType uiType = BattlePassTaskView.TaskUIType.Daily;

                var config = BattlePassConfigManager.Instance.GetTaskConfig(info.Id);
                if (config.difficulty <= (int)DifficultyType.NormalHard)
                {
                    itemCell = _dailyItem;
                    uiType = BattlePassTaskView.TaskUIType.Daily;
                }
                else if (config.difficulty <= (int)DifficultyType.Challenge)
                {
                    itemCell = _challengeItem;
                    uiType = BattlePassTaskView.TaskUIType.Challenge;
                }
                else if (config.difficulty <= (int)DifficultyType.Fixation)
                {
                    itemCell = _fixedItem;
                    uiType = BattlePassTaskView.TaskUIType.Fixed;
                }

                if (itemCell == null)
                    continue;

                var item = GameObject.Instantiate(itemCell, _contentObj.transform);
                item.gameObject.SetActive(true);

                var view = new BattlePassTaskView();
                view.Init(item, uiType);
                view.UpdateView(info);
            }
        }

        public override void ClickUIMask()
        {
            if (!canClickMask)
                return;

            canClickMask = false;

            CloseWindowWithinUIMgr(true);

            int addScore = 0;
            _taskInfos.ForEach(a =>
            {
                if (a.IsComplete)
                {
                    var config = BattlePassConfigManager.Instance.GetTaskConfig(a.Id);
                    addScore += config.reward;
                }
            });

            if (addScore <= 0)
                return;

            UIManager.Instance.OpenUI(UINameConst.UIBattlePassReward, addScore, isRefresh);
            BattlePassTaskModel.Instance.RestShowTag();
        }


        private static string constPlaceId = "battlepass";

        public static bool CanShow()
        {
            if (GuideSubSystem.Instance.IsShowingGuide())
                return false;
            
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BattlePass))
                return false;

            if (!BattlePassModel.Instance.IsOpened())
                return false;

            if (!BattlePassModel.Instance.storageBattlePass.IsShowStart)
                return false;
            
            if (BattlePassTaskModel.Instance.battlePassTask.RefreshTime == CommonUtils.GetTomorrowTimestamp((long)APIManager.Instance.GetServerTime()))
                return false;

            if (ReissueCache(true))
            {
                BattlePassTaskModel.Instance.Refresh();
                return true;
            }

            BattlePassTaskModel.Instance.Refresh();
            UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePassRefresh, BpTaskOpenType.NewTask);
            return true;
        }

        public static bool AutoShow()
        {
            if(SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game && SceneFsm.mInstance.GetCurrSceneType() != StatusType.Home && SceneFsm.mInstance.GetCurrSceneType() != StatusType.BackHome)
                return false;
            
            if (GuideSubSystem.Instance.IsGuideFrozen())
                return false;
            
            return CanShow();
        }
        
        public static bool ReissueCache(bool isInit)
        {
            if (BattlePassTaskModel.Instance.battlePassTask == null)
                return false;

            if (isInit)
            {
                if (BattlePassTaskModel.Instance.battlePassTask.RefreshTime ==
                    BattlePassTaskModel.Instance.battlePassTask.GetRewardTime)
                    return false;
            }

            if (BattlePassTaskModel.Instance.battlePassTask.CompleteDatas == null || BattlePassTaskModel.Instance.battlePassTask.CompleteDatas.Count == 0)
                return false;

            UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePassRefresh, BpTaskOpenType.Complete);
            return true;
        }
    }
}