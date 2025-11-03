using DragonPlus;
using Framework.UI;
using Framework.Utils;
using MiniGame;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class UIChapterTop : UIElement
    {
        public LocalizeTextMeshPro _coinText;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _coinText = Transform.Find("UICurrencyColumn/Root/TopGroup/CoinGroup/TextCount").GetComponent<LocalizeTextMeshPro>();
        }
        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            EventBus.Register<EventMiniGameLevelWin>(OnEventMiniGameLevelWin);
        }

        protected override void OnClose()
        {
            base.OnClose();

            EventBus.UnRegister<EventMiniGameLevelWin>(OnEventMiniGameLevelWin);
        }

        private void OnEventMiniGameLevelWin(EventMiniGameLevelWin e)
        {
            _gameObject.SetActive(true);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, true, false);
        }

        public void Show(bool show)
        {
            if(_gameObject == null)
                return;
            
            _gameObject?.SetActive(show);
        }
    }
}