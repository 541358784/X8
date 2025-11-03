using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

public class UICrocodileSuccessController : TMatch.UIWindowController
{
    #region open ui

    /// <summary>
    /// 预制体路径
    /// </summary>
    private const string PREFAB_PATH = "Prefabs/Activity/TMatch/Crocodile/UICrocodilekSuccess";

    /// <summary>
    /// 打开
    /// </summary>
    public static void Open()
    {
        TMatch.UIManager.Instance.OpenWindow<UICrocodileSuccessController>(PREFAB_PATH);
    }

    #endregion
    private Button _buttonClose;

    private Transform _myHead;
    private Transform _robotHeadGroup;
    private Transform _robotHead;
    private Transform _coinImage;

    private LocalizeTextMeshProUGUI _tipText;
    private LocalizeTextMeshProUGUI _prizeText;
    private LocalizeTextMeshProUGUI _coinText;

    private int _winPrize;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseButtonClicked);
        _myHead = transform.Find("Root/MiddleGroup/PlayersGroup/NameItemOne");
        _robotHeadGroup = transform.Find("Root/MiddleGroup/PlayersGroup/Others");
        _robotHead = transform.Find("Root/MiddleGroup/PlayersGroup/Others/NameItemOther");
        _coinImage = transform.Find("Root/MiddleGroup/CoinGroup");

        _tipText = GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/PlayersGroup/Text");
        _prizeText = GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/TGroup/PrizeGroup/NumberText");
        _coinText = GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/CoinGroup/CoinText");
    }

    protected override void OnOpenWindow(TMatch.UIWindowData data)
    {
        base.OnOpenWindow(data);
        _prizeText.SetText(CrocodileActivityModel.Instance.GetBaseConfig().RewardCnt[0].ToString());
        _winPrize = CrocodileActivityModel.Instance.GetMyPrize();
        _coinText.SetText(_winPrize.ToString());
 
        TMatch.EventDispatcher.Instance.DispatchEvent(new ResChangeEvent(ResourceId.TMCoin));
        TMatch.EventDispatcher.Instance.DispatchEvent(new GameItemChangeEvent((int)ItemType.TMCoin));

        ShowLetterOnMyHead();
        AddRobotHead();

        _buttonClose.interactable = false;
        WaitToSetButtonInteractable();
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmLavaquestSuccess,
            CrocodileActivityModel.Instance.Storage.ChallengeTimes.ToString(),_winPrize.ToString());
    }

  
    private async void WaitToSetButtonInteractable()
    {
        await Task.Delay(1500);
        _buttonClose.interactable = true;
    }

    private void ShowLetterOnMyHead()
    {
        UICrocodileMatchController.ShowLetterOnHeadIcon(_myHead, isOther: false);
    }

    private void AddRobotHead()
    {
        _robotHead.gameObject.SetActive(false);

        var robotCount = CrocodileActivityModel.Instance.Storage.RobotCount;
        var maxShowCount = 5;
        var showCount = robotCount > maxShowCount ? maxShowCount : robotCount;
        _tipText.SetTerm("");
        _tipText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormats($"&key.UI_lava_win1", robotCount.ToString()));

        for (var i = 0; i < showCount; i++)
        {
            var head = GameObject.Instantiate(_robotHead, _robotHeadGroup);
            head.gameObject.SetActive(true);

            UICrocodileMatchController.ShowLetterOnHeadIcon(head, isOther: true);
        }
    }

    private async void OnCloseButtonClicked()
    {
        _buttonClose.interactable = false;
        //100001 tm金币
        FlySystem.Instance.FlyItem_Flutter(100001, _winPrize,
            _coinImage.position,
            () =>
            {
                TMatch.UIManager.Instance.CloseWindow<UICrocodileSuccessController>();
                TMatch.UIManager.Instance.CloseWindow<UICrocodileMainController>();
                TMatch.EventDispatcher.Instance.DispatchEvent(TMatch.EventEnum.LobbyRefreshShow);
            }, null, 2.0f);
    }

    
}