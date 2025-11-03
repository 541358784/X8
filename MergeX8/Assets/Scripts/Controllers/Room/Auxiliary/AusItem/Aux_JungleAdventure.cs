
using System.Collections.Generic;
using DragonPlus;
using Farm.Model;
using UnityEngine;

public class Aux_JungleAdventure : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_JungleAdventure Instance;
    private GameObject _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint").gameObject;
        _redPointText = _redPoint.transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
        
        InvokeRepeating("UpdateUI", 0, 1);

        UpdateUI();
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.JungleAdventureStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(JungleAdventureModel.Instance.IsOpened());
        _redPoint.gameObject.SetActive(false);
        if (!gameObject.activeSelf)
            return;

        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.JungleAdventureStart) && JungleAdventureModel.Instance.IsPreheatEnd() && 
            !GuideSubSystem.Instance.IsShowingGuide())
        {
            if (FarmModel.Instance.IsFarmModel())
            {
                if((SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home || SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome) && UIManager.Instance.GetOpenUICount() == 3||
                   SceneFsm.mInstance.GetCurrSceneType() == StatusType.EnterFarm  && UIManager.Instance.GetOpenUICount() == 4)
                {
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JungleAdventureStart, "");
                }
            }
            else
            {
                if((SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home || SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome) && UIManager.Instance.GetOpenUICount() == 3)
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JungleAdventureStart, "");
            }
        }
        
        if (!JungleAdventureModel.Instance.IsPreheatEnd())
        {
            _timeText.SetText(JungleAdventureModel.Instance.GetPreheatEndTimeString());
            _redPointText.SetText("");
            _redPoint.gameObject.SetActive(false);
        }
        else
        {
            _timeText.SetText(JungleAdventureModel.Instance.GetEndTimeString());
            //_redPointText.SetText(value.ToString());
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();

        if (JungleAdventureModel.Instance.IsPreheatEnd())
        {
            //GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GardenTreasureStart);
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.JungleAdventureStart, null);
            UIManager.Instance.OpenUI(UINameConst.UIJungleAdventureMain);
        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupJungleAdventurePreview);
        }
    }
    private void OnDestroy()
    {
    }
}