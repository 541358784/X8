using Deco.Node;
using DragonPlus;
using DragonPlus.Config.Farm;
using Farm.Model;

namespace Farm.View
{
    public partial class UIFarmMainController
    {
        private void TouchTree(DecoNode node)
        {
            var status = FarmModel.Instance.GetTreeProductStatus(node);

            switch (status)
            {
                case FarmProductStatus.Finish:
                {
                    var config = FarmConfigManager.Instance.GetFarmTreeConfig(node.Id);
                    if (config == null)
                        break;

                    if (!FarmModel.Instance.IsWarehouseEnough(config.ProductNum))
                    {  
                        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                        {
                            TitleString = LocalizationManager.Instance.GetLocalizedString("UI_farminfo_barnFull_Title"),
                            DescString = LocalizationManager.Instance.GetLocalizedString("UI_farminfo_barnFull_Desc"),
                            OKButtonText = LocalizationManager.Instance.GetLocalizedString("UI_farminfo_barnFull_Button"),
                            HasCancelButton = false,
                            HasCloseButton = false,
                            OKCallback = ()=>UIManager.Instance.OpenUI(UINameConst.UIPopupFarmBag),
                        });
                        break;
                    }
                    
                    FarmModel.Instance.GainTreeProduct(node);
                    break;
                }
                case FarmProductStatus.Free:
                {
                    FarmModel.Instance.GrowTree(node);
                    break;
                }
                case FarmProductStatus.Producing:
                {
                    AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, false);
                    AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, true);
                    GetCombineMono<UIFarmMain_Control>().UpdateData(node);
                    break;
                }
            }
        }
        private void CancelTouchTree(DecoNode node)
        {
            
        }
    }
}