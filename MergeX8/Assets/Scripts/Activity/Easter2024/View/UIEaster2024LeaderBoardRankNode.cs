 using System.Collections.Generic;
 using DragonPlus;
 using Gameplay;
 using UnityEngine;
 using UnityEngine.UI;

 namespace Easter2024LeaderBoard
 {

     public class RewardGroupNode : TransformHolder
     {
         public class RewardNode : TransformHolder
         {
             private LocalizeTextMeshProUGUI _countText;
             private Image _rewardSprite;

             public RewardNode(Transform transform) : base(transform)
             {
                 _rewardSprite = transform.GetComponent<Image>();
                 _countText = base.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
             }

             public void UpdateState(ResData resData)
             {
                 if (resData == null)
                     Hide();
                 else
                 {
                     Show();
                     _rewardSprite.sprite = UserData.GetResourceIcon(resData.id, UserData.ResourceSubType.Small);
                     _countText.gameObject.SetActive(resData.count > 1);
                     _countText.SetText(resData.count.ToString());
                 }
             }
         }

         public List<RewardGroupNode.RewardNode> _rewardNodes;

         public RewardGroupNode(Transform transform) : base(transform)
         {
             _rewardNodes = new List<RewardGroupNode.RewardNode>();
             for (var i = 1; i <= 3; i++)
             {
                 _rewardNodes.Add(
                     new RewardGroupNode.RewardNode(
                         base.transform.Find(i.ToString())));
             }
         }

         public virtual void BindPlayer(Easter2024LeaderBoardPlayer player)
         {
             var rewards = player.Rewards;
             for (var i = 0; i < _rewardNodes.Count; i++)
             {
                 _rewardNodes[i].UpdateState(i < rewards.Count ? rewards[i] : null);
             }
         }
     }

     public class NormalRankNode : RankNode
     {
         public class NormalPlayerNode : TransformHolder
         {
             private Text _nameText;
             private LocalizeTextMeshProUGUI _starNumText;
             private LocalizeTextMeshProUGUI _rankText;
             public Transform _starIcon;

             public NormalPlayerNode(Transform transform) : base(transform)
             {
                 _nameText = transform.Find("NameText").GetComponent<Text>();
                 _starNumText = transform.Find("StarNum").GetComponent<LocalizeTextMeshProUGUI>();
                 _rankText = transform.Find("RankingNum").GetComponent<LocalizeTextMeshProUGUI>();
                 _starIcon = transform.Find("BGGroup/BG/Icon");
                 EnableDestroy();
             }

             public override void OnDestroy()
             {
                 EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_NAME,UpdateName);
             }
             
             public void UpdateName(BaseEvent e = null)
             {
                 _nameText.text = player.GetName();
                 // _nameText.SetText(player.GetName());
             }

             private Easter2024LeaderBoardPlayer player;
             public void BindPlayer(Easter2024LeaderBoardPlayer inPlayer)
             {
                 if (player != null && player.IsMe)
                 {
                     EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_NAME,UpdateName);
                 }
                 player = inPlayer;
                 if (player.IsMe)
                 {
                     EventDispatcher.Instance.AddEventListener(EventEnum.UPDATE_NAME,UpdateName);
                 }
                 UpdateName();
                 _starNumText.SetText(player.SortValue.ToString());
                 _rankText.SetText(player.Rank.ToString());
             }
         }

         public NormalRankNode.NormalPlayerNode _myNode;
         public NormalRankNode.NormalPlayerNode _robotNode;
         private RewardGroupNode _rewardGroup;
         private RectTransform _headIconRoot;
         private HeadIconNode HeadIcon;

         public NormalRankNode(Transform transform) : base(transform)
         {
             var myNodeTransform = transform.Find("My");
             if (myNodeTransform != null)
                _myNode = new NormalRankNode.NormalPlayerNode(myNodeTransform);
             var robotNodeTransform = transform.Find("Normal");
             if (robotNodeTransform != null)
                _robotNode = new NormalRankNode.NormalPlayerNode(robotNodeTransform);
             var rewardNodeTransform = transform.Find("RewardGroup");
             if (rewardNodeTransform != null)
                _rewardGroup = new RewardGroupNode(rewardNodeTransform);
             var headIconTransform = transform.Find("Head");
             if (headIconTransform != null)
                 _headIconRoot = headIconTransform as RectTransform;
             var headFrameIconTransform = transform.Find("BG");
             if (headFrameIconTransform != null)
             {
                 headFrameIconTransform.gameObject.SetActive(false);
             }
             EnableDestroy();
         }

         public override void OnDestroy()
         {
             EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_HEAD,UpdateHeadIcon);
         }

         public void UpdateHeadIcon(BaseEvent e=null)
         {
             if (_headIconRoot != null)
             {
                 if (HeadIcon)
                 {
                     HeadIcon.SetAvatarViewState(player.GetAvatarViewState());
                 }
                 else
                 {
                     HeadIcon = HeadIconNode.BuildHeadIconNode(_headIconRoot,player.GetAvatarViewState());
                 }
             }
         }

         private Easter2024LeaderBoardPlayer player;
         public override void BindPlayer(Easter2024LeaderBoardPlayer inPlayer)
         {
             if (player != null && player.IsMe)
                 EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_HEAD,UpdateHeadIcon);
             player = inPlayer;
             if (player.IsMe)
                 EventDispatcher.Instance.AddEventListener(EventEnum.UPDATE_HEAD,UpdateHeadIcon);
             UpdateHeadIcon();
             var showNode = player.IsMe ? _myNode : _robotNode;
             var hideNode = player.IsMe ? _robotNode : _myNode;
             hideNode?.Hide();
             showNode?.Show();
             showNode?.BindPlayer(player);
             _rewardGroup?.BindPlayer(player);
         }
     }

     public class TopRankNode : RankNode
     {
         private Text _nameText;
         private LocalizeTextMeshProUGUI _starNumText;
         private RectTransform _headIconRoot;
         private HeadIconNode HeadIcon;
         private RewardGroupNode _rewardGroup;

         public TopRankNode(Transform transform) : base(transform)
         {
             _nameText = transform.Find("Name").GetComponent<Text>();
             _starNumText = transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>();
             _headIconRoot = transform.Find("Head") as RectTransform;
             _rewardGroup = new RewardGroupNode(transform.Find("RewardGroup"));
             EnableDestroy();
         }
        
         public override void OnDestroy()
         {
             EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_HEAD,UpdateHeadIcon);
             EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_NAME,UpdateName);
         }

         public void UpdateHeadIcon(BaseEvent e = null)
         {
             if (_headIconRoot)
             {
                 if (HeadIcon)
                 {
                     HeadIcon.SetAvatarViewState(player.GetAvatarViewState());
                 }
                 else
                 {
                     HeadIcon = HeadIconNode.BuildHeadIconNode(_headIconRoot,player.GetAvatarViewState());
                 }
             }
         }

         public void UpdateName(BaseEvent e = null)
         {
             _nameText.text = player.GetName();
             // _nameText.SetText(player.GetName());
         }

         private Easter2024LeaderBoardPlayer player;
         public override void BindPlayer(Easter2024LeaderBoardPlayer inPlayer)
         {
             if (!transform.gameObject.activeSelf)
             {
                 Show();
             }
             if (player != null && player.IsMe)
             {
                 EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_HEAD,UpdateHeadIcon);
                 EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_NAME,UpdateName);
             }
             player = inPlayer;
             if (player.IsMe)
             {
                 EventDispatcher.Instance.AddEventListener(EventEnum.UPDATE_HEAD,UpdateHeadIcon);
                 EventDispatcher.Instance.AddEventListener(EventEnum.UPDATE_NAME,UpdateName);
             }
             UpdateName();
             _starNumText.SetText(player.SortValue.ToString());
             UpdateHeadIcon();
             _rewardGroup.BindPlayer(player);
         }

     }

     public abstract class RankNode : TransformHolder
     {

         public RankNode(Transform transform) : base(transform)
         {
         }

         public abstract void BindPlayer(Easter2024LeaderBoardPlayer player);
     }
 }