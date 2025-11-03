using UnityEngine.UI;

public class UIPopupButterflyWorkShopEndController:UIWindowController
{
    private Button _playBtn;
    private Button _closeBtn;
    public override void PrivateAwake()
    {
        _playBtn = GetItem<Button>("Root/Button");
        _playBtn.onClick.AddListener(OnPlayBtn);
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnCloseBtn);
    }

    public void OnPlayBtn()
    {
        AnimCloseWindow(() =>
        {       
            var window = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIButterflyWorkShopMain);
            window?.AnimCloseWindow();
        });
    }

    public void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
            var window = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIButterflyWorkShopMain);
            window?.AnimCloseWindow();
        });
    }
    
   
}