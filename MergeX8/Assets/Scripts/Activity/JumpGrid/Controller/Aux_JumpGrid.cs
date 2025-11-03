using System.Collections;
using System.Collections.Generic;
using Activity.JumpGrid;
using DragonPlus;
using UnityEngine;

public class Aux_JumpGrid : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;

    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        InvokeRepeating("UpdateUI", 0, 1);

        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.JumpGridStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(JumpGridModel.Instance.IsOpened() && !JumpGridModel.Instance.StorageJumpGrid.IsShowEndView);
        if (gameObject.activeSelf)
        {
            if (JumpGridModel.Instance.IsPreheating())
            {
                _redPoint.gameObject.SetActive(!JumpGridModel.Instance.StorageJumpGrid.IsShowPreheat);
                _timeText.SetText(JumpGridModel.Instance.GetActivityPreheatLeftTimeString());
            }
            else
            {
                _redPoint.gameObject.SetActive(JumpGridModel.Instance.IsCanClaim());
                _timeText.SetText(JumpGridModel.Instance.GetActivityLeftTimeString());
            }
        }
    }

    protected override void OnButtonClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.JumpGridStart);
        base.OnButtonClick();
        if (!JumpGridModel.Instance.StorageJumpGrid.IsShowStartView)
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupJumpGridStart);
        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIJumpGridMain);
        }
    }
}