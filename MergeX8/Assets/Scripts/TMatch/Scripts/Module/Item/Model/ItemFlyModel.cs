using System;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;

namespace TMatch
{



    /// <summary>
    /// 物品动画模块
    /// </summary>
    public class ItemFlyModel : Manager<ItemFlyModel>
    {
        public enum ItemFlyType
        {
            Default,
            PiggyBank,
            PlayOrginImage, //飞原始图片
        }

        public class ItemFlyData
        {
            public Transform Transform;
            public Action<int, ulong> Callback;
            public Action OnArrive;
        }

        private readonly Dictionary<int, List<ItemFlyData>> flyTo = new Dictionary<int, List<ItemFlyData>>();
        private readonly Dictionary<int, List<ItemFlyData>> flyOut = new Dictionary<int, List<ItemFlyData>>();

        public UIItemTop UIItemTop;

        private bool TryAdd(Dictionary<int, List<ItemFlyData>> dict, int id, Transform transform,
            Action<int, ulong> callback, Action onArrive = null)
        {
            if (!dict.ContainsKey(id)) dict.Add(id, new List<ItemFlyData>());
            if (dict[id].Find(x => x.Transform == transform && x.Callback == callback) != null) return false;
            dict[id].Add(new ItemFlyData() {Transform = transform, Callback = callback, OnArrive = onArrive});
            return true;
        }

        private bool TryRemove(Dictionary<int, List<ItemFlyData>> dict, int id, Transform transform,
            Action<int, ulong> callback)
        {
            if (!dict.ContainsKey(id)) return false;
            foreach (ItemFlyData data in dict[id])
            {
                if (data.Transform == transform && data.Callback == callback)
                {
                    dict[id].Remove(data);
                    return true;
                }
            }

            return false;
        }

        private ItemFlyData FindRegisterFlyData(Dictionary<int, List<ItemFlyData>> dict, int id)
        {
            if (!dict.ContainsKey(id) || dict[id].Count == 0) return null;
            return dict[id][dict[id].Count - 1];
        }

        /// <summary>
        /// 注册物品飞去
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="transform">位置</param>
        /// <param name="callback">回调</param>
        /// <param name="onArrive">到达回调</param>
        public void RegisterFlyTo(int id, Transform transform, Action<int, ulong> callback = null,
            Action onArrive = null)
        {
            if (!TryAdd(flyTo, id, transform, callback, onArrive))
                DebugUtil.LogWarning("Register item fly to failed! Cause transform already registered!");
        }

        /// <summary>
        /// 解注册物品飞去
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="transform">位置</param>
        /// <param name="callback">回调</param>
        public void UnregisterFlyTo(int id, Transform transform, Action<int, ulong> callback = null)
        {
            if (!TryRemove(flyTo, id, transform, callback))
                DebugUtil.LogWarning("Unregister item fly to failed! Cause transform not registered!");
        }

        /// <summary>
        /// 注册物品飞出
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="transform">位置</param>
        /// <param name="callback">回调</param>
        /// <param name="onArrive">到达回调</param>
        public void RegisterFlyOut(int id, Transform transform, Action<int, ulong> callback = null,
            Action onArrive = null)
        {
            if (!TryAdd(flyOut, id, transform, callback))
                DebugUtil.LogWarning("Register item fly out failed! Cause transform already registered!");
        }

        /// <summary>
        /// 解注册物品飞出
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="transform">位置</param>
        /// <param name="callback">回调</param>
        public void UnregisterFlyOut(int id, Transform transform, Action<int, ulong> callback = null)
        {
            if (!TryRemove(flyOut, id, transform, callback))
                DebugUtil.LogWarning("Unregister item fly out failed! Cause transform not registered!");
        }

        /// <summary>
        /// 播放物品增加动画
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="count">物品数量</param>
        /// <param name="from">位置</param>
        /// <param name="onComplete">完成回调</param>
        public void PlayItemAdd(int id, int count, Transform from, Action onComplete = null,
            ItemFlyType flyType = ItemFlyType.Default, bool useEffect = true, bool useAudio = true)
        {
            var version = UIItemTop.Version;

            if (UIItemTop == null)
            {
                DebugUtil.LogWarning("Play item add fly animation failed! Cause UIItemTop not prepare!");
                onComplete?.Invoke();
                return;
            }

            if (from == null)
            {
                DebugUtil.LogWarning("Play item add fly animation failed! Cause from is null!");
                onComplete?.Invoke();
                return;
            }

            ItemFlyData data = FindRegisterFlyData(flyTo, id);
            if (data == null)
            {
                DebugUtil.LogWarning("Play item add fly animation failed! Cause no transform registered!");
                onComplete?.Invoke();
                return;
            }

            Action callback = () =>
            {
                UIItemTop.PlayItemFlyAnimation(id, count, from, data.Transform, data.OnArrive, () =>
                {
                    // 在飞行过程中，有可能被反注册了
                    data = FindRegisterFlyData(flyTo, id);
                    data?.Callback?.Invoke(count, version);
                    onComplete?.Invoke();
                }, flyType, useEffect, useAudio);
            };

            switch ((ResourceId) id)
            {
                // case ResourceId.Booster1:
                // case ResourceId.Booster2:
                // case ResourceId.Booster3:
                //     UIItemTop.PlayBoosterAnimation(true, callback);
                //     break;
                default:
                    callback();
                    break;
            }
        }

        /// <summary>
        /// 播放物品增加动画
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="count">物品数量</param>
        /// <param name="from">位置</param>
        /// <param name="onComplete">完成回调</param>
        public void PlayItemAdd(List<int> ids, List<int> nums, Transform from, Action onComplete = null,
            ItemFlyType flyType = ItemFlyType.Default, bool useEffect = true, bool useAudio = true)
        {
            var flyCount = ids.Count;
            for (var i = 0; i < ids.Count; i++)
            {
                PlayItemAdd(ids[i], nums[i], from, () =>
                {
                    flyCount--;
                    if (flyCount > 0)
                        return;
                    onComplete?.Invoke();
                }, flyType, useEffect, useAudio);
            }
        }

        // /// <summary>
        // /// 播放物品消耗动画
        // /// </summary>
        // /// <param name="id">物品id</param>
        // /// <param name="count">物品数量</param>
        // /// <param name="to">位置</param>
        // /// <param name="onComplete">完成回调</param>
        // /// <param name="onArrive">到达回调</param>
        // public void PlayItemCost(int id, int count, Transform to, Action onComplete = null)
        // {
        //     if (UIItemTop == null)
        //     {
        //         DebugUtil.LogWarning("Play item cost fly animation failed! Cause UI not prepare!");
        //         onComplete?.Invoke();
        //         return;
        //     }
        //
        //     ItemFlyData data = FindRegisterFlyData(flyOut, id);
        //     if (data == null)
        //     {
        //         DebugUtil.LogWarning("Play item cost fly animation failed! Cause no transform registered!");
        //         onComplete?.Invoke();
        //         return;
        //     }
        //
        //     UIItemTop.PlayItemFlyAnimation(id, count, data.Transform, to, data.OnArrive, onComplete);
        //     data.Callback?.Invoke(-count, UIItemTop.Version);
        // }
    }
}