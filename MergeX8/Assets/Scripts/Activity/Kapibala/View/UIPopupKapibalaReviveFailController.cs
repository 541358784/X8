using System.Threading.Tasks;
using UnityEngine.UI;
using System;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;

namespace TMatch
{
    [AssetAddress("Prefabs/Activity/Kapibala/UIPopupKapibalaReviveFail")]
    public class UIPopupKapibalaReviveFailController : UIPopup
    {
        public override Action EmptyCloseAction => null;
        private Button CloseBtn;
        private Button StartBtn;
        

        public override void OnViewOpen(UIViewParam param)
        {
            AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Failed);
            base.OnViewOpen(param);
            // TMatchDifficulty difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchSystem.LevelController.LevelData.level);
            // CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);
            CloseBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
            CloseBtn.onClick.AddListener(CloseOnClick);
            StartBtn = transform.Find("Root/Button").GetComponent<Button>();
            StartBtn.onClick.AddListener(CloseOnClick);
        }

        public override async Task OnViewClose()
        {
            await base.OnViewClose();
        }
        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<UIPopupKapibalaReviveController>();
            SceneFsm.mInstance.ChangeState(StatusType.BackHome);
            global::UIManager.Instance.extraSiblingIndex = 0;
            global::UIManager.Instance.UpdateUIOrder();
            if (!UIKapibalaMainController.Instance)
                DragonPlus.AudioManager.Instance.PlayMusic(1, true);
            UIKapibalaMainController.Show(false);
            DecoManager.Instance.CurrentWorld.ShowByPosition();
            // global::UIRoot.Instance.EnableEventSystem = true;
        }
    }
}