using Deco.Node;
using DragonPlus;
using DragonPlus.Config.Farm;
using Farm.Model;

namespace Farm.View
{
    public partial class UIFarmMainController
    {
        private void TouchAnimal(DecoNode node)
        {
            var status = FarmModel.Instance.GetAnimalProductStatus(node);

            switch (status)
            {
                case FarmProductStatus.Finish:
                {
                    var config = FarmConfigManager.Instance.GetFarmAnimConfig(node.Id);
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
                    FarmModel.Instance.GainAnimalProduct(node);
                    break;
                }
                case FarmProductStatus.Free:
                {
                    var config = FarmConfigManager.Instance.GetFarmAnimConfig(node.Id);
                    if (config == null)
                        break;

                    if (!FarmModel.Instance.HavEnoughProduct(config.ProductCostId, config.ProductNum))
                    {
                        UIPopupFarmNoticeController.OpenNotice(config.ProductCostId, config.ProductNum, config.ProductItem);
                        break;
                    }
                    
                    FarmModel.Instance.FeedAnim(node, FarmConfigManager.Instance.GetFarmAnimConfig(node.Id));
                    FarmModel.Instance.ConsumeProductItem(config.ProductCostId, config.ProductNum);
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

        private void CancelTouchAnimal(DecoNode node)
        {
            
        }
    }
}