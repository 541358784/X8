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
    public Transform ExchangeGroup;
    // public Transform DefaultExchangeItem;
    public List<ExchangeItem> ExchangeItemList = new List<ExchangeItem>();
    public Button ExchangeSureBtn;
    public Button ExchangeCloseBtn;
    public TaskCompletionSource<bool> ExchangeTask;
    public void InitExchangeGroup()
    {
        ExchangeGroup = GetItem<Transform>("Reward");
        var temp = ExchangeGroup.gameObject.AddComponent<Canvas>();
        temp.overrideSorting = true;
        temp.sortingOrder = canvas.sortingOrder + 4;
        ExchangeGroup.gameObject.AddComponent<GraphicRaycaster>();
        // DefaultExchangeItem = GetItem<Transform>("Reward/Root/Content/1");
        // DefaultExchangeItem.gameObject.SetActive(false);
        ExchangeSureBtn = GetItem<Button>("Reward/Root/Button");
        ExchangeSureBtn.onClick.AddListener(async () =>
        {
            var totalScore = 0;
            var leftDic = new Dictionary<int, int>();
            foreach (var pair in Storage.BagGame)
            {
                var left = pair.Value % Model.GlobalConfig.TurtleExchangeScore[0];
                var score = pair.Value / Model.GlobalConfig.TurtleExchangeScore[0];
                totalScore += score * Model.GlobalConfig.TurtleExchangeScore[1];
                Model.AddBagValue(pair.Key, left);
                leftDic.Add(pair.Key, left);
            }
            Storage.BagGame.Clear();
            Storage.IsInGame = false;
            Storage.LuckyColor = 0;
            Storage.BoardState.Clear();
            Storage.BasePackageCount = 0;
            Storage.ExtraPackageCount = 0;
            var lastScore = Storage.Score;
            Model.AddScore(totalScore,"GameSettle");
            ScoreText.SetText(lastScore.ToString());
            foreach (var pair in leftDic)
            {
                var item = ExchangeItemList.Find(a => a.Config.Id == pair.Key);
                for (var i=0;i<pair.Value;i++)
                {
                    FlyGameObjectManager.Instance.FlyObject(item.Icon.gameObject,
                        item.Icon.transform.position,BagBtn.transform.position,true);
                }
            }

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
            ExchangeGroup.gameObject.SetActive(false);
            ExchangeTask.SetResult(true);
        });
        ExchangeGroup.gameObject.SetActive(true);
        foreach (var config in Model.ItemConfig)
        {
            // var clone = Instantiate(DefaultExchangeItem, DefaultExchangeItem.parent);
            // var item = clone.gameObject.AddComponent<ExchangeItem>();
            var item = transform.Find("Reward/Root/Content/" + config.Id).gameObject.AddComponent<ExchangeItem>();
            item.gameObject.SetActive(true);
            item.Init(config,Storage);
            ExchangeItemList.Add(item);
        }
        ExchangeGroup.gameObject.SetActive(false);
        GetItem<LocalizeTextMeshProUGUI>("Reward/Root/Text").SetTermFormats(
            Model.GlobalConfig.TurtleExchangeScore[0].ToString(),
            Model.GlobalConfig.TurtleExchangeScore[1].ToString());
    }
    public async Task ShowExchange()
    {
        foreach (var item in ExchangeItemList)
        {
            item.SetType(ExchangeType.Game);
        }
        ExchangeGroup.gameObject.SetActive(true);
        ExchangeGroup.GetComponent<Canvas>().overrideSorting = true;
        ExchangeTask = new TaskCompletionSource<bool>();
        await ExchangeTask.Task;
    }
    public enum ExchangeType
    {
        Bag,
        Game,
    }
    public class ExchangeItem : MonoBehaviour
    {
        public TurtlePangItemConfig Config;
        public StorageTurtlePang Storage;
        public ExchangeType Type;
        public LocalizeTextMeshProUGUI Label;
        public Image Icon;
        private Dictionary<int, int> TempBag => Type == ExchangeType.Bag ? Storage.Bag : Storage.BagGame;
        public void Init(TurtlePangItemConfig config,StorageTurtlePang storage)
        {
            Config = config;
            Storage = storage;
        }

        public void SetType(ExchangeType type)
        {
            Type = type;
            UpdateView();
        }

        public void UpdateView()
        {
            TempBag.TryGetValue(Config.Id, out var value);
            Label.SetText("X"+value);
            Icon.sprite = Config.GetTurtleIcon();
        }
        public void OnBagItemChange(EventTurtlePangBagItemChange evt)
        {
            UpdateView();
        }
        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventTurtlePangBagItemChange>(OnBagItemChange);
            Label = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            Icon = transform.Find("Image").GetComponent<Image>();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventTurtlePangBagItemChange>(OnBagItemChange);
        }
    }
}