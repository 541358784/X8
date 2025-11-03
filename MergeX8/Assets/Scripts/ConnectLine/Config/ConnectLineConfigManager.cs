using System.Collections.Generic;
using System.IO;
using Decoration.DynamicMap;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using UnityEditor;
using UnityEngine;

namespace ConnectLine
{
    public class ConnectLineConfigManager : Manager<ConnectLineConfigManager>
    {
        public List<TableConnectLineLevel> _configs;
        
        public void InitTableConfigs()
        {
            TableManager.Instance.InitLocation("configs/ConnectLine");
        
            _configs = TableManager.Instance.GetTable<TableConnectLineLevel>();
        }
    }
}