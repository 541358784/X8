using UnityEngine.UI;

public class UIBiuBiuHelpController : UIWindowController
{

    public static UIBiuBiuHelpController Instance;
    public static UIBiuBiuHelpController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIBiuBiuHelp) as UIBiuBiuHelpController;
        return Instance;
    }
    
    private Button _playBtn;
    private Button _closeBtn;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnCloseBtn);
    }

    public void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
        });
    }
}