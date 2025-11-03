using System;
using Cysharp.Threading.Tasks;
using Decoration;
using Decoration.Bubble;
using Decoration.DynamicMap;
using Decoration.WorldFogManager;
using Farm.View;
using Manager;
using MiniGame;
using Scripts.UI;
using UnityEngine;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public const int worldId = 2;

        private int[] mainUIAdaptOffset = new[]
        {
            175,
            140,
        };
        
        
        public async UniTask LoadWorld(int id, Action action = null)
        {
            storageFarm.IsEnter = true;
            
            UnLoadWorld();
            DecoManager.Instance.LoadWorlds();
            await UniTask.WaitForEndOfFrame();
            
            DecoManager.Instance.EnterWorld(id, (process) =>
            {
            }, (success) =>
            {
            });

            while (!DecoManager.Instance.IsWorldReady)
            {
                await UniTask.WaitForEndOfFrame();
            }
        
            DecoManager.Instance.ShowWorld(id,true,true);
            DecoManager.Instance.EnableUpdate = false;
            WorldFogManager.Instance.Init();
            await UniTask.WaitForEndOfFrame();
            DynamicMapManager.Instance.InitDynamicObject(DecoSceneRoot.Instance.mSceneCamera.gameObject);
            DynamicMapManager.Instance.ForceLoadCurrentChunk();
            await UniTask.WaitForEndOfFrame();
            await UniTask.WaitForSeconds(0.2f);
            
            DecoManager.Instance.EnableUpdate = true;
            UIRoot.Instance.mWorldUIRoot.gameObject.SetActive(true);
            DecoSceneRoot.Instance.mSceneCamera.gameObject.SetActive(true);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, true);
            
            PlayerManager.Instance.UpdatePlayersState(false);
            
            DecoManager.Instance.CurrentWorld.PinchMap.SetMaxCameraSize(Decoration.DecorationConfigManager.Instance.GetGlobalConfigNumber(GlobalNumberConfigKey.deco_FarmMaxCameraSize));
            
            action?.Invoke();
        }
        
        private void UnLoadWorld()
        {
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, false);
            WorldFogManager.Instance.UnLoad();
            DynamicMapManager.Instance.UnLoad();
            NodeBubbleManager.Instance.UnLoadBubble();
            DecoManager.Instance.CleanAll();
        }

        public async UniTask LeaveWorld(bool changeState = true, Action action = null)
        {
            bool isLeaveSuccess = false;
            
            DecoManager.Instance.CurrentWorldId = 1;
            
            UIFarmLoadingController.ShowLoading(() =>
            {
                if (GameModeManager.Instance.GetCurrenGameMode() == GameModeManager.CurrentGameMode.MiniGame)
                {
                    Framework.UI.UIManager.Instance.Close<UIChapter>();
                    MiniGameModel.Instance.LoadMiniGame();
                }
                
                LoadWorld(1, () =>
                {
                    float maxSize = Decoration.DecorationConfigManager.Instance.GetGlobalConfigNumber(GlobalNumberConfigKey.deco_MaxCameraSize);
                    DecoSceneRoot.Instance.mSceneCamera.orthographicSize = 16;
                    DecoManager.Instance.CurrentWorld.PinchMap.SetMaxCameraSize(maxSize);

                    UIFarmLoadingController.HideLoading(() =>
                    {
                        action?.Invoke();
                        isLeaveSuccess = true;
                    });            
                    
                    if (GameModeManager.Instance.GetCurrenGameMode() == GameModeManager.CurrentGameMode.MiniGame)
                        DecoManager.Instance.CurrentWorld?.HideByPosition();
                });
                
                Release();
                UIManager.Instance.CloseUI(UINameConst.UIFarmMain, true);
                AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, true, true);
                PlayerManager.Instance.RecoverPlayer();
            
                AdaptMainUI(false);

                if(changeState)
                    SceneFsm.mInstance.ChangeState(StatusType.BackHome);
            });

            while (!isLeaveSuccess)
            {
                await UniTask.WaitForEndOfFrame();
            }
        }

        public void AdaptMainUI(bool isAdapt)
        {
            if(UIHomeMainController.mainController == null)
                return;

            RectTransform rectTransform = (RectTransform)UIHomeMainController.mainController.transform.Find("Root").transform;

            int offset = mainUIAdaptOffset[0];
            if (CommonUtils.IsLE_16_10())
            {
                offset = mainUIAdaptOffset[1];
            }

            if (!isAdapt)
                offset = -offset;

            CommonUtils.AdaptRectTransformTop(rectTransform, offset);
        }
    }
}