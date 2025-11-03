
using DragonPlus;
// using DragonPlus.Config.Game;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class UITMatchLevelPrepareCollectActivityView : UIView
    {
        [ComponentBinder("Fill")] private Image fill;
        [ComponentBinder("DescribeBG/Text")] private LocalizeTextMeshProUGUI timeText;
        [ComponentBinder("CoinIconGroup")] private Transform coinIconGroup;
        [ComponentBinder("GemIconGroup")] private Transform gemIconGroup;

        private bool coinOpen = false;
        private bool gemOpen = false;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            Refresh();
        }

        public void Refresh()
        {
            // coinOpen = CollectCoinActivityModel.Instance.IsActivityOpened();
            // gemOpen = false;
            // if (coinOpen)
            // {
            //     var progress = CollectCoinActivityModel.Instance.GetCurRoundProgress();
            //     fill.fillAmount = Mathf.Lerp(0.1f, 0.9f, progress);
            //     for (int i = 1; i <= 3; i++)
            //     {
            //         coinIconGroup.Find($"Icon{i}").gameObject.SetActive(CollectCoinActivityModel.Instance.GetStorageActivityData().Round == i);
            //     }
            // }
            // else
            // {
            //     gemOpen = CollectDiamondActivityModel.Instance.IsActivityOpened();
            //     if (gemOpen)
            //     {
            //         var progress = CollectDiamondActivityModel.Instance.GetCurStageProgress();
            //         fill.fillAmount = Mathf.Lerp(0.1f, 0.9f, progress);
            //         for (int i = 1; i <= 3; i++)
            //         {
            //             gemIconGroup.Find($"IconBox{i}").gameObject.SetActive(CollectDiamondActivityModel.Instance.GetStorageActivityData().Stage == i);
            //         }
            //     }
            // }
            //
            // gameObject.SetActive(coinOpen || gemOpen);
            // coinIconGroup.gameObject.SetActive(coinOpen);
            // gemIconGroup.gameObject.SetActive(gemOpen);
        }

        public override void OnViewUpdate(float deltaTime)
        {
            // base.OnViewUpdate(deltaTime);
            // if (!coinOpen && !gemOpen) return;
            // if (coinOpen)
            // {
            //     if (CollectCoinActivityModel.Instance.GetActivityLeftTime() > 0)
            //         timeText.SetText(CollectCoinActivityModel.Instance.GetActivityLeftTimeString());
            //     else
            //         Refresh();
            // }
            // else
            // {
            //     if (CollectDiamondActivityModel.Instance.GetActivityLeftTime() > 0)
            //         timeText.SetText(CollectDiamondActivityModel.Instance.GetActivityLeftTimeString());
            //     else
            //         Refresh();
            // }

        }
    }
}