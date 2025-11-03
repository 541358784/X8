
using UnityEngine.UI;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using System;
using Decoration;
using DragonPlus;


namespace TMatch
{
    [AssetAddress("Prefabs/Activity/Kapibala/UIPopupKapibalaWin")]
    public class UIPopupKapibalaWinController : UIPopup
    {
        public override string CloseAnimStateName => "";
        public override Action EmptyCloseAction => null;
        private List<TMatchRewardItemData> rewardList;

        // 物品显示：
        // 动态显示：限时活动收集物品、周挑战收集物品、公会收集物品 对应玩家本局已经临时收集到的类型和数量；收集的活动元素类型和数量
        
        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);
            transform.Find($"Root/Button").GetComponent<Button>().onClick.AddListener(CloseOnClick);
            transform.Find($"Root/CloseButton").GetComponent<Button>().onClick.AddListener(CloseOnClick);
        }

        private void CloseOnClick()
        {
            // if (RemoveAdModel.Instance.IsUnlock() && (!StorageManager.Instance.GetStorage<StorageTMatch>().RemoveAd.RemoveAd) && AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.TM_FINISH_GAME))
            // {
            //     AdSubSystem.Instance.PlayInterstital(ADConstDefine.TM_FINISH_GAME, b =>
            //     {
            //         RemoveAdModel.Instance.TryToAutoOpen();
            //     });
            // }
            UIViewSystem.Instance.Close<UITMatchWinController>();

            SceneFsm.mInstance.ChangeState(StatusType.BackHome);
            global::UIManager.Instance.extraSiblingIndex = 0;
            global::UIManager.Instance.UpdateUIOrder();
            if (!UIKapibalaMainController.Instance)
                DragonPlus.AudioManager.Instance.PlayMusic(1, true);
            UIKapibalaMainController.Show(true);
            DecoManager.Instance.CurrentWorld.ShowByPosition();
            // global::UIRoot.Instance.EnableEventSystem = true;
        }
    }
}