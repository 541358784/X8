using System;
using System.Collections.Generic;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupActivityUnCollectRewardController:UIWindowController
{
    private Button _buttonClose;
    private Button _buttonCollect;
    private Transform rewardItem;

    private bool _isClose = false;
    private Action _onBtnClickCall;
    public override void PrivateAwake()
    {
        _buttonCollect = GetItem<Button>("Root/Button");
        _buttonCollect.onClick.AddListener(OnBtnCollect);
        
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonClose.onClick.AddListener(OnBtnCollect);
        _buttonClose.gameObject.SetActive(false);
        
        rewardItem = transform.Find("Root/Scroll View/Viewport/Content/Item");
        rewardItem.gameObject.SetActive(false);
    }
   
    private List<ResData> _unCollectRewards;
    private GameBIManager.ItemChangeReasonArgs _reasonArgs;
    private Action _callback;
    protected override void OnOpenWindow(params object[] objs)
    {
        _isClose = false;
        _unCollectRewards = GetParamsValue<List<ResData>>(0, objs);
        _reasonArgs = GetParamsValue<GameBIManager.ItemChangeReasonArgs>(1, objs);
        _callback = GetParamsValue<Action>(2, objs);
        _onBtnClickCall = GetParamsValue<Action>(3, objs);
        InitUI(_unCollectRewards);
    }

    public void InitUI(List<ResData> resDatas)
    {
        foreach (var res in resDatas)
        {
           var item =Instantiate(rewardItem, rewardItem.parent);
           item.gameObject.SetActive(true);
           InitRewardItem(item,res.id,res.count,res.isBuilding);
        }
    }
    private void OnBtnCollect()
    {
        if (_isClose)
            return;
        _isClose = true;
        // CommonRewardManager.Instance.PopCommonReward(_unCollectRewards, CurrencyGroupManager.Instance.currencyController,
        //     true, _reasonArgs, () => AnimCloseWindow(_callback));
        AnimCloseWindow(() =>
        {
            PopReward(_unCollectRewards, () => { });//AnimCloseWindow(_callback)
            _onBtnClickCall?.Invoke();
        });
       
    }
    public void PopReward(List<ResData> listResData,Action onEnd)
    {
        if (listResData == null || listResData.Count <= 0)
        {
            onEnd.Invoke();
            return;
        }
        int count = listResData.Count > 8 ? 8 : listResData.Count;
        var list = listResData.GetRange(0, count);
        listResData.RemoveRange(0, count);
        CommonRewardManager.Instance.PopCommonReward(list, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, _reasonArgs, animEndCall:
            () =>
            {
                PopReward(listResData,onEnd);
            });
    }
    private void InitRewardItem(Transform rewardItem, int rewardId,int rewardCount,bool isDeco)
    {
        var rewardImage = rewardItem.Find("Icon").GetComponent<Image>();
        if(rewardImage == null)
            return;
        if (isDeco)
        {
            var decoItem = DecoManager.Instance.FindItem(rewardId);
            if (decoItem != null)
            {
                rewardImage.sprite = CommonUtils.LoadDecoItemIconSprite(decoItem._node._stage.Area.Id,
                    decoItem._data._config.buildingIcon);
                var  t1=rewardItem.Find("Num")?.GetComponent<LocalizeTextMeshProUGUI>();
                if(t1!=null)
                    t1.SetText("x"+rewardCount);
            }
            return;
        }
        var tipsBtn = rewardItem.Find("TipsBtn")?.GetComponent<Button>();
        if (UserData.Instance.IsResource(rewardId))
        {
            rewardImage.sprite = UserData.GetResourceIcon(rewardId,UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig= GameConfigManager.Instance.GetItemConfig(rewardId);
            if(itemConfig != null)
                rewardImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            if (tipsBtn != null)
            {
                tipsBtn.gameObject.SetActive(true);
                tipsBtn.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemConfig, null, false,true);
                });
            }
              
        }
        var  text=rewardItem.Find("Num")?.GetComponent<LocalizeTextMeshProUGUI>();
        if(text!=null)
            text.SetText("x"+rewardCount);
    }
    private T GetParamsValue<T>(int index, object[] objects)
    {
        if (objects == null || objects.Length == 0)
            return default(T);

        if (index < 0 || index >= objects.Length)
            return default(T);

        return (T) objects[index];
    }
}