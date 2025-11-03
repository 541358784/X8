using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using Mosframe;
using UnityEngine;
using UnityEngine.UI;

public class MergeTaskItemExtraOrderRewardCouponGroup:MonoBehaviour
{
    private Transform DefaultItem;
    private StorageTaskItem StorageTask;
    private MergeTaskTipsItem MergeTaskItem;

    private Dictionary<ExtraOrderRewardCouponType, MergeTaskItemExtraOrderRewardCoupon> ItemDic =
        new Dictionary<ExtraOrderRewardCouponType, MergeTaskItemExtraOrderRewardCoupon>();
    private void Awake()
    {
        DefaultItem = transform.Find("Default");
        DefaultItem.gameObject.SetActive(false);
        UpdateView();
    }

    public void UpdateView()
    {
        
    }

    public void Init(StorageTaskItem storageTask,MergeTaskTipsItem mergeTaskItem)
    {
        StorageTask = storageTask;
        MergeTaskItem = mergeTaskItem;
        Refresh();
    }
    public void RefreshThemeDecoration()
    {
        var couponType = ExtraOrderRewardCouponType.ThemeDecoration;
        if (MergeTaskItem.ThemeDecorationGroup.gameObject.activeSelf)
        {
            var multi = ExtraOrderRewardCouponModel.Instance.GetMultiValue(couponType);
            if (multi > 1f)
            {
                var value = ThemeDecorationModel.Instance.GetTaskValue(StorageTask, false);
                value = (int)((multi - 1f) * value);
                if (!ItemDic.TryGetValue(couponType, out var item))
                {
                    item = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                        .AddComponent<MergeTaskItemExtraOrderRewardCoupon>();
                    item.Init();
                    item.gameObject.SetActive(true);
                    ItemDic.Add(couponType,item);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
                }
                item.Icon.sprite = UserData.GetResourceIcon(UserData.ResourceId.ThemeDecorationScore, UserData.ResourceSubType.Big);
                item.NumText.SetText("+"+value);
            }
            else
            {
                RemoveItem(couponType);
            }
        }
        else
        {
            RemoveItem(couponType);
        }
    }
    public void RefreshSnakeLadder()
    {
        var couponType = ExtraOrderRewardCouponType.SnakeLadder;
        if (MergeTaskItem._snakeLadderTurntableGroup.gameObject.activeSelf)
        {
            var multi = ExtraOrderRewardCouponModel.Instance.GetMultiValue(couponType);
            if (multi > 1f)
            {
                var value = SnakeLadderModel.Instance.GetTaskValue(StorageTask, false);
                value = (int)((multi - 1f) * value);
                if (!ItemDic.TryGetValue(couponType, out var item))
                {
                    item = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                        .AddComponent<MergeTaskItemExtraOrderRewardCoupon>();
                    item.Init();
                    item.gameObject.SetActive(true);
                    ItemDic.Add(couponType,item);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
                }
                item.Icon.sprite = UserData.GetResourceIcon(UserData.ResourceId.SnakeLadderTurntable, UserData.ResourceSubType.Big);
                item.NumText.SetText("+"+value);
            }
            else
            {
                RemoveItem(couponType);
            }
        }
        else
        {
            RemoveItem(couponType);
        }
    }
    public void RefreshDogHope()
    {
        var couponType = ExtraOrderRewardCouponType.DogHope;
        if (MergeTaskItem._dogHopeObj.gameObject.activeSelf)
        {
            var multi = ExtraOrderRewardCouponModel.Instance.GetMultiValue(couponType);
            if (multi > 1f)
            {
                var value = StorageTask.DogCookiesNum;
                value = (int)((multi - 1f) * value);
                if (!ItemDic.TryGetValue(couponType, out var item))
                {
                    item = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                        .AddComponent<MergeTaskItemExtraOrderRewardCoupon>();
                    item.Init();
                    item.gameObject.SetActive(true);
                    ItemDic.Add(couponType,item);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
                }
                item.Icon.sprite = MergeTaskItem._dogHopeObj.transform.Find("Icon").GetComponent<Image>().sprite;
                item.NumText.SetText("+"+value);
            }
            else
            {
                RemoveItem(couponType);
            }
        }
        else
        {
            RemoveItem(couponType);
        }
    }
    public void RefreshParrot()
    {
        var couponType = ExtraOrderRewardCouponType.Parrot;
        if (MergeTaskItem.ParrotGroup.gameObject.activeSelf)
        {
            var multi = ExtraOrderRewardCouponModel.Instance.GetMultiValue(couponType);
            if (multi > 1f)
            {
                var value = ParrotModel.Instance.GetTaskValue(StorageTask);
                value = (int)((multi - 1f) * value);
                if (!ItemDic.TryGetValue(couponType, out var item))
                {
                    item = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                        .AddComponent<MergeTaskItemExtraOrderRewardCoupon>();
                    item.Init();
                    item.gameObject.SetActive(true);
                    ItemDic.Add(couponType,item);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
                }
                item.Icon.sprite = MergeTaskItem.ParrotGroup.transform.Find("Icon").GetComponent<Image>().sprite;
                item.NumText.SetText("+"+value);
            }
            else
            {
                RemoveItem(couponType);
            }
        }
        else
        {
            RemoveItem(couponType);
        }
    }
    public void RefreshFlowerField()
    {
        var couponType = ExtraOrderRewardCouponType.FlowerField;
        if (MergeTaskItem.FlowerFieldGroup.gameObject.activeSelf)
        {
            var multi = ExtraOrderRewardCouponModel.Instance.GetMultiValue(couponType);
            if (multi > 1f)
            {
                var value = FlowerFieldModel.Instance.GetTaskValue(StorageTask);
                value = (int)((multi - 1f) * value);
                if (!ItemDic.TryGetValue(couponType, out var item))
                {
                    item = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                        .AddComponent<MergeTaskItemExtraOrderRewardCoupon>();
                    item.Init();
                    item.gameObject.SetActive(true);
                    ItemDic.Add(couponType,item);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
                }
                item.Icon.sprite = MergeTaskItem.FlowerFieldGroup.transform.Find("Icon").GetComponent<Image>().sprite;
                item.NumText.SetText("+"+value);
            }
            else
            {
                RemoveItem(couponType);
            }
        }
        else
        {
            RemoveItem(couponType);
        }
    }
    public void RefreshCoin()
    {
        var couponType = ExtraOrderRewardCouponType.Coin;
        var multi = ExtraOrderRewardCouponModel.Instance.GetMultiValue(couponType);
        if (multi > 1f)
        {
            var showItem = false;
            for (int i = 0; i < StorageTask.RewardTypes.Count; i++)
            {
                if(i >= 2)
                    break;
                var rewardType = StorageTask.RewardTypes[i];
                var rewardNum = StorageTask.RewardNums[i];
                rewardType = MainOrderManager.ChangeTaskRewardType(rewardType);
                if (rewardNum > 0 && (rewardType == (int) UserData.ResourceId.Coin ||
                                      rewardType == (int) UserData.ResourceId.RecoverCoinStar))
                {
                    showItem = true;
                    var value = rewardNum;
                    value = (int)((multi - 1f) * value);
                    if (!ItemDic.TryGetValue(couponType, out var item))
                    {
                        item = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                            .AddComponent<MergeTaskItemExtraOrderRewardCoupon>();
                        item.Init();
                        item.gameObject.SetActive(true);
                        ItemDic.Add(couponType,item);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
                    }
                    item.Icon.sprite = UserData.GetResourceIcon(rewardType, UserData.ResourceSubType.Big);
                    item.NumText.SetText("+"+value);
                }
            }
            if (!showItem)
            {
                RemoveItem(couponType);
            }
        }
        else
        {
            RemoveItem(couponType);
        }
    }

    public void RemoveItem(ExtraOrderRewardCouponType couponType)
    {
        if (ItemDic.TryGetValue(couponType, out var item))
        {
            Destroy(item.gameObject);
            ItemDic.Remove(couponType);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }
    }
    public void Refresh()
    {
        // RefreshThemeDecoration();
        // RefreshSnakeLadder();
        // RefreshDogHope();
        // RefreshCoin();
    }
}

public class MergeTaskItemExtraOrderRewardCoupon : MonoBehaviour
{
    public Image Icon;
    public LocalizeTextMeshProUGUI NumText;

    public void Init()
    {
        Icon = transform.Find("Icon").GetComponent<Image>();
        NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
    }

    // public void SetValue(UserData.ResourceId resType,float value)
    // {
    //     Icon.sprite = UserData.GetResourceIcon(resType, UserData.ResourceSubType.Big);
    //     NumText.SetText("+"+value);
    // }
}