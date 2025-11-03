using System;
using Activity.Turntable.Model;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;


public class MergeTurntableEntry : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private GameObject _redPoint;
    
    private void Awake()
    {
        var btn = transform.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupTurntableMain);
        }); 
        
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("Root/RedPoint").gameObject;
        
        InvokeRepeating("RefreshCountDown", 0, 1f);
    }
    
    private void RefreshView()
    {
        gameObject.SetActive(TurntableModel.Instance.IsOpened());
        _redPoint.gameObject.SetActive(TurntableModel.Instance.Turntable.ActivityCoin > 0);
    }
    
    private void RefreshCountDown()
    {
        RefreshView();
        
        _countDownTime.SetText(TurntableModel.Instance.GetActivityLeftTimeString());
    }
}