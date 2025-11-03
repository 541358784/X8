using System;
using DragonPlus.Config.Team;
using Scripts.UI;
using UnityEngine.UI;

public class UIPopupGuildSetIconController : UIWindowController
{
    public static UIPopupGuildSetIconController Instance;
    public static UIPopupGuildSetIconController Open(Action<int> callback)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildSetIcon,callback) as UIPopupGuildSetIconController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
    }
    
    public Action<int> Callback;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Callback = objs[0] as Action<int>;
        var iconList = TeamConfigManager.Instance.IconConfigList;
        var defaultItem = transform.Find("Root/Scroll View/Viewport/Content/Icon");
        defaultItem.gameObject.SetActive(false);
        foreach (var config in iconList)
        {
            var curConfig = config;
            if (curConfig.IsNeedCollect)
                continue;
            var item = Instantiate(defaultItem, defaultItem.parent);
            item.gameObject.SetActive(true);
            item.GetComponent<Image>().sprite = TeamManager.Instance.GetBadgeIconSprite(curConfig.Id);
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                Callback?.Invoke(curConfig.Id);
                AnimCloseWindow();
            });
        }
    }
}