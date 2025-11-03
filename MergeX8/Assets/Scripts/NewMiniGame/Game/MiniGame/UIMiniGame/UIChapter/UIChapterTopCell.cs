
using DragonPlus;
using Framework.UI;
using UnityEngine;

namespace Scripts.UI
{
    public class UIChapterTopCell : UIElement
    {
        private LocalizeTextMeshProUGUI _text;
        private Transform _finishTag;

        protected override void OnCreate()
        {
            base.OnCreate();

            _text = BindItem<LocalizeTextMeshProUGUI>("Text");
            _finishTag = BindItem("ImgFinish");
        }

        public void Init(int index, int levelId)
        {
            _text.SetText($"{index + 1}");
        }

        public void SetFinishStatus(bool isFinish)
        {
            _finishTag.gameObject.SetActive(isFinish);
        }
    }
}