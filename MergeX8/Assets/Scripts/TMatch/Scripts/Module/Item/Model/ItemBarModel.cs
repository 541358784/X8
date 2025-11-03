using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;

namespace TMatch
{


    /// <summary>
    /// 资源栏模块
    /// </summary>
    public class ItemBarModel : Manager<ItemBarModel>
    {
        /// <summary>
        /// 资源栏样式
        /// </summary>
        public enum ItemBarStyle
        {
            /// <summary>
            /// 普通
            /// </summary>
            Normal,

            /// <summary>
            /// 右侧占位
            /// </summary>
            RightPlaceHold,

            /// <summary>
            /// 左侧占位
            /// </summary>
            LeftPlaceHold,

            /// <summary>
            /// 两侧都占位
            /// </summary>
            PlaceHold
        }

        /// <summary>
        /// 数据
        /// </summary>
        private class ItemBarData
        {
            /// <summary>
            /// 左1资源数据
            /// </summary>
            public ItemBar.Data Left1Data;

            /// <summary>
            /// 左2资源数据
            /// </summary>
            public ItemBar.Data Left2Data;

            /// <summary>
            /// 右资源数据
            /// </summary>
            public ItemBar.Data RightData;

            /// <summary>
            /// 风格
            /// </summary>
            public ItemBarStyle Style;

            /// <summary>
            /// 绑定界面
            /// </summary>
            public UIWindow Window;

            /// <summary>
            /// 绑定界面名
            /// </summary>
            public string WindowName;
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        private List<ItemBarData> datas = new List<ItemBarData>();

        /// <summary>
        /// 是否刷新
        /// </summary>
        private bool isRefresh;

        /// <summary>
        /// 是否播放动画
        /// </summary>
        private bool isPlayAnimation;

        /// <summary>
        /// 界面
        /// </summary>
        public UIItemTop UIItemTop;

        private void Update()
        {
            if (UIItemTop == null || !isRefresh) return;

            if (datas.Count == 0)
            {
                UIItemTop.RefreshItemBar(null, null, null, ItemBarStyle.Normal);
                return;
            }

            var data = datas[datas.Count - 1];
            UIItemTop.RefreshItemBar(data.Left1Data, data.Left2Data, data.RightData, data.Style);
            isRefresh = false;
        }

        /// <summary>
        /// 展示数据
        /// </summary>
        /// <param name="window">ui</param>
        /// <param name="left1">左1资源数据</param>
        /// <param name="left2">左2资源数据</param>
        /// <param name="right">右资源数据</param>
        /// <param name="style">风格</param>
        public void Show(UIWindow window, ItemBar.Data left1, ItemBar.Data left2, ItemBar.Data right,
            ItemBarStyle style = ItemBarStyle.Normal)
        {
            if (window == null)
            {
                DebugUtil.LogError("Show item bar failed! window is null!");
                return;
            }

            datas.Add(new ItemBarData()
            {
                Window = window,
                WindowName = window.name,
                Left1Data = left1,
                Left2Data = left2,
                RightData = right,
                Style = style,
            });
            isRefresh = true;

            if (UIItemTop == null)
            {
                UIItemTop.Open();
            }

            Update(); //注：手动调一下是因为时序不对，有可能道具飞行的时候没有注册飞的节点。
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="window">ui</param>
        public void Hide(UIWindow window)
        {
            for (int i = datas.Count - 1; i >= 0; i--)
            {
                if (datas[i].Window == window || datas[i].Window == null)
                {
                    if (datas[i].Window == null)
                    {
                        DebugUtil.LogError("Using item bar window is null, Window name: " + datas[i].WindowName);
                    }

                    datas.RemoveAt(i);
                }
            }

            isRefresh = true;
        }
    }
}
