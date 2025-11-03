using DragonPlus;
using UnityEngine.UI;

public class UITMWinPrize:TMatch.UIWindowController
{
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI TimeText;
    public override void PrivateAwake()
    {
        CloseBtn = transform.Find("Root/CloseBtn").GetComponent<Button>();
        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0,1);
    }

    public void UpdateTime()
    {
        if (TMWinPrizeModel.Instance.IsOpened())
        {
            TimeText.SetText(TMWinPrizeModel.Instance.GetActivityLeftTimeString());
        }
        else
        {
            CloseWindowWithinUIMgr();
        }
    }
    
    private const string PREFAB_PATH = "Prefabs/Activity/TMatch/TMWinPrize/UITMWinPrize";
    public static UITMWinPrize Open()
    {
        return TMatch.UIManager.Instance.OpenWindow<UITMWinPrize>(PREFAB_PATH);
    }
}