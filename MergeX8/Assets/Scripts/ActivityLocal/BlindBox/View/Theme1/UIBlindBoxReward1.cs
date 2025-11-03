using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIBlindBoxReward1:UIBlindBoxRewardBase
{
    private Dictionary<int, GroupItem> GroupList = new Dictionary<int, GroupItem>();
    private Transform DefaultGroupItem;
    public override void PrivateAwake()
    {
        
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        DefaultGroupItem = transform.Find("Root/Scroll View/Viewport/Content/1");
        DefaultGroupItem.gameObject.SetActive(false);
        for (var i = 1; i <= Config.GroupList.Count; i++)
        {
            var groupId = Config.GroupList[i - 1];
            var groupConfig = Model.GroupConfigDic[groupId];
            var groupItem = Instantiate(DefaultGroupItem, DefaultGroupItem.parent).gameObject.AddComponent<GroupItem>();
            groupItem.gameObject.SetActive(true);
            groupItem.Init(groupConfig,this);
            GroupList.Add(groupId,groupItem);
        }
        UpdateSibling();
    }

    public void UpdateSibling()
    {
        var canCollectGroups = new List<GroupItem>();
        var noCollectGroups = new List<GroupItem>();
        var hasCollectGroups = new List<GroupItem>();
        foreach (var pair in GroupList)
        {
            var groupItem = pair.Value;
            if (Storage.CanCollectGroup(groupItem.Config))
            {
                canCollectGroups.Add(groupItem);
            }
            else if (Storage.HasCollectGroup(groupItem.Config))
            {
                hasCollectGroups.Add(groupItem);
            }
            else
            {
                noCollectGroups.Add(groupItem);
            }
        }

        for (var i = hasCollectGroups.Count - 1; i >= 0; i--)
        {
            hasCollectGroups[i].transform.SetAsFirstSibling();
        }
        for (var i = noCollectGroups.Count - 1; i >= 0; i--)
        {
            noCollectGroups[i].transform.SetAsFirstSibling();
        }
        for (var i = canCollectGroups.Count - 1; i >= 0; i--)
        {
            canCollectGroups[i].transform.SetAsFirstSibling();
        }
    }

    public override void InitCloseBtn()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
    }

    public void OnCollectGroupReward(BlindBoxGroupConfig config)
    {
        if (!Storage.CanCollectGroup(config))
            return;
        Storage.CollectGroupReward(config);
        var groupItem = GroupList[config.Id];
        groupItem.UpdateView();
        UpdateSibling();
    }

    public class GroupItem : MonoBehaviour
    {
        public BlindBoxGroupConfig Config;
        public UIBlindBoxReward1 Controller;
        public StorageBlindBox Storage => Controller.Storage;
        public GroupItemView Normal;
        public GroupItemView NormalFinish;
        public GroupItemView Special;
        public GroupItemView SpecialFinish;
        public bool IsSpecial => Config.ItemList.Count == 1;
        public bool IsCollect => Storage.CollectGroups.Contains(Config.Id);

        public void Init(BlindBoxGroupConfig config,UIBlindBoxReward1 controller)
        {
            Config = config;
            Controller = controller;
            var isSpecialConfig = Config.ItemList.Count == 1;
            Normal = transform.Find("Normal").gameObject.AddComponent<GroupItemView>();
            NormalFinish = transform.Find("NormalFinish").gameObject.AddComponent<GroupItemView>();
            Special = transform.Find("Special").gameObject.AddComponent<GroupItemView>();
            SpecialFinish = transform.Find("SpecialFinish").gameObject.AddComponent<GroupItemView>();
            if (!isSpecialConfig)
            {
                Normal.Init(Config,Controller,false,false);
                NormalFinish.Init(Config,Controller,true,false);
            }
            else
            {
                Special.Init(Config,Controller,false,true);
                SpecialFinish.Init(Config,Controller,true,true);
            }
            UpdateView();
        }

        public void UpdateView()
        {
            Normal.gameObject.SetActive(!IsCollect && !IsSpecial);
            NormalFinish.gameObject.SetActive(IsCollect && !IsSpecial);
            Special.gameObject.SetActive(!IsCollect && IsSpecial);
            SpecialFinish.gameObject.SetActive(IsCollect && IsSpecial);
        }

        public class GroupItemView : MonoBehaviour
        {
            public BlindBoxModel Model => Controller.Model;
            private bool IsCollect;
            private bool IsSpecial;
            public BlindBoxGroupConfig Config;
            public UIBlindBoxReward1 Controller;
            public StorageBlindBox Storage => Controller.Storage;
            private LocalizeTextMeshProUGUI TitleText;
            private Button CollectBtn;
            private CommonRewardItem RewardItem;
            private List<GroupNeedItem> NeedItemList = new List<GroupNeedItem>();
            public void Init(BlindBoxGroupConfig config,UIBlindBoxReward1 controller, bool isCollect, bool isSpecial)
            {
                Config = config;
                Controller = controller;
                IsCollect = isCollect;
                IsSpecial = isSpecial;
                for (var i = 0; i < Config.ItemList.Count; i++)
                {
                    var item = Config.ItemList[i];
                    var itemConfig = Model.ItemConfigDic[item];
                    var needItem = transform.Find("Need/" + (i + 1)).gameObject.AddComponent<GroupNeedItem>();
                    var count = Storage.CollectItems.GetValueOrDefault(item);
                    var needCount = 1;
                    needItem.Init(itemConfig.GetItemSprite(count < needCount),count,needCount);
                    NeedItemList.Add(needItem);
                }

                TitleText = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();
                TitleText.SetTerm(Config.NameKey);
                RewardItem = transform.Find("RewardItem").gameObject.AddComponent<CommonRewardItem>();
                var rewards = CommonUtils.FormatReward(Config.RewardId, Config.RewardNum);
                RewardItem.Init(rewards[0]);
                if (!IsCollect)
                {
                    CollectBtn = transform.Find("Button").GetComponent<Button>();
                    CollectBtn.onClick.AddListener(()=>Controller.OnCollectGroupReward(Config));
                    CollectBtn.gameObject.SetActive(Storage.CanCollectGroup(Config));
                }
            }

            public class GroupNeedItem : MonoBehaviour
            {
                private Image Icon;
                private LocalizeTextMeshProUGUI Text;
                private LocalizeTextMeshProUGUI NoText;

                public void Init(Sprite sprite,int count,int needCount)
                {
                    Icon = transform.Find("Image").GetComponent<Image>();
                    Text = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
                    NoText = transform.Find("NotText").GetComponent<LocalizeTextMeshProUGUI>();
                    Text.gameObject.SetActive(count >= needCount);
                    NoText.gameObject.SetActive(count < needCount);
                    Text.SetText(count+"/"+needCount);
                    NoText.SetText(count+"/"+needCount);
                    Icon.sprite = sprite;
                }
            }
        }
    }
}