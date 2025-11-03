using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UITurtlePangMainController
{
    public Transform BagExchangeGroup;
    // public Transform DefaultBagExchangeItem;
    public List<ExchangeItem> BagExchangeItemList = new List<ExchangeItem>();
    public Button BagExchangeSureBtn;
    public Button BagExchangeCloseBtn;
    public TaskCompletionSource<bool> BagExchangeTask;
    public void InitBagExchangeGroup()
    {
        BagExchangeGroup = GetItem<Transform>("Warehouse");
        var temp = BagExchangeGroup.gameObject.AddComponent<Canvas>();
        temp.overrideSorting = true;
        temp.sortingOrder = canvas.sortingOrder + 4;
        BagExchangeGroup.gameObject.AddComponent<GraphicRaycaster>();
        // DefaultBagExchangeItem = GetItem<Transform>("Warehouse/Root/Content/1");
        // DefaultBagExchangeItem.gameObject.SetActive(false);
        BagExchangeCloseBtn = GetItem<Button>("Warehouse/Root/Content/ButtonClose");
        BagExchangeCloseBtn.onClick.AddListener(() =>
        {
            BagExchangeGroup.gameObject.SetActive(false);
        });
        BagExchangeSureBtn = GetItem<Button>("Warehouse/Root/Content/Button");
        BagExchangeSureBtn.onClick.AddListener(() =>
        {
            var totalScore = 0;
            var keyList = Storage.Bag.Keys.ToList();
            foreach (var key in keyList)
            {
                var left = Storage.Bag[key] % Model.GlobalConfig.TurtleExchangeScore[0];
                var score = Storage.Bag[key] / Model.GlobalConfig.TurtleExchangeScore[0];
                totalScore += score * Model.GlobalConfig.TurtleExchangeScore[1];
                Storage.Bag[key] = left;
            }
            EventDispatcher.Instance.SendEventImmediately(new EventTurtlePangBagItemChange());
            var lastScore = Storage.Score;
            Model.AddScore(totalScore,"BagExchange");
            ScoreText.SetText(lastScore.ToString());
            var flyCount = Math.Min(totalScore/Model.GlobalConfig.TurtleExchangeScore[1], 10);
            for (var i = 0; i < flyCount; i++)
            {
                var index = i;
                FlyGameObjectManager.Instance.FlyObject(ScoreIcon.gameObject,
                    ExchangeSureBtn.transform.position,ScoreIcon.transform.position,true,action: () =>
                    {
                        if (index == 0)
                        {
                            ScoreText.SetText(Storage.Score.ToString());
                        }
                    });
            }
            BagExchangeTask.SetResult(true);
            BagExchangeGroup.gameObject.SetActive(false);
        });
        BagExchangeGroup.gameObject.SetActive(true);
        foreach (var config in Model.ItemConfig)
        {
            // var clone = Instantiate(DefaultBagExchangeItem, DefaultBagExchangeItem.parent);
            // var item = clone.gameObject.AddComponent<ExchangeItem>();
            var item = transform.Find("Warehouse/Root/Content/" + config.Id).gameObject.AddComponent<ExchangeItem>();
            item.gameObject.SetActive(true);
            item.Init(config,Storage);
            BagExchangeItemList.Add(item);
        }
        BagExchangeGroup.gameObject.SetActive(false);
        GetItem<LocalizeTextMeshProUGUI>("Warehouse/Root/Text").SetTermFormats(
            Model.GlobalConfig.TurtleExchangeScore[0].ToString(),
            Model.GlobalConfig.TurtleExchangeScore[1].ToString());
    }
    public async Task ShowBag()
    {
        foreach (var item in BagExchangeItemList)
        {
            item.SetType(ExchangeType.Bag);
        }
        var totalScore = 0;
        var keyList = Storage.Bag.Keys.ToList();
        foreach (var key in keyList)
        {
            var score = Storage.Bag[key] / Model.GlobalConfig.TurtleExchangeScore[0];
            totalScore += score * Model.GlobalConfig.TurtleExchangeScore[1];
        }
        BagExchangeSureBtn.interactable = totalScore > 0;
        
        BagExchangeGroup.gameObject.SetActive(true);
        BagExchangeGroup.GetComponent<Canvas>().overrideSorting = true;
        BagExchangeTask = new TaskCompletionSource<bool>();
        await BagExchangeTask.Task;
    }
}