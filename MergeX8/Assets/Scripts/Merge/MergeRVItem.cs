using System;
using ABTest;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class MergeRVItem:MonoBehaviour
{
    private Transform RedPoint;
    private void Awake()
    {
        RedPoint = transform.Find("Root/RedPoint");
        RedPoint.gameObject.SetActive(false);
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!DailyRVModel.Instance.IsRVShopOpen())
            {
                CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                {
                    DescString = LocalizationManager.Instance.GetLocalizedString("UI_tv_reward_over"),
                    HasCancelButton = false,
                    HasCloseButton = false,
                });
                return;
            }
            UIDailyRVController.Open("ClickMergeEntrance");
        });
        InvokeRepeating("RefreshRvBtnState",0,1);
    }
    public void RefreshRvBtnState()
    {
        var common = AdConfigHandle.Instance.GetCommon();
        if (common == null)
            return;
        bool isActive = gameObject.activeSelf;
        if (ABTestManager.Instance.IsOpenADTest())
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(common.MergeAdEntrance && DailyRVModel.Instance.IsUnLock() && DailyRVModel.Instance.IsRVShopOpen() && FunctionsSwitchManager.Instance.FunctionOn(FunctionsSwitchManager.FuncType.RvShop));
        }
        // if (isActive != gameObject.activeSelf)
        //     UpdateSibling();
    }
}