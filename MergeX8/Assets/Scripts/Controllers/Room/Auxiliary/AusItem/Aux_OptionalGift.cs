using System.Collections;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Game;
using Manager;
using OptionalGift;
using UnityEngine.UI;

public class Aux_OptionalGift : Aux_ItemBase
{
    private Image redPoint;
    private LocalizeTextMeshProUGUI timeLabel;
    

    protected override void Awake()
    {
        base.Awake();

        redPoint = transform.Find("RedPoint").GetComponent<Image>();
        timeLabel = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateUI", 0, 1);
    }


    public override void UpdateUI()
    {
        gameObject.SetActive(OptionGiftModel.Instance.IsOpened());
        
        if (!gameObject.activeSelf)
            return;
        redPoint.gameObject.SetActive(false);
        timeLabel.SetText(OptionGiftModel.Instance.GetActivityLeftTimeString());
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
 
        UIManager.Instance.OpenUI(UINameConst.UIPopupOptionalGiftMain);
    }


 

}