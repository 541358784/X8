using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DragonU3DSDK.Asset;
using Spine;
using Spine.Unity;
using DG.Tweening;
using DigTrench.UI;
using DragonPlus;
using DragonPlus.Config.DigTrench;
using DragonU3DSDK.Network.API.Protocol;
// using DragonPlus.Config.Makeover;
// using Makeover;
using MiniGame;
using Skeleton = Spine.Skeleton;

namespace DigTrench
{
    public partial class Model : Manager<Model>
    {
        public GameObject bodyRoot;
        private DigTrenchLevel _currentLevelConfig;
        private GameObject _level_obj;
        // private List<Coroutine> _co_ActionList = new List<Coroutine>();
        private Dictionary<string, int> _soundDic = new Dictionary<string, int>();

        protected override void InitImmediately()
        {
            base.InitImmediately();
            EventDispatcher.Instance.AddEventListener(EventEnum.FinishDigTrenchGame, QuitGame);
            EventDispatcher.Instance.AddEventListener(EventEnum.CompletedDigTrenchGame,OnFinishLevel);
        }
        void QuitGame(BaseEvent evt)//游戏弹板点击后结束游戏
        {
            StopAllCoroutines();
            UnLoadCurrentLevel();
            SceneFsm.mInstance.ChangeState(StatusType.ExitDigTrench);
        }

        public void OnFinishLevel(BaseEvent evt)
        {
            DigTrenchEntryControllerModel.Instance.FinishLevel(_currentLevelConfig);
        }

        public void LoadLevel(DigTrenchLevel levelConfig,bool isFirstTimePlay)
        {
            _currentLevelConfig = levelConfig;
            bodyRoot = new GameObject("Body");
            bodyRoot.transform.parent = DigTrench.Utils.GetOrCreateRoot().transform;

            var resPath = $"DigTrench/Level/LevelType{_currentLevelConfig.levelResType}/Level{_currentLevelConfig.id}";
            var level_prefab = ResourcesManager.Instance.LoadResource<GameObject>(resPath);
            if (level_prefab == null)
            {
                Debug.LogError("未成功加载"+resPath);
                return;   
            }

            _level_obj = Instantiate(level_prefab,bodyRoot.transform,false);
            _level_obj.transform.localPosition = new Vector3(0, 0, -500);
            _level_obj.transform.localScale =
                new Vector3(_currentLevelConfig.adaptScasle, _currentLevelConfig.adaptScasle, 1);
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameWaterLevelstart,
                data1:_currentLevelConfig.id.ToString());
            var gameModel = _level_obj.AddComponent<DigTrenchModel>();
            gameModel.Init(_level_obj.transform,isFirstTimePlay);
            
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
        }

        public void PlaySound(string soundName,bool loop=false)
        {
            var clip = ResourcesManager.Instance.LoadResource<AudioClip>(
                $"DigTrench/Levels/LevelType{_currentLevelConfig.levelResType}/Audio/{soundName}"
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
            foreach(var kvp in _soundDic)
            {
                DragonU3DSDK.Audio.AudioManager.StopSoundById(kvp.Value);
            }
            _soundDic.Clear();
        }
    }
}
