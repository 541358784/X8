using Activity.TreasureHuntModel;
using DragonPlus;
using DragonPlus.Config.TreasureHunt;
using UnityEngine;
using UnityEngine.UI;
public class UIPopupMonopolyNoHammerController : UIWindowController
{
    private LocalizeTextMeshProUGUI _numText;
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

        _numText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _numText.SetText(TreasureHuntModel.Instance.GetHammer().ToString());
    }

    private void OnBtnPlay()
    {
        AnimCloseWindow(() =>
        {
            var ui=UIManager.Instance.GetOpenedUIByPath<UITreasureHuntMainController>(UINameConst.UITreasureHuntMain);
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
            UIManager.Instance.OpenUI(UINameConst.UIPopupTreasureHuntGift);
        });
    }


    private void OnBtnCLose()
    {
        AnimCloseWindow();
    }

}
