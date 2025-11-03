using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DragonU3DSDK;
using UnityEngine;

namespace Dlugin
{
    public static class Util
    {
        public static string[] DecodeUnityMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return new string[] { };
            else
                return msg.Split(Constants.kUnitySendMessageDelimiter);
        }

        public static string[] DecodeStructString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new string[] { };
            else
                return str.Split(Constants.kStructDelimiter);
        }

        public static string EncodeStructString(params string[] str)
        {
            DebugUtil.Log("EncodeStructString:{0}", str.ToStringEx("struct"));
            string ret = "";
            if (str != null)
                ret = string.Join("" + Constants.kStructDelimiter, str);
            DebugUtil.Log("After EncodeStructString:{0}", ret.Split(Constants.kStructDelimiter).ToStringEx("struct"));
            return ret;
        }

        /**
         * map a value in [0,1] to a value in [a,b]
         * 
         */
        public static int Float01MapToInteger(float value01, int a_include, int b_include)
        {
            if (b_include < a_include)
            {
                int temp = a_include;
                a_include = b_include;
                b_include = temp;
            }

            return Mathf.Min((int)Mathf.Ceil(value01 * (float)(b_include - a_include)) + a_include, b_include);
        }


        public static string RemoveEmojiFromString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return Regex.Replace(text, @"\p{Cs}", "");
        }
    }
}