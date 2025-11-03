using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;
public class UIDogStartPreviewController:UIWindowController
{
    private Button _buttonClose;
    private Button _buttonStart;
    private LocalizeTextMeshProUGUI _time;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _buttonStart = GetItem<Button>("Root/Button");
        _buttonStart.onClick.AddListener(OnStartBtn);
        _time = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/Text");
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDogPop);
        
        InvokeRepeating("UpdateUI", 0 , 1);
    }

    private void OnStartBtn()
    {
        AnimCloseWindow(() =>
        {
        });
    }

    private void OnCloseBtn()
    {
       AnimCloseWindow();
    }
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow();
    }   
    public  void UpdateUI()
    {
        _time.SetText(DogHopeModel.Instance.GetActivityLeftTimeString());
    }
}
