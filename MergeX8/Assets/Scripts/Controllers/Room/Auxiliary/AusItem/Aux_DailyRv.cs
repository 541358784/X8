using System.Collections;
using System.Collections.Generic;
using ABTest;
using DragonPlus;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class Aux_DailyRv : Aux_ItemBase
{
    public Animator iconAni;
    private Image hintIcon;
    private Transform hintTag;
    private IEnumerator dailyIEnumerator = null;

    protected override void Awake()
    {
        base.Awake();
        iconAni = transform.Find("Icon").GetComponent<Animator>();
        hintIcon = transform.Find("HintTag/Icon").GetComponent<Image>();
        hintTag = transform.Find("HintTag");
        InvokeRepeating("UpdateUI", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.BackHomeStep, BackHomeStep);
    }


    public override void UpdateUI()
    {
        DailyRVModel.Instance.UpdateRVShopState();
        if (ABTestManager.Instance.IsOpenADTest())
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(DailyRVModel.Instance.IsRVShopOpen() && FunctionsSwitchManager.Instance.FunctionOn(FunctionsSwitchManager.FuncType.RvShop));
        
        // if (gameObject.activeSelf)
        // {
        //     StopDelayWork();
        //     dailyIEnumerator = CommonUtils.DelayWork(1, () => { UpdateUI(); });
        //     if (gameObject.activeInHierarchy)
        //         StartCoroutine(dailyIEnumerator);
        // }
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
        UIDailyRVController.Open("ClickHomeEntrance");
    }


    private void BackHomeStep(BaseEvent e)
    {
        if (e.datas == null || e.datas.Length == 0)
            return;

        if ((string) e.datas[0] != "guide")
            return;
    }

    private void OnDestroy()
    {
        StopDelayWork();
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BackHomeStep, BackHomeStep);
    }
}