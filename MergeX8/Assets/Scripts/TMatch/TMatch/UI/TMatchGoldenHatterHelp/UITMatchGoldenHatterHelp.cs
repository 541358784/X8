using DragonPlus.Config.TMatchShop;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UILevelPuffHelp")]
    public class UITMatchGoldenHatterHelp : UIPopup
    {

        public override Action EmptyCloseAction => OnCloseButtonClicked;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            GlodenHatter cfg = TMatchConfigManager.Instance.GetGlodenHatterByTimes(StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.WinningStreakCnt);
            float radio = cfg == null ? 0 : cfg.times / 3.0f;
            if (cfg != null)
            {
                if (cfg.times == 1) radio = 0.25f;
                else if (cfg.times == 2) radio = 0.62f;
            }

            transform.Find($"Root/InsideGroup/Slider").GetComponent<Slider>().value = radio;
            for (int i = 0; i < TMatchConfigManager.Instance.GlodenHatterList.Count; i++)
            {
                transform.Find($"Root/InsideGroup/InsideGroup/Item{i + 1}/Prop1/NumberText").GetComponent<TextMeshProUGUI>().SetText(TMatchConfigManager.Instance.GlodenHatterList[i].rewardCnt[0].ToString());
                transform.Find($"Root/InsideGroup/InsideGroup/Item{i + 1}/Prop2/NumberText").GetComponent<TextMeshProUGUI>().SetText(TMatchConfigManager.Instance.GlodenHatterList[i].rewardCnt[1].ToString());

                if (StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.WinningStreakCnt < (i + 1))
                {
                    // transform.Find($"Root/InsideGroup/InsideGroup/Item{i+1}/BuffIcon").GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                    transform.Find($"Root/InsideGroup/InsideGroup/Item{i + 1}/ON").gameObject.SetActive(false);
                }
                else
                {
                    transform.Find($"Root/InsideGroup/InsideGroup/Item{i + 1}/ON").gameObject.SetActive(true);
                }
            }

            this.BindEvent("Root/CloseButton", OnCloseButtonClicked);
            this.BindEvent("Root/PlayButton", OnPlayButtonClicked);
        }

        private void OnCloseButtonClicked()
        {
            UIViewSystem.Instance.Close<UITMatchGoldenHatterHelp>();
        }

        private void OnPlayButtonClicked()
        {
            UIViewSystem.Instance.Close<UITMatchGoldenHatterHelp>();
        }
    }
}