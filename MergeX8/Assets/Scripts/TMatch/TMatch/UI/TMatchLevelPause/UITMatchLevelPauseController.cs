using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.TMatch;
using UnityEngine.UI;
using System;
using Activity.TMatch.Crocodile.View;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UISuspend")]
    public class UITMatchLevelPauseController : UIPopup
    {
        public override Action EmptyCloseAction => CloseOnClick;

        [ComponentBinder("Sound")] public Button soundButton;
        [ComponentBinder("Music")] public Button musicButton;
        [ComponentBinder("Notice")] public Button noticeButton;
        [ComponentBinder("UserText")] public LocalizeTextMeshProUGUI userText;
        [ComponentBinder("VersionText")] public LocalizeTextMeshProUGUI versionText;

        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);

            TMatchDifficulty difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchModel.Instance.GetMainLevel());
            CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);
            // transform.Find($"Root/TitleGroup/TitleText").GetComponent<LocalizeTextMeshProUGUI>().SetText($"Level {TMatchModel.Instance.GetMainLevel()}");
            transform.Find($"Root/CloseButton").GetComponent<Button>().onClick.AddListener(CloseOnClick);
            transform.Find($"Root/ButtonGruop/PlayButton").GetComponent<Button>().onClick.AddListener(CloseOnClick);
            transform.Find($"Root/ButtonGruop/QuitButton").GetComponent<Button>().onClick.AddListener(QuitOnClick);

            InitSettingUI();
            EventDispatcher.Instance.AddEventListener(EventEnum.OnApplicationPause, OnApplicationPauseEvent);
        }

        public override Task OnViewClose()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.OnApplicationPause, OnApplicationPauseEvent);
            return base.OnViewClose();
        }

        private void InitSettingUI()
        {
            // 音乐音效
            musicButton.onClick.AddListener(() =>
            {
                SettingManager.Instance.MusicClose = !SettingManager.Instance.MusicClose;
                RefreshMusicButton();
            });
            soundButton.onClick.AddListener(() =>
            {
                SettingManager.Instance.SoundClose = !SettingManager.Instance.SoundClose;
                RefreshSoundButton();
            });
            RefreshMusicButton();
            RefreshSoundButton();

            // 通知
            noticeButton.onClick.AddListener(() => { DragonU3DSDK.DragonNativeBridge.OpenNotifycationSetting(); });
            RefreshNoticeButton();

            // version and user id
            string playerIdStr = DragonU3DSDK.Utils.PlayerIdToString(StorageManager.Instance.GetStorage<StorageCommon>().PlayerId);
            userText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_setting_user_id") + " " + playerIdStr);

            string resVersion = VersionManager.Instance.GetResDesplayVersion();
            versionText.SetText(string.Format("{0}", resVersion));
        }

        private void RefreshMusicButton()
        {
            var offImage = musicButton.transform.Find("Off");
            var onImage = musicButton.transform.Find("On");
            var isOff = SettingManager.Instance.MusicClose;
            offImage.gameObject.SetActive(isOff);
            onImage.gameObject.SetActive(!isOff);
        }

        public void OnApplicationPauseEvent(BaseEvent evt)
        {
            OnApplicationPauseEvent realEvt = evt as OnApplicationPauseEvent;
            if (!realEvt.pause)
            {
                RefreshNoticeButton();
            }
        }

        private void RefreshNoticeButton()
        {
            // var offImage = noticeButton.transform.Find("Off");
            // var onImage = noticeButton.transform.Find("On");
            // var isOn = NotificationManager.Instance.IsNotificationOn();
            // offImage.gameObject.SetActive(!isOn);
            // onImage.gameObject.SetActive(isOn);
        }

        private void RefreshSoundButton()
        {
            var offImage = soundButton.transform.Find("Off");
            var onImage = soundButton.transform.Find("On");
            var isOff = SettingManager.Instance.SoundClose;
            offImage.gameObject.SetActive(isOff);
            onImage.gameObject.SetActive(!isOff);
        }

        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<UITMatchLevelPauseController>();
        }

        private void QuitOnClick()
        {
            CloseOnClick();
            if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            {
                KapibalaModel.Instance.DealFail();
                UIViewSystem.Instance.Open<UIPopupKapibalaReviveFailController>();
            }
            else if (CrocodileActivityModel.Instance.IsActivityOpened() && CrocodileActivityModel.Instance.IsInChallenge())
            {
                UIViewSystem.Instance.Open<UIPopupCrocodileEndController>();
            }
            else
            {
                UIViewSystem.Instance.Open<UITMatchLevelInterruptController>();
            }
        }

    }
}