using System.Collections.Generic;
using Activity.Base;
using Activity.GardenTreasure.Model;
using Activity.JumpGrid;
using Activity.LuckyGoldenEgg;
using UnityEngine;

public partial class UIPopupSetController
{
    public static readonly string[] _activityName = new string[]
    {
        "JumpGrid",
        "GardenTreasure",
        "SeaRacing",
        "LuckyGoldenEgg",
        "Parrot",
        "FishCulture",
        "JungleAdventure",
        "FlowerField"
    };
    
    public static I_ActivityStatus[] _activityStatus = new I_ActivityStatus[]
    {
        JumpGridModel.Instance,
        GardenTreasureModel.Instance,
        SeaRacingModel.Instance,
        LuckyGoldenEggModel.Instance,
        ParrotModel.Instance,
        FishCultureModel.Instance,
        JungleAdventureModel.Instance,
        FlowerFieldModel.Instance,
    };
    
    
    private class ActivityItem
    {
        private Transform _incomplete;
        private Transform _completed;
        private Transform _completedEarly;

        private Transform _root;
        
        
        public ActivityItem(Transform root, I_ActivityStatus.ActivityStatus status)
        {
            _incomplete = root.Find("Tip1");
            _completed = root.Find("Tip2");
            _completedEarly = root.Find("Tip3");

            _incomplete.gameObject.SetActive(false);
            _completed.gameObject.SetActive(false);
            _completedEarly.gameObject.SetActive(false);

            switch (status)
            {
                case I_ActivityStatus.ActivityStatus.None:
                case I_ActivityStatus.ActivityStatus.NotParticipated:
                case I_ActivityStatus.ActivityStatus.Incomplete:
                {
                    _incomplete.gameObject.SetActive(true);
                    break;
                }
                case I_ActivityStatus.ActivityStatus.Completed:
                {
                    _completed.gameObject.SetActive(true);
                    break;
                }
                case I_ActivityStatus.ActivityStatus.CompletedEarly:
                {
                    _completedEarly.gameObject.SetActive(true);
                    break;
                }
                default:
                {
                    _incomplete.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    private List<ActivityItem> _activityItems = new List<ActivityItem>();
    private void Awake_Activity()
    {
        for (var i = 0; i < _activityName.Length; i++)
        {
            _activityItems.Add(new ActivityItem(transform.Find("Root/Activity1/Content/"+_activityName[i]), _activityStatus[i].GetActivityStatus()));
        }
    }
}