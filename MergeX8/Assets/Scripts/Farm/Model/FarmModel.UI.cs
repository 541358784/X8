using Farm.View;
using UnityEngine;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public Transform MainPlayTransform
        {
            get;
            set;
        }
        
        public Transform LevelUpTransform
        {
            get;
            set;
        }
        
        public Transform WarehouseTransform
        {
            get;
            set;
        }

        public void AnimShow(bool isShow, bool isImmediately = true)
        {
            if (isShow)
            {
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, true, isImmediately);
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Top, true, isImmediately);
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Seed, false, isImmediately);
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Machine, false, isImmediately);
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Buy, false, isImmediately);
            }
            else
            {
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, isShow, isImmediately);
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Top, isShow, isImmediately);
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Seed, isShow, isImmediately);
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Machine, isShow, isImmediately);
                AnimControlManager.Instance.AnimShow(AnimKey.Farm_Buy, isShow, isImmediately);

                UIWindow window = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIFarmMain);
                if (window != null)
                    ((UIFarmMainController)window).SetFullCloseActive(false);
            }
        }
    }
}