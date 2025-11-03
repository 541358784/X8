using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PathFile
{
    public Dictionary<string, ResProp> DicPathFile = new Dictionary<string, ResProp>(); //新打包方式打出的路径
    public Dictionary<string, string> DicPathFileOther = new Dictionary<string, string>(); //老的打包方式打出的路径
}

public class ResProp
{
    public string BatchName;
    public string path;
}