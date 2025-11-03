using Deco.Node;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Storage;
using Farm.Model;

namespace Farm.View
{
    public partial class UIFarmMainController
    {
        private void TouchGround(DecoNode node)
        {
            var status = FarmModel.Instance.GetGroundProductStatus(node);

            switch (status)
            {
                case FarmProductStatus.Finish:
                {
                    StorageGround storage = FarmModel.Instance.GetStorageGround(node);
                    if(storage == null)
                        break;
                    
                    var config = FarmConfigManager.Instance.GetFarmSeed(storage.SeedId);
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
                    
                    FarmModel.Instance.GainGroundProduct(node);
                    break;
                }
                case FarmProductStatus.Free:
                {
                    _fullClose.gameObject.SetActive(true);
                    AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, false);
                    AnimControlManager.Instance.AnimShow(AnimKey.Farm_Seed, true);
                    GetCombineMono<UIFarmMain_Seed>().UpdateData(node);
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

        private void CancelTouchGround(DecoNode node)
        {
            _fullClose.gameObject.SetActive(false);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Seed, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, true);
            GetCombineMono<UIFarmMain_Control>().UpdateData();
        }
    }
}