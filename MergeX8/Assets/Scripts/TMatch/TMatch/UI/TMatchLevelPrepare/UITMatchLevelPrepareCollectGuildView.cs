

using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine.UI;
using UnityEngine;


namespace TMatch
{
    public class UITMatchLevelPrepareCollectGuildView : UIView
    {
        [ComponentBinder("Fill")] private Image fill;
        [ComponentBinder("DescribeBG/Text")] private LocalizeTextMeshProUGUI timeText;
        [ComponentBinder("Icon")] private Image icon;

        private bool isOpen;
        private bool isInCollecting;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            Refresh();
        }

        private void Refresh()
        {
            // isOpen = CollectGuildActivityModel.Instance.IsActivityOpened();
            //
            // if (isOpen)
            // {
            //     isInCollecting = CollectGuildActivityModel.Instance.IsInCollecting();
            //     gameObject.SetActive(isInCollecting);
            //     var boxType = CollectGuildActivityModel.Instance.GetClaimBoxType();
            //     icon.sprite = ResourcesManager.Instance.GetSpriteVariant("Common01Atlas",
            //         $"ui_common_icon_entry_guildCollection_box_{boxType}");
            //     var curProgress = CollectGuildActivityModel.Instance.GetCurPorgress();
            //     fill.fillAmount = Mathf.Lerp(0.1f, 0.9f, curProgress);
            // }
            // else
            // {
            //     gameObject.SetActive(false);
            // }
        }

        public override void OnViewUpdate(float deltaTime)
        {
            // base.OnViewUpdate(deltaTime);
            // if (!isOpen || !isInCollecting) return;
            // var leftTime = CollectGuildActivityModel.Instance.GetCollectingLeftTime();
            // if (leftTime > 0)
            //     timeText.SetText(CommonUtils.FormatPropItemTime((long)leftTime));
            // else
            // {
            //     Refresh();
            // }
        }
    }
}