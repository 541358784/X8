
using System;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class MergeHappyGo: MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private LocalizeTextMeshProUGUI _text;
    private GameObject _redPoint;
    private void Awake()
    {
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _text = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("Root/RedPoint").gameObject;
        transform.GetComponent<Button>().onClick.AddListener((() =>
        {
            SceneFsm.mInstance.ChangeState(StatusType.TransitionHappyGo, StatusType.Transition, StatusType.HappyGoGame, MergeBoardEnum.HappyGo);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdIcon);

            //UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidMain,2);
        }));
        InvokeRepeating("RefreshCountDown", 0, 1f);
        Init();
    }

    public void Init()
    {
        if (_text == null)
            return;
        _text.SetText(HappyGoEnergyModel.Instance.EnergyNumber()+"/"+HappyGoEnergyModel.Instance.MaxEnergyNum);
    }
    private void RefreshCountDown()
    {
        if (!HappyGoModel.Instance.IsCanPlay())
            return;
        _redPoint.gameObject.SetActive(HappyGoModel.Instance.IsCanGetReward());
        _countDownTime.SetText(HappyGoModel.Instance.GetActivityLeftTimeString());
        _text.SetText(HappyGoEnergyModel.Instance.EnergyNumber()+"/"+HappyGoEnergyModel.Instance.MaxEnergyNum);

    }
}
