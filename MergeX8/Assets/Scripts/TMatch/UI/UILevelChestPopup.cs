using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using DragonPlus.Config.TMatch;



namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UIGradeBox")]
    public class UILevelChestPopup : UIPopup
    {

        [ComponentBinder("ContinueButton")] private Button continueButton;

        [ComponentBinder("CloseButton")] private Button closeButton;

        [ComponentBinder("HelpText")] private LocalizeTextMeshProUGUI helpText;

        public override Action EmptyCloseAction => OnCloseButtonClicked;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            continueButton.onClick.AddListener(OnCloseButtonClicked);
            closeButton.onClick.AddListener(OnCloseButtonClicked);

            // 延迟的原因是需要等原有的语言国际化脚本执行完
            DelayShowText();
        }

        private async void DelayShowText()
        {
            var totalLevels = StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.TotalLevel;
            var curIndex = StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.CurIndex;
            var curRewardInfo = TMatchConfigManager.Instance.GetLevelChestReward(curIndex);
            var nextRewardLevel = curRewardInfo.level - totalLevels + TMatchModel.Instance.GetMainLevel();
            var textStr = LocalizationManager.Instance.GetLocalizedStringWithFormats($"&key.UI_common_level_desc", nextRewardLevel.ToString());
            await Task.Delay(10);
            helpText.SetText(textStr);
        }

        private void OnCloseButtonClicked()
        {
            UIViewSystem.Instance.Close<UILevelChestPopup>();
        }
    }
}