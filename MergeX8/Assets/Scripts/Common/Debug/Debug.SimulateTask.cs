using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DragonU3DSDK.Storage;
using Merge.Order;
using Newtonsoft.Json;
using SRF;
using UnityEngine;
public partial class SROptions
{
    private const string SimulateTask = "模拟生成任务";

    
    [Category(SimulateTask)]
    [DisplayName("模拟任务数")]
    public int SimulateTaskCount
    {
        get
        {
            return SimulateTaskManager.Instance.SimulateTaskCount;
        }
        set
        {
            SimulateTaskManager.Instance.SimulateTaskCount = value;
        }
    }
    
    [Category(SimulateTask)]
    [DisplayName("模拟次数")]
    public int SimulateTimes
    {
        get
        {
            return SimulateTaskManager.Instance.SimulateTimes;
        }
        set
        {
            SimulateTaskManager.Instance.SimulateTimes = value;
        }
    }
    
    [Category(SimulateTask)]
    [DisplayName("生成Json")]
    public void CreateJsonFile()
    {
        SimulateTaskManager.Instance.SimulateAndSave();
    }
}
public class SimulateTaskManager:Manager<SimulateTaskManager>
{
    // public class CurrentClass
    // {
    //     private List<SimpleTaskStruct> value;
    //     public CurrentClass(List<SimpleTaskStruct> inValue)
    //     {
    //         value = inValue;
    //     }
    // }
    [System.Serializable]
    public class SimpleTaskStruct
    {
        public string OrgId;
        public string RewardTypes;
        public string RewardNums;
        public string ItemIds;
        public SimpleTaskStruct(StorageTaskItem s)
        {
            OrgId = s.OrgId.ToString();
            RewardTypes = GetString(s.RewardTypes);
            RewardNums = GetString(s.RewardNums);
            ItemIds = GetString(s.ItemIds);
        }

        public string GetString<T>(List<T> list)
        {
            var result = "";
            for (var i=0;i<list.Count;i++)
            {
                var element = list[i];
                result += element;
                if (i < list.Count - 1)
                {
                    result += ",";
                }
            }
            return result;
        }
    }
    public void SimulateAndSave()
    {
        // var simulateTaskTimes = 1000;
        // var allTasks = new List<List<StorageTaskItem>>();
        // for (var i = 0; i < simulateTaskTimes; i++)
        // {
        //     allTasks.Add(SimulateOnce());
        //     
        //     JsonUtility.ToJson()
        //     allTasks[0][0].OrgId//id
        //     allTasks[0][0].RewardTypes
        //     allTasks[0][0].RewardNums
        //     allTasks[0][0].ItemIds
        // }
        List<SimpleTaskStruct> simpleTaskList = new List<SimpleTaskStruct>();
        for (var i = 0; i < SimulateTimes; i++)
        {
            foreach (var task in SimulateOnce())
            {
                simpleTaskList.Add(new SimpleTaskStruct(task));
            }
        }
        var text = JsonConvert.SerializeObject(simpleTaskList);
        var index = 0;
        string filePath = Path.Combine(Application.persistentDataPath, "TaskSimulateLogOut"+index+".txt");
        while (File.Exists(filePath))
        {
            index++;
            filePath = Path.Combine(Application.persistentDataPath, "TaskSimulateLogOut"+index+".txt");
        }
        // 写入文件内容
        File.WriteAllText(filePath, text);
        Debug.Log("File written to: " + filePath);
        // 读取文件内容
        string readContent = File.ReadAllText(filePath);
        Debug.Log("File content: " + readContent);
    }
    public int SimulateTimes = 5;
    public int SimulateTaskCount = 200;
    public List<StorageTaskItem> SimulateOnce()
    {
        // var simulateTaskCount = SimulateTaskCount;
        // var DynamicNormalIndex = MainOrderManager.Instance.StorageTaskGroup.DynamicNormalIndex;
        // var DynamicNormalId = MainOrderManager.Instance.StorageTaskGroup.DynamicNormalId;
        // var curTaskList = new StorageList<StorageTaskItem>();
        // for (var i=0;i< MainOrderManager.Instance.CurTaskList.Count;i++)
        // {
        //     var curTask = MainOrderManager.Instance.CurTaskList[i];
        //     curTaskList.Add(curTask);
        //     if (curTask.Type == (int) MainOrderManager.TaskType.Special ||
        //         curTask.Type == (int) MainOrderManager.TaskType.Seal)
        //     {
        //         MainOrderManager.Instance.CurTaskList.RemoveAt(i);
        //         i--;   
        //     }
        // }
        // var taskList = new List<StorageTaskItem>();
        // while (taskList.Count < simulateTaskCount)
        // {
        //     var taskStorage = FindTask();
        //     List<StorageTaskItem> listTasks = null;
        //     Dictionary<int, int> demands = MainOrderManager.Instance.GetTaskDemands(taskStorage);
        //     if (taskStorage.Type == (int)MainOrderManager.TaskType.Readily)
        //     {
        //         Dictionary<int, int> copyData = null;
        //         if(demands != null)
        //             copyData = new Dictionary<int, int>(demands);
        //         listTasks = MainOrderManager.Instance.CompleteReadilyTask(taskStorage, copyData);
        //     }
        //     else
        //     {
        //         listTasks = MainOrderManager.Instance.CompleteTask(taskStorage);
        //     }
        //     if (listTasks != null)
        //     {
        //         taskList.AddRange(listTasks);   
        //     }
        // }
        // while (taskList.Count > simulateTaskCount)
        // {
        //     taskList.RemoveRange(simulateTaskCount,taskList.Count - simulateTaskCount);
        // }
        // MainOrderManager.Instance.StorageTaskGroup.DynamicNormalIndex = DynamicNormalIndex;
        // TaskModuleManager.Instance.StorageTaskGroup.DynamicNormalId = DynamicNormalId;
        // TaskModuleManager.Instance.CurTaskList.Clear();
        // foreach (var task in curTaskList)
        // {
        //     TaskModuleManager.Instance.CurTaskList.Add(task);
        // }
        //return taskList;
        return null;
    }
    // public StorageTaskItem FindTask()
    // {
    //     return TaskModuleManager.Instance.CurTaskList.Random();
    // }
}