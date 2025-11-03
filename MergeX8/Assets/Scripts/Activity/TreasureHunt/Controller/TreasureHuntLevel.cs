using System;
using System.Collections.Generic;
using Activity.TreasureHuntModel;
using DragonPlus;
using DragonU3DSDK;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class TreasureHuntLevel : MonoBehaviour
{
    public TreasureHuntLevelConfig _config;
    public List<LevelItem> _levelItems;
    private void Awake()
    {
        
    }

    public void Init(TreasureHuntLevelConfig config,bool IsCurrent)
    {
        _config=config;
        _levelItems = new List<LevelItem>();
        for (int i = 0; i < config.ItemCount; i++)
        {
            int index = i;
            var item=transform.Find((i + 1).ToString());
            LevelItem levelItem = new LevelItem();
            levelItem.Normal = item.Find("Normal");
            if(IsCurrent)
                levelItem.Normal.gameObject.SetActive(!TreasureHuntModel.Instance.IsBreak(index));
            levelItem.Button = item.GetComponent<Button>();
            levelItem.Button.onClick.AddListener(() =>
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TreasureHuntBreak);
                DebugUtil.Log("Break index-->"+index);
                if (!TreasureHuntModel.Instance.IsBreak(index) &&
                    UserData.Instance.GetRes(UserData.ResourceId.Hammer) > 0&& TreasureHuntModel.Instance.CanBreak)
                {
                    
                    TreasureHuntModel.Instance.BreakItem(index,_config);
                }
                else
                {
                    if (UserData.Instance.GetRes(UserData.ResourceId.Hammer) <= 0)
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupMonopolyNoHammer);
                    }
                }
            });
            levelItem.Null= item.Find("Null");
            levelItem.Null.gameObject.SetActive(false);
            item.Find("Destructio").gameObject.SetActive(true);
            if (IsCurrent && i == 0)
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(levelItem.Button.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TreasureHuntBreak, levelItem.Button.transform as RectTransform, topLayer: topLayer);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TreasureHuntDesc, levelItem.Button.transform as RectTransform);

            }
            
            _levelItems.Add(levelItem);
        }
    }
    
    public class LevelItem
    {
        public Transform Normal;
        public Transform Null;
        public Button Button;
    }
}
