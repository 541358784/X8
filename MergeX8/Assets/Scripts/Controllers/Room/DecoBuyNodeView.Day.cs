using System.Collections.Generic;
using Decoration.DaysManager;
using DragonPlus;
using UnityEngine;

public partial class DecoBuyNodeView
{
    private GameObject _dayItem;
    private GameObject _dayContentGroup;
    private List<UITask_DayItem> _dayItems = new List<UITask_DayItem>();

    
    private void Awake_Days()
    {
        _dayItem = transform.Find("Slider/RewardGroup/1").gameObject;
        _dayItem.gameObject.SetActive(false);
        
        _dayContentGroup = transform.Find("Slider/RewardGroup").gameObject;;
    }
    
    public void UpdateDays()
    {
        var dayConfig = DaysManager.Instance.GetDayConfig();
        if (dayConfig == null)
        {
            _dayItems.ForEach(a=>a._gameObject.SetActive(false));
            return;
        }

        int newNum = 0;
        
        if (_dayItems.Count < dayConfig.nodeNumber)
        {
            newNum = dayConfig.nodeNumber - _dayItems.Count;
        }
        for (int i = 0; i < newNum; i++)
        {
            var item = Instantiate(_dayItem, _dayContentGroup.transform);
            item.gameObject.SetActive(true);
            
            UITask_DayItem dayItem = new UITask_DayItem();
            dayItem.Init(item, dayConfig, i);
            _dayItems.Add(dayItem);
        }
        
        _dayItems.ForEach(a=>a._gameObject.SetActive(false));
        for (int i = 0; i < dayConfig.nodeNumber; i++)
        {
            _dayItems[i]._gameObject.SetActive(true);
            _dayItems[i].UpdateUI(i, dayConfig);
            
            if (i == 0)
            {
                _dayItems[i]._gameObject.transform.Find("Line").gameObject.SetActive(false);
            }
            else
            {
                _dayItems[i]._gameObject.transform.Find("Line").gameObject.SetActive(true);
            }
        }
    }
}