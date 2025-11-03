using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupMonopolyMiniGameController:UIWindowController
{
    private List<LocalizeTextMeshProUGUI> RewardTextList = new List<LocalizeTextMeshProUGUI>();
    private List<Image> RewardItemList = new List<Image>();
    private List<EggSelection> EggSelectionList = new List<EggSelection>();
    private int SelectIndex;
    private StorageMonopoly Storage;
    private MonopolyMiniGameConfig MiniGameConfig;
    private List<int> ResultState = new List<int>();
    private List<List<Image>> rewardIconList = new List<List<Image>>(); 
    private const int WinNeedCount = 3;
    public override void PrivateAwake()
    {
        for (var i = 0; i < 3; i++)
        {
            transform.Find("Root/RewardGroup/"+(i+1)+"/Finish").gameObject.SetActive(false);
            RewardTextList.Add(transform.Find("Root/RewardGroup/"+(i+1)+"/Text").GetComponent<LocalizeTextMeshProUGUI>());
            rewardIconList.Add(new List<Image>());
            for (var j = 0; j < 3; j++)
            {
                rewardIconList[i].Add(transform.Find("Root/RewardGroup/"+(i+1)+"/Icon ("+(j+1)+")").gameObject.GetComponent<Image>());
                rewardIconList[i][j].color = GreyColor;
            }
            RewardItemList.Add(transform.Find("Root/RewardGroup/"+(i+1)+"/Icon (4)").GetComponent<Image>());
        }
        AwakeEggSelection();
    }

    private int BetValue;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageMonopoly;
        SelectIndex = 0;
        MiniGameConfig = objs[1] as MonopolyMiniGameConfig;
        BetValue = (int)objs[2];
        for (var i = 0; i < MiniGameConfig.RewardId.Count; i++)
        {
            if (MiniGameConfig.RewardId[i] > 0)
                RewardItemList[i].sprite =
                    UserData.GetResourceIcon(MiniGameConfig.RewardId[i], UserData.ResourceSubType.Big);
            RewardTextList[i].SetText((MiniGameConfig.RewardNum[i]*Storage.GetCurBlockUpgradeRewardMultiple()*BetValue).ToString());
            ResultState.Add(0);
        }
    }

    private Color GreyColor = new Color(0.78f, 0.58f, 0.46f, 0.78f);
    public void ShowCollectEffect(int resultId,int index)
    {
        rewardIconList[resultId][index-1].color = Color.white;
        var effect = transform.Find("Root/RewardGroup/" + (resultId + 1) + "/VFX_BG_0" + index);
        effect.gameObject.SetActive(false);
        effect.gameObject.SetActive(true);
        AudioManager.Instance.PlaySound("sfx_easter2024_break_egg");
    }
    public async void HandleWin(int resultId)
    {
        for (var i = 0; i < EggSelectionList.Count; i++)
        {
            var egg = EggSelectionList[i];
            if (egg.IsOpen && egg.ResultId == resultId)
            {
                egg.PerformWin();
            }
            else if (egg.IsOpen && egg.ResultId != resultId)
            {
                egg.PerformDark();
            }
            else if (!egg.IsOpen)
            {
                egg.PerformUnOpenDark();
            }
        }
        var sortingGroup = transform.Find("Root/RewardGroup/" + (resultId + 1)).gameObject.AddComponent<Canvas>();
        sortingGroup.overrideSorting = true;
        sortingGroup.sortingLayerName = canvas.sortingLayerName;
        sortingGroup.sortingOrder = canvas.sortingOrder+1;
        transform.Find("Root/RewardGroup/"+(resultId+1)+"/Finish").gameObject.SetActive(true);
        var animator = transform.Find("Root/RewardGroup/" + (resultId + 1)).gameObject.GetComponent<Animator>();
        animator.Play("open");

        Storage.UnFinishedMiniGameConfigId = 0;
        // {
        //     var result = MiniGameConfig.ResultList.Last();
        //     var rewardId = MiniGameConfig.RewardId[result];
        //     var rewardNum = MiniGameConfig.RewardNum[result];
        //     if (rewardId < 0)
        //     {
        //         Storage.AddScore(rewardNum, "MiniGame", true);
        //     }
        //     else
        //     {
        //         UserData.Instance.AddRes(rewardId, rewardNum, new GameBIManager.ItemChangeReasonArgs()
        //         {
        //             reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonopolyGet
        //         });
        //     }
        // }

        var resId = MiniGameConfig.RewardId[resultId];
        var count = MiniGameConfig.RewardNum[resultId]*Storage.GetCurBlockUpgradeRewardMultiple()*BetValue;
        await XUtility.WaitSeconds(2f);
        var position = transform.position;
        UIMonopolyMiniGameRewardController.Open(resId,count, resultId,()=>AnimCloseWindow(() =>
        {
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIMonopolyMainController>(UINameConst.UIMonopolyMain);
            if (mainUI && resId < 0)
            {
                mainUI.FlyCarrot(position,count);
            }
        }));
    }
    public static UIPopupMonopolyMiniGameController Open(StorageMonopoly storageMonopoly,MonopolyMiniGameConfig miniGameConfig,int betValue)
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterLuckygameEnter);
        return UIManager.Instance.OpenUI(UINameConst.UIPopupMonopolyMiniGame, storageMonopoly,miniGameConfig,betValue) as
            UIPopupMonopolyMiniGameController;
    }
}