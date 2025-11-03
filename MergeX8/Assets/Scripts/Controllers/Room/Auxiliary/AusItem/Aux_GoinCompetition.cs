using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

public class Aux_CoinCompetition : Aux_ItemBase
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
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CoinCompetitionStart, transform as RectTransform, topLayer: topLayer);

    }

    public override void UpdateUI()
    {
        gameObject.SetActive(CoinCompetitionModel.Instance.IsOpened() && !CoinCompetitionModel.Instance.StorageCompetition.IsShowEndView);
        if (gameObject.activeSelf)
        {
   
            if (CoinCompetitionModel.Instance.IsPreheating())
            {
                _redPoint.gameObject.SetActive(!CoinCompetitionModel.Instance.StorageCompetition.IsShowPreheat);
                _timeText.SetText(CoinCompetitionModel.Instance.GetActivityPreheatLeftTimeString());
            }
            else
            {
                _redPoint.gameObject.SetActive(CoinCompetitionModel.Instance.IsCanClaim());
                _timeText.SetText(CoinCompetitionModel.Instance.GetActivityLeftTimeString());
            }
        }
    }

    protected override void OnButtonClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CoinCompetitionStart);
        base.OnButtonClick();
        if (!CoinCompetitionModel.Instance.StorageCompetition.IsShowStartView)
        {
            UIManager.Instance.OpenUI(UINameConst.UICoinCompetitionStart);
        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UICoinCompetitionMain);

        }

    }

}