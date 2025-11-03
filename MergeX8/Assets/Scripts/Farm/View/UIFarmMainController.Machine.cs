using Deco.Node;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Storage;
using Farm.Model;

namespace Farm.View
{
    public partial class UIFarmMainController
    {
        private void TouchMachine(DecoNode node)
        {
            var status = FarmModel.Instance.GetMachineProductStatus(node);

            switch (status)
            {
                case FarmProductStatus.Finish:
                {
                    StorageMachine storage = FarmModel.Instance.GetStorageMachine(node);
                    if(storage == null)
                        break;
                    
                    var config = FarmConfigManager.Instance.GetFarmMachineOrderConfig(storage.OrderId);
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
                    
                    FarmModel.Instance.GainMachineProduct(node);
                    break;
                }
                case FarmProductStatus.Free:
                {
                    _fullClose.gameObject.SetActive(true);
                    AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, false);
                    AnimControlManager.Instance.AnimShow(AnimKey.Farm_Machine, true);
                    GetCombineMono<UIFarmMain_Machine>().UpdateData(node);
                    break;
                }
                case FarmProductStatus.Producing:
                {
                    AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, true);
                    GetCombineMono<UIFarmMain_Control>().UpdateData(node);
                    break;
                }
            }
        }
        private void CancelTouchMachine(DecoNode node)
        {
            _fullClose.gameObject.SetActive(false);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Machine, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, true);
            GetCombineMono<UIFarmMain_Control>().UpdateData();
        }
    }
}