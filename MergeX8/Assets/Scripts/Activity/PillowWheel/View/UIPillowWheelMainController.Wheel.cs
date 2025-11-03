using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API;
using SRF;
using UnityEngine;

public partial class UIPillowWheelMainController
{
    private Dictionary<int, Wheel> WheelDic = new Dictionary<int, Wheel>();

    public void InitWheelGroup()
    {
        var wheelConfigs = PillowWheelModel.Instance.TurntableConfigList;
        foreach (var config in wheelConfigs)
        {
            var wheel = transform.Find("Root/Wheel").gameObject.AddComponent<Wheel>();
            wheel.Init(config.Id);
            WheelDic.Add(config.Id,wheel);
        }
        var lightObj = transform.Find("Root/Wheel/Selected");
        lightObj.gameObject.SetActive(false);
    }
    
    public class Wheel : MonoBehaviour
    {
        public List<WheelItem> ItemList = new List<WheelItem>();
        public PillowWheelTurntableConfig Config;
        public int Level;
        public Transform LightObj;

        public void Init(int level)
        {
            Level = level;
            Config = PillowWheelModel.Instance.TurntableConfigList.Find(a => a.Id == Level);
            if (Config == null)
            {
                Debug.LogError("找不到等级"+Level+"的轮盘配置");
                return;   
            }
            for (var i = 0; i < Config.TurntableResultList.Count; i++)
            {
                var key = Level + "_" + (i + 1);
                var itemObject = transform.Find("Gird/" + key);
                if (itemObject == null)
                {
                    Debug.LogError("找不到"+key+"的UI");
                    continue;
                }

                var item = itemObject.gameObject.AddComponent<WheelItem>();
                var id = Config.TurntableResultList[i];
                if (id == 0)
                {
                    item.Init(null);
                }
                else
                {
                    var config = PillowWheelModel.Instance.ResultConfigList.Find(a => a.Id == id);
                    if (config == null)
                    {
                        Debug.LogError("找不到"+id+"的ResultConfig");
                        continue;
                    }
                    item.Init(config);
                }
                ItemList.Add(item);
            }
            LightObj = transform.Find("Selected");
        }

        private const int SpeedUpStep = 10;
        private const int StopStep = 10;
        private float TimeStepMax = 0.3f;
        private float TimeStepMin = 0.05f;
        private float SpinTime = 3f;
        
        public async Task SpinToIndex(int startIndex,int targetIndex)
        {
            LightObj.gameObject.SetActive(true);
            var speedUpTimeList = new List<float>();
            for (var i = 0; i < SpeedUpStep; i++)
            {
                speedUpTimeList.Add(TimeStepMax - ((TimeStepMax - TimeStepMin)/SpeedUpStep) * i);
            }
            var speedDownTimeList = new List<float>();
            for (var i = 0; i < StopStep; i++)
            {
                speedDownTimeList.Add(TimeStepMin + ((TimeStepMax - TimeStepMin)/StopStep) * (i+1));
            }
            var curIndex = startIndex;
            Light(curIndex);
            for (var i = 0; i < speedUpTimeList.Count; i++)
            {
                await XUtility.WaitSeconds(speedUpTimeList[i]);
                curIndex++;
                curIndex %= ItemList.Count;
                Light(curIndex);
            }
            var curTime = APIManager.Instance.GetServerTime();
            var endTime = curTime + SpinTime * XUtility.Second;
            while (endTime > APIManager.Instance.GetServerTime())
            {
                await XUtility.WaitSeconds(TimeStepMin);
                if (!this)
                    return;
                curIndex++;
                curIndex %= ItemList.Count;
                Light(curIndex);
            }

            var stopStartIndex = targetIndex - StopStep;
            while (stopStartIndex < 0)
            {
                stopStartIndex += ItemList.Count;
            }
            while (curIndex != stopStartIndex)
            {
                await XUtility.WaitSeconds(TimeStepMin);
                if (!this)
                    return;
                curIndex++;
                curIndex %= ItemList.Count;
                Light(curIndex);
            }
            for (var i = 0; i < speedDownTimeList.Count; i++)
            {
                await XUtility.WaitSeconds(speedDownTimeList[i]);
                if (!this)
                    return;
                curIndex++;
                curIndex %= ItemList.Count;
                Light(curIndex);
            }
        }

        public void Light(int index)
        {
            AudioManager.Instance.PlaySound("sfx_wheelspin_rollsound");
            if (LightObj)
                LightObj.position = ItemList[index].transform.position;
        }
        
    }

    public class WheelItem : MonoBehaviour
    {
        private PillowWheelResultConfig Config;
        private CommonRewardItem RewardItem;
        private Transform SpecialItem;
        private Transform Arrow;
        private Transform Finish;

        public void UpdateFinishState()
        {
            if (Config != null)
                Finish.gameObject.SetActive(PillowWheelModel.Instance.Storage.CollectState.Contains(Config.Id));
        }
        public void Init(PillowWheelResultConfig config)
        {
            RewardItem = transform.Find("Item").gameObject.AddComponent<CommonRewardItem>();
            SpecialItem = transform.Find("ActivityItem");
            Arrow = transform.Find("Arrow");
            Finish = transform.Find("Finish");
            Config = config;
            if (Config == null)
            {
                RewardItem.gameObject.SetActive(false);
                SpecialItem.gameObject.SetActive(false);
                Arrow.gameObject.SetActive(true);
                Finish.gameObject.SetActive(false);
            }
            else
            {
                Arrow.gameObject.SetActive(false);
                Finish.gameObject.SetActive(PillowWheelModel.Instance.Storage.CollectState.Contains(Config.Id));
                if (Config.RewardId < 0)
                {
                    RewardItem.gameObject.SetActive(false);
                    SpecialItem.gameObject.SetActive(true);
                    foreach (var item in SpecialItem.GetChildren())
                    {
                        item.gameObject.SetActive(false);
                    }
                    SpecialItem.Find((-Config.RewardId).ToString())?.gameObject.SetActive(true);
                }
                else
                {
                    RewardItem.gameObject.SetActive(true);
                    SpecialItem.gameObject.SetActive(false);
                    var reward = new ResData(Config.RewardId, Config.RewardNum);
                    RewardItem.Init(reward);
                }
            }
        }
    }
}