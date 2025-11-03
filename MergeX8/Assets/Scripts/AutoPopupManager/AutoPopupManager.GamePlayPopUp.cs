using Gameplay.UI.EnergyTorrent;

namespace AutoPopupManager
{
    public partial class AutoPopupManager
    {
        private AutoPopUI[] gamePlayingAutoPopUIArray;

        private void InitGamePlayingAutoPopUI()
        {
            gamePlayingAutoPopUIArray = new AutoPopUI[]
            {
                new AutoPopUI(EnergyTorrentModel.Instance.CanShowUIFix, new[] {UINameConst.UIPopupEnergyTorrentMain,UINameConst.UIPopupEnergyTorrentMainX8,UINameConst.UIPopupEnergyTorrentStart,UINameConst.UIStore,UINameConst.UIPopupReward}),
                new AutoPopUI(EnergyTorrentModel.Instance.CanShowUIFix, ()=>CardCollectionModel.Instance.GetCurrentMainPopupPath().ToArray()),
                new AutoPopUI(EnergyTorrentModel.Instance.CanShowUI, new[] {UINameConst.UIPopupEnergyTorrentMain,UINameConst.UIPopupEnergyTorrentMainX8,UINameConst.UIPopupEnergyTorrentStart,UINameConst.UIStore,UINameConst.UIPopupReward}),
            };
        }
    }
}