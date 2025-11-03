using Activity.TreasureMap;
using DragonPlus;
using UnityEngine.UI;

public partial class UIKeepPetMainController
{
    private Button TreasureMapButton;
    private LocalizeTextMeshProUGUI TreasureMapButtonText;
    public void InitTreasureMap()
    {
        TreasureMapButton = GetItem<Button>("Root/ButtonLeft/TreasureMap");
        TreasureMapButton.onClick.AddListener(OnClickTreasureMapBtn);
        TreasureMapButtonText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonLeft/TreasureMap/Text");
        InvokeRepeating("RefreshTreasureMapBtn",0,1);
    }
    public void RefreshTreasureMapBtn()
    {
        TreasureMapButton.gameObject.SetActive(TreasureMapModel.Instance.IsOpen());
        if(TreasureMapButton.gameObject.activeSelf)
            TreasureMapButtonText.SetText(TreasureMapModel.Instance.GetActivityLeftTimeString());
    }
    public void OnClickTreasureMapBtn()
    {
        if (!TreasureMapModel.Instance.IsOpen())
            return;
        UIManager.Instance.OpenUI(UINameConst.UIPopupTreasureMap);
    }
}