using UnityEngine.UI;

namespace OneLine
{
    /// <summary>
    /// 一笔画的界面接口
    /// </summary>
    public interface IOneLineView
    {
        /// <summary>
        /// 绘制图形的RawImage
        /// </summary>
        RawImage DrawOnImage { get; }

        /// <summary>
        /// 当游戏开始
        /// </summary>
        void OnStart(OneLineGame game);

        /// <summary>
        /// 开始绘制
        /// </summary>
        void OnBeginDraw(OneLineGraphic.Point startPoint);

        /// <summary>
        /// 当绘制
        /// </summary>
        /// <param name="drawingPixel"></param>
        /// <param name="completeProgress">绘制进度</param>
        void OnDraw(Pixel drawingPixel, float completeProgress);

        /// <summary>
        /// 当成功
        /// </summary>
        void OnSuccess();

        /// <summary>
        /// 当失败
        /// </summary>
        void OnFailed(bool moveFlag);

        /// <summary>
        /// 当重置
        /// </summary>
        void OnReset();
    }
}