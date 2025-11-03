using System;
using System.Reflection;
using System.Collections.Generic;
using IFix;

//1、配置类必须打[Configure]标签
//2、必须放Editor目录
[Configure]
public class IFixConfig
{
    static bool Match(string value, string[] set)
    {
        foreach (string item in set)
        {
            if (value.IndexOf(item) != -1)
            {
                return true;
            }
        }

        return false;
    }

    [IFix]
    static IEnumerable<Type> hotfix
    {
        get
        {
            List<Type> list = new List<Type>();
            string[] nameFilters = new string[]
            {
                "+<",                           // 匿名类
                "^Max",                         // 以Max开头的类
                "^com.",
                "^SRF.",
                "^Org.",
                "^SRDebugger.",
                "^BestHTTP.",
                "^PlatformSupport.",
                "^DragonU3DSDK.Storage.",
                "^DragonU3DSDK.Network.",
                "^DragonU3DSDK.Asset.",
                "^DragonU3DSDK.SDKEvents.",
                "^DragonPlus.Bi.",
                "^<PrivateImplementationDetails>+",
            };

            foreach (Type type in Assembly.Load("Assembly-CSharp").GetTypes())
            {
                string name = "^" + type.ToString() + "$";

                if (Match(name, nameFilters))
                {
                    continue;
                }

                list.Add(type);
            }

            UnityEngine.Debug.Log("IFixConfig Types " + list.Count + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Arrays.ToString(list.ToArray()));
            return list;
        }
    }

    [Filter]
    static bool Filter(System.Reflection.MethodInfo methodInfo)
    {
        return false;
    }
}