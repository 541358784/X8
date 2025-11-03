
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;

public class UIPopupMermaidStartPreviewController : UIWindowController
{
    private Button _buttonClose;
    private Button _buttonHelp;
    private Button _button;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _descText;
    private LocalizeTextMeshProUGUI _descText2;
    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _descText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
        _descText2 = GetItem<LocalizeTextMeshProUGUI>("Root/Text (1)");
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonHelp = GetItem<Button>("Root/ButtonHelp");
        _button = GetItem<Button>("Root/Button");
        _button.onClick.AddListener(OnBtnPlay);
        _buttonHelp.onClick.AddListener(OnBtnHelp);
        _buttonClose.onClick.AddListener(OnBtnClose);
        InvokeRepeating("UpdateTimeText",1,1);
        UpdateTimeText();
    }
    private void UpdateTimeText()
    {
        if (MermaidModel.Instance.IsPreheating())
        {
            _button.gameObject.SetActive(false);
            _timeText.transform.parent.gameObject.SetActive(true);
            _timeText.SetText(MermaidModel.Instance.GetActivityPreheatLeftTimeString());
            _descText2.gameObject.SetActive(false);
        }
        else
        {
            _button.gameObject.SetActive(true);
            _timeText.transform.parent.gameObject.SetActive(false);
            _timeText.SetText(MermaidModel.Instance.GetActivityLeftTimeString());
            _descText2.gameObject.SetActive(true);
        }
    }
    private void OnBtnPlay()
    {
        AnimCloseWindow(() =>
        {
            MermaidModel.Instance.StartActivity();
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMermaidPop);
            //UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidMain);
            
            if(!MermaidModel.Instance.IsPreheating())
                StorySubSystem.Instance.Trigger(StoryTrigger.CloseUI, "UIPopupMermaidStartPreview");
        });
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        if(!MermaidModel.Instance.IsPreheating())
            MermaidModel.Instance.StartActivity();
    }

    private void OnBtnHelp()
    {
        UIManager.Instance.OpenUI(UINameConst.MermaidHelp);
    }

    private void OnBtnClose()
    {
        AnimCloseWindow(() =>
        {
            if(!MermaidModel.Instance.IsPreheating())
                StorySubSystem.Instance.Trigger(StoryTrigger.CloseUI, "UIPopupMermaidStartPreview");
        });
    }
    
    public override void ClickUIMask()
    {
        OnBtnClose();
    }
}
