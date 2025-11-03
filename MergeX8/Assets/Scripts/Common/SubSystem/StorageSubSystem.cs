using System.Collections.Generic;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Framework;
using UnityEngine;


namespace Gameplay
{
    public class StorageSubSystem : SubSystem, IInitable
    {
        public void Init()
        {
            List<StorageBase> storageBases = new List<StorageBase>();
            storageBases.Add(new StorageCommon());
            storageBases.Add(new StorageHome());
            storageBases.Add(new StorageGame());
            storageBases.Add(new StorageDecoration());
            
            
            storageBases.Add(new StorageTMatch());
            storageBases.Add(new StorageCurrencyTMatch());
            storageBases.Add(new StorageDecorationGuide());
            
            
            storageBases.Add(new StorageScrew());
            storageBases.Add(new StorageMiniGames());
            
            storageBases.Add(new StorageFarm());
            
            storageBases.Add(new StorageASMR());
            storageBases.Add(new StorageMiniGameVersion());
            
            StorageManager.Instance.Init(storageBases);

            //因操作频繁 在游戏频繁存档卡 改成5秒存档一次
            StorageManager.Instance.LocalInterval = 5;
            StorageManager.Instance.RemoteInterval = 90;
        }

        public void Release()
        {
        }
    }
}