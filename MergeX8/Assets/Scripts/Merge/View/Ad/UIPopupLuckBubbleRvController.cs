using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupLuckBubbleRvController : UIWindowController
{
    private Button _buttonClose;
    private Button _buttonAd;

    private CommonRewardItem _rewardItem;
    private StorageMergeItem _storageMergeItem;

    private TextMeshProUGUI _textTime;

    private Transform _inTag;
    
    private int _bubbleIndex = -1;

    private bool _canUseActiveIn = false;
    private string _activeInPlaceId;

    public override void PrivateAwake()
    {
        _buttonClose = transform.Find("Root/ButtonClose").GetComponent<Button>();
        _buttonAd = transform.Find("Root/WatchButton").GetComponent<Button>();
        
        _inTag = transform.Find("Root/WatchButton/Tip");

        _buttonClose.onClick.AddListener((() => AnimCloseWindow()));


        _rewardItem = transform.Find("Root/ItemGroup/Item").gameObject.AddComponent<CommonRewardItem>();

        _textTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<TextMeshProUGUI>();


        InvokeRepeating(nameof(UpdateCdTime), 0, 1);
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _bubbleIndex = (int)objs[0];
        _canUseActiveIn = (bool)objs[1];
        _activeInPlaceId = (string)objs[2];
        _storageMergeItem = MergeManager.Instance.GetBoardItem(_bubbleIndex, MergeBoardEnum.Main);
        _rewardItem.Init(new ResData(_storageMergeItem.Id, 1));
        _inTag.gameObject.SetActive(_canUseActiveIn);
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip, AdLocalOperate.Open);

        if (_canUseActiveIn)
        {
            _buttonAd.onClick.AddListener((() =>
            {
                MergeManager.Instance.PauseAllCdTime(MergeBoardEnum.Main);
                AdSubSystem.Instance.PlayInterstital(_activeInPlaceId, (b =>
                        {
                            if (b)
                            {
                                AdSubSystem.Instance.ActiveInToRvGet(ADConstDefine.RV_BUBBLE_OPEN);
                                BreakBubble();
                            }
                        }
                    ));
            }));
        }
        else
        {
            UIAdRewardButton.Create(ADConstDefine.RV_BUBBLE_OPEN, UIAdRewardButton.ButtonStyle.Disable,
                _buttonAd.gameObject,
                (s) =>
                {
                    if (s)
                        BreakBubble();
                }
                , true,
                onBtnClick: () => { MergeManager.Instance.PauseAllCdTime(MergeBoardEnum.Main); }
            );
        }
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip, AdLocalOperate.Close,AdLocalSkipScene.LuckBubble);
    }


    private void BreakBubble()
    {
        int selectGridId = MergeManager.Instance.GetBoardItem(_bubbleIndex, MergeBoardEnum.Main).Id;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBubbleClickRv);

        MergeManager.Instance.ResumAllCdTime(MergeBoardEnum.Main);
        MergeManager.Instance.SetBoardItem(_bubbleIndex, _storageMergeItem.Id, 1, RefreshItemSource.webUnlock,
            MergeBoardEnum.Main);

        MergeMainController.Instance.MergeBoard.GetGridByIndex(_bubbleIndex).board
            .PlayBubbleAnimation("disappear");
        SendBubbleOpen(selectGridId, "rv");
        //x8 平移 错误注释
        EventDispatcher.Instance.DispatchEvent(EventEnum.BreakBubble);

        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip, AdLocalOperate.Operate);
        CloseWindowWithinUIMgr(true);
    }

    public void SendBubbleOpen(int id, string biType)
    {
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (itemConfig == null)
            return;
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeBubble,
            itemAId = itemConfig.id,
            ItemALevel = itemConfig.level,
            isChange = true,
            extras = new Dictionary<string, string>
            {
                { "type", biType }
            }
        });
    }

    private void UpdateCdTime()
    {
        if (_bubbleIndex < 0)
            return;

        int cd = MergeManager.Instance.GetBubbleLeftCdTime(_bubbleIndex, MergeBoardEnum.Main);
        if (cd == 0)
        {
            CloseWindowWithinUIMgr(true);
        }
        else
        {
            _textTime.SetText(TimeUtils.GetTimeString(cd));
        }
    }
    
}