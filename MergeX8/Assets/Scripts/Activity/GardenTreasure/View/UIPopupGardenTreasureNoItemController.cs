using Activity.TreasureHuntModel;
using DragonPlus;
using DragonPlus.Config.TreasureHunt;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
public class UIPopupGardenTreasureNoItemController : UIWindowController
{
    private LocalizeTextMeshProUGUI _shovelNumText;
    private LocalizeTextMeshProUGUI _bombNumText;
    private Button _closeBtn;
    private Button _playBtn;
    private Button _buyBtn;

    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnBtnCLose);       
        
        _playBtn = GetItem<Button>("Root/ButtonPlay");
        _playBtn.onClick.AddListener(OnBtnPlay);
   
        _playBtn = GetItem<Button>("Root/ButtonBuy");
        _playBtn.onClick.AddListener(OnBtnBuy);

        _shovelNumText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
        _bombNumText= GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup2/Text");
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _shovelNumText.SetText(UserData.Instance.GetRes(UserData.ResourceId.GardenShovel).ToString());
        _bombNumText.SetText(UserData.Instance.GetRes(UserData.ResourceId.GardenBomb).ToString());
    }

    private void OnBtnPlay()
    {
        AnimCloseWindow(() =>
        {
            var ui=UIManager.Instance.GetOpenedUIByPath<UIGardenTreasureMainController>(UINameConst.UIGardenTreasureMain);
            ui.AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();   
                }
            });
        });
    }

    private void OnBtnBuy()
    {
        AnimCloseWindow(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupGardenTreasureGift, "0");
        });
    }


    private void OnBtnCLose()
    {
        AnimCloseWindow();
    }

}
