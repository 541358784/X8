using DragonPlus;
using UnityEngine.UI;

namespace Activity.JungleAdventure.Controller
{
    public class UIPopupJungleAdventurePreviewController : UIWindowController
    {
        private LocalizeTextMeshProUGUI _timeText;
        private Button _closeButton;
        private Button _tureButton;
        
        public override void PrivateAwake()
        {
            _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _closeButton = transform.Find("Root/Button").GetComponent<Button>();
            _closeButton.onClick.AddListener(()=>AnimCloseWindow());
            
            _tureButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _tureButton.onClick.AddListener(()=>AnimCloseWindow());
            
            InvokeRepeating("UpdateTime", 0, 1);
        }

        private void UpdateTime()
        {
            _timeText.SetText(JungleAdventureModel.Instance.GetPreheatEndTimeString());
        }

        private const string coolTimeKey = "UIPopupJungleAdventurePreviewController";
        public static bool CanShow()
        {
            if (!JungleAdventureModel.Instance.IsOpened())
                return false;

            if (JungleAdventureModel.Instance.IsPreheatEnd())
                return false;
            
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
                return false;
            
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,CommonUtils.GetTimeStamp());
            UIManager.Instance.OpenWindow(UINameConst.UIPopupJungleAdventurePreview);
            return true;
        }
    }
}