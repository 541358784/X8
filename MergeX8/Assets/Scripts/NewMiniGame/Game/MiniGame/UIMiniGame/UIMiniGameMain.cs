// #define MINI_GAME_DEBUG

using ASMR;
using Framework.UI;
using Framework.Utils;
using MiniGame;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class UIMiniGameMain : UIPopupView
    {
        protected override bool AutoBgClose => false;

        private Transform _content;
        private GameObject _chapterItemCell;

        public Transform Cell1;
        public Transform ExitBtn => _closeBtn.transform;

        public static void Open()
        {
            UIManager.Instance.OpenWindow(UINameConst.UIPopupTask, true);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            BindItems();
        }

        private void BindItems()
        {
            const string contentPath = "Root/MiddleGroup/Scroll View/Viewport/Content";
            const string cellPath = contentPath + "/ChapterItem";

            _content = BindItem(contentPath);
            _chapterItemCell = BindItem(cellPath).gameObject;
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            InitLayout(out var cellCnt);
            InitLocation(cellCnt);
            
             EventBus.Register<EventGuideMaskClick>(OnEventGuideMaskClick);
            // BITool.SendGameEvent(BiEventMatchRush3D.Types.GameEventType.GameEventMinigameUiOpen);
        }

        protected override void OnCloseBtnClick()
        {
            base.OnCloseBtnClick();

            //先注释
             // if (StorageBundle.Instance.TMatch.MainLevelId == 1)
             // {
             //     GuideTool.Instance.StartGuide(GuideTriggerType.EnterHome, $"Login-{StorageBundle.Instance.TMatch.MainLevelId}");
             // }
             // else
             // {
             //     GuideTool.Instance.StartGuide(GuideTriggerType.CloseMiniGameMain, $"UIMiniGameMainClose-{StorageBundle.Instance.TMatch.MainLevelId}");
             // }
            
             EventBus.Send<EventFinishCurrHomeTask>();
        }

        public override void OnOpenFinish()
        {
            base.OnOpenFinish();

             var minUnFinishedChapter = MiniGameModel.Instance.GetMinUnFinishedChapterId();
             var minLevel = MiniGameModel.Instance.GetMinUnfinishLevelInChapter(minUnFinishedChapter);
             // 先注释
             // GuideTool.Instance.StartGuide(GuideTriggerType.OpenUI, $"UIMiniGameMain-{minUnFinishedChapter}-{minLevel}");
        }

        protected override void OnClose()
        {
            base.OnClose();
            EventBus.UnRegister<EventGuideMaskClick>(OnEventGuideMaskClick);
        }

        /// <summary>
        /// 初始化章节列表
        /// </summary>
        private void InitLayout(out int cellCnt)
        {
            var totalChapterCount = MiniGameModel.Instance.ChapterConfigList.Count;

            // 初始化章节列表
            for (var i = 1; i <= totalChapterCount + 1; i++)
            {
                var newChapterItemCell = i == 1 ? _chapterItemCell : GameObject.Instantiate(_chapterItemCell, _content);
                var cell = BindElement<UIChapterCell>(newChapterItemCell, new ChapterItemCellData
                {
                    chapterId = i,
                    isComingSoon = i > totalChapterCount,
                });

                if (i == 1) Cell1 = cell.Transform;
            }

            cellCnt = totalChapterCount + 1;
        }

        /// <summary>
        /// 滚动定位到进展中的章节
        /// </summary>
        private void InitLocation(int cellCnt)
        {
            const int showCenter = 1;

            var cellHeight = _chapterItemCell.GetComponent<RectTransform>().sizeDelta.y;
            var contentPosIndex = cellCnt / 2f;
            var moveDelta = contentPosIndex - showCenter;
            var moveDeltaIntPart = (int)moveDelta;
            var spacing = _content.GetComponent<VerticalLayoutGroup>().spacing;
            var move = moveDelta * cellHeight + moveDeltaIntPart * spacing;

            var chapterUnlock = MiniGameModel.Instance.GetMinUnFinishedChapterId();
            var chapterCenter = chapterUnlock - 0.5f;
            var delta = chapterCenter - contentPosIndex;
            move += delta * cellHeight;

            var rectPos = _content.GetComponent<RectTransform>().anchoredPosition;
            rectPos.y += move;
            _content.GetComponent<RectTransform>().anchoredPosition = rectPos;
        }

         private void OnEventGuideMaskClick(EventGuideMaskClick e)
         {
             // switch (e.target)
             // {
             //     case GuideTarget.MiniGame_Main_Exit:
             //         OnCloseBtnClick();
             //         break;
             // }
         }
    }
}