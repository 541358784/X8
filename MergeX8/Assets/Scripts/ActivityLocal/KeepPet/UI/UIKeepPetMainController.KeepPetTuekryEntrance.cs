using DragonPlus;
using UnityEngine.UI;

public partial class UIKeepPetMainController
{
    private Button KeepPetTurkeyButton;
    private LocalizeTextMeshProUGUI KeepPetTurkeyButtonText;
    private KeepPetTurkeyShopRedPoint KeepPetTurkeyEntranceRedPointRedPoint;
    public void InitKeepPetTurkey()
    {
        KeepPetTurkeyButton = GetItem<Button>("Root/ButtonRight/KeepPetTurkeyButton");
        KeepPetTurkeyButton.onClick.AddListener(OnClickKeepPetTurkeyBtn);
        KeepPetTurkeyButtonText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonRight/KeepPetTurkeyButton/Text");
        InvokeRepeating("RefreshKeepPetTurkeyBtn",0,1);
        if (KeepPetTurkeyModel.Instance.IsInitFromServer())
        {
            KeepPetTurkeyEntranceRedPointRedPoint = transform.Find("Root/ButtonRight/KeepPetTurkeyButton/RedPoint")
                .gameObject.AddComponent<KeepPetTurkeyShopRedPoint>();
            KeepPetTurkeyEntranceRedPointRedPoint.Init(KeepPetTurkeyModel.Instance.Storage);   
        }
    }
    public void RefreshKeepPetTurkeyBtn()
    {
        KeepPetTurkeyButton.gameObject.SetActive(KeepPetTurkeyModel.Instance.IsOpened());
        if(KeepPetTurkeyButton.gameObject.activeSelf)
            KeepPetTurkeyButtonText.SetText(KeepPetTurkeyModel.Instance.GetActivityLeftTimeString());
    }
    public void OnClickKeepPetTurkeyBtn()
    {
        if (!KeepPetTurkeyModel.Instance.IsOpened())
            return;
        UIKeepPetTurkeyShopController.Open(KeepPetTurkeyModel.Instance.Storage);
    }
}