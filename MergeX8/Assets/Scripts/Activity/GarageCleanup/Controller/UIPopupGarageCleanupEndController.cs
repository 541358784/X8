using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupGarageCleanupEndController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;
    private Button _buttonClose;
    private Button _buttonShow;
    private LocalizeTextMeshProUGUI _descText;
    private StorageGarageCleanup _storageGarageCleanup;
    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/BGGroup/Text");
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonShow = GetItem<Button>("Root/Button");
        _buttonShow.onClick.AddListener(OnBtnShow);
        _descText = GetItem<LocalizeTextMeshProUGUI>("Root/BGGroup/Text (1)");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        _storageGarageCleanup = (StorageGarageCleanup)objs[0];
        _storageGarageCleanup.IsActivityEnd = true;
        
        if (GarageCleanupModel.Instance.IsActivityFinish(_storageGarageCleanup))
        {
            _descText.SetTerm("ui_cleanup_desc14");
        }
        else
        {
            _descText.SetTerm("ui_cleanup_desc9");
        }

        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventGarageCleanEndData,GarageCleanupModel.Instance.GetRevealNum(_storageGarageCleanup).ToString());

    }
    
    
    private void OnBtnShow()
    {
        AnimCloseWindow();
    }

    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

}