using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ABTest;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509.Qualified;
using Difference;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using Merge.Order;
using Newtonsoft.Json;
using UnityEngine;

public class OrderConfigManager : Singleton<OrderConfigManager>
{
    public enum SpineType
    {
        Normal,
        Time,
        KeepPet
    }
    
    private List<TableOrderFix> _orderFixs;
    private List<TableOrderFix> _orderFixs_2;
    private List<TableOrderFix> _orderFixs_3;
    public List<TableOrderAppend> _orderAppends;
    public List<TableOrderHeadSpine> _orderHeadSpines;
    public List<TableOrderCreate> _orderCreates;
    public List<TableOrderAppend> _orderVaildAppends = new List<TableOrderAppend>();
    private Dictionary<int, Dictionary<int, TableOrderRandom>>  _orderRandoms = new Dictionary<int, Dictionary<int, TableOrderRandom>>();
    private Dictionary<int, TableOrderFilter> _orderFilters = new Dictionary<int, TableOrderFilter>();
    
    
    private Dictionary<int, Dictionary<int, TableOrderRandom>> _orderRandoms_2 = new Dictionary<int, Dictionary<int, TableOrderRandom>>();
    private Dictionary<int, TableOrderFilter> _orderFilters_2 = new Dictionary<int, TableOrderFilter>();
    
    public Dictionary<int, TableOrderItem> _orderItems = new Dictionary<int, TableOrderItem>();
    public List<TableOrderItemWeight> _orderItemWeights = new List<TableOrderItemWeight>();
    public List<TableOrderExtend> _orderExtends;
    
    private Dictionary<int, TableOrderHeadSpine> _headSpinesDic = new Dictionary<int, TableOrderHeadSpine>();
    public Dictionary<int, List<TableOrderHeadSpine>> _headSpineDic = new Dictionary<int, List<TableOrderHeadSpine>>();
    
    public void InitTableConfigs()
    {
        TableManager.Instance.InitLocation("configs/Order");
        
        _orderFixs = TableManager.Instance.GetTable<TableOrderFix>();
        _orderItemWeights = TableManager.Instance.GetTable<TableOrderItemWeight>();
        _orderAppends = TableManager.Instance.GetTable<TableOrderAppend>();

        TextAsset json2 = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/order","orderfix_2"));
        _orderFixs_2 = TableManager.DeSerialize<TableOrderFix>(json2.text);
        
        TextAsset json3 = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/order","orderfix_3"));
        _orderFixs_3 = TableManager.DeSerialize<TableOrderFix>(json3.text);
            
        _orderHeadSpines =  TableManager.Instance.GetTable<TableOrderHeadSpine>();
        _orderCreates =  TableManager.Instance.GetTable<TableOrderCreate>();
        
        _orderExtends =  TableManager.Instance.GetTable<TableOrderExtend>();
        
        _orderRandoms_2.Clear();
        TextAsset jsonRandom = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/order","orderRandom_2"));
        var tableOrderRandoms= TableManager.DeSerialize<TableOrderRandom>(jsonRandom.text);
        foreach (TableOrderRandom kv in tableOrderRandoms)
        {
            if (!_orderRandoms_2.ContainsKey(kv.payLevelGroup))
                _orderRandoms_2[kv.payLevelGroup] = new Dictionary<int, TableOrderRandom>();
            
            _orderRandoms_2[kv.payLevelGroup].Add(kv.GetID(), kv);
        }
        
        _orderRandoms.Clear();
        List<TableOrderRandom> tableData = TableManager.Instance.GetTable<TableOrderRandom>();
        foreach (TableOrderRandom kv in tableData)
        {
            if (!_orderRandoms.ContainsKey(kv.payLevelGroup))
                _orderRandoms[kv.payLevelGroup] = new Dictionary<int, TableOrderRandom>();
            
            _orderRandoms[kv.payLevelGroup].Add(kv.GetID(), kv);
        }
        
        _orderFilters_2.Clear();
        TextAsset filterRandom = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/order","orderFilter_2"));
        var tableOrderFilter= TableManager.DeSerialize<TableOrderFilter>(filterRandom.text);
        foreach (TableOrderFilter kv in tableOrderFilter)
        {
            _orderFilters_2.Add(kv.GetID(), kv);
        }
        
        _orderVaildAppends.Clear();
        _orderAppends.ForEach(a =>
        {
            if(a.unLockType > 0)
                _orderVaildAppends.Add(a);
        });
        InitTable(_orderFilters);
        InitTable(_orderItems);
        
        
        _headSpinesDic.Clear();
        _headSpineDic.Clear();

        foreach (var headSpine in _orderHeadSpines)
        {
            _headSpinesDic.Add(headSpine.id, headSpine);
            if (!_headSpineDic.ContainsKey(headSpine.type))
                _headSpineDic[headSpine.type] = new List<TableOrderHeadSpine>();
            
            _headSpineDic[headSpine.type].Add(headSpine);
        }
    }
    
    private void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if(config == null)
            return;
    
        List<T> tableData = TableManager.Instance.GetTable<T>();
        if(tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }

    public int GetItemPrice(int itemId)
    {
        if (!_orderItems.ContainsKey(itemId))
            return 0;

        return _orderItems[itemId].price;
    }

    public TableOrderRandom GetRandomConfig(int randomType)
    {
        if (GetRandomConfig().ContainsKey(randomType))
            return GetRandomConfig()[randomType];

        return null;
    }
    
    public TableOrderFilter GetOrderFilter(int filterId)
    {
        if (GetFilterConfig().ContainsKey(filterId))
            return GetFilterConfig()[filterId];

        return null;
    }

    public TableOrderItem GetOrderItem(int itemId)
    {
        if (_orderItems.ContainsKey(itemId))
            return _orderItems[itemId];

        return null;
    }
    
    
    public TableOrderHeadSpine GetHeadSpine(int id)
    {
        if (!_headSpinesDic.ContainsKey(id))
            return null;

        return _headSpinesDic[id];
    }

    public List<TableOrderHeadSpine> GetHeadSpines(SpineType type)
    {
        if (!_headSpineDic.ContainsKey((int)type))
            return null;

        return _headSpineDic[(int)type];
    }
    public string GetSpineName(int id, int type = 0)
    {
        var spineData = GetHeadSpine(id);
        if (spineData == null)
            return "";

        switch (type)
        {
            case 0:
                return spineData.spineName;
            case 1:
                return spineData.normalAnim;
            case 2:
                return spineData.happyAnim;
            default:
                return spineData.normalAnim;
        }
    }
    
    public TableOrderCreate GetOrderCreateConfig(int id)
    {
        return _orderCreates.Find(a => a.id == id);
    }

    public List<TableOrderFix> OrderFixConfigs
    {
        get
        {
            if (DifferenceManager.Instance.IsDiffPlan_C())
                return _orderFixs_3;

            if (DifferenceManager.Instance.IsOpenDifference)
                return _orderFixs_2;

            return _orderFixs;
        }
    }

    private Dictionary<int, TableOrderRandom> GetRandomConfig()
    {
        int group = PayLevelModel.Instance.GetCurPayLevelConfig().OrderGroupId;

        Dictionary<int, Dictionary<int, TableOrderRandom>> orderData;
        if (MainOrderManager.Instance.IsOpenOrderPlanB())
            orderData = _orderRandoms_2;
        else
            orderData = _orderRandoms;

        if (orderData.ContainsKey(group))
            return orderData[group];

        return orderData[orderData.Keys.First()];
    }
    
    
    private Dictionary<int, TableOrderFilter> GetFilterConfig()
    {
        if (MainOrderManager.Instance.IsOpenOrderPlanB())
            return _orderFilters_2;

        return _orderFilters;
    }
    
    public int GetOrderItemPrice(int itemId)
    {
        var config = GetOrderItemConfig(itemId);
        if (config == null)
            return 0;

        return config.price;
    }
    
    public bool IsOrderItem(int itemId)
    {
        return _orderItems.ContainsKey(itemId);
    }

    public TableOrderItem GetOrderItemConfig(int itemId)
    {
        if (!IsOrderItem(itemId))
            return null;

        return _orderItems[itemId];
    }
}