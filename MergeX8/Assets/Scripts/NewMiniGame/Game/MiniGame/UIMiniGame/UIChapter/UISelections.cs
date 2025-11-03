using Framework.UI;
using Framework.Utils;
using MiniGame;
using UnityEngine;

namespace Scripts.UI
{
    public class UISelections : UIElement
    {
        private Transform _defaultSelect;


        protected override void OnCreate()
        {
            base.OnCreate();

            _defaultSelect = BindItem("ButtonSelectItem");
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            GameObject.SetActive(false);
            _defaultSelect.gameObject.SetActive(false);

             EventBus.Register<EventMiniGameLevelFailRetryClicked>(OnEventMiniGameLevelFailRetryClicked);
             EventBus.Register<EventMiniGameLevelWin>(OnEventMiniGameLevelWin);
             EventBus.Register<EventMiniGameSelectionClicked>(OnEventMiniGameSelectionClicked);
        }

        protected override void OnClose()
        {
            base.OnClose();

             EventBus.UnRegister<EventMiniGameLevelFailRetryClicked>(OnEventMiniGameLevelFailRetryClicked);
             EventBus.UnRegister<EventMiniGameLevelWin>(OnEventMiniGameLevelWin);
             EventBus.UnRegister<EventMiniGameSelectionClicked>(OnEventMiniGameSelectionClicked);
        }

        private void OnEventMiniGameLevelFailRetryClicked(EventMiniGameLevelFailRetryClicked e)
        {
            Show(true);
        }

        private void OnEventMiniGameLevelWin(EventMiniGameLevelWin e)
        {
            Show(false);
        }

        public void Show(bool show)
        {
            _gameObject.SetActive(show);
        }

        private void OnEventMiniGameSelectionClicked(EventMiniGameSelectionClicked e)
        {
            Show(false);
        }

        public void InitSelections(int subLevelId, int levelId)
        {
            const int count = 3;

            _defaultSelect.gameObject.SetActive(true);
            while (_transform.childCount < 3) Object.Instantiate(_defaultSelect.gameObject, _transform);

            for (var i = 0; i < count; i++)
            {
                BindElement<UISelectCell>(_transform.GetChild(i).gameObject, new UISelectCell.Data(subLevelId, i, levelId));
            }
        }
    }
}