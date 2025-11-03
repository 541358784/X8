
using DragonU3DSDK.Asset;
using Framework.UI;
using Framework.Utils;
using MiniGame;
using Spine.Unity;
using UnityEngine;

namespace Scripts.UI
{
    public class UIMiniGameButton : UIElement
    {
        private Transform _skeletonGroup;
        private Transform _comingSoonGroup;
        private Transform _newTag;
        private Transform _spineParent;

        protected override void OnCreate()
        {
            base.OnCreate();

            _skeletonGroup = BindItem("SpinePoint");
            _comingSoonGroup = BindItem("ComingSoonGroup");
            _newTag = BindItem("NewTagGroup");
            _spineParent = _transform.Find("SpinePoint");

            BindButtonEvent(OnMiniGameBtnClicked);

            if (_spineParent.childCount > 0) Object.Destroy(_spineParent.GetChild(0).gameObject);
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            var isFinishAllLevel = MiniGameModel.Instance.IsMiniGameAllFinish();
            _skeletonGroup.gameObject.SetActive(!isFinishAllLevel);
            _comingSoonGroup.gameObject.SetActive(isFinishAllLevel);
            _newTag.gameObject.SetActive(MiniGameModel.Instance.HaveNewLevelUnlocked());

            if (MiniGameModel.Instance.IsOpen())
            {
                LoadMiniGameGate();
            }
            else
            {
                GameObject.SetActive(false);
            }

            
            EventBus.Register<EventGuideMaskClick>(OnEventGuideMaskClick);
        }

        protected override void OnClose()
        {
            base.OnClose();

            
            EventBus.UnRegister<EventGuideMaskClick>(OnEventGuideMaskClick);
        }

            
         private void OnEventGuideMaskClick(EventGuideMaskClick e)
         {
             //先注释
             // switch (e.target)
             // {
             //     case GuideTarget.Home_MiniGame:
             //         OnMiniGameBtnClicked();
             //         break;
             // }
         }

        private void OnMiniGameBtnClicked()
        {
            UIMiniGameMain.Open();

            //先注释
            //BITool.SendGameEvent(BiEventMatchRush3D.Types.GameEventType.GameEventMinigameIconClick);
        }

        public void LoadMiniGameGate()
        {
            var miniGame = LoadMiniGameIcon(_spineParent);
            if (miniGame is null) return;

            miniGame.transform.localScale = Vector3.one;
            miniGame.transform.localPosition = Vector3.zero;
            miniGame.transform.localRotation = Quaternion.identity;

            var spine = miniGame.transform.GetComponent<SkeletonGraphic>();
            spine?.MatchRectTransformWithBounds();
        }


        public GameObject LoadMiniGameIcon(Transform parent)
        {
            var chapterId = MiniGameModel.Instance.GetMinUnFinishedChapterId();
            var config = MiniGameModel.Instance.GetChapterConfig(chapterId);
            if (config == null) return null;

            var iconName = config.HomeBtn;
            var prefabPath = $"NewMiniGame/UIEntry/Prefab/{iconName}";

            var originalPrefab = ResourcesManager.Instance.LoadResource<GameObject>(prefabPath);

            return Object.Instantiate(originalPrefab, parent);
        }
    }
}