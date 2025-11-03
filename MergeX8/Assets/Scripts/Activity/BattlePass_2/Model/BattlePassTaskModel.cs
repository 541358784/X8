using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using SomeWhere;
using UnityEngine;

namespace Activity.BattlePass_2
{
    public class BattlePassTaskModel : Singleton<BattlePassTaskModel>
    {
        public StorageBattlePassTask battlePassTask
        {
            get
            {
                if (BattlePassModel.Instance.storageBattlePass == null)
                    return null;
                
                return BattlePassModel.Instance.storageBattlePass.BattlePassTask;
            }
        }

        public void InitTask()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskRefresh);
            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskRefresh);
            
            if(battlePassTask == null)
                return;
            
            if(battlePassTask.FixationTask.TaskInfos.Count > 0)
                return;

            battlePassTask.RefreshTime = CommonUtils.GetTomorrowTimestamp2((long)APIManager.Instance.GetServerTime());
            battlePassTask.GetRewardTime = battlePassTask.RefreshTime;
            Refresh_Daily();
            Refresh_Challenge();
            Refresh_Fixation();
        }
        
        public void Refresh()
        {
            if(!BattlePassModel.Instance.IsOpened())
                return;
            
            battlePassTask.RefreshTime =CommonUtils.GetTomorrowTimestamp2((long)APIManager.Instance.GetServerTime());
            if (battlePassTask.CompleteDatas.Count == 0)
                battlePassTask.GetRewardTime = battlePassTask.RefreshTime;
            
            Refresh_Daily();
            Refresh_Challenge();
            Refresh_Fixation();
            
            EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_REFRESH);
        }

        public string GetActiveTime()
        {
            if(!BattlePassModel.Instance.IsOpened())
            {
                return "00:00";
            }
        
            long diffValue = (long)battlePassTask.RefreshTime*1000 - (long)APIManager.Instance.GetServerTime();
            if (diffValue < 0)
                return "00:00";
            return CommonUtils.FormatLongToTimeStr(diffValue);
        }

        private void Refresh_Fixation()
        {
            var config = BattlePassConfigManager.Instance.GetTasksByDifficulty((int)DifficultyType.Fixation);
            battlePassTask.FixationTask.TaskInfos.Clear();
            battlePassTask.FixationTask.TaskInfos.Add(new StorageBattlePassTaskInfo());
                
            battlePassTask.FixationTask.TaskInfos[0].Id  = config[0].id;
        }
        
        private void Refresh_Daily()
        {
            battlePassTask.DailyTask.TaskInfos.Clear();

            int index = 0;
            int resId = -1;
            foreach (var kv in BattlePassConfigManager.Instance._dicBattlePassTask)
            {
                int loopCount = 3;
                if (kv.Key == 1)
                    loopCount = 6;
                
                for (int i = 0; i < loopCount; i++)
                {
                    if (!kv.Value.ContainsKey(i+1))
                        continue;
                    var config = kv.Value[i+1];
                    if (kv.Key == (int)TaskType.Consume)
                    {
                        if (resId < 0)
                        {
                            var pickOne = config.Random();
                            resId = pickOne.mergeid;
                        }

                        List<TableBattlePassTask> tempList = new List<TableBattlePassTask>(config);
                        for (int j = 0; j < tempList.Count; j++)
                        {
                            if(tempList[j].mergeid == resId)
                                continue;
                            
                            tempList.RemoveAt(j);
                            j--;
                        }

                        config = tempList;
                    }
                    
                    battlePassTask.DailyTask.TaskInfos.Add(new StorageBattlePassTaskInfo());
                    battlePassTask.DailyTask.TaskInfos[index].Id = config.Random().id;
                    battlePassTask.DailyTask.TaskInfos[index].TotalNum = 0;
                    index++;
                }
            }
        }
        
        private void Refresh_Challenge()
        {
            battlePassTask.ChallengeTask.TaskInfos.Clear();
            var config = BattlePassConfigManager.Instance.GetTasksByDifficulty((int)DifficultyType.Challenge);
            List<TableBattlePassTask> randomList = new List<TableBattlePassTask>();
            randomList.AddRange(config);
            
            for (int i = 0; i < 2; i++)
            {
                battlePassTask.ChallengeTask.TaskInfos.Add(new StorageBattlePassTaskInfo());
                TableBattlePassTask data = randomList.Random();
                battlePassTask.ChallengeTask.TaskInfos[i].Id = data.id;

                randomList.Remove(data);
            }
        }

        private void TaskRefresh(BaseEvent e)
        {
            if (!BattlePassModel.Instance.IsOpened())
                return;
            TaskType type = (TaskType)e.datas[0];
            int param = (int)e.datas[1];
            int num = (int)e.datas[2];
            
            battlePassTask.FixationTask.TaskInfos.ForEach(a =>
            {
                TaskRefresh(a, type, param, num, true);
            });

            bool isAllComplete = true;
            bool isSendEvent = true;
            for (int i = 0; i < battlePassTask.DailyTask.TaskInfos.Count; i++)
            {
                var info = battlePassTask.DailyTask.TaskInfos[i];
                if(info.IsComplete)
                    continue;

                TaskRefresh(info, type, param, num, isSendEvent);
                isSendEvent = false;
                isAllComplete = false;
                if(i==battlePassTask.DailyTask.TaskInfos.Count-1 && info.IsComplete)
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpHardtaskPop,battlePassTask.ChallengeTask.TaskInfos[0].Id.ToString());

            }

            if (isAllComplete)
            {
                for (int i = 0; i < battlePassTask.ChallengeTask.TaskInfos.Count; i++)
                {
                    var info = battlePassTask.ChallengeTask.TaskInfos[i];
                    if(info.IsComplete)
                        continue;

                    TaskRefresh(info, type, param, num, true);
                    break;
                }
            }
        }
        
        private bool TaskRefresh(StorageBattlePassTaskInfo info, TaskType type, int param, int num, bool isSendEvent)
        {
            TableBattlePassTask config = BattlePassConfigManager.Instance.GetTaskConfig(info.Id);
            if (config == null)
                return false;

            if((config.type != (int)type))
                return false;
            
            switch (config.type)
            {
                case (int)TaskType.Serve:
                {
                    info.TotalNum++;
                    break;
                }
                case (int)TaskType.MergerItem:
                {
                    if(config.mergeid != param)
                        return false;
                    
                    info.TotalNum++;
                    break;
                }
                case (int)TaskType.Consume:
                {
                    if(config.mergeid != param)
                        return false;
                    
                    info.TotalNum += num;
                        
                    break;
                }
                case (int)TaskType.MergerNum:
                {
                    info.TotalNum++;
                    break;
                }
                case (int)TaskType.GetRes:
                {
                    if(config.mergeid != param)
                        return false;
                    
                    info.TotalNum += num;
                    break;
                }
            }

            info.IsComplete = info.TotalNum >= config.number;
            if(isSendEvent)
                EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_COMPLETE, config, info.TotalNum, config.number,num);
           
            if (info.IsComplete)
            {
                if (config.difficulty == (int)DifficultyType.Fixation)
                {
                    info.TotalNum -= config.number;
                }
                info.CompleteNum++;
                AddCompleteData(info.Id);
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpTaskFinish,info.Id.ToString(),info.TotalNum.ToString());

            }
            return info.IsComplete;
        }

        public bool IsCompleteDailyTask(int taskID)
        {
            for (int i = 0; i < battlePassTask.DailyTask.TaskInfos.Count; i++)
            {
                var info = battlePassTask.DailyTask.TaskInfos[i];
                if (taskID == info.Id && info.IsComplete)
                    return true;
            }

            return false;
        }
        public void AddCompleteData(int id)
        {
            if(!battlePassTask.CompleteDatas.ContainsKey(id))
                battlePassTask.CompleteDatas.Add(id, 0);

            battlePassTask.CompleteDatas[id]++;
        }

        public int GetCompleteNum()
        {
            if (battlePassTask == null)
                return 0;

            return battlePassTask.CompleteDatas.Count;
        }
        
        public void RestShowTag()
        {
            battlePassTask.GetRewardTime = battlePassTask.RefreshTime;
            battlePassTask.CompleteDatas.Clear();
            
            battlePassTask.FixationTask.TaskInfos.ForEach(a =>
            {
                a.IsComplete = false;
                a.CompleteNum = 0;
                a.IsShow = false;
            });

            for (int i = 0; i < battlePassTask.DailyTask.TaskInfos.Count; i++)
            {
                var info = battlePassTask.DailyTask.TaskInfos[i];
                if(!info.IsComplete)
                    continue;

                info.IsShow = true;
            }
     
            for (int i = 0; i < battlePassTask.ChallengeTask.TaskInfos.Count; i++)
            {
                var info = battlePassTask.ChallengeTask.TaskInfos[i];
                if(!info.IsComplete)
                    continue;

                info.IsShow = true;
            }
        }
    }
}