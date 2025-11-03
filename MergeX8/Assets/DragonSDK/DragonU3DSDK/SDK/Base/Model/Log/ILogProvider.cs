using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;

namespace Dlugin
{
    namespace PluginStructs
    {
        [System.Serializable]
        public class PARAMErrorReport
        {
            public string logFilePath;
            public string reason;
            public string[] stackTrace;
            public string gameVersion;
            public int urgentLevel;
            public string userId;
            public string userContact;
            public long utcTimeInSeconds;
            public string description;
        }
    }
    public abstract class ILogProvider : IServiceProvider
    {
        public abstract void SendLog(string logFilePath);
        public abstract void SendErrorReport(PARAMErrorReport report);
    }
}
