namespace AutoPopupManager
{
    public partial class AutoPopupManager
    {
        private AutoPopUI[] enterHappyGoAutoPopUIArray;

        private void InitHappyGameAutoPopUI()
        {
            enterHappyGoAutoPopUIArray = new AutoPopUI[]
            {
                new AutoPopUI(HappyGoPackModel.CanShowPack, new[] {UINameConst.UIPopupHappyGoGift1,UINameConst.UIPopupHappyGoGift2}),
            };
        }
    }
}