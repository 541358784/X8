using System.Collections.Generic;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.MiniGame
{
    public class UIPopupMiniGameController : UIWindow
    {
        public enum MiniGameType
        {
            None,
            DigTrench, //挖沟
            ConnectLine,//连线
            Psychology,//心理学
        }

        private GameObject _scrollViewItem;
        private GameObject _selectItem;

        private List<MiniGameInterface> _interfaces = new List<MiniGameInterface>();
        
        public override void PrivateAwake()
        {
            _scrollViewItem = transform.Find("Root/Scroll View").gameObject;
            _scrollViewItem.gameObject.SetActive(false);

            _selectItem = transform.Find("Root/Choose/1").gameObject;
            _selectItem.gameObject.SetActive(false);
            
            transform.Find("Root/CloseButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                ClickUIMask();
            });
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            var setting = Makeover.Utils.GetMiniGameSettings();
            foreach (var config in setting)
            {
                var type = (MiniGameType)config.type;
                type = type == MiniGameType.None ? MiniGameType.DigTrench : type;
                
                // if (!Makeover.Utils.IsOn(type))
                //     continue;

                MiniGameInterface iGameInterface = null;
                switch (type)
                {
                    case MiniGameType.DigTrench:
                    {
                        iGameInterface = new UIMiniGameDigTrench();
                        break;
                    }
                    case MiniGameType.ConnectLine:
                    {
                        iGameInterface = new UIMiniGameConnectLine();
                        break;
                    }
                    case MiniGameType.Psychology:
                    {
                        iGameInterface = new UIMiniGamePsychology();
                        break;
                    }
                }

                if(iGameInterface == null)
                    continue;
                
                var scrollView = GameObject.Instantiate(_scrollViewItem, _scrollViewItem.transform.parent, false);
                scrollView.gameObject.SetActive(true);
                
                var selectItem = GameObject.Instantiate(_selectItem, _selectItem.transform.parent, false);
                selectItem.gameObject.SetActive(true);
                
                iGameInterface.Init(scrollView, selectItem, type, config, SelectType);
                
                _interfaces.Add(iGameInterface);
            }

            SelectType(GetDefaultType());
        }
        
        private MiniGameType GetDefaultType()
        {
            var defaultType = (MiniGameType)StorageManager.Instance.GetStorage<StorageHome>().MiniGame.DefaultType;
            defaultType = defaultType == MiniGameType.None ? MiniGameType.DigTrench : defaultType;
            
            // if (Makeover.Utils.IsOn(defaultType))
            //     defaultType = MiniGameType.DigTrench;

            var setting = Makeover.Utils.GetMiniGameSettings();
            foreach (var config in setting)
            {
                if (defaultType == (MiniGameType) config.type)
                    return defaultType;
            }
            return (MiniGameType)setting[0].type;
        }

        private void SelectType(MiniGameType type)
        {
            foreach (var miniGameInterface in _interfaces)
            {
                miniGameInterface.Select(miniGameInterface._miniGameType == type);
            }
        }
    }
}