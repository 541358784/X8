using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class Aux_Shop : Aux_ItemBase
{    
    private IEnumerator dailyIEnumerator = null;
    private Transform _redPoint;
    protected override void Awake()
    {
        base.Awake();
        _redPoint = transform.Find("Tips");
        UpdateUI();
        InvokeRepeating("UpdateUI", 0, 1);
    }


    public override void UpdateUI()
    {
        int itemId = 0;
        bool show = StoreModel.Instance.IsCanBuyFreeStoreItem(out itemId);
        if(_redPoint!=null)
            _redPoint.gameObject.SetActive(show);
        if (gameObject.activeSelf)
        {
            // StopDelayWork();
            // dailyIEnumerator = CommonUtils.DelayWork(1, () => { UpdateUI(); });
            // StartCoroutine(dailyIEnumerator);
        }
    }

    private void StopDelayWork()
    {
        if (dailyIEnumerator == null)
            return;

        StopCoroutine(dailyIEnumerator);
    }
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIStoreController.OpenUI();
    }


}