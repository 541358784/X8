using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_LevelUpPackage : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private Transform RedPoint;
    private LocalizeTextMeshProUGUI RedPointText;
    private StorageLevelUpPackageSinglePackage PackageStorage;
    public void BindPackage(StorageLevelUpPackageSinglePackage package)
    {
        PackageStorage = package;
    }
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint");
        RedPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        RedPointText.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
        EventDispatcher.Instance.AddEvent<EventLevelUpPackageEnd>(OnPackageEnd);
    }
    
    public void OnPackageEnd(EventLevelUpPackageEnd evt)
    {
        if (evt.PackageStorage == PackageStorage)
            UpdateUI();
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(PackageStorage.IsActive());
        if (!gameObject.activeSelf)
            return;
        if (PackageStorage.IsActive())
        {
            _timeText.SetText(CommonUtils.FormatLongToTimeStr((long)PackageStorage.GetLeftTime()));   
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (PackageStorage.IsActive())
        {
            UIPopupLevelUpPackageController.Open(PackageStorage);
        }
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventLevelUpPackageEnd>(OnPackageEnd);
    }
}
