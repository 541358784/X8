using UnityEngine.UI;

public class UIPopupSummerWatermelonBreadStartController:UIWindowController
{
    private Button _playBtn;
    private Button _closeBtn;
    public override void PrivateAwake()
    {
        _playBtn = GetItem<Button>("Root/Button");
        _playBtn.onClick.AddListener(OnPlayBtn);
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnCloseBtn);
    }

    public void OnPlayBtn()
    {
        AnimCloseWindow(() =>
        {
            OpenGuide();
        });
    }

    public void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
            OpenGuide();
        });
    }
    
    public void OpenGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SummerWatermelonBreadStart) &&
            GuideSubSystem.Instance.GetTarget(GuideTargetType.SummerWatermelonBreadStart,"") != null)
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);   
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SummerWatermelonBreadStart, null);   
            }
            else
            {
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SummerWatermelonBreadStart, null);   
            }
        }
        else
        {
            SummerWatermelonBreadModel.Instance.OpenMainPopup();
        }
    }
}