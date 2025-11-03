using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
public partial class DropBallGame
{
    public class DataUnit
    {
        public float BallMinX{ get; set; }
        public float BallMaxX{ get; set; }
        // public float LuckyX{ get; set; }
        public int PlayCount{ get; set; }
        public int FreezeCount{ get; set; }
        public int LuckyCount{ get; set; }
        public List<int> ResultCount{ get; set; }
        public float LuckyPercent{ get; set; }
        public List<float> ResultPercent{ get; set; }
        public DataUnit(float ballMinX,float ballMaxX)
        {
            BallMinX = Math.Min(ballMinX,ballMaxX);
            BallMaxX = Math.Max(ballMinX,ballMaxX);
            // LuckyX = luckyX;
            PlayCount = 0;
            FreezeCount = 0;
            LuckyCount = 0;
            ResultCount = new List<int>(){0,0,0,0,0,0,0};
            LuckyPercent = 0f;
            ResultPercent = new List<float>() {0, 0, 0, 0, 0, 0, 0};
        }
    }
    public int CollectCount = 1000;
    // public float LuckyMoveStep = 100f;
    private bool IsInDataCollect = false;
    
    private SortedDictionary<string, DataUnit> DataMap = new SortedDictionary<string, DataUnit>();
    private Ball PlayingBall;
    private DataUnit CollectingData;
    private bool IsTriggerResult = false;
    public async void StartCollectData()
    {
        var pointList = new List<float>()
        {
            BallMinX,-250, -150, -50, 50, 150, 250,BallMaxX
        };
        if (IsInDataCollect)
            return;
        IsInDataCollect = true;
        PlayingBall = Instantiate(DefaultBall.gameObject, DefaultBall.transform.parent).AddComponent<Ball>();
        PlayingBall.gameObject.SetActive(true);
        PlayingBall.Simulated = true;
        PlayingBall.Init(this,Easter2024BallType.DataCollection);

        for (var i = 0; i < pointList.Count-1; i++)
        {
            var start = pointList[i];
            var end = pointList[i + 1];
            var key = start + "_" + end;
            if (!DataMap.ContainsKey(key))
            {
                DataMap.Add(key,new DataUnit(start,end));
            }
            var data = DataMap[key];
            CollectBallState(data);
        }
        IsInDataCollect = false;
        // SaveToFile();
    }
    
    public void CollectBallState(DataUnit data)
    {
        CollectingData = data;
        for (var i = 0; i < CollectCount; i++)
        {
            // var task = new TaskCompletionSource<bool>();
            // StartCoroutine(CommonUtils.DelayWorkFrame(1, ()=>task.SetResult(true)));
            // await task.Task;
            var tempP = LuckyGroupTrans.transform.localPosition;
            tempP.x = Random.Range(LuckyNodeMinX,LuckyNodeMaxX+1);
            LuckyGroupTrans.transform.localPosition = tempP;
            LuckyNodeSpeedX = Random.Range(0, 2) == 0 ? LuckyNodeSpeedXValue : -LuckyNodeSpeedXValue;
            
            PlayingBall.ResetBallState();
            PlayingBall.transform.localPosition = new Vector3(Random.Range(data.BallMinX,data.BallMaxX),0,0);
            PlayingBall.Rigidbody.velocity = Vector2.zero;
            PlayingBall.Rigidbody.angularVelocity = 0f;
            CollectingData.PlayCount++;
            IsTriggerResult = false;
            var samePositionCount = 0;
            while (!IsTriggerResult)
            {
                if (PlayingBall.CheckSamePosition())
                {
                    i--;
                    CollectingData.PlayCount--;
                    CollectingData.FreezeCount++;
                    break;
                }
                UpdateLuckyNodePosition(Time.fixedDeltaTime);
                Physics2D.Simulate(Time.fixedDeltaTime);
            }
            // SaveCount++;
            // if (SaveCount >= SaveInterval)
            // {
            //     SaveCount = 0;
            //     SaveToFile();
            // }
        }
        CollectingData.LuckyPercent = (float) CollectingData.LuckyCount / CollectingData.PlayCount * 100f;
        for (var i = 0; i < 7; i++)
        {
            CollectingData.ResultPercent[i] = (float) CollectingData.ResultCount[i] / CollectingData.PlayCount * 100f;
        }
        SaveToFile();
    }

    public void DataCollectionTriggerResult(Ball ball, int resultIndex)
    {
        CollectingData.ResultCount[resultIndex]++;
        IsTriggerResult = true;
    }
    public void DataCollectionTriggerLuckPoint(Ball ball)
    {
        CollectingData.LuckyCount++;
    }

    public void SaveToFile()
    {
        var dataToJson = JsonConvert.SerializeObject(DataMap);
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        // 设置要创建的文件的完整路径
        string filePath = Path.Combine(desktopPath, "DataCollection.txt");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(dataToJson);
        }

        int PlayCount = 0;
        int LuckyCount = 0;
        List<int> ResultCount = new List<int>(){0, 0, 0, 0, 0, 0, 0};
        foreach (var dataPair in DataMap)
        {
            var data = dataPair.Value;
            PlayCount += data.PlayCount;
            LuckyCount += data.LuckyCount;
            for (var i = 0; i < 7; i++)
            {
                ResultCount[i] += data.ResultCount[i];
            }
        }

        var log = "";
        log += ("投掷次数 = " + PlayCount);
        var luckyPercent = (float) LuckyCount / PlayCount * 100f;
        log += (" 幸运概率 = "+luckyPercent);
        for (var i = 0; i < 7; i++)
        {
            var resultPercent = (float) ResultCount[i] / PlayCount * 100f;
            log += (" 篮子" + (i + 1) + "概率 = " + resultPercent);
        }
        Debug.LogError(log);
    }
}
[CustomEditor(typeof(DropBallGame))]
public class DropBallGameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draws the default inspector

        DropBallGame script = (DropBallGame)target;

        if (GUILayout.Button("跑数据"))
        {
            script.StartCollectData();
        }
    }
}
#endif