using System.Collections.Generic;
using Activity.TreasureHuntModel;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;
public class UIPopupTreasureHuntStartController : UIWindowController
{
    private Button _closeBtn;
    private Button _startBtn;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _buttonText;
    private LocalizeTextMeshProUGUI _descText;

    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnBtnCLose);
        _startBtn = GetItem<Button>("Root/Button");
        _buttonText = GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text");
        _startBtn.onClick.AddListener(OnBtnStart);
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _descText = GetItem<LocalizeTextMeshProUGUI>("Root/BGGroup/Text");
        InvokeRepeating("RefreshTime",0,1);
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_startBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TreasureHuntStart, _startBtn.transform as RectTransform, topLayer: topLayer);
        
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (!TreasureHuntModel.Instance.IsPreheating())
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TreasureHuntStart,null);
    }

    public void RefreshTime()
    {
        if (TreasureHuntModel.Instance.IsPreheating())
        {
            _timeText.SetText(TreasureHuntModel.Instance.GetActivityPreheatLeftTimeString());
            _buttonText.SetTerm("UI_button_ok");
            _descText.SetTerm("ui_goldmatch_desc9");
            _startBtn.gameObject.SetActive(false);
        }
        else
        {
            _timeText.SetText(TreasureHuntModel.Instance.GetActivityLeftTimeString());
            _buttonText.SetTerm("UI_button_start");
            _descText.SetTerm("ui_JungleAdventure_Preheat_Desc");
            _startBtn.gameObject.SetActive(true);

        }

    }
    private void OnBtnStart()
    {
        if (TreasureHuntModel.Instance.IsPreheating())
        {
            AnimCloseWindow();
        }
        else
        {
            AnimCloseWindow(() =>
            {
            
                UIManager.Instance.OpenUI(UINameConst.UITreasureHuntMain);
            });
            TreasureHuntModel.Instance.Start();
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TreasureHuntStart);
        }
      
    }

    private void OnBtnCLose()
    {
        AnimCloseWindow();
    }

}
