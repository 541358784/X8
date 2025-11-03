using System;
using DragonU3DSDK.Account;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace Gameplay.UI.BindEmail
{
    public class UIPopupBindEmailController : UIWindowController
    {
        private UIPopupBindEmail_ShowReward _stage_showReward;
        private UIPopupBindEmail_Eighteen _stage_eighteen;
        private UIPopupBindEmail_ErrorTips _stage_errorTips;
        private UIPopupBindEmail_InputEmail _stage_inputEmail;
        private UIPopupBindEmail_Code _stage_inputCode;
        private UIPopupBindEmail_GetReward _stage_getReward;

        private bool _isRecoverUI = false;
        
        public override void PrivateAwake()
        {
            _stage_showReward = InitCombineMono<UIPopupBindEmail_ShowReward>("Root/Step1");
            _stage_showReward.Init(this);
            
            _stage_eighteen = InitCombineMono<UIPopupBindEmail_Eighteen>("Root/Step2");
            _stage_eighteen.Init(this);
            
            _stage_inputEmail = InitCombineMono<UIPopupBindEmail_InputEmail>("Root/Step3");
            _stage_inputEmail.Init(this);
            
            _stage_inputCode = InitCombineMono<UIPopupBindEmail_Code>("Root/Step4");
            _stage_inputCode.Init(this);
            
            _stage_getReward = InitCombineMono<UIPopupBindEmail_GetReward>("Root/Step5");
            _stage_getReward.Init(this);
            
            _stage_errorTips = InitCombineMono<UIPopupBindEmail_ErrorTips>("Root/Exit");
            _stage_errorTips.Init(this);
        }

        private T InitCombineMono<T>(string path) where T : MonoBehaviour
        {
            return transform.Find(path).gameObject.AddComponent<T>();
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            if (objs != null && objs.Length > 0)
                _isRecoverUI = true;
        }
        
        

        private void Start()
        {
            int stage = StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.Stage;
            stage = (stage == 3 || stage == 4) ? stage : 0;

            NextStage(stage);
        }

        public void NextStage(int stage)
        {
            StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.Stage = stage;
            
            _stage_showReward.gameObject.SetActive(false);
            _stage_eighteen.gameObject.SetActive(false);
            _stage_inputEmail.gameObject.SetActive(false);
            _stage_inputCode.gameObject.SetActive(false);
            _stage_getReward.gameObject.SetActive(false);
            _stage_errorTips.gameObject.SetActive(false);
            switch (stage)
            {
                case 0:
                {
                    _stage_showReward.gameObject.SetActive(true);
                    break;
                }
                case 1:
                {
                    _stage_eighteen.gameObject.SetActive(true);
                    break;
                }
                case 2:
                {
                    _stage_inputEmail.gameObject.SetActive(true);
                    break;
                }
                case 3:
                {
                    _stage_inputCode.gameObject.SetActive(true);
                    break;
                }
                case 4:
                {
                    _stage_getReward.gameObject.SetActive(true);
                    break;
                }
            }
        }

        public void ShowErrorTips(string key)
        {
            _stage_errorTips.gameObject.SetActive(true);
            _stage_errorTips.ShowErrorTips(key);
        }

        public string GetErrorKeyByCode(int code)
        {
            switch (code)
            {
                case 34:
                {
                    return "ui_bind_email_formatError";
                }
                case 35:
                {
                    return "ui_bind_email_verifyError";
                }
                case 36:
                {
                    return "ui_bind_email_countError";
                }
                case 21:
                {
                    return "ui_bind_email_alreadyBindedError";
                }
            }

            return "";
        }

        public static bool CanShowBindEmailButton()
        {
            if (AccountManager.Instance.HasBindEmail())
                return false;

            int level = GlobalConfigManager.Instance.GetNumValue("build_email_unlock_level");
            if (ExperenceModel.Instance.GetLevel() < level)
                return false;

            if (StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.IsGetReward)
                return false;
            
            return true;
        }

        public const string constPlaceId = "UIPopupBindEmailController";
        public static bool CanShow()
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.IsGetReward)
                return false;
            
            var storageBuildEmail = StorageManager.Instance.GetStorage<StorageHome>().BuildEmail;
            if (storageBuildEmail.Stage == 3 || storageBuildEmail.Stage == 4 && !storageBuildEmail.IsGetReward)
            {
                UIManager.Instance.OpenWindow(UINameConst.UIPopupBindEmail);
                return true;
            }
            
            if (!CanShowBindEmailButton())
                return false;

            int popNum = GlobalConfigManager.Instance.GetNumValue("build_email_pop_num");
            if (storageBuildEmail.AutoPopNum > popNum)
                return false;
            
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
                return false;
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,CommonUtils.GetTimeStamp());

            storageBuildEmail.AutoPopNum++;
            UIManager.Instance.OpenWindow(UINameConst.UIPopupBindEmail);
            return true;
        }

        public static bool HideEmail()
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.IsGetReward)
                return true;
            
            if (AccountManager.Instance.HasBindEmail())
                return true;
            
            var storageBuildEmail = StorageManager.Instance.GetStorage<StorageHome>().BuildEmail;
            int popNum = GlobalConfigManager.Instance.GetNumValue("build_email_pop_num");
            if (storageBuildEmail.AutoPopNum > popNum)
                return true;

            return false;
        }

        public override void AnimCloseWindow(Action afterCloseFunc = null, bool destroy = true, Action beforeCloseFunc = null)
        {
            base.AnimCloseWindow(() =>
            {
                if (_isRecoverUI)
                {
                    UIManager.Instance.OpenWindow(UINameConst.UIPopupSet2);
                }
            }, destroy);
        }
    }
}