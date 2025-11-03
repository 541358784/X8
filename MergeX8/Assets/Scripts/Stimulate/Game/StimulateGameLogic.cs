using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Stimulate.Event;
using Stimulate.View;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Stimulate.Model
{
    public partial class StimulateGameLogic : Manager<StimulateGameLogic>
    {
        private enum PlayType
        {
            Spine=1,
            Merge=2,
        }
        
        private GameObject _uiPrefab;
        private UIStimulateController _uiScript;
        private TableStimulateNodes _nodeConfig;
        private Dictionary<string, PlayableAsset> _playableAssets = new Dictionary<string, PlayableAsset>();
        private Dictionary<int, int> _soundIds = new Dictionary<int, int>();
        
        public void LoadLevel()
        {
            Release();
            
            var resPath = $"Stimulate/Levels/Level{StimulateModel.Instance._config.levelId}/Prefabs/UIStimulate";
            var prefab = ResourcesManager.Instance.LoadResource<GameObject>(resPath);
            if(prefab == null)
                return;
            
            _uiPrefab = Instantiate(prefab, UIRoot.Instance.mUIRoot.transform, false) as GameObject;
            _uiScript = _uiPrefab.AddComponent<UIStimulateController>();
        }

        public void Release()
        {
            if(_uiPrefab != null)
                DestroyImmediate(_uiPrefab);

            _uiPrefab = null;
            _uiScript = null;
            _playableAssets.Clear();

            StopSounds();
        }

        public void EnterGame(TableStimulateNodes config)
        {
            _nodeConfig = config;

            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrstart, _nodeConfig.id.ToString());

            switch (config.type)
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
            }
        }

        public void ExitGame(TableStimulateNodes config)
        {
            if (!StimulateModel.Instance.IsFinish(StimulateModel.Instance._config))
            {
                if (!config.defaultAudio.IsEmptyString())
                    PlaySound(config.defaultAudio, true);
            }
            
            bool isSuccess = false;
            switch (config.type)
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
                            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrend, _nodeConfig.id.ToString());
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
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrend, _nodeConfig.id.ToString());
                    break;
                }
            }
        }

        public PlayableAsset GetPlayableAsset(string name)
        {
            if (_playableAssets.ContainsKey(name))
                return _playableAssets[name];
            
            var resPath = $"Stimulate/Levels/Level{StimulateModel.Instance._config.levelId}/Prefabs/{name}";
            var prefab = ResourcesManager.Instance.LoadResource<PlayableAsset>(resPath);
            if (prefab == null)
                return null;
            
            _playableAssets.Add(name, prefab);

            return prefab;
        }

        public void PlaySound(string name, bool loop = false)
        {
            var resPath = $"Stimulate/Levels/Level{StimulateModel.Instance._config.levelId}/Audio/{name}";

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
    }
}