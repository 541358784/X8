using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine;
using UnityEngine;
using UnityEngine.UI;

public class UISeaRacingRewardController:UIWindowController
{
    private Button CloseBtn;
    private HeadIconNode HeadIcon;
    private RectTransform _headIconRoot;
    private LocalizeTextMeshProUGUI RankText;
    
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        _headIconRoot = transform.Find("Root/Player/Head") as RectTransform;
        RankText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
    }

    private StorageSeaRacingRound Storage;
    public void BindStorage(StorageSeaRacingRound storage)
    {
        Storage = storage;
        for (var i = 1; i <= 3; i++)
        {
            transform.Find("Root/Box/"+i).gameObject.SetActive(i == Storage.SortController().MyRank);
        }
        UpdateHeadIcon();
        RankText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("ui_searacing_reward_desc",storage.SortController().MyRank.ToString()));
    }
    public void UpdateHeadIcon(BaseEvent e=null)
    {
        if (_headIconRoot != null)
        {
            if (HeadIcon)
            {
                HeadIcon.SetAvatarViewState(Storage.SortController().Me.GetAvatarViewState());
            }
            else
            {
                HeadIcon = HeadIconNode.BuildHeadIconNode(_headIconRoot,Storage.SortController().Me.GetAvatarViewState());
                HeadIcon.ShowHeadIconFrame(false);
            }
        }
    }
    public void OnClickCloseBtn()
    {
        CloseBtn.enabled = false;
        var rewards = CommonUtils.FormatReward(Storage.UnCollectRewards);
        Storage.UnCollectRewards.Clear();
        Storage.IsFinish = true;
        UserData.Instance.AddRes(rewards,
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.SearacingGet));
        foreach (var reward in rewards)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSearacingOverReward,
                data1:reward.id.ToString(),data2:reward.count.ToString());   
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSearacingOverState,
            data1:Storage.RoundConfigId.ToString(),data2:Storage.SortController().MyRank.ToString());
        var createRoundState = SeaRacingModel.Instance.CurStorageSeaRacingWeek.CreateRound();
        var mainPopup =
            UIManager.Instance
                .GetOpenedUIByPath<UISeaRacingMainController>(UINameConst.UISeaRacingMain);
        if (mainPopup)
        {
            mainPopup.AnimCloseWindow();
        }
        AnimCloseWindow(() =>
        {
            UISeaRacingOpenBoxController.Open(rewards, Storage.SortController().MyRank,
                () =>
                {
                    if (createRoundState)
                    {
                        SeaRacingModel.CanShowStartPopup();
                    }
                    else
                    {
                        SeaRacingModel.CanShowUnCollectRewardsUI();
                    }
                });
        });
        // CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.GetCurrencyUseController(), true,
        //     bi:new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.SearacingGet), 
        //     animEndCall:() =>
        // {
        //     AnimCloseWindow(() =>
        //     {
        //         var mainPopup = UIManager.Instance.GetOpenedUIByPath<UISeaRacingMainController>(UINameConst.UISeaRacingMain);
        //         if (mainPopup)
        //         {
        //             mainPopup.AnimCloseWindow();
        //         }
        //         if (SeaRacingModel.Instance.CurStorageSeaRacingWeek.CreateRound())
        //         {
        //             SeaRacingModel.CanShowStartPopup();
        //         }
        //         else
        //         {
        //             SeaRacingModel.CanShowUnCollectRewardsUI();
        //         }
        //     });
        // });
        
    }
}