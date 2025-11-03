using Activity.TreasureMap;
using DragonPlus;
using UnityEngine.UI;

public partial class UIKeepPetMainController
{
    private Button ThreeOneGiftBtn;
    private LocalizeTextMeshProUGUI ThreeOneGiftBtnText;
    public void InitThreeOneGiftBtn()
    {
        ThreeOneGiftBtn = GetItem<Button>("Root/ButtonRight/KeepPetThreeGift");
        ThreeOneGiftBtn.onClick.AddListener(OnClickThreeOneGiftBtn);
        ThreeOneGiftBtnText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonRight/KeepPetThreeGift/Text");
        InvokeRepeating("RefreshThreeOneGiftBtn",0,1);
    }
    public void RefreshThreeOneGiftBtn()
    {
        ThreeOneGiftBtn.gameObject.SetActive(KeepPetModel.Instance.IsUnlockThreeOneGift() && !Storage.ThreeOneStoreBuyState);
        if(ThreeOneGiftBtn.gameObject.activeSelf)
            ThreeOneGiftBtnText.SetText(KeepPetModel.Instance.GetGiftRestTimeString());
    }
    public void OnClickThreeOneGiftBtn()
    {
        if (!KeepPetModel.Instance.IsUnlockThreeOneGift())
            return;
        if (Storage.ThreeOneStoreBuyState)
            return;
        UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetThreeOneGift);
    }
}