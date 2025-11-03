using System.Reflection;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Quality;
using DragonU3DSDK.Storage;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// MyMain is the only entry monobehavior for this project
    /// </summary>
    public class MyMain : MonoBehaviour
    {
        private static MyGame _game;

        public static MyGame Game
        {
            get { return _game; }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitComponents();

            _game = new MyGame();
            _game.Init();

            bool isOpenSRDebug = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey("OpenSRDebug");
            if (Debug.isDebugBuild || isOpenSRDebug)
            {
                FieldInfo fieldInfo = SRDebugger.Settings.Instance.GetType().GetField("_triggerPosition",
                    BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding);
                fieldInfo.SetValue(SRDebugger.Settings.Instance, SRDebugger.PinAlignment.TopCenter);

                SRDebug.Init();
                SRServiceManager.GetService<IDebugTriggerService>().IsEnabled = true;
                
                Debug.unityLogger.logEnabled = true;
            }
        }

        private void OnApplicationQuit()
        {
            DragonPlus.GameBIManager.Instance.SendGameEventImmediately(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventLogout,
                data1: MergeManager.LeftEmptyGridCount(MergeBoardEnum.Main).ToString(),
                data2:EnergyModel.GetEnergyNumber().ToString(),
                data3:EnergyModel.GetCoinNumber().ToString());
            PlayerPrefs.Save();
            StorageManager.Instance.SaveToLocal();
        }

        private void Start()
        {
            _game.Start();
            //启动游戏
            SceneFsm.mInstance.StartGame();
        }

        private void Update()
        {
            _game.Update();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            _game.OnApplicationPause(pauseStatus);
            PlayerPrefs.Save();
            StorageManager.Instance.SaveToLocal();
            AppIconChangerSystem.Instance.OnApplicationPause(pauseStatus);
        }

        private void OnDestroy()
        {
            _game.Release();
        }

        private void InitComponents()
        {
            gameObject.AddComponent<CustomQualityManager>();
            gameObject.AddComponent<GameMain>();
            gameObject.AddComponent<SceneFsm>();
            gameObject.AddComponent<SDKEventsHandler>();
            gameObject.AddComponent<GameBIManager>();
        }
    }
}