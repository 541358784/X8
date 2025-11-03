using System.Collections.Generic;
using DigTrench;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Google.Android.PerformanceTuner;
using UnityEngine;
using UnityEngine.UI;

public abstract class UITabulationItem
{
    public abstract bool IsFinish();
    public abstract bool IsUnlock();
    public GameObject _gameObject;
    // public abstract void Init(GameObject gameObject, int configId, int index);

    public int ConfigId;
    public int _index;
    public UITabulationCell _normalCell, _lockCell, _comingsoonCell;
    public abstract void SetConfig(int configId);
    public abstract void CreateCell();
    public virtual void Init(GameObject gameObject, int configId, int index)
    {
        CreateCell();
        ConfigId = configId;
        SetConfig(ConfigId);
        _gameObject = gameObject;
        _index = index;
        
        _normalCell.Init(gameObject.transform.Find("Normal").gameObject, ConfigId, 0,_index);
        _lockCell.Init(gameObject.transform.Find("Lock").gameObject, ConfigId, 1,_index);
        _comingsoonCell.Init(gameObject.transform.Find("comingsoon").gameObject, ConfigId, 2,_index);

        UpdateUI();

        if (_normalCell._playButton == null)
            return;
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_normalCell._playButton.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.AsmrPlay, _normalCell._playButton.transform as RectTransform, topLayer:topLayer, targetParam:ConfigId.ToString());
        
        if(!GuideSubSystem.Instance.isFinished(215))
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ASMR_Play, ConfigId.ToString(),  ConfigId.ToString());
    }
    
    public void UpdateUI()
    {
        if (ConfigId == -1)
        {
            _normalCell._gameObject.SetActive(false);
            _lockCell._gameObject.SetActive(false);
            _comingsoonCell._gameObject.SetActive(true);
            return;
        }
        
        bool isUnlock = IsUnlock();
        _normalCell._gameObject.SetActive(isUnlock);
        _lockCell._gameObject.SetActive(!isUnlock);
        _comingsoonCell._gameObject.SetActive(false);
    }
    
    public virtual void BeginDownLoad(int configId)
    {
        _normalCell.BeginDownLoad(configId);
    }
    
    public virtual void EndDownLoad(int configId)
    {
        _normalCell.EndDownLoad(configId);
    }
    
    public virtual void DownLoadProgress(int configId, float progress)
    {
        _normalCell.DownLoadProgress(configId, progress);
    }
}

public abstract class UITabulationCell
{
    public Button _playButton;
    public Image _icon;
    public LocalizeTextMeshProUGUI _levelText;
    public LocalizeTextMeshProUGUI _lockText;
    public Slider _slider;
    public LocalizeTextMeshProUGUI _sliderText;
    public GameObject _passObj;
    public GameObject _gameObject;
    public Button _downLoadButton;
    public Slider _downLoad;
    public LocalizeTextMeshProUGUI _downLoadText;
    public Button RvButton;
    public abstract void OnClickPlayBtn();
    public abstract void OnClickDownloadBtn();
    public abstract bool IsFinish();
    public abstract bool IsUnlock();
    public abstract Sprite GetIcon();
    public abstract int GetUnlockDecoNodeNum();
    public abstract Dictionary<string, string> GetNeedDownloadAssets();
    public abstract void SetConfig(int configId);
    
    public int _type = 0;
    //type =0 normal  =1 lock =2comingsoon
    public int _index;
    public int ConfigId;
    public virtual void Init(GameObject gameObject, int configId, int type, int index)
    {
        _index = index+1;
        _gameObject = gameObject;
        ConfigId = configId;
        SetConfig(ConfigId);
        _type = type;
        
        if(type == 2 || ConfigId < 0)
            return;
        
        _playButton = gameObject.transform.Find("PlayButton").GetComponent<Button>();
        
        _levelText= gameObject.transform.Find("LevelText").GetComponent<LocalizeTextMeshProUGUI>();

        if (type == 0)
        {
            _downLoadButton = gameObject.transform.Find("Download/DownloadButton").GetComponent<Button>();
            _downLoad = gameObject.transform.Find("Download/ProgressSlider").GetComponent<Slider>();
            _downLoadText = gameObject.transform.Find("Download/ProgressSlider/ProgressText").GetComponent<LocalizeTextMeshProUGUI>();
            _downLoadButton.gameObject.SetActive(false);
            _downLoad.gameObject.SetActive(false);
            
            _downLoad.value = 0;
            _downLoadText.SetText("");
            RvButton = gameObject.transform.Find("RvButton").GetComponent<Button>();
        }

        switch (type)
        {
            case 0:
            {
                _icon = gameObject.transform.Find("LevelIcon").GetComponent<Image>();
                _slider = gameObject.transform.Find("Slider").GetComponent<Slider>();
                _sliderText = gameObject.transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
                _passObj = gameObject.transform.Find("Pass").gameObject;
                
                _playButton.onClick.AddListener(() =>
                {
                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.AsmrPlay, ConfigId.ToString());
                    
                    var uipopView = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupGameTabulation);
                    if (uipopView != null)
                    {
                        UILoadingTransitionController.Show(null);
                        uipopView.AnimCloseWindow(OnClickPlayBtn);
                    }
                    else
                    {
                        UILoadingTransitionController.Show(OnClickPlayBtn);
                    }
                });
                if (IsFinish())
                {
                    UIAdRewardButton.Create(ADConstDefine.RV_REPLAY_MINIGAME, UIAdRewardButton.ButtonStyle.Disable, RvButton.gameObject,
                        (s) =>
                        {
                            if (s)
                            {
                                var uipopView = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupGameTabulation);
                                if (uipopView != null)
                                {
                                    uipopView.CloseWindowWithinUIMgr(true);
                                }
                                OnClickPlayBtn();
                            }
                        }, false, null, () =>
                        {
                            GameBIManager.Instance.SendGameEvent(
                                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigame2NdtryRv);
                            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.AsmrPlay, ConfigId.ToString());
                        });   
                }

                _downLoadButton.onClick.AddListener(() =>
                {
                    _downLoadButton.gameObject.SetActive(false);
                    _downLoad.gameObject.SetActive(true);
                    OnClickDownloadBtn();
                });
                break;
            }
            case 1:
            {
                _lockText= gameObject.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
                break;
            }
        }
        
        UpdateUI();
    }
    public virtual void UpdateUI()
    {
        bool isFinish = IsFinish();
        bool isUnlock = IsUnlock();
        
        _levelText.SetText(LocalizationManager.Instance.GetLocalizedString("ui_asmr_level_num") + _index);
        switch (_type)
        {
            case 0:
            {
                float value = isFinish?1f:0f;
                _slider.value = value;
                _sliderText.SetText(isFinish ? "100%" : (int)(value*100)+"%");
                
                _passObj.gameObject.SetActive(isFinish);
                _slider.gameObject.SetActive(isUnlock&&!isFinish);

                _playButton.gameObject.SetActive(!isFinish);
                RvButton.gameObject.SetActive(isFinish);
                _downLoadButton.gameObject.SetActive(false);
                _downLoad.gameObject.SetActive(false);
                
                _icon.sprite = GetIcon();

                if (isUnlock)
                {
                    var downLoadAsset = GetNeedDownloadAssets();
                    if (downLoadAsset != null && downLoadAsset.Count > 0)
                    {
                        _playButton.gameObject.SetActive(false);
                        RvButton.gameObject.SetActive(false);
                        _downLoadButton.gameObject.SetActive(true);
                        _downLoad.gameObject.SetActive(false);
                    }
                }
                break;
            }
            case 1:
            {
                _lockText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_asmr_unlock_day"),GetUnlockDecoNodeNum()));
                break;
            }
        }
    }
    public void updateProgress(float progress, string extralInfo)
    {
        _downLoad.value = progress;
        _downLoadText.SetText((int)(progress*100) + "%");
    }

    public abstract bool NeedUpdateOnDownload(int configId);
    public virtual void BeginDownLoad(int configId)
    {
        if (!NeedUpdateOnDownload(configId))
            return;
        _downLoadButton.gameObject.SetActive(false);
        _downLoad.gameObject.SetActive(true);
    }
    
    public virtual void EndDownLoad(int configId)
    {
        if (!NeedUpdateOnDownload(configId))
            return;
        UpdateUI();
    }
    
    public virtual void DownLoadProgress(int configId, float progress)
    {
        if (!NeedUpdateOnDownload(configId))
            return;
        updateProgress(progress, "");
    }
}