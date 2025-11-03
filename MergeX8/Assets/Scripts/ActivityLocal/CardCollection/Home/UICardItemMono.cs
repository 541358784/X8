using System;
using System.Collections;
using DragonPlus;
using DragonPlus.Config.CardCollect;
using UnityEngine;
using UnityEngine.UI;

namespace ActivityLocal.CardCollection.Home
{
    public class UICardItemMono : MonoBehaviour
    {
        public TableCardCollectionTheme _config;
        private Button _onOpenCardButton;
        public CardCollectionCardThemeState ThemeState;
        private Image Icon;
        private Transform NewFlag;
        private Transform ReopenFlag;
        
        
        private void Awake()
        {
            _onOpenCardButton = transform.GetComponent<Button>();
            _onOpenCardButton.onClick.AddListener(OnClickOpenCard);
            
            DownloadGroup = transform.Find("DownLoad");
            DownloadSlider = transform.Find("DownLoad/ProgressSlider").GetComponent<Slider>();
            DownLoadSliderText = transform.Find("DownLoad/ProgressSlider/ProgressText")
                .GetComponent<LocalizeTextMeshProUGUI>();
            DownloadSlider.gameObject.SetActive(false);
            DownloadBtn = transform.Find("DownLoad/DownloadButton").GetComponent<Button>();
            DownloadBtn.gameObject.SetActive(true);
            DownloadBtn.onClick.AddListener(OnClickDownloadBtn);
            Icon = transform.Find("Icon").GetComponent<Image>();
            //CardCollectionModel.Instance.GetCardThemeState
            NewFlag = transform.Find("New");
            ReopenFlag = transform.Find("Back");
            NewFlag.gameObject.SetActive(false);
            ReopenFlag.gameObject.SetActive(false);
        }

        public void Init(TableCardCollectionTheme config)
        {
            _config = config;
            ThemeState = CardCollectionModel.Instance.GetCardThemeState(_config.Id);
            DownloadGroup.gameObject.SetActive(!ThemeState.IsResReady());
            Icon.sprite = ThemeState.GetIconSprite();
            NewFlag.gameObject.SetActive(CardCollectionActivityModel.Instance.IsInitFromServer() && (
                _config.Id == CardCollectionActivityModel.Instance.CurStorage.ThemeId ||
                _config.Id == CardCollectionModel.Instance.GetCardThemeState(CardCollectionActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme().CardThemeConfig.Id));
            if (/*CardCollectionModel.Instance.ThemeInUse == null && */
                CardCollectionActivityModel.Instance.IsInitFromServer() &&(
                    (
                _config.Id == CardCollectionActivityModel.Instance.CurStorage.ThemeId && 
                !CardCollectionModel.Instance.IsResReady(CardCollectionModel.Instance.GetCardThemeState(CardCollectionActivityModel.Instance.CurStorage.ThemeId))
                ) || (
                        _config.Id == CardCollectionModel.Instance.GetCardThemeState(CardCollectionActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme().CardThemeConfig.Id &&
                        !CardCollectionModel.Instance.IsResReady(CardCollectionModel.Instance.GetCardThemeState(CardCollectionActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme())
                        )))
            {
                OnClickDownloadBtn();
            }
            ReopenFlag.gameObject.SetActive(CardCollectionReopenActivityModel.Instance.IsInitFromServer() &&(
                                            _config.Id == CardCollectionReopenActivityModel.Instance.CurStorage.ThemeId ||
                                            _config.Id == CardCollectionModel.Instance.GetCardThemeState(CardCollectionReopenActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme().CardThemeConfig.Id));
            if (/*CardCollectionModel.Instance.ThemeReopenInUse == null &&*/ 
                CardCollectionReopenActivityModel.Instance.IsInitFromServer() &&(
                    (
                        _config.Id == CardCollectionReopenActivityModel.Instance.CurStorage.ThemeId && 
                        !CardCollectionModel.Instance.IsResReady(CardCollectionModel.Instance.GetCardThemeState(CardCollectionReopenActivityModel.Instance.CurStorage.ThemeId))
                    ) || (
                        _config.Id == CardCollectionModel.Instance.GetCardThemeState(CardCollectionReopenActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme().CardThemeConfig.Id &&
                        !CardCollectionModel.Instance.IsResReady(CardCollectionModel.Instance.GetCardThemeState(CardCollectionReopenActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme())
                    )))
            {
                OnClickDownloadBtn();
            }
        }

        private void OnClickOpenCard()
        {
            if(CardCollectionModel.Instance.IsResReady(ThemeState))
                UICardMainController.Open(ThemeState);
            else
            {
                OnClickDownloadBtn();
            }
        }

        private Slider DownloadSlider;
        private LocalizeTextMeshProUGUI DownLoadSliderText;
        private Button DownloadBtn;
        private Transform DownloadGroup;
        private bool InDownload = false;
        public void OnClickDownloadBtn()
        {
            if (InDownload)
                return;
            InDownload = true;
            DownloadSlider.gameObject.SetActive(true);
            DownloadSlider.value = 0;
            DownLoadSliderText.SetText("0%");
            ThemeState.TryDownloadRes((p,s) =>
            {
                if (!this)
                    return;
                DownloadSlider.value = p;
                DownLoadSliderText.SetText(((int)(p*100))+"%");
                
            }, (success) =>
            {
                if (!this)
                    return;
                DownloadSlider.gameObject.SetActive(false);
                DownloadGroup.gameObject.SetActive(!ThemeState.IsResReady());
                InDownload = false;
            });
            DownloadBtn.gameObject.SetActive(false);
        }
    }
}