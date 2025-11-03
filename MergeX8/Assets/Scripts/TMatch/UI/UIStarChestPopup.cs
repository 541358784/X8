using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Storage;
using Framework;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using DragonU3DSDK.Asset;
using DragonU3DSDK;
using System.Collections.Generic;
using System;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UIStarsBox")]
    public class UIStarChestPopup : UIPopup
    {

        [ComponentBinder("ContinueButton")] private Button continueButton;

        [ComponentBinder("CloseButton")] private Button closeButton;

        [ComponentBinder("Root/InsideGroup/Slider")]
        private Slider slider;

        [ComponentBinder("Root/InsideGroup/Slider/PlanText")]
        private TextMeshProUGUI progressText;


        public override Action EmptyCloseAction => OnCloseButtonClicked;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            continueButton.onClick.AddListener(OnCloseButtonClicked);
            closeButton.onClick.AddListener(OnCloseButtonClicked);

            var totalStars = StorageManager.Instance.GetStorage<StorageTMatch>().StarChest.TotalStars;
            var curIndex = StorageManager.Instance.GetStorage<StorageTMatch>().StarChest.CurIndex;
            var curRewardInfo = TMatchConfigManager.Instance.GetStarChestReward(curIndex);
            float curProgress = (float)totalStars / (float)curRewardInfo.star;
            curProgress = curProgress > 1.0f ? 1.0f : curProgress;
            slider.value = curProgress;
            var num1 = totalStars > curRewardInfo.star ? curRewardInfo.star : totalStars;
            progressText.text = $"{num1}/{curRewardInfo.star}";
        }

        private void OnCloseButtonClicked()
        {
            UIViewSystem.Instance.Close<UIStarChestPopup>();
        }
    }
}