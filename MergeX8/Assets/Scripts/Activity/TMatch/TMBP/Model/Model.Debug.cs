using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Newtonsoft.Json;
using OutsideGuide;
using TMatch;
// namespace TMatch
// {
    public partial class SROptions
    {
        private const string TMBP = "TMBP";
        [Category(TMBP)]
        [global::SROptions.DisplayName("清数据")]
        public void CleanData()
        {
            TMBPModel.Instance.Data.Clear();
                        
            StorageDecorationGuide storage = StorageManager.Instance.GetStorage<StorageDecorationGuide>();
            if (storage.GuideData.ContainsKey(10142))
            {
                storage.GuideData.Remove(10142);
            }
            if (storage.GuideData.ContainsKey(10143))
            {
                storage.GuideData.Remove(10143);
            }
            if (DecoGuideManager.Instance.GetDataDict().ContainsKey(10142))
            {
                DecoGuideManager.Instance.GetDataDict().Remove(10142);
            }
            if (DecoGuideManager.Instance.GetDataDict().ContainsKey(10143))
            {
                DecoGuideManager.Instance.GetDataDict().Remove(10143);
            }
        }

        private int _addLevel = 1;
        [Category(TMBP)]
        [global::SROptions.DisplayName("增加等级值")]
        public int Level
        {
            get { return _addLevel; }
            set { _addLevel = value; }
        }
        [Category(TMBP)]
        [global::SROptions.DisplayName("增加等级")]
        public void AddLevel()
        {
            TMBPModel.Instance.AddLevel(_addLevel);
        }
        
        private int _addExp = 1;
        [Category(TMBP)]
        [global::SROptions.DisplayName("增加经验值")]
        public int Exp
        {
            get { return _addExp; }
            set { _addExp = value; }
        }
        [Category(TMBP)]
        [global::SROptions.DisplayName("增加经验")]
        public void AddExp()
        {
            TMBPModel.Instance.AddExp(_addExp);
        }
    }
// }