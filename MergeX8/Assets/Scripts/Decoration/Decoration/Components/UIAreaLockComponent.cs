using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DragonPlus;
using System.Threading.Tasks;
using DragonU3DSDK.Asset;
namespace Decoration
{
    /// <summary>
    /// 区域锁
    /// </summary>
    public class UIAreaLockComponent : FollowTargetBase
    {
        private LocalizeTextMeshProUGUI _loadingText;
        private Button _downloadButton;
        private int _areaId;
        private Slider _slider;
        private LocalizeTextMeshProUGUI _downloadText;

        private void Awake()
        {
            _loadingText = this.transform.Find("ProgressSlider/ProgressText").GetComponent<LocalizeTextMeshProUGUI>();
            _slider = this.transform.Find("ProgressSlider").GetComponent<Slider>();
            _downloadText = transform.Find("DownloadButton/BtnImage/Label").GetComponent<LocalizeTextMeshProUGUI>();
            _downloadButton = this.transform.Find("DownloadButton").GetComponent<Button>();
            _downloadButton.onClick.AddListener(OnDownloadClicked);
            _slider.value = 0f;
            _loadingText.m_TmpText.text = "0%";

            showDownloadButton(true);
        }

        private void OnEnable()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.LANGUAGE_CHNAGE, onLanguageChanged);
        }

        private void OnDisable()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.LANGUAGE_CHNAGE, onLanguageChanged);
        }

        private void onLanguageChanged(BaseEvent e)
        {
            _downloadText.SetTerm("UI_common_download");
        }

        private void showDownloadButton(bool show)
        {
            _downloadButton.gameObject.SetActive(show);
            _slider.gameObject.SetActive(!show);
        }

        private void OnDownloadClicked()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                CommonUtils.ShowTipAtTouchPosition("UI_explain_fail");
            }
            else
            {
                downloadAreaRes();
            }
        }

        private async void downloadAreaRes()
        {
            showDownloadButton(false);

            var downLoadFinish = false;
            var downloadTaskList = AssetCheckManager.Instance.DownloadAreaFiles(_areaId, (success) => downLoadFinish = true);

            while (!downLoadFinish)
            {
                AssetCheckManager.Instance.UpdateProgressFromDownloadInfoList(downloadTaskList, updateProgress);
                await Task.Delay(100);
            }

            foreach (var info in downloadTaskList)
            {
                if (info.result != DownloadResult.Success)
                {
                    showDownloadButton(true);
                    CommonUtils.ShowTipAtTouchPosition("UI_explain_fail");
                    return;
                }
            }

            Hide();

            refreshAreaMask();
        }

        private void refreshAreaMask()
        {
            var area = DecoManager.Instance.FindArea(_areaId);
            area?.SetResReady();
            EventDispatcher.Instance.DispatchEvent(EventEnum.SHOW_BUILD_BUBBLE,true);
        }

        public void SetData(int areaId)
        {
            _areaId = areaId;
        }

        private void updateProgress(float progress, string info)
        {
            _slider.value = progress;
            _loadingText.m_TmpText.text = string.Format("<mspace=mspace=32>{0}%</mspace>", (progress * 100).ToString("#0"));
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}