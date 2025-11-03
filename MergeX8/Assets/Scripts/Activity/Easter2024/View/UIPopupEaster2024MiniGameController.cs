using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupEaster2024MiniGameController:UIWindowController
{
    private List<LocalizeTextMeshProUGUI> RewardTextList = new List<LocalizeTextMeshProUGUI>();
    private List<EggSelection> EggSelectionList = new List<EggSelection>();
    private int SelectIndex;
    private StorageEaster2024 Storage;
    private Easter2024MiniGameConfig MiniGameConfig;
    private List<int> ResultState = new List<int>();
    private List<List<Image>> rewardIconList = new List<List<Image>>(); 
    private const int WinNeedCount = 3;
    public override void PrivateAwake()
    {
        for (var i = 0; i < 3; i++)
        {
            RewardTextList.Add(transform.Find("Root/RewardGroup/"+(i+1)+"/Text").GetComponent<LocalizeTextMeshProUGUI>());
            rewardIconList.Add(new List<Image>());
            for (var j = 0; j < 3; j++)
            {
                rewardIconList[i].Add(transform.Find("Root/RewardGroup/"+(i+1)+"/Icon ("+(j+1)+")").gameObject.GetComponent<Image>());
                rewardIconList[i][j].color = Color.grey;
            }
        }
        AwakeEggSelection();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageEaster2024;
        SelectIndex = 0;
        var miniGameConfigIdPool = Storage.GetCurLevel().MiniGamePool;
        var miniGameConfigPool = new List<Easter2024MiniGameConfig>();
        var weightPool = new List<int>();
        for (var i = 0; i < miniGameConfigIdPool.Count; i++)
        {
            var config = Easter2024Model.Instance.MiniGameConfig[miniGameConfigIdPool[i]];
            miniGameConfigPool.Add(config);
            weightPool.Add(config.Weight);
        }
        var index = Utils.RandomByWeight(weightPool);
        MiniGameConfig = miniGameConfigPool[index];
        for (var i = 0; i < MiniGameConfig.ScoreList.Count; i++)
        {
            RewardTextList[i].SetText(MiniGameConfig.ScoreList[i].ToString());
            ResultState.Add(0);
        }
    }

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

        var winValue = MiniGameConfig.ScoreList[resultId];
        var mainUI = UIManager.Instance.GetOpenedUIByPath<UIEaster2024MainController>(UINameConst.UIEaster2024Main);
        Easter2024Model.Instance.AddScore(winValue,"MiniGame",mainUI);
        Easter2024Model.Instance.ReduceLuckyPoint(Easter2024Model.Instance.MiniGameNeedLuckyPointCount);
        await XUtility.WaitSeconds(2f);
        var position = transform.position;
        UIEaster2024MiniGameRewardController.Open(winValue, resultId,()=>AnimCloseWindow(() =>
        {
            if (mainUI)
            {
                mainUI.FlyCarrot(position,winValue);
            }
        }));
    }
    public static UIPopupEaster2024MiniGameController Open(StorageEaster2024 storageEaster2024)
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterLuckygameEnter);
        return UIManager.Instance.OpenUI(UINameConst.UIPopupEaster2024MiniGame, storageEaster2024) as
            UIPopupEaster2024MiniGameController;
    }
}