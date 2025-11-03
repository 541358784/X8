using Activity.TreasureMap;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupKeepPetPatrolRewardController : UIWindowController
{
    public static UIPopupKeepPetPatrolRewardController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetPatrolReward) as
            UIPopupKeepPetPatrolRewardController;
    }
    private Button CloseBtn;
    private Transform TreasureMapGroup;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(()=>AnimCloseWindow());
        TreasureMapGroup = GetItem<Transform>("Root/RewardGroup/Map");
        if (TreasureMapGroup)
            TreasureMapGroup.gameObject.SetActive(TreasureMapModel.Instance.IsOpen());
    }
}