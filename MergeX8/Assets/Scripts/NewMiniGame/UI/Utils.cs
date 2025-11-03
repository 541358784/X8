using System;
using UnityEngine;

namespace Framework.UI
{
    public static class Utils
    {
        public static void SetLocalPositionZ(this Transform trans, float z)
        {
            trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, z);
        }

        public static void DestroyAllChildren(Transform rootTransform)
        {
            rootTransform?.gameObject.RemoveAllChildren();
        }

        public static long TotalSeconds()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static long TotalMilliseconds()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        public static void Reset(this Transform t)
        {
            if (t == null)
            {
                return;
            }

            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
        }

        /// <summary>合并Int</summary>
        public static long CombineInts(int high, int low)
        {
            // 将高位部分移动到高32位，然后与低位部分进行或运算
            return ((long)high << 32) | (uint)low;
        }

        public static (int high, int low) SplitLong(long combined)
        {
            // 提取高32位和低32位
            int high = (int)(combined >> 32);
            int low = (int)combined;
            return (high, low);
        }
    }
}