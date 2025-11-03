using System.Collections;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

public class Aux_GarageCleanup : Aux_ItemBase
{
    private IEnumerator dailyIEnumerator = null;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        InvokeRepeating("UpdateUI",0,1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(GarageCleanupModel.Instance.IsOpened());
        if (gameObject.activeSelf)
        {
            _redPoint.gameObject.SetActive(GarageCleanupModel.Instance.IsCanGet());
            _timeText.SetText(GarageCleanupModel.Instance.GetActivityLeftTimeString());
        }
 
        // if(BackHomeControl.isFristPopUI && !GuideSubSystem.Instance.IsShowingGuide() && 
        //    (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
        //     SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome))
        //     GarageCleanupModel.Instance.UpdateActivity();
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (!GarageCleanupModel.Instance.StorageGarageCleanup.IsShowStart)
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupStart);
        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIGarageCleanupMain);

        }

    }

}