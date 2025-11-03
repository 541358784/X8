using Decoration;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

public enum CoinCompetitionOpenType
{
    
}
public class UICoinCompetitionStartController : UIWindowController
{
    
    private Button _closeBtn;
    private GameObject _startGroup;
    private Button _startBtn;
    
    private GameObject _remindGroup;
    private LocalizeTextMeshProUGUI _timeText;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _startBtn = GetItem<Button>("Root/Start/Button");
        _startGroup = GetItem("Root/Start");
        
        _remindGroup = GetItem("Root/Remind");
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/Remind/TimeGroup/TimeText");
        _startBtn.onClick.AddListener(OnBtnPlay);
        _closeBtn.onClick.AddListener(OnBtnClose);
        InvokeRepeating("UpdateTimeText",1,1);
        UpdateTimeText();
        if (CoinCompetitionModel.Instance.IsPreheating())
        {
            CoinCompetitionModel.Instance.StorageCompetition.IsShowPreheat = true;
        }
    }
    private void UpdateTimeText()
    {
        if (CoinCompetitionModel.Instance.IsPreheating())
        {
            _startGroup.SetActive(false);
            _remindGroup.SetActive(true);
            _timeText.SetText(CoinCompetitionModel.Instance.GetActivityPreheatLeftTimeString());
            CoinCompetitionModel.Instance.StorageCompetition.IsShowPreheat = true;
        }
        else
        {
            _startGroup.SetActive(true);
            _remindGroup.SetActive(false);
            _timeText.SetText(CoinCompetitionModel.Instance.GetActivityLeftTimeString());
        }
    }
    protected override void OnOpenWindow(params object[] objs)
    {
     
    }
    
    private void OnBtnPlay()
    {
        AnimCloseWindow(() =>
        {
            CoinCompetitionModel.Instance.StartActivity();
            OpenGuide();
        });
    }
    private void OnBtnClose()
    {
 
        AnimCloseWindow(() =>
        {
            if (!CoinCompetitionModel.Instance.IsPreheating())
            {
                CoinCompetitionModel.Instance.StartActivity();
                OpenGuide();
            }
        });
    }

    private void OpenGuide()
    {
        if (!GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CoinCompetitionStart, null))
        {
            UIManager.Instance.OpenUI(UINameConst.UICoinCompetitionMain);
        }
    }
    public override void ClickUIMask()
    {
        OnBtnClose();
    }
    
}