using System;
using System.Collections.Generic;
using System.Text;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;

namespace Framework
{
    public static class StringUtils
    {
        public static bool ParsePosAndRot(string str, out Vector3 position, out Quaternion rotation, out float fov)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var strs = str.Split(';');

                    if (strs != null && strs.Length == 3)
                    {
                        position = StringToVector3(strs[0]);
                        rotation = Quaternion.Euler(StringToVector3(strs[1]));
                        fov = float.Parse(strs[2]);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            position = Vector3.one;
            rotation = Quaternion.identity;
            fov = 55;
            return false;
        }

        /// <summary>
        /// 字符串转Vector3
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Vector3 StringToVector3(string str)
        {
            try
            {
                string[] arrPosition = str.Split(',');
                float x = float.Parse(arrPosition[0]);
                float y = float.Parse(arrPosition[1]);
                float z = float.Parse(arrPosition[2]);

                Vector3 Position = new Vector3(x, y, z);
                return Position;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            return Vector3.zero;
        }

        public static int StringToInt(string x)
        {
            try
            {
                return Convert.ToInt32(x);
            }
            catch (Exception ex)
            {
                DragonU3DSDK.DebugUtil.LogError(ex);
                return 0;
            }
        }

        public static uint StringToUInt(string x)
        {
            try
            {
                return Convert.ToUInt32(x);
            }
            catch (Exception ex)
            {
                DragonU3DSDK.DebugUtil.LogError(ex);
                return 0;
            }
        }

        /// <summary>
        /// "1,2,3"字符串转int list
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<int> StringToIntList(string str)
        {
            var result = new List<int>();
            try
            {
                string[] arr = str.Split(',');
                if (arr != null)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        result.Add(StringToInt(arr[i]));
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            return result;
        }

        public static Color StringToColor(string xBase16)
        {
            try
            {
                var num = Convert.ToInt32(xBase16, 16);
                var b = num % 256;
                num = num >> 8;
                var g = num % 256;
                num = num >> 8;
                var r = num;
                return new Color((float) r / 255, (float) g / 255, (float) b / 255);
            }
            catch (Exception ex)
            {
                DragonU3DSDK.DebugUtil.LogError(ex);
                return Color.white;
            }
        }

        public static string UPPER_UNDERLINE_2_UpperCamelCase(string UPPER_UNDERLINE_STRING)
        {
            if (string.IsNullOrEmpty(UPPER_UNDERLINE_STRING))
            {
                return UPPER_UNDERLINE_STRING;
            }

            var words = UPPER_UNDERLINE_STRING.Split('_');

            if (words != null)
            {
                var sb = new StringBuilder();
                foreach (var word in words)
                {
                    var sb1 = new StringBuilder(word.ToLower());
                    if (word.Length > 0)
                    {
                        var c = sb1[0];
                        var C = char.ToUpper(c);
                        sb1.Replace(c, C, 0, 1);
                    }

                    sb.Append(sb1);
                }

                return sb.ToString();
            }
            else
            {
                return UPPER_UNDERLINE_STRING;
            }
        }

        public static string GetDummyString(int length)
        {
            return new string(' ', length);
        }
    }
}