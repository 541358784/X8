using Activity.DiamondRewardModel.Model;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;

public class Aux_DiamondReward : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_DiamondReward Instance;
    private GameObject _redPoint;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint").gameObject;
        _redPoint.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(DiamondRewardModel.Instance.IsOpened());
        
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(DiamondRewardModel.Instance.GetActivityLeftTimeString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIManager.Instance.OpenUI(UINameConst.UIDiamondRewardMain);

        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventDiamondrewardEnter, "1",
            DiamondRewardModel.Instance.DiamondReward.Level.ToString());

    }
    private void OnDestroy()
    {
    }
}
