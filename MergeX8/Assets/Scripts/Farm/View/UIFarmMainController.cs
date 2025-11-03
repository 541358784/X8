using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public partial class UIFarmMainController: UIWindowController
    {
        public enum CombineType : int
        {
            Level,
            Machine,
            Order,
            Seed,
            Control,
            BuyPanel,
        }
        
        private string[] combinePath = new string[]
        {
            "Root/TopGroup/Lv",
            "Root/Machine",
            "Root/TopGroup/Scroll View",
            "Root/Seed",
            "Root/BottomGroup",
            "Root/BuyPanel",
        };

        private Type[] types = new Type[]
        {
            typeof(UIFarmMain_Level),
            typeof(UIFarmMain_Machine),
            typeof(UIFarmMain_Order),
            typeof(UIFarmMain_Seed),
            typeof(UIFarmMain_Control),
            typeof(UIFarmMain_Buy),
        };
        
        private Dictionary<CombineType, IInitContent> _dicCombineMono = new Dictionary<CombineType, IInitContent>();

        private GameObject _fullClose;
        public static UIFarmMainController Instance;
        
        public override void PrivateAwake()
        {
            Instance = this;
            CommonUtils.AdaptRectTransformTop((RectTransform)transform.Find("Root"), CommonUtils.SafeAreaOffset());
            
            _fullClose = transform.Find("Root/FullScreenClose").gameObject;
            _fullClose.transform.GetComponent<Button>().onClick.AddListener(ClickFullClose);
            _fullClose.gameObject.SetActive(false);
            
            
            foreach (CombineType type in Enum.GetValues(typeof(CombineType)))
            {
                int index = (int)type;
                var mono = transform.Find(combinePath[index]).gameObject.AddComponent(types[index]);

                var iface =  mono as IInitContent;
                
                iface.InitContent(this);
                _dicCombineMono.Add(type, iface);

                switch (type)
                {
                    case CombineType.Level:
                        break;
                    case CombineType.Control:
                        AnimControlManager.Instance.InitAnimControl(AnimKey.Farm_Contrl, mono.gameObject, false);
                        break;
                    case CombineType.Seed:
                        AnimControlManager.Instance.InitAnimControl(AnimKey.Farm_Seed, mono.gameObject, false);
                        break;
                    case CombineType.Machine:
                        AnimControlManager.Instance.InitAnimControl(AnimKey.Farm_Machine, mono.gameObject, false);
                        break;
                    case CombineType.BuyPanel:
                        AnimControlManager.Instance.InitAnimControl(AnimKey.Farm_Buy, mono.gameObject, false);
                        break;
                }
            }
            
            AnimControlManager.Instance.InitAnimControl(AnimKey.Farm_Top, transform.Find("Root/TopGroup").gameObject, false);
            
            
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Top, true, true);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, true, true);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Seed, false, true);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Machine, false, true);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Buy, false, true);

            RegisterEvent();
        }

        private void OnDestroy()
        {
            UnRegisterEvent();
            
            AnimControlManager.Instance.RemoveAnim(AnimKey.Farm_Contrl);
            AnimControlManager.Instance.RemoveAnim(AnimKey.Farm_Seed);
            AnimControlManager.Instance.RemoveAnim(AnimKey.Farm_Machine);
            AnimControlManager.Instance.RemoveAnim(AnimKey.Farm_Buy);
            AnimControlManager.Instance.RemoveAnim(AnimKey.Farm_Top);
        }

        private void ClickFullClose()
        {
            _fullClose.gameObject.SetActive(false);
            
            AnimControlManager.Instance.AnimShow(AnimKey.Main_Group, true);

            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, true);
            
            
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Seed, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Machine, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Buy, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, true);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Top, true);
        }

        public T GetCombineMono<T>()
        {
            int index = -1;
            for (var i = 0; i < types.Length; i++)
            {
                if(typeof(T) != types[i])
                    continue;

                index = i;
                break;
            }
            
            if(index < 0)
                return default(T);

            return (T)_dicCombineMono[(CombineType)index];
        }

        public void SetFullCloseActive(bool isActive)
        {
            _fullClose.gameObject.SetActive(isActive);
        }
    }
}