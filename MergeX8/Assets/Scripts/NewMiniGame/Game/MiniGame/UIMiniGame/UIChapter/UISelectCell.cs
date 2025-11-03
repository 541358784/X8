using DragonU3DSDK.Asset;
using Framework.UI;
using Framework.Utils;
using MiniGame;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class UISelectCell : UIElement
    {
        public class Data : UIData
        {
            public int subLevelId;
            public int index;
            public int levelId;

            public Data(int subLevelId, int index, int levelId)
            {
                this.subLevelId = subLevelId;
                this.index = index;
                this.levelId = levelId;
            }
        }

        private Image _icon;

        private Data _data;

        protected override void OnCreate()
        {
            base.OnCreate();

            _icon = BindItem<Image>("ImgIcon");

            BindButtonEvent(OnBtnClick);
        }


        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            _data = data as Data;

            var icons = MiniGameModel.Instance.GetSubLevelConfig(_data.subLevelId).SelectIcons;

            _icon.sprite = ResourcesManager.Instance.GetSpriteVariant(MiniGameModel.MiniGameAtlas, $"{icons[_data.index]}");
        }

        private void OnBtnClick()
        {
            EventBus.Send(new EventMiniGameSelectionClicked(_data.index, _data.subLevelId, _data.levelId));
        }
    }
}