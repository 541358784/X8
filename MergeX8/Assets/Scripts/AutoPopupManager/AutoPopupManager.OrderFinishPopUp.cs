namespace AutoPopupManager
{
    public partial class AutoPopupManager
    {
        private AutoPopUI[] taskFinishAutoPopUIArray;
        private void InitFinishAutoPopUI()
        {
            taskFinishAutoPopUIArray = new AutoPopUI[]
            {
                new AutoPopUI(UIPopupMysteryGiftController.CanShowUI, new[] {UINameConst.UIPopupMysteryGift,UINameConst.UIPopupReward}),
            };
        }
    }
}