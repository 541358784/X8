using System;
using System.Collections.Generic;
using Activity.Base;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Vip.Model;
using Newtonsoft.Json;

namespace Scripts.UI
{
    public class PlayerInfoExtra
    {
        public string UserName;
        public int AvatarIcon;
        public int AvatarFrameIcon;
        public long TeamId;
        public string TeamName;
        public int Level;
        public Dictionary<string,I_ActivityStatus.ActivityStatus> StatusList;
        public string BlindBoxCollectState;
        public string CardCollectStateGolden;
        public string CardCollectStateNormal;
        public int VipLevel;

        public static PlayerInfoExtra GetMyPlayerInfoExtra()
        {
            var extra = new PlayerInfoExtra();
            extra.UserName = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName;
            extra.AvatarIcon = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId;
            extra.AvatarFrameIcon = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconFrameId;
            extra.TeamId = StorageManager.Instance.GetStorage<StorageHome>().Team.LastTeamId;
            extra.TeamName = StorageManager.Instance.GetStorage<StorageHome>().Team.LastTeamName;
            extra.Level = ExperenceModel.Instance.GetLevel();
            extra.StatusList = new Dictionary<string,I_ActivityStatus.ActivityStatus>();
            for (var i = 0; i < UIPopupSetController._activityStatus.Length; i++)
            {
                var key = UIPopupSetController._activityName[i];
                var activity = UIPopupSetController._activityStatus[i];
                extra.StatusList.Add(key,activity.GetActivityStatus());
            }
            extra.BlindBoxCollectState = BlindBoxModel.Instance.BlindBoxCollectStateStr();
            extra.CardCollectStateGolden = CardCollectionModel.Instance.GoldenCardThemeCollectStateStr();
            extra.CardCollectStateNormal = CardCollectionModel.Instance.NormalCardThemeCollectStateStr();
            extra.VipLevel = VipStoreModel.Instance.VipLevel();
            return extra;
        }

        public static PlayerInfoExtra GetNormalPlayerInfoExtra()
        {
            var extra = new PlayerInfoExtra();
            extra.UserName = "DefaultName";
            extra.AvatarIcon = 1;
            extra.AvatarFrameIcon = 1;
            extra.TeamId = 0;
            extra.TeamName = "";
            extra.Level = 1;
            extra.StatusList = new Dictionary<string,I_ActivityStatus.ActivityStatus>();
            for (var i = 0; i < UIPopupSetController._activityStatus.Length; i++)
            {
                var key = UIPopupSetController._activityName[i];
                extra.StatusList.Add(key,I_ActivityStatus.ActivityStatus.None);
            }

            extra.BlindBoxCollectState = "0/0";
            extra.CardCollectStateGolden = "0/0";
            extra.CardCollectStateNormal = "0/0";
            extra.VipLevel = 0;
            return extra;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static PlayerInfoExtra FromJson(string json)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<PlayerInfoExtra>(json);
                return result != null ? result:GetNormalPlayerInfoExtra();
            }
            catch (Exception e)
            {
                return GetNormalPlayerInfoExtra();
            }
        }
    }
}