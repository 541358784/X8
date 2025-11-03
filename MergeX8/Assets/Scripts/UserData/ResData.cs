using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Config;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Framework.Wrapper;
using Gameplay;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResData
{
    public int id;
    public int count;
    public bool isBuilding;

    public ResData(int id, int count, bool isBuilding = false)
    {
        this.id = id;
        this.count = count;
        this.isBuilding = isBuilding;
    }
    public ResData(UserData.ResourceId id, int count)
    {
        this.id = (int)id;
        this.count = count;
    }

    public static string GetValueTextFormat(ResData data)
    {
        return $"{data.count:N0}";
    }

    public static ResData Parse(string config)
    {
        if (string.IsNullOrEmpty(config))
            return new ResData(0, 0);

        if (config.Equals("0"))
            return new ResData(0, 0);

        try
        {
            string[] arr = config.Split(',');
            var id = StringUtils.StringToInt(arr[0]);
            var count = StringUtils.StringToInt(arr[1]);
            return new ResData(id, count);
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
            DebugUtil.LogError("cfg:" + config);
        }

        return new ResData(0, 0);
    }

    public static List<ResData> ParseList(string config)
    {
        if (string.IsNullOrEmpty(config))
        {
            return null;
        }

        var itemList = new List<ResData>();

        var prices = config.Split(';');
        for (int i = 0; i < prices.Length; i++)
        {
            itemList.Add(Parse(prices[i]));
        }

        return itemList;
    }
}


public class UIItemData : UIGameObjectWrapper
{
    public enum ValueTextFormat
    {
        OnlyNumber,
        PlusNumber,
        MultiNumber,
    }

    public ResData ItemData { get; private set; }
    private Image _imgIcon;
    private LocalizeTextMeshProUGUI _txtName;
    private LocalizeTextMeshProUGUI _txtValue;
    private ResObject _resObject;
    private Color _defaultTextColor;

    public UIItemData(GameObject go, string imgKey, string nameTxtKey, string valueTxtKey) : base(go)
    {
        _imgIcon = TryGetItem<Image>(imgKey);

        if (!string.IsNullOrEmpty(nameTxtKey))
        {
            _txtName = TryGetItem<LocalizeTextMeshProUGUI>(nameTxtKey);
        }

        if (!string.IsNullOrEmpty(valueTxtKey))
        {
            _txtValue = TryGetItem<LocalizeTextMeshProUGUI>(valueTxtKey);
            if (_txtValue != null)
            {
                _defaultTextColor = _txtValue.GetColor();
            }
        }
    }

    public static ResData Parse(string cfg)
    {
        ResData item = null;
        try
        {
            string[] arr = cfg.Split(',');
            item = new ResData(StringUtils.StringToInt(arr[0]), StringUtils.StringToInt(arr[1]));
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }

        return item;
    }

    public void SetData(ResData itemData, ValueTextFormat valueTextFormat, bool setPic, bool redIfLack)
    {
        ItemData = itemData;

        if (_imgIcon != null && setPic)
        {
            _imgIcon.sprite = Gameplay.UserData.GetResourceIcon(itemData.id, UserData.ResourceSubType.Small);
        }

        string text;
        switch (valueTextFormat)
        {
            case ValueTextFormat.OnlyNumber:
                text = $"{ResData.GetValueTextFormat(itemData):N0}";
                break;
            case ValueTextFormat.PlusNumber:
                text = $"+ {ResData.GetValueTextFormat(itemData):N0}";
                break;
            case ValueTextFormat.MultiNumber:
                text = $"x {ResData.GetValueTextFormat(itemData):N0}";
                break;
            default:
                text = $"x {ResData.GetValueTextFormat(itemData):N0}";
                break;
        }

        if (_txtValue != null)
        {
            _txtValue.SetText(text);
            if (redIfLack && itemData.count > UserData.Instance.GetRes((UserData.ResourceId) itemData.id))
            {
                _txtValue.SetColor(UnityEngine.Color.red);
            }
            else
            {
                _txtValue.SetColor(_defaultTextColor);
            }
        }

        if (_txtName != null)
        {
            _txtName.SetText(UserData.GetResourceName((UserData.ResourceId) itemData.id));
        }
    }

    public void DirectlySetText(string text)
    {
        _txtValue.SetText(text);
    }

    public GameObject GetImage()
    {
        return _imgIcon?.gameObject;
    }

    public LocalizeTextMeshProUGUI GetValueText()
    {
        return _txtValue;
    }

    public void Dispose()
    {
        _imgIcon = null;
        _txtName = null;
        _txtValue = null;
        ItemData = null;
        _resObject?.Dispose();
        _resObject = null;
    }
}