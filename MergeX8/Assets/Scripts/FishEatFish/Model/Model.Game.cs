using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DragonU3DSDK.Asset;
using Spine;
using Spine.Unity;
using DG.Tweening;
using FishEatFishSpace;
using DragonPlus;
using DragonPlus.Config.FishEatFish;
using DragonU3DSDK.Network.API.Protocol;
// using DragonPlus.Config.Makeover;
// using Makeover;
using MiniGame;
using Skeleton = Spine.Skeleton;

namespace FishEatFishSpace
{
    public partial class Model : Manager<Model>
    {
        public GameObject bodyRoot;
        private FishEatFishLevel _currentLevelConfig;
        private GameObject _level_obj;
        // private List<Coroutine> _co_ActionList = new List<Coroutine>();
        private Dictionary<string, int> _soundDic = new Dictionary<string, int>();

        protected override void InitImmediately()
        {
            base.InitImmediately();
            EventDispatcher.Instance.AddEventListener(EventEnum.FinishFishEatFishGame, QuitGame);
            EventDispatcher.Instance.AddEventListener(EventEnum.CompletedFishEatFishGame,OnFinishLevel);
        }
        void QuitGame(BaseEvent evt)//游戏弹板点击后结束游戏
        {
            StopAllCoroutines();
            UnLoadCurrentLevel();
            SceneFsm.mInstance.ChangeState(StatusType.ExitFishEatFish);
        }

        public void OnFinishLevel(BaseEvent evt)
        {
            FishEatFishEntryControllerModel.Instance.FinishLevel(_currentLevelConfig);
        }

        public void LoadLevel(FishEatFishLevel levelConfig,bool isFirstTimePlay)
        {
            _currentLevelConfig = levelConfig;
            bodyRoot = new GameObject("Body");
            bodyRoot.transform.parent = FishEatFishSpace.Utils.GetOrCreateRoot().transform;
            if (levelConfig.type == 1)
            {
                var fishGame = FishGame.CreateGame(bodyRoot.transform);
                fishGame.Initial(levelConfig.levelId,levelConfig.id);
            }
            else if (levelConfig.type == 2)
            {
                var fishGame = FishGameTwo.CreateGame(bodyRoot.transform);
                fishGame.Initial(levelConfig.levelId,levelConfig.id);
            }
            var mainUI = UIManager.Instance.OpenUI(UINameConst.UIFishEatFishMain) as UIFishEatFishMainController;
            mainUI.BindLevelConfig(levelConfig.id,isFirstTimePlay,levelConfig.type);
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFishLevelstart,
                data1:_currentLevelConfig.id.ToString());
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
                $"FishEatFish/Levels/LevelType{_currentLevelConfig.levelResType}/Audio/{soundName}"
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
