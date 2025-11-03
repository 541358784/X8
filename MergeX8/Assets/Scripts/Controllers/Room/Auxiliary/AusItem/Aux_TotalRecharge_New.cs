using DragonPlus;
using TotalRecharge_New;
using UnityEngine;

public class Aux_TotalRecharge_New : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointLabel;
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointLabel = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(TotalRechargeModel_New.Instance.IsOpen());
        if (!gameObject.activeSelf)
            return;
      
        _timeText.SetText(TotalRechargeModel_New.Instance.GetLeftTimeString());
        
        _redPoint.gameObject.SetActive(TotalRechargeModel_New.Instance.IsHaveCanClaim());
        _redPointLabel.gameObject.SetActive(false);
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIManager.Instance.OpenUI(UINameConst.UIPopupTotalRecharge_New);
    }
  
}
