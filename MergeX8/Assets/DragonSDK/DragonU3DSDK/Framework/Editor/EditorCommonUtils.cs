/*-------------------------------------------------------------------------------------------
// Copyright (C) 2019 北京，天龙互娱
//
// 模块名：EditorCommonUtils
// 创建日期：2020-4-9
// 创建者：guomeng.lu
// 模块描述：编辑器相关通用方法
//-------------------------------------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System;

namespace DragonU3DSDK
{
    public class EditorCommonUtils
    {
        public static readonly MD5CryptoServiceProvider md5Crypto = new MD5CryptoServiceProvider();

        public static bool IsPOT(float f, float p)
        {
            float l = Mathf.Log(f, p);
            return l.Equals(Mathf.RoundToInt(l));
        }

        public static float ToLargerPOT(float f, float p)
        {
            int l = Mathf.CeilToInt(Mathf.Log(f, p));
            return Mathf.Pow(p, l);
        }

        public static string TrimStart(string source, string trimStr)
        {
            if (source.IndexOf(trimStr) == 0)
            {
                source = source.Substring(trimStr.Length);
            }
            return source;
        }

        public static string TrimEnd(string source, string trimStr)
        {
            int length = Mathf.Abs(source.Length - trimStr.Length);
            if (source.LastIndexOf(trimStr) == length)
            {
                source = source.Substring(0, length);
            }
            return source;
        }

        public static string TrimExtension(string fileName)
        {
            return TrimEnd(fileName, GetExtension(fileName));
        }

        public static string SameStart(string a, string b)
        {
            int maxLength = Mathf.Min(a.Length, b.Length);
            int index;
            for (index = 0; index < maxLength; index++)
            {
                if (a[index] != b[index])
                {
                    break;
                }
            }

            return a.Substring(0, index);
        }

        public static string SameEnd(string a, string b)
        {
            int maxLength = Mathf.Min(a.Length, b.Length);
            int index;
            for (index = 0; index < maxLength; index++)
            {
                if (a[a.Length - index - 1] != b[b.Length - index - 1])
                {
                    break;
                }
            }

            return a.Substring(a.Length - index);
        }

        public static List<T2> Select<T1, T2>(IEnumerable<T1> enumerable, string regexName, System.Func<T1, string> getter, System.Func<T1, T2> setter)
        {
            List<T2> result = new List<T2>();
            foreach (T1 item in enumerable)
            {
                if (Regex.IsMatch(getter(item), regexName, RegexOptions.IgnoreCase))
                {
                    result.Add(setter(item));
                }
            }
            return result;
        }

        public static List<T> Select<T>(IEnumerable<T> enumerable, string regexName, System.Func<T, string> getter)
        {
            return Select(enumerable, regexName, getter, (a) => a);
        }

        public static List<string> Select(IEnumerable<string> enumerable, string regexName)
        {
            return Select(enumerable, regexName, (a) => a, (a) => a);
        }

        public static byte[] ReadBytes(string name)
        {
            try
            {
                return File.ReadAllBytes(name);
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Read bytes failed : " + name + "\n" + e);
                return null;
            }
        }

        public static Texture2D ReadTexture(string name, TextureFormat format = TextureFormat.ARGB32)
        {
            Texture2D tex = null;
            byte[] bytes = ReadBytes(name);
            if (bytes != null)
            {
                tex = new Texture2D(0, 0, format, false);
                tex.LoadImage(bytes);
            }
            return tex;
        }

        public static bool WriteBytes(string name, byte[] bytes)
        {
            try
            {
                CreateDirectory(Path.GetDirectoryName(name));
                File.WriteAllBytes(name, bytes);
                return true;
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Write bytes failed : " + name + "\n" + e);
                return false;
            }
        }

        public static bool WriteText(string name, string text)
        {
            try
            {
                CreateDirectory(Path.GetDirectoryName(name));
                File.WriteAllText(name, text);
                return true;
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Write text failed : " + name + "\n" + e);
                return false;
            }
        }

        public static bool DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Delete file failed : " + path + "\n" + e);
                return false;
            }
        }

        public static bool CreateDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return true;
            }

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return true;
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Create directory failed : " + path + "\n" + e);
                return false;
            }
        }

        public static bool MoveDirectory(string srcPath, string destPath)
        {
            if (srcPath == destPath)
            {
                return true;
            }

            try
            {
                srcPath = NormalizeDirectory(srcPath);
                destPath = NormalizeDirectory(destPath);
                Directory.CreateDirectory(destPath);
                string[] dirs = Directory.GetDirectories(srcPath, "*", SearchOption.AllDirectories);
                string[] files = Directory.GetFiles(srcPath, "*", SearchOption.AllDirectories);
                for (int i = 0; i < dirs.Length; i++)
                {
                    string name = dirs[i].Substring(srcPath.Length);
                    Directory.CreateDirectory(destPath + name);
                }
                for (int i = 0; i < files.Length; i++)
                {
                    string name = files[i].Substring(srcPath.Length);
                    MoveFile(files[i], destPath + name);
                }
                Directory.Delete(srcPath, true);
                return true;
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Move directory failed : ", srcPath, " --> ", destPath, "\n", e);
                return false;
            }
        }

        public static bool DeleteDirectory(string path, bool recursive = false)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    System.Threading.Thread.Sleep(100);
                    Directory.Delete(path, recursive);
                    System.Threading.Thread.Sleep(200);
                }
                return true;
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Delete directory failed : " + path + "\n" + e);
                return false;
            }
        }

        public static bool CopyFile(string srcPath, string destPath)
        {
            try
            {
                CreateDirectory(Path.GetDirectoryName(destPath));
                File.Copy(srcPath, destPath, true);
                return true;
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Copy file failed : " + srcPath + " --> " + destPath + "\n" + e);
                return false;
            }
        }

        public static bool MoveFile(string srcPath, string destPath)
        {
            if (srcPath == destPath)
            {
                return true;
            }

            try
            {
                CreateDirectory(Path.GetDirectoryName(destPath));
                if (File.Exists(srcPath))
                {
                    File.Delete(destPath);
                    File.Move(srcPath, destPath);
                }
                return true;
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Move file failed : " + srcPath + " --> " + destPath + "\n" + e);
                return false;
            }
        }

        public static string GetDirectoryName(string path)
        {
            return NormalizePath(Path.GetDirectoryName(path));
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public static string NormalizePath(string path)
        {
            return path.Replace('\\', '/');
        }

        public static string NormalizeDirectory(string path)
        {
            path = NormalizePath(path);
            if (path != "" && !path.EndsWith("/"))
            {
                path += "/";
            }
            return path;
        }

        public static string GetCurrentDirectory()
        {
            try
            {
                return NormalizeDirectory(Directory.GetCurrentDirectory());
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("Get current directory failed" + "\n" + e);
                return "";
            }
        }

        public static string GetExtension(string name)
        {
            string suffix = "";

            int index = name.LastIndexOf(".");
            if (index != -1)
            {
                suffix = name.Substring(index);
            }

            return suffix;
        }

        public static string GetFileMD5(string name)
        {
            try
            {
                using (Stream stream = File.OpenRead(name))
                {
                    byte[] md5Bytes = md5Crypto.ComputeHash(stream);
                    string md5 = BitConverter.ToString(md5Bytes);
                    return md5.Replace("-", "").ToLower();
                }
            }
            catch
            {
                return "";
            }
        }

        // 获取对象的属性
        public static Dictionary<string, object> GetValues(object o, Dictionary<string, object> dict, string prefix = "")
        {
            Type type = o.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            FieldInfo[] fieldInfos = type.GetFields(bindingFlags);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];
                string key = !string.IsNullOrEmpty(prefix) ? prefix + fieldInfo.Name : fieldInfo.Name;
                dict.Add(key, fieldInfo.GetValue(o));
            }

            PropertyInfo[] propertyInfos = type.GetProperties(bindingFlags);
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                string key = !string.IsNullOrEmpty(prefix) ? prefix + propertyInfo.Name : propertyInfo.Name;
                dict.Add(key, propertyInfo.GetValue(o, null));
            }

            return dict;
        }

        // 获取对象的属性
        public static Dictionary<string, object> GetValues(object o)
        {
            return GetValues(o, new Dictionary<string, object>());
        }

        public static string ToString<T>(T[] array)
        {
            string separator = ", ";
            string s = "[";
            for (int i = 0; i < array.Length; i++)
            {
                s += (i > 0 ? separator : "") + array[i];
            }
            s += "]";
            return s;
        }

        public static string ToString(object[] array)
        {
            return ToString<object>(array);
        }

        public static void StartProcess(string name, string arguments = "")
        {
            Process process = new Process();
            process.StartInfo.FileName = name;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }
    }
}