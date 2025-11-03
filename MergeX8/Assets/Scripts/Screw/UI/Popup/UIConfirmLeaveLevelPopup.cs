// using UnityEngine;
// using UnityEngine.UI;
//
// namespace ScrewMatch.ScrewGame
// {

using Screw;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;
using UnityEngine.UI;

public interface IConfirmWindowHandler
{
    public void OnConfirm();
    public void OnQuit();
}


[Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupConfirmLeaveLevel")]
public class UIConfirmLeaveLevelPopup : UIWindowController
{
    [UIBinder("ButtonClose")] private Button _closeButton;


    [UIBinder("ExitButton")] private Button _leaveButton;

    [UIBinder("Scroll View")] private Transform _exitGroups;

    private IConfirmWindowHandler _handler;

    //错误注释
    //private ConfirmLeaveLevelGroupWidget confirmLeaveLevelGroupWidget;

    private ScrewGameContext _context;

    private bool _needRecoveryBanner;


    public override void PrivateAwake()
    {
        ComponentBinderUI.BindingComponent(this, transform);

        RegisterEvent();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        _handler = (IConfirmWindowHandler)objs[0];
        _context = (ScrewGameContext)objs[1];
        
        InitMolePopup();
        
        //错误注释
        // if (_needRecoveryBanner)
        //     GameApp.Get<AdSys>().HideBanner();
        //confirmLeaveLevelGroupWidget = CreateWidget<ConfirmLeaveLevelGroupWidget>(_exitGroups.gameObject, true, _context);
    }

    private void InitMolePopup()
    {
        //错误注释
        // var activity = GameApp.Get<ActivitySys>().GetActivity<Activity_MoleSprint>(ActivityType.MoleSprint);
        // if (activity != null && activity.IsActivityOpened() && activity.IsInChallenge())
        // {
        //     CreateWidgetByPath<Mole_InfoPopup>(gameObject.transform, "UISealSprintInPopUp");
        //     _needRecoveryBanner = true;
        // }
    }

    public void RegisterEvent()
    {
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        _leaveButton.onClick.AddListener(OnLeaveButtonClicked);
    }


    private void OnCloseButtonClicked()
    {
        _closeButton.interactable = false;
        _leaveButton.interactable = false;
        AnimCloseWindow();
        _handler.OnQuit();
        
        //错误注释
        // if (_needRecoveryBanner)
        //     GameApp.Get<AdSys>().TryShowCloseBanner();
    }

    private async void OnLeaveButtonClicked()
    {
        //错误注释
        // if (confirmLeaveLevelGroupWidget != null && !confirmLeaveLevelGroupWidget.QuitAni())
        //     return;
        _closeButton.interactable = false;
        _leaveButton.interactable = false;
        AnimCloseWindow();
        if (_context.afterExitLevelHandlers != null && _context.afterExitLevelHandlers.Count > 0)
        {
            for (int i = 0; i < _context.afterExitLevelHandlers.Count; i++)
            {
                await _context.afterExitLevelHandlers[i].Invoke(_context);
            }
        }

        _handler.OnConfirm();
    }
}