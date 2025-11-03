using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;

namespace Dlugin
{
    public class GroupLogProvider : GroupServiceProvider
    {
        public void SendLog(string logFilePath)
        {
            foreach(IServiceProvider provider in m_AllService)
            {
                ILogProvider iLog = provider as ILogProvider;
                iLog.SendLog(logFilePath);
            }
        }
        public void SendErrorReport(PARAMErrorReport report)
        {
            foreach (IServiceProvider provider in m_AllService)
            {
                ILogProvider iLog = provider as ILogProvider;
                iLog.SendErrorReport(report);
            }
        }
    }
}
