using System.Collections.Generic;
using DragonPlus;
using Framework;
using Gameplay;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIEasterStartController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _descText;
    private Button _buttonClose;
    private Button _buttonShow;
    
    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _descText = GetItem<LocalizeTextMeshProUGUI>("Root/BGGroup/Text");
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonShow = GetItem<Button>("Root/Button");
        _buttonShow.onClick.AddListener(OnBtnShow);
        InvokeRepeating("UpdateTimeText",1,1);
        UpdateTimeText();
    }

    private void OnBtnShow()
    {
        AnimCloseWindow(() =>
        {
            if (EasterModel.Instance.IsShowStart())
                return;
            if (string.IsNullOrEmpty(EasterModel.Instance.ActivityId))
                return;
            var mergeItem = MergeManager.Instance.GetEmptyItem();
            var easterConfig = EasterModel.Instance.GetEasterConfig();
            if (easterConfig == null)
                return;
            mergeItem.Id = easterConfig.StartBuild;
            mergeItem.State = 1;
            MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main,1,true);
            EasterModel.Instance.StartActivity();
            ResData res = new ResData(easterConfig.StartBuild, 1);
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonEasterBuildingGet,
                itemAId = mergeItem.Id,
                isChange = true,
            });
            CoroutineManager.Instance.StartCoroutine(FlyGameObjectManager.Instance.ItemFlyLogic(res, () =>
            {
            }, UIHomeMainController.mainController.MainPlayTransform));

        });
}

    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

    private void UpdateTimeText()
    {
        if (EasterModel.Instance.IsPreheating())
        {
            _buttonShow.gameObject.SetActive(false);
            _timeText.SetText(EasterModel.Instance.GetActivityPreheatLeftTimeString());
            _descText.SetTerm("ui_easter_bunny_desc");
        }
        else
        {
            _buttonShow.gameObject.SetActive(true);
            _timeText.SetText(EasterModel.Instance.GetActivityLeftTimeString());
            _descText.SetTerm("ui_easter_bunny_desc2");
        }
    }

}