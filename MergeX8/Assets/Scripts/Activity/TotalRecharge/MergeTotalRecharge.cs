
using Activity.TotalRecharge;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class MergeTotalRecharge : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private GameObject _redPoint;
    private void Awake()
    {
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("Root/RedPoint").gameObject;
        transform.GetComponent<Button>().onClick.AddListener((() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupTotalRecharge,2);
        }));
        InvokeRepeating("RefreshCountDown", 0, 1f);
    }

    private void RefreshCountDown()
    {
        if (!TotalRechargeModel.Instance.IsOpen())
            return;
        _redPoint.gameObject.SetActive(TotalRechargeModel.Instance.IsHaveCanClaim());
        _countDownTime.SetText(TotalRechargeModel.Instance.GetActivityLeftTimeString());
    }

}
