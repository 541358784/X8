
using System.Collections.Generic;
using Activity.TreasureMap;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UITreasureMapRewardController : UIWindowController
{
    private Button _closeBtn;
    private Image _icon;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnBtnClose);
        _icon = GetItem<Image>("Root/RewardIcon");
     
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        AudioManager.Instance.PlaySoundById(145);
    }

   
    
    private void OnBtnClose()
    {
        CloseWindowWithinUIMgr(true);        
    }

    public static UITreasureMapRewardController Open()
    {
        if (!TreasureMapModel.Instance.IsOpen())
        {
            return null;
        }
        return UIManager.Instance.OpenUI(UINameConst.UITreasureMapReward) as UITreasureMapRewardController;
    }
}
