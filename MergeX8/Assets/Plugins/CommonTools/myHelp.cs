using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class myHelp
{
    /// <summary>
    /// 克隆对象放进指定父中，设置坐标和旋转都比例都为0，比例都为1
    /// </summary>
    /// <param name="CloneTarget">要克隆的对象</param>
    /// <param name="parent">要放入的父对象</param>
    /// <returns></returns>
    public static GameObject CloneAndSetParent(GameObject CloneTarget, GameObject parent)
    {
        if (CloneTarget==null)
        {
            Debug.LogError("CloneTarget为空");
            return null;
        }
        
        if (parent==null)
        {
            Debug.LogError("parent为空");
            return null;
        }
        
        GameObject obj = Object.Instantiate(CloneTarget,parent.transform,false) as GameObject;
        return obj;
    }
    
    /// <summary>
    /// 克隆一个对象后反回克隆出的子对象，并且放进指定的父中，设置本地坐标和比例和旋转
    /// </summary>
    /// <param name="CloneTarget">要克隆的对象</param>
    /// <param name="parent">要放入的父对象</param>
    /// <param name="position">本地坐标</param>
    /// <param name="Q">本地旋转</param>
    /// <param name="scale">本地比例</param>
    /// <returns></returns>
    public static GameObject CloneAndSetParent(GameObject CloneTarget, GameObject parent, Vector3 position, Quaternion Q, Vector3 scale)
    {
        GameObject G = Object.Instantiate(CloneTarget, position, Q) as GameObject;
        G.transform.SetParent(parent.transform);
        G.transform.localScale = scale;
        G.transform.localPosition = position;
        return G;
    }
    
    /// <summary>
    /// 清理一个对象下的所有子对象
    /// </summary>
    /// <param name="obj"></param>
    public static void ClearChildsImmediate(GameObject obj)
    {
        if (obj==null)
        {
            return;
        }
        
        int count = obj.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Object.DestroyImmediate(obj.transform.GetChild(0).gameObject);
        }
    }
    
    public static List<string> GetAllFile(string sSourcePath)
    {
        List <String> list = new List<string>();
        if (string.IsNullOrEmpty(sSourcePath) || !Directory.Exists(sSourcePath))
        {
            Debug.Log("目标路径不对");
            return list;
        }
        
        DirectoryInfo theFolder = new DirectoryInfo(sSourcePath);
        FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);
        foreach (FileInfo NextFile in thefileInfo)  //遍历文件
        {
            if (!NextFile.FullName.Contains(".DS_Store") && !NextFile.FullName.Contains(".meta"))
            {
                list.Add(NextFile.FullName);
            }
        }

        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        foreach (DirectoryInfo NextFolder in dirInfo)
        {
            FileInfo[] fileInfo = NextFolder.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo NextFile in fileInfo)  //遍历文件
            {
                if (!NextFile.FullName.Contains(".DS_Store") && !NextFile.FullName.Contains(".meta"))
                {
                    list.Add(NextFile.FullName);
                }
            }
        }
        return list;
    }
    
    public static List<string> GetAllDirectory(string sSourcePath)
    {
        List <String> list = new List<string>();
        if (string.IsNullOrEmpty(sSourcePath) || !Directory.Exists(sSourcePath))
        {
            Debug.Log("目标路径不对");
            return list;
        }
        
        DirectoryInfo theFolder = new DirectoryInfo(sSourcePath);

        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        foreach (DirectoryInfo NextFolder in dirInfo)
        {
            list.Add(NextFolder.FullName);
        }
        return list;
    }
    
    public static void CreateFile(string path, string filename, string info)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        StreamWriter sw;
        FileInfo t = new FileInfo(path + "/" + filename.ToLower());
        sw = t.CreateText();
        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();
    }

    
    public static string FindAndGetFilePath(string fileName,string sSourcePath)
    {
        if (string.IsNullOrEmpty(sSourcePath) || !Directory.Exists(sSourcePath) || string.IsNullOrEmpty(fileName))
        {
            Debug.Log("目标路径不对");
            return "";
        }
        
        DirectoryInfo theFolder = new DirectoryInfo(sSourcePath);
        FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);
        foreach (FileInfo NextFile in thefileInfo)  //遍历文件
        {
            if (string.Equals(NextFile.Name,fileName))
            {
                return (NextFile.FullName);
            }
        }

        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        foreach (DirectoryInfo NextFolder in dirInfo)
        {
            FileInfo[] fileInfo = NextFolder.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo NextFile in fileInfo)  //遍历文件
            {
                if (string.Equals(NextFile.Name,fileName))
                {
                    return (NextFile.FullName);
                }
            }
        }

        return "";
    }

    public static string GetAssetRelativePathFormFullPath(string fullPath)
    {
        fullPath = fullPath.Replace($@"\", "/");
        return fullPath.Replace(Application.dataPath+"/", "");
    }
}
