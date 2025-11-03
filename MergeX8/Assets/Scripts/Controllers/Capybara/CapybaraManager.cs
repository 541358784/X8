using DragonU3DSDK.Storage;

namespace Gameplay.UI.Capybara
{
    public class CapybaraManager : Singleton<CapybaraManager>
    {
        public const string recordKey = "CapybaraRecordKey";
        
        public StorageHome storageHome
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>();
            }
        }
        
        public void Init()
        {
            if(storageHome.RcoveryRecord.ContainsKey(recordKey))
                return;
            
            storageHome.RcoveryRecord.Add(recordKey, true);
        }

        public bool IsOpenCapybara()
        {
            return storageHome.RcoveryRecord.ContainsKey(recordKey);
        }
    }
}