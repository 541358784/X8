
using DragonPlus;
using TotalRecharge_New;
using UnityEngine;
using UnityEngine.UI;

public class MergeTotalRecharge_New : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private GameObject _redPoint;
    private void Awake()
    {
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("Root/RedPoint").gameObject;
        transform.GetComponent<Button>().onClick.AddListener((() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupTotalRecharge_New,2);
        }));
        InvokeRepeating("RefreshCountDown", 0, 1f);
    }

    private void RefreshCountDown()
    {
        if (!TotalRechargeModel_New.Instance.IsOpen())
            return;
        _redPoint.gameObject.SetActive(TotalRechargeModel_New.Instance.IsHaveCanClaim());
        _countDownTime.SetText(TotalRechargeModel_New.Instance.GetLeftTimeString());
    }

}
