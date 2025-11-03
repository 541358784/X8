using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_HappyGo : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_button.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.HappyGoButton, _button.transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(HappyGoModel.Instance.IsOpened() );
        if (!gameObject.activeSelf)
            return;
        if (HappyGoModel.Instance.IsPreheating())
        {
            _timeText.SetText(HappyGoModel.Instance.GetActivityPreheatLeftTimeString());
        }
        else
        {
            if(HappyGoModel.Instance.GetActivityLeftTime()<=0)
                _timeText.SetTerm("ui_event_mermaid_extend_time_button2");
            else
            {
                _timeText.SetText(HappyGoModel.Instance.GetActivityLeftTimeString());
            }
        }
        _redPoint.gameObject.SetActive(HappyGoModel.Instance.IsCanGetReward()&&HappyGoModel.Instance.IsStart());

    }

    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (HappyGoModel.Instance.IsWaitBuyExtendDay())
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoExtend);
        }
        else
        {
            if (HappyGoModel.Instance.IsStart())
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.HappyGoButton);
                ShakeManager.Instance.ShakeSelection();
                AudioManager.Instance.PlaySound(SfxNameConst.button_s);
                SceneFsm.mInstance.TransitionGame(MergeBoardEnum.HappyGo);
            }
            else
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoStart);
            }
        }

        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdIcon);
    }
    private void OnDestroy()
    {
    }
}
