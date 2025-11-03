using System.Collections;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Game;
using UnityEngine.UI;

public class Aux_DailyBonus : Aux_ItemBase
{
    private IEnumerator iEnumerator = null;
    private Image redPoint;

    protected override void Awake()
    {
        base.Awake();

        redPoint = transform.Find("RedPoint").GetComponent<Image>();
        InvokeRepeating("UpdateUI", 0, 1);
    }


    public override void UpdateUI()
    {
       gameObject.SetActive(false);
        if (!gameObject.activeSelf)
            return;
    }

    private void StopDelayWork()
    {
        if (iEnumerator == null)
            return;

        StopCoroutine(iEnumerator);
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        // if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyBonus))
        // {
        //     UIManager.Instance.OpenUI(UINameConst.UIDailyBouns);
        //     GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDailySignInDesc, 
        //         DailyBonusModel.Instance.CheckConsecutiveLoginDays().ToString(),StorageManager.Instance.GetStorage<StorageHome>().DailyBonus.TotalClaimDay.ToString() );
        //
        // }
    }


    private void OnDestroy()
    {
        StopDelayWork();
    }

}