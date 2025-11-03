using System.Collections.Generic;
using DragonPlus;
using UnityEngine;


public class GameViewBIManager : Singleton<GameViewBIManager>
{
    private Dictionary<string, string> biViews = new Dictionary<string, string>()
    {
        {UINameConst.UIStore, "store"},
        {UINameConst.UIStoreGame, "shop"},
        {UINameConst.UIDailyRV, "tv_reward"},
        {UINameConst.UIPopupBuyEnergy, "energy"},
    };

    public void OpenView(string uiName)
    {
        if (uiName.IsEmptyString())
            return;

        if (!biViews.ContainsKey(uiName))
            return;

    }

    public void CloseView(string uiName)
    {
        if (uiName.IsEmptyString())
            return;

        if (!biViews.ContainsKey(uiName))
            return;

   
    }

    private string GetViewTag()
    {
        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            return "game";

        return "home";
    }
}