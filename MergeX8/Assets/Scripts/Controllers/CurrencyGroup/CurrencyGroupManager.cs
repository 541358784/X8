using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;

/// <summary>
/// 顶部资源栏管理方便访问资源数据
/// </summary>
public class CurrencyGroupManager : Manager<CurrencyGroupManager>
{
    public UICurrencyGroupController currencyController;
    public enum CurrencyShowType
    {
        Home,
        Game,
        HappyGo,
    }
    public void Show()
    {
        currencyController = UIManager.Instance.OpenUI(UINameConst.UICurrencyGroup) as UICurrencyGroupController;
        currencyController.UpdateUI();
        currencyController.NotchAdapte();
        currencyController.UpdateShowType(CurrencyShowType.Home);
    }

    public void UpdateShowType(CurrencyShowType showType)
    {
        currencyController?.UpdateShowType(showType);
    }

    public void UpdateText(UserData.ResourceId type, int count, int resNum, float time = 0.5f,
        System.Action callBack = null)
    {
        currencyController.UpdateText(type, count, resNum, time, callBack);
    }

    public void UpdateText(UICurrencyGroupController curController, UserData.ResourceId type, int count, int resNum,
        float time = 0.5f, System.Action callBack = null)
    {
        curController = curController == null ? currencyController : curController;
        curController.UpdateText(type, count, resNum, time, callBack);
    }

    public void CostCurrency(UserData.ResourceId type, int count, GameBIManager.ItemChangeReasonArgs reason)
    {
        switch (type)
        {
            case UserData.ResourceId.Coin:
                UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, count, reason);
                break;
            case UserData.ResourceId.Energy:
                UserData.Instance.ConsumeRes(UserData.ResourceId.Energy, count, reason);
                // EnergyModel.Instance.CostEnergy(count, reason);
                break;    
            case UserData.ResourceId.HappyGo_Energy:
                HappyGoEnergyModel.Instance.CostEnergy(count, reason);
                break;
            case UserData.ResourceId.Diamond:
                UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, count, reason);
                break;
            case UserData.ResourceId.Exp:
                UserData.Instance.ConsumeRes(UserData.ResourceId.Exp, count, reason);
                break;
        }
    }

    /// <summary>
    /// 资源数字滚动
    /// </summary>
    /// <param name="resId"></param>
    /// <param name="subNum"></param>
    /// <param name="time"></param>
    public void UpdateText(UserData.ResourceId resId, int subNum, int resNum, float time)
    {
        if (currencyController == null)
            return;

        currencyController.UpdateText(resId, subNum, resNum, time);
    }

    public void TriggerGuide()
    {
        if (currencyController == null)
            return;

        currencyController.TriggerGuide();
    }


    public Transform GetIconTransform(UserData.ResourceId resId)
    {
        if (currencyController == null)
            return null;

        return currencyController.GetIconTransform(resId);
    }

    public Vector3 GetResourcePosition(UserData.ResourceId resId)
    {
        if (currencyController == null)
            return Vector3.zero;

        return currencyController.GetResourcePosition(resId);
    }

    public void PlayShakeAnim(UserData.ResourceId resId)
    {
        if (currencyController == null)
            return;

        currencyController.PlayShakeAnim(resId);
    }

    public void PlayShakeAnim(UICurrencyGroupController curController, UserData.ResourceId resId)
    {
        curController = curController == null ? currencyController : curController;
        curController.PlayShakeAnim(resId);
    }

    public UICurrencyGroupController GetCurrencyUseController()
    {
        return currencyController;
    }

    public void SetCanvasSortOrder(int order)
    {
        UICurrencyGroupController controller = GetCurrencyUseController();
        if (controller == null)
            return;
        controller.SetCanvasSortOrder(order);
    }

    public void RecoverCanvasSortOrder()
    {
        UICurrencyGroupController controller = GetCurrencyUseController();
        if (controller == null)
            return;
        controller.RecoverCanvasSortOrder();
    }
    
}