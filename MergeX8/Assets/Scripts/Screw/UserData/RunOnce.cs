using System;
using DragonU3DSDK.Storage;

namespace Screw.UserData
{
    public class RunOnce
    {
        private static StorageScrew storageScrew
        {
            get
            {
                return  StorageManager.Instance.GetStorage<StorageScrew>();
            }
        }
        
        public static void OnRunOnce(string key, Action action)
        {
            if(storageScrew.Recored.ContainsKey(key))
                return;
            
            storageScrew.Recored.Add(key, key);
            action?.Invoke();
        }
    }

    public interface IRunOnce
    {
        public void OnRunOnce();
    }
}