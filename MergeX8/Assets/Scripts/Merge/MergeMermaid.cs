
using System;
using DG.Tweening;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class MergeMermaid: MonoBehaviour
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
            UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidMain,2);
        }));
        InvokeRepeating("RefreshCountDown", 0, 1f);
        Init();
    }

    public void Init()
    {
        if (_text == null)
            return;
        _text.SetText(MermaidModel.Instance.GetScore().ToString());
    }
    private void RefreshCountDown()
    {
        if (!MermaidModel.Instance.IsOpened())
            return;
        _redPoint.gameObject.SetActive(MermaidModel.Instance.IsCanClaim());
        _countDownTime.SetText(MermaidModel.Instance.GetActivityLeftTimeString());
    }

    public void SetText(int oldValue)
    {
        var newValue = UserData.Instance.GetRes(UserData.ResourceId.Mermaid);
        DOTween.To(() => oldValue, x => oldValue = x, newValue, 1f).OnUpdate(() =>
        {
            _text.SetText(oldValue.ToString());
        });

    }

}
