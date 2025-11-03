using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using DG.Tweening;
using UnityEngine.Tilemaps;
using System;
using Deco.World;
using Deco.Node;
using StorageItem=DragonU3DSDK.Storage.Decoration.StorageItem;
namespace Deco.Item
{
    using TableItem=Decoration.TableItem;
    public enum ItemSourceType
    {
        Default = 0,
        Activity,    //1 活动获得
    }
    
    internal class DecoItemData
    {
        private DecoItem _item;

        internal TableItem _config = null;
        internal StorageItem _storage = null;
        private StorageItem _displayStorage = null;

        internal bool isOwned
        {
            get => _storage.Status == (int) DecoItem.Status.Owned;
        }

        internal bool IsActivityItem => _config.source == (int) ItemSourceType.Activity;
        internal bool IsNormalItem => _config.source == (int) ItemSourceType.Default;
        
        public DecoItemData(DecoItem item, TableItem config, StorageItem storage)
        {
            _item = item;
            _config = config;
            _storage = storage;
            _displayStorage = storage;
        }

        internal EActivityType GetActivityType()
        {
            if (IsActivityItem)
            {
                if (Enum.TryParse<EActivityType>(_config.sourceParam, out var eventType))
                {
                    return eventType;
                }
            }
            return EActivityType.NONE;
        }

        internal void unlock()
        {
            if ((int) Node.DecoNode.Status.Unlock != _storage.Status) _storage.Status = (int) Node.DecoNode.Status.Unlock;

            if (!IsActivityItem) //活动来源的建筑，默认不解锁，由活动获取, 默认来源的建筑，根据价格是否默认解锁
            {
                if (_config.price <= 0)
                {
                    buy();
                }
            }
        }

        internal void buy()
        {
            if ((int) Node.DecoNode.Status.Owned != _storage.Status)
            {
                _storage.Status = (int) Node.DecoNode.Status.Owned;
            }

            //EventDispatcher.Instance.DispatchEvent(new BaseEvent(EventEnum.DECORATION_UNLOCK_ITEM, _config.id));
        }
        internal void Lock()
        {
            if ((int) Node.DecoNode.Status.Lock != _storage.Status)
            {
                _storage.Status = (int) Node.DecoNode.Status.Lock;
            }

            //EventDispatcher.Instance.DispatchEvent(new BaseEvent(EventEnum.DECORATION_UNLOCK_ITEM, _config.id));
        }

        internal void DEBUG_SET_LOCKED()
        {
            _storage.Status = (int) Node.DecoNode.Status.Lock;
        }
    }
}