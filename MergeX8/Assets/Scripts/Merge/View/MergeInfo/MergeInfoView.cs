using System;
using UnityEngine;


public class MergeInfoView : Singleton<MergeInfoView>
{
    private int openMergeInfoId = -1;

    public bool OpenMergeInfo(TableMergeItem config, Action closeAction = null, bool isShowGetResource = true, bool _isShowProbability=false)
    {
        if (config == null)
            return false;


        Action action = null;
        var window =
            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
        if (window != null && window.mergeId > 0)
        {
            if (config.id != window.mergeId)
                openMergeInfoId = window.mergeId;

            isShowGetResource = window.IsResourcesActive();
            action = () => { OpenMergeInfo(openMergeInfoId, isShowGetResource:isShowGetResource); };
        }
        else
        {
            action = () => { openMergeInfoId = -1; };
        }

        if (MergeManager.Instance.IsBox(config))
        {
            CloseAllMergeInfo();
            if (config.type ==  (int)MergeItemType.flashsaleBox)
            {
                config = GameConfigManager.Instance.GetItemConfig(60000);
            }

            OpenMergeBoxInformation(config, _isShowProbability,() =>
            {
                action?.Invoke();
                closeAction?.Invoke();
            });
        }
        else if (MergeManager.Instance.IsChoiceBox(config))
        {
            CloseAllMergeInfo();
            OpenMergeChoiceBoxInformation(config, _isShowProbability,() =>
            {
                action?.Invoke();
                closeAction?.Invoke();
            });
        }
        else
        {
            CloseAllMergeInfo();
            OpenMergeInformation(config, () =>
            {
                action?.Invoke();
                closeAction?.Invoke();
            }, isShowGetResource);
        }

        return true;
    }

    public bool OpenMergeInfo(int mergeConfigId, Action closeAction = null, bool isShowGetResource = true, bool _isShowProbability=false)
    {
        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(mergeConfigId);

        return OpenMergeInfo(config, closeAction, isShowGetResource,_isShowProbability);
    }

    public void CloseAllMergeInfo()
    {
        UIManager.Instance.CloseUI(UINameConst.UIPopupMergeInformation);
        UIManager.Instance.CloseUI(UINameConst.MergeInformationTips);
    }

    //显示物品合成链
    private bool OpenMergeInformation(TableMergeItem config, Action closeAction = null, bool isShowGetResource= true)
    {
        var window = UIManager.Instance.OpenUI(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
        window.SetInfomations(config, closeAction, isShowGetResource);
        
        return true;
    }

    //显示宝箱产出链
    private bool OpenMergeBoxInformation(TableMergeItem config, bool _isShowProbability=false, Action closeAction = null)
    {
        var window =
            UIManager.Instance.OpenUI(UINameConst.UIPopupMergeInformationExplain) as UIPopupMergeInformationExplainController;
        window.SetInfomations(config, closeAction,_isShowProbability);

        return true;
    }
    //显示自选箱子产出链
    private bool OpenMergeChoiceBoxInformation(TableMergeItem config, bool _isShowProbability=false, Action closeAction = null)
    {
        var tableChoiceChest = GameConfigManager.Instance.GetChoiceChest(config.id);
        if(tableChoiceChest == null)
            return false;
            
        if(tableChoiceChest.item == null || tableChoiceChest.item.Length == 0)
            return false;

        if (tableChoiceChest.item.Length == 3)
        {
            var window = UIManager.Instance.OpenUI(UINameConst.UIPopupMergeInformationOneOutOfThree) as UIPopupMergeInformationOneOutOfThreeController;
            window.SetInfomations(config, closeAction,_isShowProbability);
        }
        else if(tableChoiceChest.item.Length == 5)
        {
            var window = UIManager.Instance.OpenUI(UINameConst.UIPopupMergeInformationOneOutOfFive) as UIPopupMergeInformationOneOutOfFiveController;
            window.SetInfomations(config, closeAction,_isShowProbability);
        }

        return true;
    }
}