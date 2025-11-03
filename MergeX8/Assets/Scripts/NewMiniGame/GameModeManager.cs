using System;
using Cysharp.Threading.Tasks;
using Decoration;
using DragonU3DSDK.Storage;
using MiniGame;
using MiniGame.View;
using Scripts.UI;

namespace Manager
{
    public class GameModeManager : Singleton<GameModeManager>
    {
        public enum GameMode
        {
            None,
            DecoAndMerge,
            MiniAndMerge,
            Count,
        }
        
        public enum CurrentGameMode
        {
            None,
            Deco,
            MiniGame,
            Count,
        }
        
        private StorageHome _storageGlobal => StorageManager.Instance.GetStorage<StorageHome>(); 
        public void InitGameMode()
        {
            if(_storageGlobal.GameMode != (int)GameMode.None)
                return;

            if (_storageGlobal.IsFirstLogin || !AdLocalConfigHandle.Instance.IsOpenMiniGame())
            {
                _storageGlobal.GameMode = (int)GameMode.DecoAndMerge;
                _storageGlobal.CurrentGameMode = (int)CurrentGameMode.Deco;
                return;
            }

            if (MiniGameModel.Instance.IsOpen())
            {
                _storageGlobal.GameMode = (int)GameMode.MiniAndMerge;
                _storageGlobal.CurrentGameMode = (int)CurrentGameMode.MiniGame;
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests["OpenMiniGame"] = "TRUE";
            }
            else
            {
                _storageGlobal.GameMode = (int)GameMode.DecoAndMerge;
                _storageGlobal.CurrentGameMode = (int)CurrentGameMode.Deco;
            }
        }

        public GameMode GetGameMode()
        {
            return (GameMode)_storageGlobal.GameMode;
        }
        
        public void SetGameMode(GameMode mode)
        {
            _storageGlobal.GameMode = (int)mode;
        }
        
        public CurrentGameMode GetCurrenGameMode()
        {
            return (CurrentGameMode)_storageGlobal.CurrentGameMode;
        }

        public void SetCurrentGameMode(CurrentGameMode mode)
        {
            if(_storageGlobal.GameMode != (int)GameMode.MiniAndMerge)
                return;
            
            _storageGlobal.CurrentGameMode = (int)mode;

        }

        public async UniTask RefreshGameStatus(bool isLoading, Action action = null)
        {
            if (!isLoading)
            {
                switch (GetCurrenGameMode())
                {
                    case CurrentGameMode.Deco:
                    {
                        ShowDeco();
                        action?.Invoke();
                        break;
                    }
                    case CurrentGameMode.MiniGame:
                    {
                        ShowMiniGame();
                        action?.Invoke();
                        break;
                    }
                }
                
                return;
            }

            bool isShow = false;
            
            UIUIMiniGameLoadingController.ShowLoading(() =>
            {
                isShow = true;
                
                switch (GetCurrenGameMode())
                {
                    case CurrentGameMode.Deco:
                    {
                        ShowDeco();
                        action?.Invoke();
                        break;
                    }
                    case CurrentGameMode.MiniGame:
                    {
                        ShowMiniGame();
                        action?.Invoke();
                        break;
                    }
                }
            });

            while (!isShow)
            {
                await UniTask.WaitForEndOfFrame();
            }

            await UniTask.WaitForSeconds(0.5f);
            
            UIUIMiniGameLoadingController.HideLoading(action);
        }

        private void ShowDeco()
        {
            DecoManager.Instance.EnableUpdate = true;
            DecoManager.Instance.CurrentWorld?.ShowByPosition();
            UIRoot.Instance.mWorldUIRoot.gameObject.SetActive(true);
            DecoSceneRoot.Instance.mSceneCamera.gameObject.SetActive(true);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, false);
                    
            Framework.UI.UIManager.Instance.Close<UIChapter>();
        }

        private void ShowMiniGame()
        {
            var uiChapter = Framework.UI.UIManager.Instance.GetView<UIChapter>();
            int chapterId = MiniGameModel.Instance.CurrentChapter;
            if (uiChapter == null || chapterId != uiChapter.ChapterId)
            {
                Framework.UI.UIManager.Instance.Close<UIChapter>();
                MiniGameModel.Instance.LoadMiniGame();
            }
                    
            UIRoot.Instance.mWorldUIRoot.gameObject.SetActive(false);
            DecoManager.Instance.EnableUpdate = false;
            DecoManager.Instance.CurrentWorld?.HideByPosition();
        }
    }
}