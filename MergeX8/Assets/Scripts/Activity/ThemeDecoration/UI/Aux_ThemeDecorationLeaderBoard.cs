using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_ThemeDecorationLeaderBoard : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private StorageThemeDecorationLeaderBoard Storage;
    
    private LocalizeTextMeshProUGUI _rankText;
    public void SetStorage(StorageThemeDecorationLeaderBoard storage)
    {
        Storage = storage;
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        transform.Find("RedPoint").gameObject.SetActive(false);
        _rankText = transform.Find("LvText").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);

        SetStorage(ThemeDecorationLeaderBoardModel.Instance.CurStorageThemeDecorationLeaderBoardWeek);
    }

    public override void UpdateUI()
    {
        if (!gameObject)
            return;
        gameObject.SetActive(Storage != null && Storage.ShowEntrance());
        if (!gameObject.activeSelf)
            return;
        if (!Storage.IsTimeOut())
        {
            _timeText.SetText(Storage.GetLeftTimeText());
            _rankText.gameObject.SetActive(Storage.IsStorageWeekInitFromServer());
            if (_rankText.gameObject.activeSelf)
            {
                _rankText.SetText("No."+Storage.SortController().MyRank);
            }
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (!Storage.IsResExist())
            return;
        if (!Storage.IsTimeOut())
        {
            ThemeDecorationLeaderBoardModel.OpenMainPopup(Storage);
        }
    }
    private void OnDestroy()
    {
    }
}
