using System;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace DragonPlus
{
    public class PrivacyModel
    {
        private static PrivacyModel instance = null;
        private static readonly object syslock = new object();

        public static PrivacyModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new PrivacyModel();
                        }
                    }
                }

                return instance;
            }
        }

        private PrivacyModel()
        {
        }

        public void SetPrivacyAgreed(bool agreed)
        {
            PlayerPrefs.SetInt("GDPR", 1);

            StorageHome home = StorageManager.Instance.GetStorage<StorageHome>();
            home.Privacy_agreed = agreed;
        }


        public bool ShouldShowPrivacy()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            StorageHome home = StorageManager.Instance.GetStorage<StorageHome>();

            bool isAgreedStorage = home.Privacy_agreed;

            bool isAgreedGlobal = PlayerPrefs.HasKey("GDPR");

            if (isAgreedStorage && !isAgreedGlobal)
            {
                home.Privacy_agreed = true;
            }

            return !(home.Privacy_agreed || isAgreedGlobal);
#else
            return false;
#endif
            return false;
        }

        public void TryToShowPrivacy()
        {
            if (!ShouldShowPrivacy())
            {
                return;
            }
            
            UIManager.Instance.OpenUI("PrivacyPolicy", windowType:UIWindowType.Normal, windowLayer:UIWindowLayer.Max, type:typeof(PrivacyPolicyController), false);
        }
    }
}