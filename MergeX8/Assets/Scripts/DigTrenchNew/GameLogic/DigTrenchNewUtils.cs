using UnityEngine;

namespace DigTrenchNew
{
    public enum DigTrenchNewPixelState
    {
        Default=0,
        None=1,
        Cover=2,
        Water=3,
        Win=4,
        Game=5,
        Error=6,
    }
    public static class DigTrenchNewUtils
    {
        public static void GetPixelAtWorldPosition(this SpriteRenderer renderer, Vector3 worldPosition,out int x,out int y)
        {
            // 获取 Sprite 的纹理
            Texture2D texture = renderer.sprite.texture;

            // 将世界坐标转换为 Sprite 的局部坐标
            Vector3 localPosition = renderer.transform.InverseTransformPoint(worldPosition);

            // 获取 Sprite 的边界
            Bounds bounds = renderer.bounds;
            Vector2 spriteSize = bounds.size;

            // 计算 UV 坐标
            Vector2 uv = new Vector2(
                (localPosition.x + spriteSize.x / 2) / spriteSize.x,
                (localPosition.y + spriteSize.y / 2) / spriteSize.y
            );

            // 将 UV 坐标映射到纹理的像素坐标
            int pixelX = Mathf.FloorToInt(uv.x * texture.width);
            int pixelY = Mathf.FloorToInt(uv.y * texture.height);
            x = pixelX;
            y = pixelY;
        }
    }
}