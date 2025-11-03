
using System;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class MergeBattlePass: MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private LocalizeTextMeshProUGUI _redPointText;
    private Transform _redPoint;
    
    private void Awake()
    {
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("Root/RedPoint");
        _redPointText = transform.Find("Root/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        transform.GetComponent<Button>().onClick.AddListener((() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIBattlePassMain,2);
        }));
        InvokeRepeating("RefreshCountDown", 0, 1f);
        _redPointText.gameObject.SetActive(true);
    }
    
    private void RefreshCountDown()
    {
        if(!gameObject.activeSelf)
            return;
        
        _countDownTime.SetText(Activity.BattlePass.BattlePassModel.Instance.GetActivityLeftTimeString());
        int num = Activity.BattlePass.BattlePassTaskModel.Instance.GetCompleteNum();
        num += Activity.BattlePass.BattlePassModel.Instance.CanGetRewardCount();
        _redPoint.gameObject.SetActive(num>0);
        _redPointText.SetText(num.ToString());
    }

}
