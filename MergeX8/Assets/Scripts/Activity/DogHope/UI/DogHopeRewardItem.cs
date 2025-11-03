
using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class DogHopeRewardItem: MonoBehaviour
{
    private Transform _finish;
    private Transform _right;
    private Transform _right2;
    private Transform _err;
    private Image _icon;
    private List<Image> _multiIconList = new List<Image>();
    private Image _icon3;
    private Image _bg;
    private Image _bg2;
     

    public enum RewardStatus
    {
        None,
        Finish,
        Right,
        Err
    }
    private void Awake()
    {
        _finish = transform.Find("Finish");
        _right = transform.Find("Right");
        _right2 = transform.Find("Right2");
        _err = transform.Find("Err");
        _icon = transform.Find("Icon").GetComponent<Image>();
        var multiIconStartIndex = 1;
        while (true)
        {
            var multiIconTrans = transform.Find("Icon" + multiIconStartIndex);
            if (!multiIconTrans)
                break;
            multiIconStartIndex++;
            _multiIconList.Add(multiIconTrans.GetComponent<Image>());
        }
        _bg = transform.Find("BG").GetComponent<Image>();
        _bg2 = transform.Find("BG2").GetComponent<Image>();
    }

    public void Init(List<int> itemID,List<int>  itemNum,RewardStatus status)
    {
        GameObject tex;
        if (itemID.Count==1)
        {
            _icon.gameObject.SetActive(true);
            foreach (var multiIcon in _multiIconList)
            {
                multiIcon.gameObject.SetActive(false);
            }
            _bg.gameObject.SetActive(true);
            _bg2.gameObject.SetActive(false);
            if (UserData.Instance.IsResource(itemID[0]))
            {
                _icon.sprite = UserData.GetResourceIcon(itemID[0], UserData.ResourceSubType.Reward);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID[0]);
                _icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }

            tex = _icon.FindChild("Text");
            tex .SetActive(true);
            tex. GetComponent<LocalizeTextMeshProUGUI>().SetText("x"+itemNum[0]);
        }
        else
        {
            _icon.gameObject.SetActive(false);
            for (var i=0;i<_multiIconList.Count;i++)
            {
                var multiIcon = _multiIconList[i];
                if (i < itemID.Count)
                {
                    multiIcon.gameObject.SetActive(true);   
                    
                    if (UserData.Instance.IsResource(itemID[i]))
                    {
                        multiIcon.sprite = UserData.GetResourceIcon(itemID[i], UserData.ResourceSubType.Reward);
                    }
                    else
                    {
                        var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID[i]);
                        multiIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                    }
                    tex = multiIcon.FindChild("Text");
                    tex.SetActive(true);
                    tex.GetComponent<LocalizeTextMeshProUGUI>().SetText("x"+itemNum[i]);
                }
                else
                {
                    multiIcon.gameObject.SetActive(false);
                }
            }
            _bg.gameObject.SetActive(false);
            _bg2.gameObject.SetActive(true);
        }
        _finish.gameObject.SetActive(true);

        switch (status)
        {
     
           case RewardStatus.None:
               _right.gameObject.SetActive(false);
               _err.gameObject.SetActive(false);
               break;    
           case RewardStatus.Finish:
               _err.gameObject.SetActive(false);
               if (itemID.Count == 1)
               {
                   _right.gameObject.SetActive(true);
                   _right2.gameObject.SetActive(false);
               }
               else
               {
                   _right.gameObject.SetActive(false);
                   _right2.gameObject.SetActive(true);
               }
               break;    
           case RewardStatus.Err:
               _right.gameObject.SetActive(false);
               _err.gameObject.SetActive(true);
               break;
        }
   
    }
}
