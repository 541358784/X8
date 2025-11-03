using System;
using System.Collections;
using System.Collections.Generic;
using Activity.CrazeOrder.Model;
using Activity.DiamondRewardModel.Model;
using Activity.GardenTreasure.Model;
using Activity.LimitTimeOrder;
using Activity.Matreshkas.Model;
using Activity.SaveTheWhales;
using Activity.TimeOrder;
using Activity.TotalRecharge;
using Activity.TreasureHuntModel;
using Activity.Turntable.Model;
using ActivityLocal.CardCollection.Home;
using Decoration.DaysManager;
using ExtraEnergy;
using Farm.Model;
using Framework;
using Gameplay.UI.EnergyTorrent;
using Gameplay.UI.UpdateRewardManager;
using Merge.UnlockMergeLine;
using OptionalGift;
using TotalRecharge_New;
using UnityEngine;

namespace AutoPopupManager
{
    public partial class AutoPopupManager : Manager<AutoPopupManager>
    {
        public static Dictionary<AutoPopupLogicGroup, bool> IsInPopUIViewLogic = new Dictionary<AutoPopupLogicGroup, bool>();
        public static Dictionary<AutoPopupLogicGroup, Queue<AutoPopUI>> ExtraPopupQueue = new Dictionary<AutoPopupLogicGroup, Queue<AutoPopUI>>();

        public class AutoPopUI
        {
            public Func<bool> _func;
            private string[] _uiNamesPrivate;
            private Func<string[]> _uiNamesGetFunc;

            public string[] _uiNames
            {
                get
                {
                    if (_uiNamesGetFunc != null)
                        return _uiNamesGetFunc();
                    return _uiNamesPrivate;
                }
            }

            public bool _isSingle;

            public AutoPopUI(Func<bool> func, string[] uiNames, bool isSingle = false)
            {
                _func = func;
                _uiNamesPrivate = uiNames;
                _isSingle = isSingle;
            }

            public AutoPopUI(Func<bool> func, Func<string[]> uiNamesGetFunc, bool isSingle = false)
            {
                _func = func;
                _uiNamesGetFunc = uiNamesGetFunc;
                _isSingle = isSingle;
            }
        }


        public static Queue<AutoPopUI> ExtraPopupQueueByGroup(AutoPopupLogicGroup groupId = AutoPopupLogicGroup.Normal)
        {
            if (ExtraPopupQueue.TryGetValue(groupId, out var queue))
                return queue;
            queue = new Queue<AutoPopUI>();
            ExtraPopupQueue.Add(groupId, queue);
            return queue;
        }

        public enum AutoPopupLogicGroup
        {
            Normal = 1,
            TaskFinish = 2, //CoinRush和养狗每日任务通用
        }

        public void Init()
        {
            InitTimeAutoPopUI();
            InitFirstAutoPopUI();
            InitGamePlayingAutoPopUI();
            InitGameAutoPopUI();
            InitHappyGameAutoPopUI();
            InitHomePopUI();
            InitLevelUpPopUI();
            InitFinishAutoPopUI();
            InitFarmPopUI();
        }
        
        public void PushExtraPopup(AutoPopUI ui, AutoPopupLogicGroup groupId = AutoPopupLogicGroup.Normal)
        {
            ExtraPopupQueueByGroup(groupId).Enqueue(ui);
            if (!IsInPopUIViewLogic.TryGetValue(groupId, out var isInLogic) || !isInLogic /* &&
                (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
                 SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome)*/)
            {
                CoroutineManager.Instance.StartCoroutine(PopUIViewLogic(null, 99, groupId: groupId));
            }
        }

        public IEnumerator PopUIViewLogic(AutoPopUI[] uiArray, int popNum, StatusType popupQueueStateType = StatusType.Non, AutoPopupLogicGroup groupId = AutoPopupLogicGroup.Normal, bool printLog = true)
        {
            if (GuideSubSystem.Instance.IsShowingGuide(printLog))
                yield break;

            if (StorySubSystem.Instance.IsShowing)
                yield break;

            if (StoryMovieSubSystem.Instance.IsShowing)
                yield break;
            if (IsInPopUIViewLogic.TryGetValue(groupId, out var isInLogic) && isInLogic)
            {
                Debug.LogError("弹窗队列未完成时又开启弹窗队列" + " groupId=" + groupId);
                yield break;
            }

            IsInPopUIViewLogic.TryAdd(groupId, true);
            int autoPopNum = 0;
            var uiArrayIndex = 0;
            uiArray ??= Array.Empty<AutoPopUI>();
            while (ExtraPopupQueueByGroup(groupId).Count > 0 || uiArrayIndex < uiArray.Length)
            {
                
                if (GuideSubSystem.Instance.IsShowingGuide())
                {
                    IsInPopUIViewLogic.Remove(groupId);
                    yield break;
                }

                if (StorySubSystem.Instance.IsShowing)
                {
                    IsInPopUIViewLogic.Remove(groupId);
                    yield break;
                }

                if (StoryMovieSubSystem.Instance.IsShowing)
                {
                    IsInPopUIViewLogic.Remove(groupId);
                    yield break;
                }

                if (popupQueueStateType != StatusType.Non)
                {
                    if (SceneFsm.mInstance.GetCurrSceneType() != popupQueueStateType)
                    {
                        IsInPopUIViewLogic.Remove(groupId);
                        yield break;
                    }
                }

                AutoPopUI func = null;
                var useExtra = false;
                if (!(ExtraPopupQueueByGroup(groupId).Count > 0))
                {
                    func = uiArray[uiArrayIndex];
                    uiArrayIndex++;
                }
                else
                {
                    func = ExtraPopupQueueByGroup(groupId).Dequeue();
                    useExtra = true;
                }
                
                try
                {
                    if (!func._func())
                        continue;
                }
                catch(Exception e)
                {
                    Debug.LogError("异常2"+e);
                    IsInPopUIViewLogic.Remove(groupId);
                }
                if (!useExtra)
                    autoPopNum++;
                UIRoot.Instance.EnableEventSystem = true;
                var uiNames = func._uiNames;
                Debug.Log((useExtra ? "使用额外弹窗队列" : "使用UIArray数组") + uiNames.ArrayToString());
                if (uiNames == null || uiNames.Length == 0)
                    continue;

                if (func._isSingle)
                {
                    autoPopNum = popNum;
                }

                int uiNum = uiNames.Length;

                while (uiNum > 0)
                {
                    try
                    {
                        uiNum = uiNames.Length;
                        for (int i = 0; i < uiNames.Length; i++)
                        {
                            if (uiNames[i].IsEmptyString())
                            {
                                uiNum--;
                                continue;
                            }

                            if (GuideSubSystem.Instance.IsShowingGuide())
                            {
                                IsInPopUIViewLogic.Remove(groupId);
                                yield break;
                            }

                            if (StorySubSystem.Instance.IsShowing)
                            {
                                IsInPopUIViewLogic.Remove(groupId);
                                yield break;
                            }

                            if (StoryMovieSubSystem.Instance.IsShowing)
                            {
                                IsInPopUIViewLogic.Remove(groupId);
                                yield break;
                            }

                            var dlg = UIManager.Instance.GetOpenedUIByPath(uiNames[i]);
                            if (dlg == null)
                            {
                                uiNum--;
                                continue;
                            }

                            if (dlg.isActiveAndEnabled)
                                continue;

                            uiNum--;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("报错1");
                        throw e;
                    }

                    yield return new WaitForEndOfFrame();
                }

                if (autoPopNum >= popNum)
                    break;
            }

            IsInPopUIViewLogic.Remove(groupId);
        }

        public static int GetPopUINum()
        {
            if (ExperenceModel.Instance.GetLevel() < 5)
                return 3;

            return 5;
        }

        public IEnumerator LevelUpPopUIViewLogic(Action animEnd = null)
        {
            SetPause(true);
            
            yield return new WaitForEndOfFrame();
            yield return CoroutineManager.Instance.StartCoroutine(PopUIViewLogic(levelUpAutoPopUIArray, GetPopUINum()));

            SetPause(false);
            animEnd?.Invoke();
            yield break;
        }

        public IEnumerator FirstPopUIViewLogic(Action animEnd = null)
        {
            SetPause(true);
            
            if(FarmModel.Instance.IsFarmModel())
                yield return StartCoroutine(PopUIViewLogic(_farmAutoPopUI, GetPopUINum(), StatusType.Home));
            
            yield return StartCoroutine(PopUIViewLogic(firstAutoPopUIArray, GetPopUINum(), StatusType.Home));

            SetPause(false);
            animEnd?.Invoke();
        }

        public IEnumerator EnterGamePopUIViewLogic(Action animEnd = null)
        {
            SetPause(true);
            yield return StartCoroutine(PopUIViewLogic(enterGameAutoPopUIArray, GetPopUINum(), StatusType.Game));

            SetPause(false);
            animEnd?.Invoke();
        }

        public IEnumerator GamePayingPopUIViewLogic(Action animEnd = null)
        {
            SetPause(true);
            yield return StartCoroutine(PopUIViewLogic(gamePlayingAutoPopUIArray, GetPopUINum(), StatusType.Game));

            SetPause(false);
            animEnd?.Invoke();
        }

        public IEnumerator EnterHappyGoPopUIViewLogic(Action animEnd = null)
        {
            SetPause(true);
            yield return StartCoroutine(PopUIViewLogic(enterHappyGoAutoPopUIArray, GetPopUINum(), StatusType.HappyGoGame));

            SetPause(false);
            animEnd?.Invoke();
        }

        public IEnumerator TaskFinishPopUIViewLogic(Action animEnd = null)
        {
            SetPause(true);
            yield return StartCoroutine(PopUIViewLogic(taskFinishAutoPopUIArray, GetPopUINum(), StatusType.Game));

            SetPause(false);
            animEnd?.Invoke();
        }

        public IEnumerator HomePopUIViewLogic(Action animEnd = null)
        {
            SetPause(true);
            yield return StartCoroutine(PopUIViewLogic(autoPopUIArray, GetPopUINum(), StatusType.BackHome));

            SetPause(false);
            animEnd?.Invoke();
        }
        
        public IEnumerator FarmPopUIViewLogic(Action animEnd = null)
        {
            SetPause(true);
            yield return StartCoroutine(PopUIViewLogic(_farmAutoPopUI, GetPopUINum(), StatusType.Non));

            SetPause(false);
            animEnd?.Invoke();
        }
    }
}