using System;
using System.Collections;
using System.Collections.Generic;
using Activity.GardenTreasure.Model;
using Activity.Turntable.Model;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_GardenTreasure : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_GardenTreasure Instance;
    private GameObject _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    private int _guideNum = 0;
    
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint").gameObject;
        _redPointText = _redPoint.transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
        
        InvokeRepeating("UpdateUI", 0, 1);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.GardenTreasureStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(GardenTreasureModel.Instance.IsOpened());
        int value = UserData.Instance.GetRes(UserData.ResourceId.GardenShovel);
        _redPoint.gameObject.SetActive(value > 0);
        
        if (!gameObject.activeSelf)
            return;
        
        if (!GardenTreasureModel.Instance.IsPreheatEnd())
        {
            _timeText.SetText(GardenTreasureModel.Instance.GetPreheatEndTimeString());
            _redPointText.SetText("");
            _redPoint.gameObject.SetActive(false);
        }
        else
        {
            _timeText.SetText(GardenTreasureModel.Instance.GetEndTimeString());
            _redPointText.SetText(value.ToString());
        }
        

        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.GardenTreasureStart) && GardenTreasureModel.Instance.IsPreheatEnd() && 
            !GuideSubSystem.Instance.IsShowingGuide() && (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home || SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome))
        {
            _guideNum++;
            if (UIManager.Instance.GetOpenUI(UINameConst.UIPopupMergeLevelTipsShow))
            {
                _guideNum = 0;
                return;
            }
            if (_guideNum >= 3)
            {
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.GardenTreasureStart, "");
                _guideNum = 0;
            }
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();

        if (GardenTreasureModel.Instance.IsPreheatEnd())
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GardenTreasureStart);
            UIManager.Instance.OpenUI(UINameConst.UIGardenTreasureMain);
        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupGardenTreasureStart);
        }
    }
    private void OnDestroy()
    {
    }
}
