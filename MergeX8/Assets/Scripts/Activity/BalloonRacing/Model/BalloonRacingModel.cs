using System;
using System.Collections.Generic;
using System.Linq;
using Activity.BalloonRacing.Dynamic;
using DragonU3DSDK.Storage;
using UnityEngine;
using DragonPlus;
using DragonPlus.Config.BalloonRacing;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Dynamic;
using SomeWhere;
using Random = UnityEngine.Random;

namespace Activity.BalloonRacing
{
    public class BalloonRacingModel : ActivityEntityBase
    {
        public override string Guid => "OPS_EVENT_TYPE_BALLOON_RACING";

        private static BalloonRacingModel _instance;
        public static BalloonRacingModel Instance => _instance ?? (_instance = new BalloonRacingModel());

        private bool _isFlyCollection = false;

        /// <summary>
        /// 登录检测竞速结果
        /// </summary>
        private bool _loginCheckResult = false;

        public StorageBalloonRacing Storage
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().BalloonRacingDic; }
        }


        /// <summary>
        /// 正常配置
        /// </summary>
        private List<TableBalloonRacingReward> _listNormalConfigs = new List<TableBalloonRacingReward>();

        /// <summary>
        /// 循环配置
        /// </summary>
        private List<TableBalloonRacingReward> _listCycleConfigs = new List<TableBalloonRacingReward>();


        /// <summary>
        /// 当前阶段
        /// </summary>
        public TableBalloonRacingReward CurRacing;

        private string coolTimeKey = "CarRacing";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitAuto()
        {
            Instance.Init();
        }

        private BalloonRacingModel()
        {
            global::TMatch.Timer.Register(10, CheckRobotScoreUpdate, null, true);
        }


        public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
            ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
        {
            base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson, activitySubType);
            BalloonRacingConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        protected override void InitServerDataFinish()
        {
            base.InitServerDataFinish();
            if (Storage.ActivityId != ActivityId)
            {
                Storage.Clear();
                Storage.ActivityId = ActivityId;
            }

            InitConfig();
            JoinOrInitRacing(false);
        }

        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.BalloonRacing);
        }

        private void InitConfig()
        {
            _listNormalConfigs.Clear();
            _listCycleConfigs.Clear();
            _listNormalConfigs = BalloonRacingConfigManager.Instance.TableBalloonRacingRewardList.FindAll(p => p.Stage == 1);
            _listCycleConfigs = BalloonRacingConfigManager.Instance.TableBalloonRacingRewardList.FindAll(p => p.Stage == 2);
            _listNormalConfigs.Sort((a, b) => a.Order - b.Order);
            _listCycleConfigs.Sort((a, b) => a.Order - b.Order);
        }

        /// <summary>
        /// 加入竞速
        /// </summary>
        /// <param name="isJoin">是否是点击加入竞速</param>
        public void JoinOrInitRacing(bool isJoin)
        {
            //该周期内没参加
            if (Storage.CurRacingIndex == 0)
            {
                if (isJoin)
                {
                    Storage.JoinTime = (long)APIManager.Instance.GetServerTime();
                    Storage.CurRacingIndex = 1;
                    Storage.RunRounds = 1;
                    CurRacing = BalloonRacingConfigManager.Instance.TableBalloonRacingRewardList.Find(p =>
                        p.Id == Storage.CurRacingIndex);
                    InitRacingPlayer();
                    Storage.IsDone = false;
                    Storage.IsAward = false;
                    Storage.DoneList.Clear();
                    EventDispatcher.Instance.DispatchEvent(EventEnum.BALLOON_RACING_JOIN);
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventRaceStart,
                        Storage.RunRounds.ToString());
                }
            }
            else
            {
                if (Storage.IsDone)
                {
                    if (isJoin)
                    {
                        Storage.JoinTime = (long)APIManager.Instance.GetServerTime();

                        int rankIndex = Storage.PlayerList.FindIndex(p => p.IsMe);
                        //赢了
                        if (rankIndex >= 0 && rankIndex < CurRacing.RewardRank)
                        {
                            TableBalloonRacingReward last =
                                BalloonRacingConfigManager.Instance.TableBalloonRacingRewardList.Find(p =>
                                    p.Id == Storage.CurRacingIndex);
                            CurRacing = GetNextConfig(last);
                            Storage.CurRacingIndex = CurRacing.Id;
                            Storage.RunRounds++;
                        }

                        InitRacingPlayer();
                        Storage.IsDone = false;
                        Storage.IsAward = false;
                        Storage.DoneList.Clear();
                        EventDispatcher.Instance.DispatchEvent(EventEnum.BALLOON_RACING_JOIN);
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventRaceStart,
                            Storage.RunRounds.ToString());
                    }
                    else
                    {
                        CurRacing = BalloonRacingConfigManager.Instance.TableBalloonRacingRewardList.Find(p =>
                            p.Id == Storage.CurRacingIndex);
                    }
                }
                else
                {
                    CurRacing = BalloonRacingConfigManager.Instance.TableBalloonRacingRewardList.Find(p =>
                        p.Id == Storage.CurRacingIndex);
                }
            }
        }

        /// <summary>
        /// 是否显示红点
        /// </summary>
        /// <returns></returns>
        public bool ShowRedPoint()
        {
            if (!IsOpened())
            {
                return false;
            }

            if (Storage.IsDone)
            {
                return true;
            }

            if (!IsJoinRacing())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 初始化竞速玩家
        /// </summary>
        private void InitRacingPlayer()
        {
            Storage.PlayerList.Clear();
            var me = new StorageBalloonRacingPlayer()
            {
                IsMe = true,
                PlayerName = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName,
                PlayerHead = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId,
                CurScore = 0,
                LastScore = 0,
                CurRefreshTime = 0,
                Seat = Storage.PlayerList.Count,
                IsDone = false
            };
            Storage.PlayerList.Add(me);
            //var robotConfig=GetSpeedRaceRobotCfgByStage(StorageSpeedRace.Stage);
            List<string> randomName = new List<string>();

            List<int> randomSeat = new List<int>() { 1, 2, 3, 4 };

            for (var i = 0; i < 4; i++)
            {
                string name = "";
                int loopNum = 1;
                while (loopNum < 10)
                {
                    loopNum++;
                    name = BalloonRacingConfigManager.Instance.TableBalloonRacingRobotNameList.RandomPickOne().Name;
                    if (randomName.Contains(name))
                        continue;

                    randomName.Add(name);
                    break;
                }

                int seat = randomSeat.RandomPickOne();
                randomSeat.Remove(seat);
                var robot = new StorageBalloonRacingPlayer()
                {
                    IsMe = false,
                    PlayerName = name,
                    PlayerHead = GlobalConfigManager.Instance._tableAvatars.RandomPickOne().id,
                    CurScore = 0,
                    LastScore = 0,
                    CurRefreshTime = 0,
                    Seat = i + 1,
                    IsDone = false
                };
                CalRobotScoreChange(Storage.JoinTime, CurRacing.Collect, robot, seat);
                Storage.PlayerList.Add(robot);
            }
        }

        /// <summary>
        /// 计算机器人积分变动
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="targetScore"></param>
        /// <param name="robot"></param>
        private void CalRobotScoreChange(long beginTime, int targetScore, StorageBalloonRacingPlayer robot, int seat)
        {
            TableBalloonRacingRobot robotConfig =
                BalloonRacingConfigManager.Instance.TableBalloonRacingRobotList.Find(p => p.Id == CurRacing.Id);


            List<int> scoreGroup = new List<int>();
            List<int> scoreWeight = new List<int>();

            switch (seat)
            {
                case 1:
                    scoreGroup = robotConfig.AddRange1;
                    scoreWeight = robotConfig.TimeRange1;
                    break;
                case 2:
                    scoreGroup = robotConfig.AddRange2;
                    scoreWeight = robotConfig.TimeRange2;
                    break;
                case 3:
                    scoreGroup = robotConfig.AddRange3;
                    scoreWeight = robotConfig.TimeRange3;
                    break;
                case 4:
                    scoreGroup = robotConfig.AddRange4;
                    scoreWeight = robotConfig.TimeRange4;
                    break;
            }

            int tempScore = 0;
            long tempTime = 0;
            bool firstCal = true;
            while (tempScore < targetScore)
            {
                TableBalloonRacingScore score = BalloonRacingConfigManager.Instance.TableBalloonRacingScoreList.First();

                //第一轮变动默认第一组配置
                if (firstCal)
                {
                    score = BalloonRacingConfigManager.Instance.TableBalloonRacingScoreList.Find(p => p.Id == scoreGroup[0]);
                    firstCal = false;
                }
                else
                {
                    int group = scoreGroup[scoreWeight.RandomIndexByWeight()];
                    score = BalloonRacingConfigManager.Instance.TableBalloonRacingScoreList.Find(p => p.Id == group);
                }

                int calCount = score.CoutRange[score.CoutWeight.RandomIndexByWeight()];
                for (int i = 0; i < calCount; i++)
                {
                    int changeScore = Random.Range(score.AddRange[0], score.AddRange[1]);
                    int changeTime = Random.Range(score.TimeRange[0], score.TimeRange[1]);
                    tempScore += changeScore;
                    tempTime += changeTime * 1000;
                    robot.AddScoreList.Add((ulong)(beginTime + tempTime), tempScore);
                    if (tempScore >= targetScore)
                        break;
                }
            }
        }

        /// <summary>
        /// 计算竞速是否可结算
        /// </summary>
        private void CalRacingEnd()
        {
            if (CurRacing == null)
                return;

            //已结算不再加分
            if (Storage.IsDone)
                return;

            for (int i = 0; i < Storage.PlayerList.Count; i++)
            {
                StorageBalloonRacingPlayer cur = Storage.PlayerList[i];
                if (cur.IsDone)
                    continue;

                //达到此档的分数
                if (cur.CurScore >= CurRacing.Collect && !Storage.DoneList.Contains(cur.Seat))
                {
                    cur.IsDone = true;
                    Storage.DoneList.Add(cur.Seat);
                    //已达到结算人数或者我赢了
                    if (Storage.DoneList.Count >= CurRacing.RewardRank || cur.IsMe)
                    {
                        Storage.IsDone = true;
                        EventDispatcher.Instance.DispatchEvent(EventEnum.BALLOON_RACING_DONE);

                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 检测机器人积分变动
        /// </summary>
        public void CheckRobotScoreUpdate()
        {
            if (!IsInitFromServer())
                return;

            if (CurRacing == null)
                return;

            //已经结算不再检测
            if (Storage.IsDone)
            {
                return;
            }

            ulong curTime = APIManager.Instance.GetServerTime();
            bool hasChange = false;

            List<StorageBalloonRacingPlayer> listChange = new List<StorageBalloonRacingPlayer>();

            for (int i = 0; i < Storage.PlayerList.Count; i++)
            {
                StorageBalloonRacingPlayer cur = Storage.PlayerList[i];
                if (cur.IsMe)
                    continue;

                foreach (var kv in cur.AddScoreList)
                {
                    if (curTime >= kv.Key && cur.CurScore < kv.Value)
                    {
                        cur.LastScore = cur.CurScore;
                        cur.CurScore = kv.Value;
                        cur.CurRefreshTime = kv.Key;
                        hasChange = true;
                    }
                }

                if (hasChange)
                {
                    listChange.Add(cur);
                    CalRacingEnd();
                }

                if (Storage.IsDone)
                    break;

                hasChange = false;
            }


            if (listChange.Count > 0)
            {
                SortRank();
                EventDispatcher.Instance.DispatchEvent(EventEnum.BALLOON_RACING_SCORE_UPDATE, listChange);
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        private void SortRank()
        {
            Storage.PlayerList.Sort((a, b) =>
            {
                if (a.IsDone && b.IsDone)
                {
                    if (a.CurRefreshTime > b.CurRefreshTime)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (a.IsDone && !b.IsDone)
                {
                    return -1;
                }
                else if (!a.IsDone && b.IsDone)
                {
                    return 1;
                }
                else
                {
                    if (a.CurScore != b.CurScore)
                    {
                        if (a.CurScore > b.CurScore)
                        {
                            return -1;
                        }
                        else if (a.CurScore == b.CurScore)
                        {
                            if (a.CurRefreshTime > b.CurRefreshTime)
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (a.Seat > b.Seat)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 下一轮配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private TableBalloonRacingReward GetNextConfig(TableBalloonRacingReward config)
        {
            int nextOrder = config.Order + 1;
            if (config.Stage == 1)
            {
                if (nextOrder <= _listNormalConfigs.Count)
                {
                    return _listNormalConfigs.Find(p => p.Order == nextOrder);
                }
                else
                {
                    return _listCycleConfigs.Find(p => p.Order == 1);
                }
            }
            else
            {
                if (nextOrder <= _listCycleConfigs.Count)
                {
                    return _listCycleConfigs.Find(p => p.Order == nextOrder);
                }
                else
                {
                    return _listCycleConfigs.Find(p => p.Order == 1);
                }
            }
        }

        /// <summary>
        /// 增加合成物品
        /// </summary>
        /// <param name="mergeId"></param>
        /// <param name="scrPos"></param>
        /// <param name="from"></param>
        public void AddMergeScore(int mergeId, Vector3 scrPos, string from)
        {
            if (!IsOpened())
                return;

            TableBalloonRacingItem item = BalloonRacingConfigManager.Instance.TableBalloonRacingItemList.Find(p => p.Id == mergeId);

            if (item == null)
                return;

            AddScore(item.Price, scrPos, true);
        }

        public void AddBubbleScore(int diamond, Vector3 scrPos)
        {
            if (!IsOpened())
                return;

            int Coefficient = BalloonRacingConfigManager.Instance.TableBalloonRacingSettingList[0].Bubble_coefficient;

            int score = Math.Max(1, Mathf.RoundToInt(diamond * (1.0f * Coefficient / 100)));

            AddScore(score, scrPos, true);
        }

        public void AddFlashBuyScore(int diamond, Vector3 scrPos)
        {
            if (!IsOpened())
                return;

            int Coefficient = BalloonRacingConfigManager.Instance.TableBalloonRacingSettingList[0].FlashBuy_coefficient;

            int score = Math.Max(1, Mathf.RoundToInt(diamond * (1.0f * Coefficient / 100)));

            AddScore(score, scrPos, true);
        }

        /// <summary>
        /// 增加随机任务收集数量
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="coin"></param>
        /// <param name="scrPos"></param>
        public void AddRandomTaskScore(int seat, int coin, Vector3 scrPos)
        {
            if (!IsOpened())
                return;

            int Coefficient = BalloonRacingConfigManager.Instance.TableBalloonRacingSettingList[0].Order_coefficient;

            //随机任务保底1
            int max = Math.Max(1, Mathf.RoundToInt(coin * (1.0f * Coefficient / 100)));

            AddScore(max, scrPos, false);
        }

        /// <summary>
        /// 获取随机任务分数
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="coin"></param>
        public int GetRandomTaskScore(StorageTaskItem storageOrderItem)
        {
            if (!IsOpened())
                return 0;

            int Coefficient = BalloonRacingConfigManager.Instance.TableBalloonRacingSettingList[0].Order_coefficient;

            int coin = CommonUtils.GetTaskValue(storageOrderItem);
            //随机任务保底1
            int max = Math.Max(1, Mathf.RoundToInt(coin * (1.0f * Coefficient / 100)));

            return max;
        }

        public void AddScoreDebug(int collection)
        {
            AddScore(collection, Vector3.zero, false);
        }

        /// <summary>
        /// 增加自己的竞速积分
        /// </summary>
        /// <param name="score"></param>
        /// <param name="scrPos"></param>
        public void AddScore(int score, Vector3 scrPos, bool isShowPrompt)
        {
            if (!IsJoinRacing())
                return;

            //已结算不再加分
            if (Storage.IsDone)
                return;

            if (score <= 0) return;

            StorageBalloonRacingPlayer me = Storage.PlayerList.Find(p => p.IsMe);
            me.LastScore = me.CurScore;
            me.CurScore = me.LastScore + score;
            me.CurRefreshTime = APIManager.Instance.GetServerTime();

            FlyScore(score, scrPos);

            if (isShowPrompt)
            {
                MergePromptManager.Instance.ShowTextPromptMultiple("+" + score, scrPos, 1.5f);
            }

            CalRacingEnd();
            SortRank();
            List<StorageBalloonRacingPlayer> listChange = new List<StorageBalloonRacingPlayer>();
            listChange.Add(me);
            EventDispatcher.Instance.DispatchEvent(EventEnum.BALLOON_RACING_SCORE_UPDATE, listChange);
        }

        public int GetMyScore()
        {
            StorageBalloonRacingPlayer me = Storage.PlayerList.Find(p => p.IsMe);
            if (me == null)
                return 0;

            return me.CurScore;
        }

        public void FlyScore(int count, Vector3 scrPos)
        {
            _isFlyCollection = true;
            var gameEntry = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<Game_BalloonRacing>(typeof(DynamicEntry_Game_BalloonRacing));
            Vector3 endPos = Vector3.zero;
            if (gameEntry != null && gameEntry.gameObject.activeInHierarchy)
            {
                endPos = gameEntry.transform.position;
            }
            else
            {
                var homeEntry = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<Home_BalloonRacing>(typeof(DynamicEntry_Home_BalloonRacing));
                if (homeEntry != null)
                    endPos = homeEntry.transform.position;
                else
                {
                    endPos = UIHomeMainController.mainController.GamePosition;
                }
            }

            Vector3 localPos = UIRoot.Instance.transform.InverseTransformPoint(scrPos);
            localPos.y -= 20;
            // GameObject efObj = FlyGameObjectManager.Instance.PlayNumEffect(localPos, count);
            // if (efObj != null)
            // {
            //     efObj.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
            // }


            int min = Math.Min(count, 8);
            int Space = Mathf.CeilToInt(count * 1f / min);


            int index = 0;
            while (count > 0)
            {
                bool isEnd = false;
                int change = Space;
                if (count >= change)
                    count -= change;
                else
                {
                    change = count;
                    count -= change;
                    isEnd = true;
                }


                FlyGameObjectManager.Instance.FlyObject(gameEntry.Target, scrPos, endPos, true, 0.7f, 0.1f * index,
                    () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(endPos);
                        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.STAGE_COLLECTION_FLY_SUCCESS, change);
                        if (isEnd)
                            _isFlyCollection = false;
                    });

                index++;
            }
        }

        public void DebugReset()
        {
            Storage.Clear();
            CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey);
            InitServerDataFinish();
        }

        public bool IsOpened()
        {
            return base.IsOpened() && UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BalloonRacing);
        }

        public bool IsShowReward()
        {
            return IsOpened() && IsJoinRacing();
        }

        /// <summary>
        /// 是否加入竞速
        /// </summary>
        /// <returns></returns>
        public bool IsJoinRacing()
        {
            return Storage.CurRacingIndex > 0 && !Storage.IsDone;
        }

        /// <summary>
        /// 是否显示加入竞速界面
        /// </summary>
        /// <returns></returns>
        public bool CanShowJoinRacing()
        {
            if (!IsOpened())
                return false;

            if (Storage.CurRacingIndex <= 0 || (Storage.CurRacingIndex > 0 && Storage.IsAward))
            {
                if (!Storage.IsJoin)
                {
                    UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingStart);
                }
                else
                {
                    if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingStart);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 记录打开加入进入弹窗
        /// </summary>
        public void RecordOpenJoinRacing()
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
                CommonUtils.GetTimeStamp());
        }

        /// <summary>
        /// 登录检测竞速结果
        /// </summary>
        /// <returns></returns>
        public bool LoginCheckRacingEnding()
        {
            if (!IsOpened())
                return false;

            if (_loginCheckResult)
                return false;

            _loginCheckResult = true;


            //已经结束或者没加入不检测
            if ((Storage.IsDone && Storage.IsAward) || CurRacing == null)
                return false;

            CheckRobotScoreUpdate();

            if (Storage.IsDone)
            {
                UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingMain);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 定时检测结果显示
        /// </summary>
        /// <returns></returns>
        public bool TimeCheckRacingEnding()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BalloonRacing))
            {
                return false;
            }
            // if (!UIManager.Instance.CanShowOtherWindows())
            // {
            //     return false;
            // }
            
            if (GuideSubSystem.Instance.IsShowingGuide())
            {
                return false;
            }

            if (GuideSubSystem.Instance.IsGuideFrozen())
                return false;
            
            if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIBalloonRacingMain) != null)
                return false;


            //已经结束或者没加入不检测
            if ((Storage.IsDone && Storage.IsAward) || CurRacing == null)
                return false;

            if (Storage.IsDone)
            {
                UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingMain, false);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置机器人分数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="score"></param>
        public void SetRobotScore(int index, int score)
        {
            for (int i = 0; i < Storage.PlayerList.Count; i++)
            {
                if (!Storage.PlayerList[i].IsMe && i == index)
                {
                    Storage.PlayerList[i].CurScore = score;
                    Storage.PlayerList[i].LastScore = score;
                }
            }

            CheckRobotScoreUpdate();
        }

        public void TryOpenMain()
        {
            if (!IsOpened()) return;
            if (Storage.CurRacingIndex <= 0 || (Storage.CurRacingIndex > 0 && Storage.IsAward))
            {
                UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingStart);
            }
            else
            {
                UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingMain);
            }
        }
    }
}