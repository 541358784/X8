using UnityEngine;

namespace OneLine
{
    /// <summary>
    /// 一笔画游戏创建所需数据
    /// </summary>
    public struct OneLineOrder
    {
        /// <summary>
        /// 关卡图像路径点等数据
        /// </summary>
        public OneLineGraphic Graphic;

        /// <summary>
        /// 关卡模板图像
        /// </summary>
        public Texture2D Template;

        /// <summary>
        /// 模板线条颜色
        /// </summary>
        public Color TemplateColor;

        /// <summary>
        /// 绘制的线条颜色
        /// </summary>
        public Color DrawColor;

        /// <summary>
        /// 成功时线条颜色
        /// </summary>
        public Color SuccessColor;

        /// <summary>
        /// 失败时线条颜色
        /// </summary>
        public Color FailedColor;

        /// <summary>
        /// 自动吸附到点的距离
        /// </summary>
        public float AdsorbToPointDistance;

        /// <summary>
        /// UI渲染相机
        /// </summary>
        public Camera UICamera;
    }
}