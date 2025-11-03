using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

public class Aux_Mermaid : Aux_ItemBase
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
        _redPoint.Find("Label").gameObject.SetActive(false);
        _redPoint.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI",0,1);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MermaidStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(MermaidModel.Instance.IsOpened());
        if (gameObject.activeSelf)
        {
            _redPoint.gameObject.SetActive(MermaidModel.Instance.IsCanClaim());
            if (MermaidModel.Instance.IsPreheating())
            {
                _timeText.SetText(MermaidModel.Instance.GetActivityPreheatLeftTimeString());
            }
            else
            {
                if(MermaidModel.Instance.GetActivityLeftTime()<=0)
                    _timeText.SetTerm("ui_event_mermaid_extend_time_button2");
                else
                {
                    _timeText.SetText(MermaidModel.Instance.GetActivityLeftTimeString());

                }
            }
        }
    }

    protected override void OnButtonClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MermaidStart);
        
        base.OnButtonClick();
        if (MermaidModel.Instance.IsPreheating())
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterAdvance);

            UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidStartPreview);
        }
        else
        {
            if (!MermaidModel.Instance.IsShowStart())
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPop);
                UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidStartPreview);
            }
            else
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidMain);
            }
        }
    }
}