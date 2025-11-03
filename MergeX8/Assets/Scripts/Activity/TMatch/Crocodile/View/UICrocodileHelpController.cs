using DragonPlus;
using UnityEngine.UI;

public class UICrocodileHelpController : TMatch.UIWindowController
{
    #region open ui

    /// <summary>
    /// 预制体路径
    /// </summary>
    private const string PREFAB_PATH = "Prefabs/Activity/TMatch/Crocodile/UICrocodileHelp";

    /// <summary>
    /// 打开
    /// </summary>
    public static void Open()
    {
        TMatch.UIManager.Instance.OpenWindow<UICrocodileHelpController>(PREFAB_PATH);
    }

    #endregion
    private LocalizeTextMeshProUGUI _rewardTipText;
    public override void PrivateAwake()
    {
        GetItem<Button>("Root/ButtonClose").onClick.AddListener(OnCloseButtonClicked);
        _rewardTipText = GetItem<LocalizeTextMeshProUGUI>("Root/3/Text");
    }

    protected override void OnOpenWindow(TMatch.UIWindowData data)
    {
        base.OnOpenWindow(data);
        _rewardTipText.SetTerm("");
        _rewardTipText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_lava_rules3",
            CrocodileActivityModel.Instance.GetBaseConfig().RewardCnt[0].ToString()));
    }

   

    private void OnCloseButtonClicked()
    {
        CloseWindowWithinUIMgr();
    }

  
}