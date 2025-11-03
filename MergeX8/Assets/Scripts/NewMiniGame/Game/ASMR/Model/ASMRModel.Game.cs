using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using MiniGame;
using UnityEngine;
using AsmrLevel = fsm_new.AsmrLevel;

namespace ASMR
{
    public partial class ASMRModel : Manager<ASMRModel>
    {
        //完成的动作计数
        private int _finishedActionCount = 0;
        private int _actionFinishedCheckCount = 0;

        public StorageASMRLevel _storageLevel;

        private int _stepIndex = 0;

        //当前步骤完成计数
        private int _currentStepCount = 0;

        private string _levelResPath;

        private AsmrLevel _asmrLevel;

        private bool _taskRunging = false;


        public int GetCurrentCompltete()
        {
            return _asmrLevel.GetCurrentCompltete();
        }

        public void LoadLevelbyLevelId(int levelId, AttachData data)
        {
            AudioManager.Instance.PauseAllMusic();
            PlayerManager.Instance.HidePlayer();
            
            AttData = data;
            _levelConfig = AsmrLevelConfigs.Find(c => c.Id == levelId);

            _levelResPath = $"NewMiniGame/ASMR/{levelId}/Prefabs/Level";

            _storageLevel = GetLevelStorageByLevelId(_levelConfig.Id);

            if (_levelConfig != null)
            {
                var levelPrefab = ResourcesManager.Instance.LoadResource<GameObject>(_levelResPath); //已释放
                if (levelPrefab == null) return;

                var level = Object.Instantiate(levelPrefab);
                level.transform.Reset();

                _asmrLevel = new AsmrLevel(level);
                _asmrLevel.Init(_levelConfig);
            }

            UIHomeMainController.mainController.AnimShowMainUI(false, true);
        }

        public void UnLoadCurrentLevel()
        {
            AudioManager.Instance.ResumeAllMusic();
            PlayerManager.Instance.RecoverPlayer();

            if (_asmrLevel != null)
            {
                _asmrLevel.Release();
                _asmrLevel = null;
            }

            _soundList.ForEach(p => ResourcesManager.Instance.ReleaseRes(p, true));
            
            

            UIHomeMainController.mainController.AnimShowMainUI(true, true);
        }

        public void QuitGame()
        {
            UnLoadCurrentLevel();
        }

        public void FinishedLevel()
        {
            if (_storageLevel != null)
            {
                _storageLevel.IsFinished = true;
                _storageLevel.MaxComplete = 100;
            }
        }

        private List<string> _soundList = new();

        public void PlaySound(string soundName, bool loop = false)
        {
            var path = $"NewMiniGame/ASMR/{_levelConfig.ResId}/Audio/{soundName}";
            var clip = ResourcesManager.Instance.LoadResource<AudioClip>(path); //已释放
            _soundList.Add(path);

            if (clip == null) return;

            AudioManager.Instance.PlaySound(clip, loop);
        }

        public void PlayCommonSound(string soundName, bool loop = false)
        {
            var path = $"NewMiniGame/ASMR/Common/Audio/{soundName}";
            var clip = ResourcesManager.Instance.LoadResource<AudioClip>(path); //已释放
            _soundList.Add(path);

            if (clip == null) return;

            AudioManager.Instance.PlaySound(clip, loop);
        }


        public void PlayMusic(string music, bool loop = true)
        {
            var path = $"NewMiniGame/Asmr/LevelMain/Level{_levelConfig.ResId}/Audio/{music}";
            var clip = ResourcesManager.Instance.LoadResource<AudioClip>(path); //已释放
            _soundList.Add(path);

            if (clip == null) return;

            AudioManager.Instance.PlayMusic(clip);
        }

        public void Debug_Finish()
        {
            _asmrLevel.ChangeState_Win();
        }

        public void Update()
        {
            if (_levelConfig != null && _asmrLevel != null) _asmrLevel.Update();
        }

        public void FixedUpdate()
        {
            if (_levelConfig != null && _asmrLevel != null) _asmrLevel.FixedUpdate();
        }
    }
}