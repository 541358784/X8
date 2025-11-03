using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Game;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_StarBoxMono : Aux_ItemBase
{
    public Slider _slider;

    public StorageHome storageHome;

    //public TableStarChest starChest;
    public Animator iconAni;


    protected override void Awake()
    {
        _slider = transform.Find("Slider").GetComponent<Slider>();
        iconAni = transform.Find("Icon").GetComponent<Animator>();
        UpdateUI();
        EventDispatcher.Instance.AddEventListener(EventEnum.BackHomeStep, BackHomeStep);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(false);
        storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        //starChest = GlobalConfigManager.Instance.GetTableStarChest(storageHome.StarBoxCount);
    }


    protected override void OnButtonClick()
    {
        //var list = RewardModal.Instance.GetChestReward(starChest.chestId);
        //foreach (var resData in list)
        //{
        //    var reason = new GameBIManager.ItemChangeReasonArgs
        //    {
        //        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.StarChest,
        //    };
        //}
        ////扣星
        //storageHome.StarBoxCount += 1;
        //AudioManager.Instance.PlaySound(SfxNameConst.sfx_star_reward);
        //UpdateUI();
        //UIRewardBoxController.ShowAndUpdateUserData(list,UIRewardBoxController.ChestType.Star, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.StarChest), () =>
        //{

        //});

        //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFteFirstStarChestTap, "StarChestTap");
        //GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StarBox);
    }


    private void BackHomeStep(BaseEvent e)
    {
        if (e.datas == null || e.datas.Length == 0)
            return;

        if ((string) e.datas[0] != "guide")
            return;


        //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFteFirstStarChestPop, "StarChestPop");
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BackHomeStep, BackHomeStep);
    }
}