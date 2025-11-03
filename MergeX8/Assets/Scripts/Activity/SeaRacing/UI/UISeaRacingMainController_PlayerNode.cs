using System;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public partial class UISeaRacingMainController : UIWindowController
{
    public class PlayerNode : MonoBehaviour
    {
        public SeaRacingPlayer Player;
        private LocalizeTextMeshProUGUI RankText;
        private HeadIconNode HeadIcon;
        private RectTransform _headIconRoot;
        private void Awake()
        {
            RankText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            _headIconRoot = transform.Find("Head") as RectTransform;
        }

        public virtual void BindPlayer(SeaRacingPlayer player)
        {
            Player = player;
            RankText.SetText(Player.Rank.ToString());
            UpdateHeadIcon();
        }
        public void UpdateHeadIcon(BaseEvent e=null)
        {
            if (_headIconRoot != null)
            {
                if (HeadIcon)
                {
                    HeadIcon.SetAvatarViewState(Player.GetAvatarViewState());
                }
                else
                {
                    HeadIcon = HeadIconNode.BuildHeadIconNode(_headIconRoot,Player.GetAvatarViewState());
                    HeadIcon.ShowHeadIconFrame(false);
                }
            }
        }
        
        public virtual void RefreshPlayer(SeaRacingPlayer player)
        {
            RankText.SetText(Player.Rank.ToString());
        }
    }

    public class PlayerNodeMe : PlayerNode
    {
        public override void BindPlayer(SeaRacingPlayer player)
        {
            base.BindPlayer(player);
            gameObject.SetActive(player.IsMe);
        }
    }
    public class PlayerNodeRobot : PlayerNode
    {
        public override void BindPlayer(SeaRacingPlayer player)
        {
            base.BindPlayer(player);
            gameObject.SetActive(!player.IsMe);
        }
    }

    public class PlayerNodeController : MonoBehaviour
    {
        private int CurPointIndex;
        public SeaRacingPlayer Player;
        private PlayerNodeMe Me;
        private PlayerNodeRobot Robot;
        private RectTransform RectTransform;
        private Animator Animator;

        private void Awake()
        {
            Me = transform.Find("Root/Mine").gameObject.AddComponent<PlayerNodeMe>();
            Robot = transform.Find("Root/Other").gameObject.AddComponent<PlayerNodeRobot>();
            RectTransform = transform as RectTransform;
            Animator = GetComponent<Animator>();
        }

        private UISeaRacingMainController MainController;
        public void BindMainController(UISeaRacingMainController mainController)
        {
            MainController = mainController;
        }

        private const float JumpTime = 0.167f;
        private const float MoveTime = 0.3f;
        public void BindPlayer(SeaRacingPlayer player)
        {
            Player = player;
            Me.BindPlayer(Player);
            Robot.BindPlayer(Player);
            CurPointIndex = Mathf.FloorToInt(player.SortValue / MainController.UnitScore);
            RectTransform.anchoredPosition = MainController.GetPlayerAnchorPosition(Player);
        }

        public void RefreshPlayer()
        {
            Me.RefreshPlayer(Player);
            Robot.RefreshPlayer(Player);
        }
        public bool IsNeedUpdateState()
        {
            var nowPointIndex = Mathf.FloorToInt(Player.SortValue / MainController.UnitScore);
            if (CurPointIndex != nowPointIndex)
                return true;
            var targetAnchorPosition = MainController.GetPlayerAnchorPosition(Player);
            if (targetAnchorPosition != RectTransform.anchoredPosition)
                return true;
            return false;
        }
        public async Task UpdateState()
        {
            Me.BindPlayer(Player);
            Robot.BindPlayer(Player);
            var nowPointIndex = Mathf.FloorToInt(Player.SortValue / MainController.UnitScore);
            if (CurPointIndex != nowPointIndex)
            {
                var oldCurPointIndex = CurPointIndex;
                CurPointIndex = nowPointIndex;
                for (var i = oldCurPointIndex + 1; i < nowPointIndex; i++)
                {
                    var targetAnchorPosition = MainController.GetCrossPlayerAnchorPosition(i);
                    Animator.PlayAnimation("jump_succeed");
                    await XUtility.WaitSeconds(0.083f);
                    var task = new TaskCompletionSource<bool>();
                    RectTransform.DOJumpAnchorPos(targetAnchorPosition,50f,1,JumpTime).SetEase(Ease.Linear).OnComplete(()=>task.SetResult(true));
                    await task.Task;
                    await XUtility.WaitSeconds(0.3f);
                }
                {
                    var targetAnchorPosition = MainController.GetPlayerAnchorPosition(Player);
                    Animator.PlayAnimation("jump_succeed");
                    await XUtility.WaitSeconds(0.083f);
                    var task = new TaskCompletionSource<bool>();
                    RectTransform.DOJumpAnchorPos(targetAnchorPosition, 50f,1,JumpTime).SetEase(Ease.Linear).OnComplete(()=>task.SetResult(true));
                    await task.Task;
                    // if (Player.IsMe)
                    // {
                    //     if (MainController.Storage.Score >= MainController.Storage.MaxScore)
                    //     {
                    //         SeaRacingModel.OpenRoundRewardPopup(MainController.Storage);
                    //     }   
                    // }
                }
            }
            else
            {
                var targetAnchorPosition = MainController.GetPlayerAnchorPosition(Player);
                if (targetAnchorPosition != RectTransform.anchoredPosition)
                {
                    var task = new TaskCompletionSource<bool>();
                    RectTransform.DOAnchorPos(targetAnchorPosition, MoveTime).SetEase(Ease.Linear).OnComplete(()=>task.SetResult(true));
                    await task.Task;
                }
            }
        }
    }
}