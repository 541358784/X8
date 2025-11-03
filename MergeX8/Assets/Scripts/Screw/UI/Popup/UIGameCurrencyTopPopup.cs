using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIGameCurrencyTop")]
    public class UIGameCurrencyTopPopup : UIWindowController
    {
        [UIBinder("CurrencyLive")]
        private Transform energyTransform;
        
        [UIBinder("CurrencyCoin")]
        private Transform coinTransform;

        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);
        }
        
        public void OnCreate()
        {
            //错误注释
            // base.OnCreate();
            // CreateWidget<CoinWidget>(coinTransform.gameObject);
            // CreateWidget<EnergyWidget>(energyTransform.gameObject);
        }
    }
}