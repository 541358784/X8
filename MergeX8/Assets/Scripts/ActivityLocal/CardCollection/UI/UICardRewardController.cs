using System;
using System.Collections.Generic;
using ActivityLocal.CardCollection.Home;
using DragonPlus;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UICardRewardController:UIWindowController
{
    private Transform RewardItem;
    private List<CommonRewardItem> RewardItemList = new List<CommonRewardItem>();
    private List<RewardData> RewardDataList = new List<RewardData>();
    private Action Callback;
    private Button CloseButton;
    private Dictionary<int, Transform> BackBoardList = new Dictionary<int, Transform>();
    private Image CardBookIcon;
    private LocalizeTextMeshProUGUI CardBookName;
    public override void PrivateAwake()
    {
        RewardItem = transform.Find("Root/RewardGroup/Item");
        RewardItem.gameObject.SetActive(false);
        CloseButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseButton.onClick.AddListener(OnClickCloseButton);
        CardBookIcon = transform.Find("Root/CardBook/Mask/Icon").GetComponent<Image>();
        BackBoardList.Add(1,transform.Find("Root/CardBook/BGGroup/Blue"));
        BackBoardList.Add(2,transform.Find("Root/CardBook/BGGroup/Purple"));
        BackBoardList.Add(3,transform.Find("Root/CardBook/BGGroup/Gold"));
        CardBookName = transform.Find("Root/CardBook/NameText").GetComponent<LocalizeTextMeshProUGUI>();
    }

    private CardCollectionCardBookState CardBookState;
    
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CardBookState = objs[0] as CardCollectionCardBookState;
        Callback = objs.Length > 1 ? objs[1] as Action : null;
        var rewardList = CardBookState.CompletedReward;
        CardBookIcon.sprite = CardBookState.GetIconSprite();
        foreach (var pair in BackBoardList)
        {
            pair.Value.gameObject.SetActive(CardBookState.CardBookConfig.Level == pair.Key);
        }
        CardBookName.SetTerm(CardBookState.NameKey);
        for (int i = 0; i < rewardList.Count; i++)
        {
            var reward = rewardList[i];
            var item = Instantiate(RewardItem.gameObject, RewardItem.parent)
                .AddComponent<CommonRewardItem>();
            item.gameObject.SetActive(true);
            item.Init(reward);
            RewardItemList.Add(item);
            RewardData rewardData = new RewardData();
            rewardData.gameObject = item.gameObject;
            rewardData.image = item._image;
            rewardData.numText = item._numText;
            rewardData.UpdateReward(reward);
            item.gameObject.SetActive(true);
            RewardDataList.Add(rewardData);
        }

        ClickEnable = false;
        XUtility.WaitSeconds(1.5f, () => ClickEnable = true);
    }

    private bool ClickEnable = false;
    public void OnClickCloseButton()
    {
        if (!ClickEnable)
            return;
        ClickEnable = false;
        CloseButton.gameObject.SetActive(false);
        AnimCloseWindow(() =>
        {
            Callback?.Invoke();
        });
        FlyGameObjectManager.Instance.FlyObject(RewardDataList, CurrencyGroupManager.Instance.currencyController, () =>
        {
            
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
            CommonRewardManager.Instance.PopupCacheReward();
            foreach (var resData in RewardDataList)
            {
                GameObject.Destroy(resData.gameObject);
            }
        },ignoreEventSystem:true);
    }

    public static void Open(CardCollectionCardBookState cardBookState,Action callback = null)
    {
        var uiPath = cardBookState.CardThemeStateList[0].GetCardUIName(CardUIName.UIType.UICardRewardBook);
        if (!UIManager.Instance.GetOpenedUIByPath(uiPath))
            UIManager.Instance.OpenUI( uiPath,cardBookState,callback);
    }
}