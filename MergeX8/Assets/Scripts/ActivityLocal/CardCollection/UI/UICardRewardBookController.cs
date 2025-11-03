using System;
using System.Collections.Generic;
using ActivityLocal.CardCollection.Home;
using DragonPlus;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UICardRewardBookController:UIWindowController
{
    private Transform RewardItem;
    private List<CommonRewardItem> RewardItemList = new List<CommonRewardItem>();
    private List<RewardData> RewardDataList = new List<RewardData>();
    private Action Callback;
    private Button CloseButton;

    // private Transform ThemeOld;
    // private Transform TitleOld;
    // private Transform ThemeNew;
    // private Transform TitleNew;
    // private Image CardThemeIcon;
    public override void PrivateAwake()
    {
        RewardItem = transform.Find("Root/RewardGroup/Item");
        RewardItem.gameObject.SetActive(false);
        CloseButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseButton.onClick.AddListener(OnClickCloseButton);
        // CardThemeIcon = transform.Find("Root/CardBook1").GetComponent<Image>();
    }

    private CardCollectionCardThemeState CardThemeState;
    
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CardThemeState = objs[0] as CardCollectionCardThemeState;
        Callback = objs.Length > 1 ? objs[1] as Action : null;
        var rewardList = CardThemeState.CompletedReward;
        // CardThemeIcon.sprite = CardThemeState.GetIconSprite();

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

        ClickEnable = 0;
        if (CardThemeState.CardThemeConfig.UpGradeTheme > 0)
        {
            var starCount = CardCollectionModel.Instance.CheckUnlockTheme();
            var starText = transform.Find("Root/Star/Text").GetComponent<LocalizeTextMeshProUGUI>();
            starText.SetText("x"+starCount);
            var starText2 = transform.Find("Root/Star/Text (1)").GetComponent<LocalizeTextMeshProUGUI>();
            starText2.SetTermFormats(starCount.ToString());
            // ThemeOld = transform.Find("Root/CardBook");
            // TitleOld = transform.Find("Root/BG");
            // ThemeNew = transform.Find("Root/CardBookNew");
            // TitleNew = transform.Find("Root/BGNew");
            // ThemeOld.gameObject.SetActive(true);
            // TitleOld.gameObject.SetActive(true);
            // ThemeNew.gameObject.SetActive(false);
            // TitleNew.gameObject.SetActive(false);
            CardThemeState.GetUpGradeTheme().CreateTaskEntrance();
            CardThemeState.GetUpGradeTheme().CreateAuxEntrance();
            XUtility.WaitSeconds(1f, () =>
            {
                FlyGameObjectManager.Instance.FlyObject(RewardDataList,
                    CurrencyGroupManager.Instance.currencyController, () =>
                    {

                        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                        CommonRewardManager.Instance.PopupCacheReward();
                        foreach (var resData in RewardDataList)
                        {
                            GameObject.Destroy(resData.gameObject);
                        }
                    },ignoreEventSystem:true);
            });
            XUtility.WaitSeconds(4f, () => ClickEnable = 1);
        }
        else
        {
            XUtility.WaitSeconds(1.5f, () => ClickEnable = 1);
        }
    }

    private int ClickEnable = 0;
    public void OnClickCloseButton()
    {
        if (ClickEnable == 0)
            return;
        var btnState = ClickEnable;
        ClickEnable = 0;
        if (btnState == 1)
        {
            if (CardThemeState.CardThemeConfig.UpGradeTheme > 0)
            {
                // ThemeOld.gameObject.SetActive(false);
                // TitleOld.gameObject.SetActive(false);
                // ThemeNew.gameObject.SetActive(true);
                // TitleNew.gameObject.SetActive(true);
                // ClickEnable = 0;
                // XUtility.WaitSeconds(1f, () => ClickEnable = 2);
                CloseButton.gameObject.SetActive(false);
                AnimCloseWindow(() =>
                {
                    Callback?.Invoke();
                });    
            }
            else
            {
                FlyGameObjectManager.Instance.FlyObject(RewardDataList, CurrencyGroupManager.Instance.currencyController, () =>
                {
            
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                    CommonRewardManager.Instance.PopupCacheReward();
                    foreach (var resData in RewardDataList)
                    {
                        GameObject.Destroy(resData.gameObject);
                    }
                },ignoreEventSystem:true);
                CloseButton.gameObject.SetActive(false);
                AnimCloseWindow(() =>
                {
                    Callback?.Invoke();
                });    
            }    
        }
        // else
        // {
        //     CloseButton.gameObject.SetActive(false);
        //     AnimCloseWindow(() =>
        //     {
        //         Callback?.Invoke();
        //     });  
        // }
        
    }

    public static void Open(CardCollectionCardThemeState cardThemeState,Action callback = null)
    {
        var uiPath = cardThemeState.GetCardUIName(CardUIName.UIType.UICardRewardTheme);
        if (!UIManager.Instance.GetOpenedUIByPath(uiPath))
            UIManager.Instance.OpenUI( uiPath,cardThemeState,callback);
    }
}