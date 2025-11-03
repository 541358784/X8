using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Filthy;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Filthy.Event;
using Filthy.Model;
using Filthy.Procedure;
using Filthy.View;
using UnityEngine;

namespace Filthy.Game
{
    public partial class FilthyGameLogic : Manager<FilthyGameLogic>
    {
         private enum PlayType
        {
            Spine=1,
            Merge=2,
            Screw=3,
        }
        
        private GameObject _uiPrefab;
        public UIFilthyController _uiScript;
        public UIFilthyProcedureController _uiProcedureScript;
        private FilthyNodes _nodeConfig;
        private Dictionary<int, int> _soundIds = new Dictionary<int, int>();

        public ProcedureLogic _procedureLogic;
        
        public void LoadLevel()
        {
            Release();
            
            var resPath = ConstValue.ConstValue.FilthyMainLevelPath(FilthyModel.Instance.ResLevelId());
            var prefab = ResourcesManager.Instance.LoadResource<GameObject>(resPath, addToCache:false);
            if(prefab == null)
                return;
            
            _uiPrefab = Instantiate(prefab, UIRoot.Instance.mUIRoot.transform, false) as GameObject;

            _procedureLogic = new ProcedureLogic(FilthyModel.Instance.ResLevelId(), _uiPrefab.gameObject.transform);
            if (_procedureLogic.IsProcedureLevel())
            {
                _uiProcedureScript = _uiPrefab.AddComponent<UIFilthyProcedureController>();
            }
            else
            {
                _uiScript = _uiPrefab.AddComponent<UIFilthyController>();
            }

        }

        public void Release()
        {
            _procedureLogic = null;
            
            if(_uiPrefab != null)
                DestroyImmediate(_uiPrefab);

            _uiPrefab = null;
            _uiScript = null;
            _uiProcedureScript = null;

            StopSounds();
        }

        public void EnterGame(FilthyNodes config)
        {
            _nodeConfig = config;

            //GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrstart, _nodeConfig.Id.ToString());

            switch (config.Type)
            {
                case (int)PlayType.Spine:
                {
                    StopSounds();

                    LoadSpineLevel(config);
            
                    UILoadingTransitionController.Show(null);
                    XUtility.WaitSeconds(0.3f, () =>
                    {
                        _uiPrefab?.gameObject.SetActive(false);
            
                        UILoadingTransitionController.Hide(() =>
                        {
                            EnterSpineFinishAnim();
                        });
                    });
                    
                    break;
                }
                case (int)PlayType.Merge:
                {
                    LoadMergeLevel(config);
                    break;
                }
                case (int)PlayType.Screw:
                {
                    StopSounds();

                    LoadScrewLevel(config);
            
                    UILoadingTransitionController.Show(null);
                    XUtility.WaitSeconds(0.3f, () =>
                    {
                        _uiPrefab?.gameObject.SetActive(false);
            
                        UILoadingTransitionController.Hide(() =>
                        {
                            EnterSpineFinishAnim();
                        });
                    });
                    break;
                }
            }
        }

        public void ExitGame(FilthyNodes config)
        {
            if (!FilthyModel.Instance.IsFinish(FilthyModel.Instance._config.LevelId))
            {
                if (!config.DefaultAudio.IsEmptyString())
                    PlaySound(config.DefaultAudio, true);
            }
            
            bool isSuccess = false;
            switch (config.Type)
            {
                case (int)PlayType.Spine:
                {
                    UILoadingTransitionController.Show(null);
                    XUtility.WaitSeconds(0.3f, () =>
                    {
                        _uiPrefab?.gameObject.SetActive(true);
                
                        bool isRefreshState = ExitSpineLevel();
                        isSuccess = isRefreshState;
                        if(isSuccess)
                            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrend, _nodeConfig.Id.ToString());
                        if(isRefreshState)
                            EventDispatcher.Instance.DispatchEvent(ConstEvent.Event_Refresh_State);
                        
                        _nodeConfig = null;
                        UILoadingTransitionController.Hide(()=>
                        {
                        });
                    });
                    break;
                }
                case (int)PlayType.Merge:
                {
                    bool isRefreshState = ExitMergeLevel();
                    
                    isSuccess = isRefreshState;
                    if(isRefreshState)
                        EventDispatcher.Instance.DispatchEvent(ConstEvent.Event_Refresh_State);
                    if(isSuccess)
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrend, _nodeConfig.Id.ToString());
                    break;
                }
                case (int)PlayType.Screw:
                {
                    bool isRefreshState = ExitScrewLevel();
                    
                    _uiPrefab?.gameObject.SetActive(true);
                    
                    isSuccess = isRefreshState;
                    if(isRefreshState)
                        EventDispatcher.Instance.DispatchEvent(ConstEvent.Event_Refresh_State);
                    if(isSuccess)
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrend, _nodeConfig.Id.ToString());
                    
                    break;
                }
            }
        }

        public void PlaySound(string name, bool loop = false)
        {
            var resPath = ConstValue.ConstValue.FilthyAudioPath(FilthyModel.Instance.ResLevelId(), name);

            var clip = ResourcesManager.Instance.LoadResource<AudioClip>(resPath);
            if(clip == null)
                return;
            
            int soundId = DragonU3DSDK.Audio.AudioManager.PlaySound(clip,loop);
            if(_soundIds.ContainsKey(soundId))
                return;
            
            _soundIds.Add(soundId, soundId);
        }

        public void StopSounds()
        {
            foreach (var keyValuePair in _soundIds)
            {
                DragonU3DSDK.Audio.AudioManager.StopSoundById(keyValuePair.Value);
            }
            
            _soundIds.Clear();
        }

        public void Update()
        {
            if(_procedureLogic == null)
                return;
            
            _procedureLogic.Update();
        }

        public void TriggerProcedure(TriggerType type, string param)
        {
            if(_procedureLogic == null)
                return;
            
            _procedureLogic.TriggerProcedure(type, param);
        }

        public void SetFithyUIActive(bool isActive)
        {
            _uiPrefab?.gameObject.SetActive(isActive);
        }
    }
}