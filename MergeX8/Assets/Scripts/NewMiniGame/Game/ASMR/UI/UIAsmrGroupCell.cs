using System;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework.UI;
using MiniGame;

namespace ASMR
{
    public class UIAsmrGroupCell : UIElement
    {
        public enum State
        {
            Normal,
            Actvie,
            Finished
        }

        private Transform _normalGroup;
        private Transform _currentGroup;
        private Transform _countGroup;
        private Transform _finishGroup;
        private LocalizeTextMeshProUGUI _countText;
        private Image _icon;
        private Image _icon2;

        protected override void OnCreate()
        {
            base.OnCreate();

            _normalGroup = BindItem("NormalGroup");
            _currentGroup = BindItem("NowGroup");
            _countGroup = BindItem("CountGroup");
            _finishGroup = BindItem("ImgFinish");
            _countText = BindItem<LocalizeTextMeshProUGUI>("CountGroup/Text");
            _icon = BindItem<Image>("NowGroup/ImgIcon");
            _icon2 = BindItem<Image>("NormalGroup/ImgIcon");
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            SetState(State.Normal);
        }

        public void InitUI(int stepCount, int totalStepCount, string iconName)
        {
            _countText.SetText($"{stepCount}/{totalStepCount}");
            _icon.sprite = ResourcesManager.Instance.GetSpriteVariant(MiniGameModel.MiniGameAtlas, iconName);
            _icon2.sprite = _icon.sprite;
        }

        public void SetProgress(int stepCount, int totalStepCount)
        {
            _countText.SetText($"{stepCount}/{totalStepCount}");
        }

        public void SetState(State state)
        {
            _normalGroup.gameObject.SetActive(state == State.Normal);
            _currentGroup.gameObject.SetActive(state == State.Actvie);
            _countGroup.gameObject.SetActive(state == State.Actvie);
            _finishGroup.gameObject.SetActive(state == State.Finished);
        }
    }
}