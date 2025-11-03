using Activity.TreasureHuntModel;
using DragonPlus;
using DragonPlus.Config.TreasureHunt;
using UnityEngine;
using UnityEngine.UI;
public class UIPopupTreasureHuntGiftController : UIWindowController
{
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _timeText;

    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnBtnCLose);
        for (int i = 1; i <= 3; i++)
        {
           var item=  transform.Find("Root/Gift" + i).gameObject.AddComponent<TreasureHuntGiftItem>();
           item.Init(TreasureHuntConfigManager.Instance.TreasureHuntStoreConfigList[i-1]);
        }
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("RefreshTime",0,1);
    }
    public void RefreshTime()
    {
        _timeText.SetText(TreasureHuntModel.Instance.GetActivityLeftTimeString());
    }
    private void OnBtnCLose()
    {
        AnimCloseWindow();
    }

}
