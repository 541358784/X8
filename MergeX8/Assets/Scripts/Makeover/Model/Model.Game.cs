using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonU3DSDK.Asset;
using Spine;
using Spine.Unity;
using DG.Tweening;
using DragonPlus.Config.Makeover;
using Makeover;
using MiniGame;
using Skeleton = Spine.Skeleton;

namespace ASMR
{
    public partial class Model : Manager<Model>
    {
        public GameObject bodyRoot;
        private TableMoLevel _currentLevelConfig;
        private GameObject _level_obj;
        private List<Coroutine> _co_ActionList = new List<Coroutine>();
        private Dictionary<string, int> _soundDic = new Dictionary<string, int>();
        //完成的动作计数
        private int _finishedActionCount = 0;
        private int _actionFinishedCheckCount = 0;
        private List<TableStep> _current_Level_Steps = new List<TableStep>();
        private int _stepIndex = 0;

        //当前步骤完成计数
        private int _currentStepCount = 0;

        private AsmrLevel _asmrLevel;
        public int StepIndex
        {
            get { return _stepIndex; }
        }
        private bool _taskRunging = false;
        public bool TaskRuning
        {
            get { return _taskRunging; }
        }
        public int GetCurrentCompltete()
        {
            //代码保护
            if(_current_Level_Steps.Count==0)
            {
                return 0;
            }
            var complete = (int)(_stepIndex * 100 / _current_Level_Steps.Count);
            complete = Mathf.Min(complete, 100);
            return complete;
        }
        public void LoadLevel(TableMoLevel levelConfig)
        {
            _currentLevelConfig = levelConfig;
            bodyRoot = new GameObject("Body");
            bodyRoot.transform.parent = Makeover.Utils.GetOrCreateRoot().transform;

            var resPath = $"Makeover/Levels/Level{_currentLevelConfig.levelId}/Prefabs/{_currentLevelConfig.resName}";
            var level_prefab = ResourcesManager.Instance.LoadResource<GameObject>(resPath);
            if (level_prefab == null)
                return;

            if (_currentLevelConfig != null && _currentLevelConfig.newLevel)
            {
                var level = GameObject.Instantiate(level_prefab);
                level.transform.Reset();
                level.transform.localPosition = new Vector3(0, 0, -500);

                _asmrLevel = new AsmrLevel(level);
                _asmrLevel.Init(_currentLevelConfig.subID);
            }
            else
            {
                _level_obj = Instantiate(level_prefab,bodyRoot.transform,false);

                _level_obj.transform.localPosition = new Vector3(0, 0, -500);
            
                _level_obj.transform.localScale =
                    new Vector3(_currentLevelConfig.adaptScasle, _currentLevelConfig.adaptScasle, 1);
                
                _current_Level_Steps = MakeoverConfigManager.Instance.stepList.FindAll( a => a.levelId == levelConfig.stepLevelId);
                //总是从第一步开始玩
                _stepIndex = 0;
                StartStep(_current_Level_Steps[_stepIndex]);
            }
        }

        public void UnLoadCurrentLevel()
        {

            StopAll();
            
            if (_level_obj != null)
            {
                _level_obj.transform.SetParent(null);
                GameObject.DestroyImmediate(_level_obj);
                _level_obj = null;
            }
            if (bodyRoot != null)
            {
                GameObject.DestroyImmediate(bodyRoot);
                bodyRoot = null;
            }
            _currentLevelConfig = null;

            _current_Level_Steps.Clear();
        }

        public void StartStep(TableStep stepConfig)
        {
            _currentStepCount = 0;
            _taskRunging = false;
            if (stepConfig.startTrigger > 0)
            {
                ExcuteTimeLine(stepConfig.startTrigger);
                _taskRunging = false;
            }
        }

        public int StartNextStep()
        {
            if (_stepIndex < _current_Level_Steps.Count)
            {
                var stepConfig = _current_Level_Steps[_stepIndex];
                StartStep(stepConfig);
            }
            else
            {
                FinishedLevel();
                var uiGame = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
                uiGame.SetFinishLevel();
            }
            return _stepIndex;
        }


        public void ExitGame()
        {
            UnLoadCurrentLevel();
            
            if (_asmrLevel != null)
            {
                _asmrLevel.Release();
                _asmrLevel = null;    
            }
        }

        public void FinishedStep(TableStep preStepConfig)
        {
            _stepIndex++;
            
            //_stepIndex是从0开始的所以这里自加完后再运算
            var complete = (int)(_stepIndex * 100 / _current_Level_Steps.Count);
            complete = Mathf.Min(complete, 100);

            var uiGame = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
            if (_stepIndex >= _current_Level_Steps.Count)
            {
                FinishedLevel();
                uiGame?.SetFinishLevel();
            }
            else
            {
                if (preStepConfig.finishedTrigger > 0)
                {
                    ExcuteTimeLine(
                        preStepConfig.finishedTrigger,
                        () =>
                        {
                            uiGame?.GoToNextStep(_stepIndex, complete);
                        }
                    );
                }
                else
                {
                    uiGame?.GoToNextStep(_stepIndex, complete);
                }
            }
        }

        public void FinishedLevel()
        {
            MakeoverModel.Instance.FinishLevel(_currentLevelConfig);
            StopAllCoroutines();
        }

        public void ExcuteTimeLine(int timeLineId, System.Action finished_cb = null)
        {
            foreach (var _co in _co_ActionList)
            {
                StopCoroutine(_co);
            }
            _co_ActionList.Clear();

            if (_level_obj != null)
            {
                var timeLineConfig = MakeoverConfigManager.Instance.timeLineList.Find(
                    tl => tl.id == timeLineId
                );
                if (timeLineConfig != null)
                {
                    RunTimeLine(timeLineConfig, finished_cb);
                }
            }
        }

        void RunTimeLine(TableCustomerTimeLine timeLineConfig, System.Action finished_cb)
        {
            _finishedActionCount = 0;
            _actionFinishedCheckCount = 0;
            _taskRunging = true;
            int count = Mathf.Min(
                timeLineConfig.excuteStartTime.Length,
                timeLineConfig.excuteActionId.Length
            );
            for (int i = 0; i < count; i++)
            {
                var actionConfig = MakeoverConfigManager.Instance.actionList.Find(
                    a => a.id == timeLineConfig.excuteActionId[i]
                );
                var _co_Action = StartCoroutine(
                    E_ExcuteAction(
                        actionConfig,
                        timeLineConfig.excuteStartTime[i],
                        timeLineConfig,
                        finished_cb
                    )
                );
                _co_ActionList.Add(_co_Action);
                if (
                    timeLineConfig.NeedWatingActionFinished != null
                    && i < timeLineConfig.NeedWatingActionFinished.Length
                    && timeLineConfig.NeedWatingActionFinished[i] == 1
                )
                {
                    _actionFinishedCheckCount++;
                }
            }
        }

        IEnumerator E_ExcuteAction(
            TableAction actionConfig,
            int delayMs,
            TableCustomerTimeLine timeLineConfig,
            System.Action finished_cb
        )
        {
            yield return new WaitForSeconds(delayMs / 1000.0f);
            //DebugUtil.Log($"Arthur---->E_ExcuteAction[{actionConfig.Id}],delayMs={delayMs}!");
            ExcuteAction(actionConfig, timeLineConfig, finished_cb);
        }

        void ExcuteAction(
            TableAction actionConfig,
            TableCustomerTimeLine timeLineConfig,
            System.Action finished_cb
        )
        {
            // 1:显/隐藏对象组(参数:0-隐藏 1-显示,参1:对象组名)
            // 2:渐隐对象组(参数:持续时间,参数1:对象组名)
            // 3:渐显对象组(参数:持续时间,参数1:对象组名))
            // 4:Spine对象组动画(参数:0不循环,1循环,-1暂停在首帧，-2暂停在末帧,参数1:对象组名,参数2:对象组动画名)
            // 5:Spine对象组换肤(参数1:对象组名,参数2:皮肤名)
            // 6:动画控制器对象组(参数:0不循环,1循环,参数1:对象组名,参数2:对象组动画名)
            // 7:移动位置(参数:0-瞬间移动，数值ms移动持续时间,参数1:对象名,参数2:新位置坐标x,y,z)
            // 8:旋转(参数:新欧拉角值x,y,z,参数1:对象名)
            // 9:缩放(参数:新缩放值x,y,z,参数1:对象名)
            // 10:播放音效(参数:0-不循环 1-循环，参数1:音效名)
            switch (actionConfig.type)
            {
                case 1:
                {
                    Action_ShowHide(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                case 4:
                {
                    Action_SpinePlayAnimaion(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                case 5:
                {
                    Action_SpineSetSkin(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                case 7:
                {
                    Action_Move(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                case 10:
                {
                    Action_PlaySound(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                case 11:
                {
                    Action_SpinePause(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                case 12:
                {
                    Action_SpineResume(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                case 13:
                {
                    Action_StopSound(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                case 14:
                {
                    Action_SpinePlayAnimaionByTime(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                case 15:
                {
                    Action_ActionStop(actionConfig, timeLineConfig, finished_cb);
                    break;
                }
                default:
                    break;
            }
        }

        private void OnTimeLineCheckEnd(TableCustomerTimeLine timeLineConfig, System.Action finished_cb)
        {
            _finishedActionCount++;
            if (_finishedActionCount >= _actionFinishedCheckCount)
            {
                if (timeLineConfig.finishedExcuteActionId != null)
                {
                    for (int i = 0; i < timeLineConfig.finishedExcuteActionId.Length; i++)
                    {
                        var actionConfig = MakeoverConfigManager.Instance.actionList.Find(
                            a => a.id == timeLineConfig.finishedExcuteActionId[i]
                        );
                        //必须保证这个actionConfig的finishedTrigger是0.否则会死循环
                        actionConfig.finishedTrigger = 0;
                        ExcuteAction(actionConfig, null, null);
                    }
                }
                foreach (var _co in _co_ActionList)
                {
                    StopCoroutine(_co);
                }
                _co_ActionList.Clear();

                _taskRunging = false;

                //需要计数的才回检查是否下一步
                if (timeLineConfig.isEndTriggerStepCount)
                {
                    _currentStepCount++;
                    if (finished_cb != null)
                    {
                        finished_cb.Invoke();
                    }
                    CheckStepFinished();
                }
                else
                {
                    if (finished_cb != null)
                    {
                        finished_cb.Invoke();
                    }
                }
            }
        }

        private void CheckStepFinished()
        {
            var stepConfig = _current_Level_Steps[_stepIndex];
            if (_currentStepCount >= stepConfig.finishedCount)
            {
                FinishedStep(stepConfig);
            }
        }

        private void Action_ShowHide(TableAction actionConfig,TableCustomerTimeLine timeLineConfig,System.Action finished_cb)
        {
            if(actionConfig==null) return;
            if(_level_obj==null) return;
            if (actionConfig.parameters1 == null)
                return;
            for (int i = 0; i < actionConfig.parameters1.Length; i++)
            {
                var tr = _level_obj.transform.Find($"Root/{actionConfig.parameters1[i]}");
                if (tr != null)
                {
                    tr.gameObject.SetActive(actionConfig.parameters[0] == 1);
                }
            }

            if (actionConfig.finishedTrigger == 1)
            {
                OnTimeLineCheckEnd(timeLineConfig, finished_cb);
            }
        }

        private void Action_SpinePlayAnimaion(TableAction actionConfig,TableCustomerTimeLine timeLineConfig,System.Action finished_cb)
        {
            if(actionConfig==null) 
                return;
            if(_level_obj==null) 
                return;
            
            if (
                actionConfig.parameters == null
                || actionConfig.parameters1 == null
                || actionConfig.parameters2 == null
            )
                return;
            int count = Mathf.Min(
                actionConfig.parameters.Length,
                actionConfig.parameters1.Length,
                actionConfig.parameters2.Length
            );
            for (int i = 0; i < count; i++)
            {
                var tr = _level_obj.transform.Find($"Root/{actionConfig.parameters1[i]}");
                if (tr != null)
                {
                    var spineAnimation = tr.GetComponent<SkeletonAnimation>();
                    if (spineAnimation != null)
                    {
                        if (actionConfig.parameters[i] == -2)
                        {
                            var trackEntry = spineAnimation.AnimationState.SetAnimation(
                                0,
                                actionConfig.parameters2[i],
                                false
                            );
                            spineAnimation.Update(0);
                            trackEntry.TrackTime = trackEntry.Animation.Duration;
                            trackEntry.TimeScale = 0;
                        }
                        else if (actionConfig.parameters[i] == -1)
                        {
                            var trackEntry = spineAnimation.AnimationState.SetAnimation(
                                0,
                                actionConfig.parameters2[i],
                                false
                            );
                            spineAnimation.Update(0);
                            trackEntry.TrackTime = 0;
                            trackEntry.TimeScale = 0;
                        }
                        else
                        {
                            var trackEntry = spineAnimation.AnimationState.SetAnimation(
                                0,
                                actionConfig.parameters2[i],
                                actionConfig.parameters[i] == 1
                            );

                            if (actionConfig.finishedTrigger == 1)
                            {
                                trackEntry.Complete += entry =>
                                {
                                    OnTimeLineCheckEnd(timeLineConfig, finished_cb);
                                };
                            }
                            spineAnimation.Update(0);
                            trackEntry.TrackTime = 0;
                            trackEntry.TimeScale = 1;
                        }
                    }
                }
            }
        }
        
        private void Action_SpinePlayAnimaionByTime(
            TableAction actionConfig,
            TableCustomerTimeLine timeLineConfig,
            System.Action finished_cb
        )
        {
            if(actionConfig==null) return;
            if(_level_obj==null) return;
            if (
                actionConfig.parameters == null
                || actionConfig.parameters.Length<2
                || actionConfig.parameters1 == null
                || actionConfig.parameters2 == null
            )
                return;

            var tr = _level_obj.transform.Find($"Root/{actionConfig.parameters1[0]}");
            if (tr != null)
            {
                var spineAnimation = tr.GetComponent<SkeletonAnimation>();
                if (spineAnimation != null)
                {
                    var trackEntry = spineAnimation.AnimationState.SetAnimation(
                        0,
                        actionConfig.parameters2[0],
                        false
                    );
                    float entTime = actionConfig.parameters[1] == 0 ? trackEntry.Animation.Duration : ((float)actionConfig.parameters[1] / 1000.0f);
                    trackEntry.AnimationStart = ((float)actionConfig.parameters[0] / 1000.0f);;
                    trackEntry.AnimationEnd = entTime;
                    trackEntry.TimeScale = 1;
                }
            }
        }
        private void Action_SpineSetSkin(
            TableAction actionConfig,
            TableCustomerTimeLine timeLineConfig,
            System.Action finished_cb
        )
        {
            if(actionConfig==null) return;
            if(_level_obj==null) return;
            if (actionConfig.parameters1 == null || actionConfig.parameters2 == null||
                actionConfig.parameters1.Length<1||
                actionConfig.parameters2.Length<1)
                return;
            int count =actionConfig.parameters1.Length;
            for (int i = 0; i < count; i++)
            {
                var tr = _level_obj.transform.Find($"Root/{actionConfig.parameters1[i]}");
                if (tr != null)
                {
                    var spineAnimation = tr.GetComponent<SkeletonAnimation>();
                    if (spineAnimation != null)
                    {
                        Skeleton skeleton = spineAnimation.Skeleton;
                        var skin_src = new Skin(actionConfig.parameters1[i]);
                        for (int k = 0; k < actionConfig.parameters2.Length;k++)
                        {
                            Skin skin = skeleton.Data.FindSkin(actionConfig.parameters2[k]);
                            if (skin != null)
                            {
                                skin_src.AddSkin(skin);
                            }
                        }
                        skeleton.SetSkin(skin_src);
                        skeleton.SetToSetupPose();
                        
                    }
                }
            }
            if (actionConfig.finishedTrigger == 1)
            {
                OnTimeLineCheckEnd(timeLineConfig, finished_cb);
            }
        }
        private void Action_SpinePause(TableAction actionConfig,TableCustomerTimeLine timeLineConfig,System.Action finished_cb)
        {
            if(actionConfig==null) return;
            if(_level_obj==null) return;
             if (actionConfig.parameters1 == null)
                return;
            int count = actionConfig.parameters1.Length;
            for (int i = 0; i < count; i++)
            {
                var tr = _level_obj.transform.Find($"Root/{actionConfig.parameters1[i]}");
                if (tr != null)
                {
                    var spineAnimation = tr.GetComponent<SkeletonAnimation>();
                    if (spineAnimation != null)
                    {
                        spineAnimation.timeScale = 0;
                    }
                }
            }
            if (actionConfig.finishedTrigger == 1)
            {
                OnTimeLineCheckEnd(timeLineConfig, finished_cb);
            }
        }
        private void Action_SpineResume(TableAction actionConfig, TableCustomerTimeLine timeLineConfig,System.Action finished_cb)
        {
            if(actionConfig==null) return;
            if(_level_obj==null) return;
             if (actionConfig.parameters1 == null)
                return;
            int count = actionConfig.parameters1.Length;
            for (int i = 0; i < count; i++)
            {
                var tr = _level_obj.transform.Find($"Root/{actionConfig.parameters1[i]}");
                if (tr != null)
                {
                    var spineAnimation = tr.GetComponent<SkeletonAnimation>();
                    if (spineAnimation != null)
                    {
                        spineAnimation.timeScale = 1;
                    }
                }
            }
            if (actionConfig.finishedTrigger == 1)
            {
                OnTimeLineCheckEnd(timeLineConfig, finished_cb);
            }
        }
        private void Action_Move(TableAction actionConfig, TableCustomerTimeLine timeLineConfig,System.Action finished_cb)
        {
            if(actionConfig==null) return;
            if(_level_obj==null) return;
            if (
                actionConfig.parameters == null
                || actionConfig.parameters1 == null
                || actionConfig.parameters2 == null
                || actionConfig.parameters2.Length < 3
            )
                return;
            if (actionConfig.parameters.Length > 0)
            {
                if (actionConfig.parameters[0] <= 0)
                {
                    var tr = _level_obj.transform.Find($"Root/{actionConfig.parameters1[0]}");
                    if (tr != null)
                    {
                        float.TryParse(actionConfig.parameters2[0], out float x);
                        float.TryParse(actionConfig.parameters2[1], out float y);
                        float.TryParse(actionConfig.parameters2[2], out float z);
                        tr.localPosition = new Vector3(x, y, z);
                    }
                    if (actionConfig.finishedTrigger == 1)
                    {
                        OnTimeLineCheckEnd(timeLineConfig, finished_cb);
                    }
                }
                else
                {
                    var tr = _level_obj.transform.Find($"Root/{actionConfig.parameters1[0]}");
                    if (tr != null)
                    {
                        float.TryParse(actionConfig.parameters2[0], out float x);
                        float.TryParse(actionConfig.parameters2[1], out float y);
                        float.TryParse(actionConfig.parameters2[2], out float z);
                        var targetPos = new Vector3(x, y, z);
                        tr.DOLocalMove(
                            targetPos,
                            (float)actionConfig.parameters[0] / 1000.00f
                        ).onComplete = () =>
                        {
                            if (actionConfig.finishedTrigger == 1)
                            {
                                OnTimeLineCheckEnd(timeLineConfig, finished_cb);
                            }
                        };
                    }
                }
            }
        }

        private void Action_PlaySound(TableAction actionConfig,TableCustomerTimeLine timeLineConfig, System.Action finished_cb)
        {
             if(actionConfig==null) return;
            if (
                actionConfig.parameters == null
                || actionConfig.parameters1 == null
                || actionConfig.parameters1.Length <= 0
            )
                return;
            
            if (actionConfig.parameters.Length > 0)
            {
                bool loop = actionConfig.parameters[0] > 0;
                PlaySound(actionConfig.parameters1[0],loop);
            }
        }
         private void Action_StopSound(TableAction actionConfig,TableCustomerTimeLine timeLineConfig,System.Action finished_cb)
        {
            if (
                actionConfig.parameters1 == null
                || actionConfig.parameters1.Length <= 0
            )
                return;
            if (actionConfig.parameters1.Length > 0)
            {
                StopPlaySound(actionConfig.parameters1[0]);
            }
            if (actionConfig.finishedTrigger == 1)
            {
                OnTimeLineCheckEnd(timeLineConfig, finished_cb);
            }
        }
        private void Action_ActionStop(TableAction actionConfig,TableCustomerTimeLine timeLineConfig,System.Action finished_cb)
        {
            if(actionConfig==null) return;
            if (
                actionConfig.parameters == null
                || actionConfig.parameters.Length<1
            )
                return;

            if (actionConfig.finishedTrigger == 1)
            {
                OnTimeLineCheckEnd(timeLineConfig, finished_cb);
            }
        }
        public void PlayMusic(string musicName)
        {
            var clip = ResourcesManager.Instance.LoadResource<AudioClip>(
                $"Module/Asmr/Audio/{musicName}"
            );
            if (clip != null)
            {
                DragonU3DSDK.Audio.AudioManager.PlayMusic(clip, 1, true, true);
            }
        }

        public void PlaySound(string soundName,bool loop=false)
        {
            var clip = ResourcesManager.Instance.LoadResource<AudioClip>(
                $"Makeover/Levels/Level{_currentLevelConfig.levelId}/Audio/{soundName}"
            );
            //DebugUtil.Log($"Arthur---->PlaySound,soundName={soundName},Time={Time.time},loop={false}!");
            if (clip != null)
            {
               int soundId=DragonU3DSDK.Audio.AudioManager.PlaySound(clip,loop);
                if(!_soundDic.ContainsKey(soundName))
                {
                    _soundDic.Add(soundName,soundId);
                }
                else
                {
                    _soundDic[soundName]=soundId;
                }
            }
        }
        public void PlayCommonSound(string soundName,bool loop=false)
        {
            var clip = ResourcesManager.Instance.LoadResource<AudioClip>(
                $"Module/Asmr/Audio/{soundName}"
            );
            if (clip != null)
            {
               int soundId=DragonU3DSDK.Audio.AudioManager.PlaySound(clip,loop);
               if(!_soundDic.ContainsKey(soundName))
                {
                    _soundDic.Add(soundName,soundId);
                }
                else
                {
                    _soundDic[soundName]=soundId;
                }
            }
        }
        public void StopPlaySound(string soundName)
        {
            if(_soundDic.ContainsKey(soundName))
            {
               int soundId=_soundDic[soundName];
               DragonU3DSDK.Audio.AudioManager.StopSoundById(soundId);
                _soundDic.Remove(soundName);
            }
        }

        public void StopAll()
        {
            foreach (var _co in _co_ActionList)
            {
                StopCoroutine(_co);
            }
            _co_ActionList.Clear();
            foreach(var kvp in _soundDic)
            {
                DragonU3DSDK.Audio.AudioManager.StopSoundById(kvp.Value);
            }
            _soundDic.Clear();
        }
        
        private void Update()
        {
            if(_currentLevelConfig!=null && _currentLevelConfig.newLevel) _asmrLevel?.Update();
        }

        private void FixedUpdate()
        {
            if(_currentLevelConfig!=null && _currentLevelConfig.newLevel) _asmrLevel?.FixedUpdate();
        }
    }
}
