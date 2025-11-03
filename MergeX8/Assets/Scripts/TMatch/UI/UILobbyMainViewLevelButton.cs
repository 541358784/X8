using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class UILobbyMainViewLevelButton : UIView
    {
        public static int MAXLevel = int.MaxValue;

        private static Transform topView;

        public static Transform GetTopView()
        {
            return topView;
        }

        [ComponentBinder("LevelButton")] public Button levelButton;

        [ComponentBinder("LevelNumber/NumberText")]
        public LocalizeTextMeshProUGUI levelText;

        [ComponentBinder("TagIcon")] public Image tagIcon;
        [ComponentBinder("PullIcon")] public Image pullIcon;
        [ComponentBinder("GameButton")] public Button miniGameButton;
        [ComponentBinder("ExitButton")] public Button exitButton;

        protected override bool IsChildView => true;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            topView = transform;
            levelButton.onClick.AddListener(LevelOnClick);
            exitButton.onClick.AddListener(TMatchModel.Instance.Exit);
            miniGameButton.onClick.AddListener(MiniGameOnClick);
            RefreshUI();
            EventDispatcher.Instance.AddEventListener(EventEnum.LobbyRefreshShow, OnLobbyRefreshShow);
            
            topView = levelButton.transform;
        }

        public override Task OnViewClose()
        {
            topView = null;
            EventDispatcher.Instance.RemoveEventListener(EventEnum.LobbyRefreshShow, OnLobbyRefreshShow);
            return base.OnViewClose();
        }

        private void LevelOnClick()
        {
            if (TMatchModel.Instance.GetMainLevel() > UILobbyMainViewLevelButton.MAXLevel) return;

            // AudioSysManager.Instance.PlaySound(SfxNameConst.button_s);
            UIViewSystem.Instance.Open<UITMatchLevelPrepareView>();
            if (!StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.GlodenHatterGuid &&
                TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelGoldenHatterUnlock)
            {
                UIViewSystem.Instance.Open<UITMatchGoldenHatterHelp>();
                StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.GlodenHatterGuid = true;
            }

            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.LobbyLevelButton);
        }

        public void GuideClickLevel()
        {
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmFristLevle1Play);
            LevelOnClick();
        }

        private async void ShowGoldenHatter(int level, bool anim)
        {
            if (level > 3) level = 3;
            pullIcon.gameObject.SetActive(true);
            pullIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, $"ui_main_level_pull_{level}");
            if (anim)
            {
                Animator animator = pullIcon.GetComponent<Animator>();
                animator.Play("appear");
                animator.Update(0.0f);
                AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
                if (clipInfos.Length > 0)
                {
                    await Task.Delay((int)(clipInfos[0].clip.length * 1000.0f));
                }

                await Task.Yield(); //防止动画没有播放的情况下直接finish导致时序错误触发异常，需要等待一祯finish
                LobbyTaskSystem.Instance.FinishCurrentTask();
            }
        }

        private void InitGoldenHatter()
        {
            var winCount = StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.WinningStreakCnt;
            if (winCount > 3)
                ShowGoldenHatter(winCount, false);
        }

        public bool TryShowGoldenHatter()
        {
            int WinningStreakCnt = StorageManager.Instance.GetStorage<StorageTMatch>().GlodenHatter.WinningStreakCnt;
            int lastWinnigStreakCnt = PlayerPrefs.GetInt("LobbyMain.LevelButton.Streak", 0);
            PlayerPrefs.SetInt("LobbyMain.LevelButton.Streak", WinningStreakCnt);
            if (WinningStreakCnt > 0)
            {
                bool anim = WinningStreakCnt > lastWinnigStreakCnt && WinningStreakCnt <= 3;
                ShowGoldenHatter(WinningStreakCnt, anim);
                return anim;
            }

            return false;
        }

        private void MiniGameOnClick()
        {
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MakeoverLevelButton);
        }

        private async void OnLobbyRefreshShow(BaseEvent evt)
        {
            RefreshUI();
        }

        private void OnMakeoverSelectSkip(BaseEvent evt)
        {
            RefreshUI();
            TryShowGoldenHatter();
        }

        private void RefreshUI()
        {
            var canEnterMiniGame = false;
            levelButton.gameObject.SetActive(!canEnterMiniGame);
            miniGameButton.gameObject.SetActive(canEnterMiniGame);

            if (!canEnterMiniGame)
            {
                // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.LobbyLevelButton, levelButton.transform.Find("BG"));
                pullIcon.gameObject.SetActive(false);

                if (TMatchModel.Instance.GetMainLevel() <= UILobbyMainViewLevelButton.MAXLevel)
                {
                    TMatchDifficulty diff = TMatchConfigManager.Instance.GetDifficulty(TMatchModel.Instance.GetMainLevel());
                    tagIcon.gameObject.SetActive(diff != TMatchDifficulty.Normal);
                    if (diff == TMatchDifficulty.Hard)
                    {
                        tagIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, "ui_common_level_tag_1");
                        tagIcon.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("UI_levelstart_level_empty1");
                    }
                    else if (diff == TMatchDifficulty.Demon)
                    {
                        tagIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, "ui_common_level_tag_2");
                        tagIcon.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("UI_levelstart_level_empty2");
                    }

                    var levelTextStr = TMatchModel.Instance.GetMainLevel().ToString();
                    levelText.SetText(levelTextStr);
                }
                else
                {
                    levelText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_weekchallenge_5"));
                }
            }
            else
            {
                // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MakeoverLevelButton, miniGameButton.transform.Find("BG"));
            }

            InitGoldenHatter();
        }
    }
}