using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityLocal.CardCollection.Home;
using DG.Tweening;
using DragonPlus.Config.CardCollect;
using DragonU3DSDK.Audio;
using Gameplay;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using AudioManager = DragonPlus.AudioManager;
using Vector2 = System.Numerics.Vector2;

public class UIOpenCardController:UIWindowController
{
    public class CardItemNode : MonoBehaviour
    {
        public class CardGroupWithNewFlag : UICardBookController.CardBookPage.CardItem.BaseGroup
        {
            private Image Icon;
            private Transform NewFlag;
            // private List<Transform> NewStarList = new List<Transform>();
            // private Transform NewStarGroup;
            private Dictionary<int, Transform> BGList = new Dictionary<int, Transform>();
            public override void Awake()
            {
                base.Awake();
                Icon = transform.Find("Mask/Icon").GetComponent<Image>();
                NewFlag = transform.Find("NewFlag");
                // NewStarGroup = transform.Find("StarGroupNew");
                // for (var i = 1; NewStarGroup.Find(i.ToString()); i++)
                // {
                //     NewStarList.Add(NewStarGroup.Find(i.ToString()));
                // }
                for (var i = 1; i <= 5; i++)
                {
                    BGList.Add(i,transform.Find("BGGroup/"+i));
                }
            }
            public void EnableNewFlag(bool newCardFlag)
            {
                NewFlag?.gameObject.SetActive(newCardFlag);
                // NewStarGroup.gameObject.SetActive(newCardFlag);
            }
            public override void UpdateViewState()
            {
                // base.UpdateViewState();
                for (var i = 0; i < StarList.Count; i++)
                {
                    StarList[i].gameObject.SetActive(CardItemState.CardItemConfig.Level >= i+1);
                }
                Icon.sprite = CardItemState.GetCardSprite();
                // for (var i = 0; i < NewStarList.Count; i++)
                // {
                //     NewStarList[i].gameObject.SetActive(CardItemState.CardItemConfig.level > i);
                // }
                foreach (var pair in BGList)
                {
                    pair.Value.gameObject.SetActive(pair.Key == CardItemState.CardItemConfig.Level);
                }
            }
        }

        private CardGroupWithNewFlag CardGrop;
        private Transform CardMoveNode;
        private Animator Animator;
        private void Awake()
        {
            CardGrop = transform.Find("Card/Content").gameObject.AddComponent<CardGroupWithNewFlag>();
            CardMoveNode = transform.Find("Card");
            Animator = transform.GetComponent<Animator>();
        }

        private bool NewCardFlag;
        public void BindCardItemState(CardCollectionCardItemState cardItemState, bool newCardFlag)
        {
            NewCardFlag = newCardFlag;
            CardGrop.BindCardItemState(cardItemState);
            CardGrop.EnableNewFlag(NewCardFlag);
        }

        public void HideCard()
        {
            CardMoveNode.gameObject.SetActive(false);
        }

        public Task PlayPopAnimation(Vector3 srcPosition)
        {
            CardMoveNode.gameObject.SetActive(true);
            CardMoveNode.position = srcPosition;
            Animator.PlayAnimation("Open");
            var task = new TaskCompletionSource<bool>();
            CardMoveNode.DOLocalMove(Vector3.zero, 0.4f).OnComplete(() =>
            {
                task.SetResult(true);
            });
            return task.Task;
        }
        
        public Task PlayCollectAnimation(Vector3 dstPosition)
        {
            // AudioManager.Instance.PlaySound(42);
            
            CardMoveNode.gameObject.SetActive(true);
            // var dstLocalPosition = CardGrop.transform.parent.InverseTransformPoint(dstPosition);
            Animator.PlayAnimation("RowFly");
            var task = new TaskCompletionSource<bool>();
            var midPosition = CardMoveNode.position + (CardMoveNode.position - dstPosition).GetUnit();
            CardMoveNode.DOMove(midPosition, 0.2f);
            var tangle = CardMoveNode.position - dstPosition;
            var quaternion = Quaternion.Euler(0, 0, -Mathf.Atan2(tangle.x, tangle.y)*Mathf.Rad2Deg);
            CardMoveNode.DORotateQuaternion(quaternion, 0.3f);
            XUtility.WaitSeconds(0.3f, () =>
            {
                CardMoveNode.DOMove(dstPosition, 0.25f).OnComplete(() =>
                {
                    CardMoveNode.position = dstPosition;
                    XUtility.WaitFrames(1, () =>
                    {
                        HideCard();
                        task.SetResult(true);
                    });
                }).SetEase(Ease.Linear);
            });
            return task.Task;
        }
    }
    public class CardRowNode : MonoBehaviour
    {
        private const int MaxCardCount = 3;
        private List<CardItemNode> CardItemList = new List<CardItemNode>();
        private Transform DefaultContainer;
        private void Awake()
        {
            DefaultContainer = transform.Find("Container");
            DefaultContainer.gameObject.SetActive(false);
        }
        public bool IsFull()
        {
            return CardItemList.Count >= MaxCardCount;
        }
        public CardItemNode AddCard(CardCollectionCardItemState cardItemState, bool newCardFlag)
        {
            var cardItemNodeObj = Instantiate(DefaultContainer, transform);
            cardItemNodeObj.gameObject.SetActive(true);
            var cardItemNode = cardItemNodeObj.gameObject.AddComponent<CardItemNode>();
            cardItemNode.BindCardItemState(cardItemState, newCardFlag);
            CardItemList.Add(cardItemNode);
            return cardItemNode;
        }
    }

    public class CardBookCollectGroup : MonoBehaviour
    {
        private CardCollectionEntranceRedPoint RedPoint;
        private Animator Animator;
        private void Awake()
        {
            RedPoint = transform.Find("RedPoint").gameObject.AddComponent<CardCollectionEntranceRedPoint>();
            Animator = gameObject.GetComponent<Animator>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            RedPoint.UpdateViewState();
            Animator.PlayAnimation("appear");
        }

        public void OnCardFlyIn()
        {
            Animator.PlayAnimation("collect");
        }
        
        public void Hide()
        {
            Animator.PlayAnimation("disappear");
        }
        
    }
    private Button CloseButton;
    private List<CardRowNode> CardRowNodeList = new List<CardRowNode>();
    private Transform DefaultCardRowNode;
    private List<CardItemNode> CardItemList = new List<CardItemNode>();
    // private CardBookCollectGroup CardBookNode;
    private Animator TapTextAnimator;
    private SkeletonGraphic SpineSkeleton1;
    // private SkeletonGraphic SpineSkeleton2;
    public override void PrivateAwake()
    {
        CloseButton = transform.Find("Root/CloseButton").GetComponent<Button>();
        CloseButton.onClick.AddListener(OnClickCloseButton);
        CardRowNodeList.Clear();
        DefaultCardRowNode = transform.Find("Root/RewardGroup/Row");
        DefaultCardRowNode.gameObject.SetActive(false);
        // var rewardGroup = transform.Find("Root/RewardGroup");
        // for (var i = 0; i<rewardGroup.childCount; i++)
        // {
        //     var cardRowNode = rewardGroup.GetChild(i).gameObject.AddComponent<CardRowNode>();
        //     CardRowNodeList.Add(cardRowNode);
        // }
        // CardBookNode = transform.Find("Root/CardBook").gameObject.AddComponent<CardBookCollectGroup>();
        // CardBookNode.gameObject.SetActive(false);
        TapTextAnimator = transform.Find("Root/CloseButton/Text").GetComponent<Animator>();
        TapTextAnimator.gameObject.SetActive(false);
        SpineSkeleton1 = transform.Find("Root/cardholdeSpine").GetComponent<SkeletonGraphic>();
        // SpineSkeleton2 = transform.Find("Root/cardholdeSpine2").GetComponent<SkeletonGraphic>();
    }

    private TableCardCollectionCardPackage CardPackageConfig;
    private List<CardCollectionCardItemState> CardItemStateList;
    private List<bool> NewCardFlagList;
    private Action Callback;

    protected override async void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CardPackageConfig = objs[0] as TableCardCollectionCardPackage;
        CardItemStateList = objs[1] as List<CardCollectionCardItemState>;
        NewCardFlagList = objs[2] as List<bool>;
        Callback = objs.Length > 3 ? objs[3] as Action : null;
        if (CardPackageConfig == null || CardItemStateList == null || NewCardFlagList == null)
            return;
        CardItemList.Clear();
        var rowIndex = 0;
        for (var i = 0; i < CardItemStateList.Count; i++)
        {
            CreateRow(rowIndex);
            var cardItemNode = CardRowNodeList[rowIndex].AddCard(CardItemStateList[i],NewCardFlagList[i]);
            CardItemList.Add(cardItemNode);
            if (CardRowNodeList[rowIndex].IsFull())
                rowIndex++;
        }
        for (var i = 0; i < CardItemList.Count; i++)
        {
            CardItemList[i].HideCard();
        }
        if (CloseButton.gameObject.TryGetComponent<ShieldButtonOnClick>(out var shield))
        {
            shield.enabled = false;
        }
        EnableClick = false;
        SetSpineSkin(CardPackageConfig);
        SpineSkeleton1.gameObject.SetActive(false);
        // SpineSkeleton2.gameObject.SetActive(false);
        // await XUtility.WaitSeconds(0.5f);
        await PlayCardPackageOpenAnimation();
        await PlayPopCardAnimation();
        await PlayCardPackageDisappearAnimation();
        EnableClick = true;
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CardPackageClose, null);
    }

    public void CreateRow(int rowIndex)
    {
        if (rowIndex >= CardRowNodeList.Count)
        {
            var cardRowObj = Instantiate(DefaultCardRowNode, DefaultCardRowNode.parent);
            cardRowObj.gameObject.SetActive(true);
            cardRowObj.name = "Row" + rowIndex;
            var cardRowNode = cardRowObj.gameObject.AddComponent<CardRowNode>();
            CardRowNodeList.Add(cardRowNode);
        }
    }

    public void SetSpineSkin(TableCardCollectionCardPackage packageConfig)
    {
        SpineSkeleton1.initialSkinName = "Card"+packageConfig.Level;
        // SpineSkeleton2.initialSkinName = "blue";
        SpineSkeleton1.Skeleton.SetSkin("Card"+packageConfig.Level);
        // SpineSkeleton2.Skeleton.SetSkin("blue");
    }
    public Task PlayCardPackageOpenAnimation()
    {
        XUtility.WaitSeconds(1.2f, () => AudioManager.Instance.PlaySound("sfx_card_open"));
        SpineSkeleton1.gameObject.SetActive(true);
        // SpineSkeleton1.AnimationState.SetAnimation(0, "Appear", false);
        // SpineSkeleton1.AnimationState.Update(0);
        // SpineSkeleton2.gameObject.SetActive(true);
        // SpineSkeleton2.AnimationState.SetAnimation(0, "Appear", false);
        // SpineSkeleton2.AnimationState.Update(0);
        // return XUtility.WaitSeconds(2.067f);
        return _animator.PlayAnimationAsync("appear");
    }

    public Task PlayCardPackageDisappearAnimation()
    {
        // AudioManager.Instance.PlaySound("sfx_card_end");
        // SpineSkeleton1.AnimationState.SetAnimation(0, "Out", false);
        // SpineSkeleton1.AnimationState.Update(0);
        // SpineSkeleton2.AnimationState.SetAnimation(0, "Out", false);
        // SpineSkeleton2.AnimationState.Update(0);
        TapTextAnimator.gameObject.SetActive(true);
        TapTextAnimator.PlayAnimation("appear");
        _animator.PlayAnimation("Collect");
        return Task.CompletedTask;
    }

    public async Task PlayPopCardAnimation()
    {
        var cardPackagePosition = transform.Find("Root/CardBox").position;
        var completedTask = new List<Task>();
        for (var i = 0; i < CardItemList.Count; i++)
        {
            AudioManager.Instance.PlaySound("sfx_card_start");
            // _animator.PlayAnimation("spitout");
            SpineSkeleton1.AnimationState.SetAnimation(0, "Open", false);
            SpineSkeleton1.AnimationState.Update(0);
            // SpineSkeleton2.AnimationState.SetAnimation(0, "Open", false);
            // SpineSkeleton2.AnimationState.Update(0);
            var tempCardItem = CardItemList[i];
            var task = new TaskCompletionSource<bool>();
            XUtility.WaitSeconds(0.2f, () =>
            {
                tempCardItem.PlayPopAnimation(cardPackagePosition).AddCallBack(() => task.SetResult(true)).WrapErrors();
            });
            completedTask.Add(task.Task);
            await XUtility.WaitSeconds(0.5f);
        }
        await Task.WhenAll(completedTask);
    }
    // public async Task PlayCollectCardAnimation()
    // {
    //     TapTextAnimator.PlayAnimation("disappear");
    //     CardBookNode.Show();
    //     var cardBookPosition = transform.Find("Root/CardBook").position;
    //     var completedTask = new List<Task>();
    //     for (var i = 0; i < CardItemList.Count; i++)
    //     {
    //         completedTask.Add(CardItemList[i].PlayCollectAnimation(cardBookPosition).AddCallBack(() =>
    //         {
    //             CardBookNode.OnCardFlyIn();
    //         }));
    //         await XUtility.WaitSeconds(0.1f);
    //     }
    //     await Task.WhenAll(completedTask);
    //     await XUtility.WaitSeconds(0.3f);
    //     CardBookNode.Hide();
    // }

    private bool EnableClick { get; set; }
    public async void OnClickCloseButton()
    {
        if (!EnableClick)
            return;
        EnableClick = false;
        // CloseButton.gameObject.SetActive(false);
        // await PlayCollectCardAnimation();
     
        CloseWindowWithinUIMgr(true);
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CardMergeEntrance))
        {
            var cardCollectionIcon = MergeTaskTipsController.Instance.MergeCardCollection;
            if (cardCollectionIcon != null && cardCollectionIcon.gameObject.activeInHierarchy)
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(cardCollectionIcon.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CardMergeEntrance, cardCollectionIcon.transform as RectTransform, topLayer:topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CardMergeEntrance,null))
                {
                    if(MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-cardCollectionIcon.transform.localPosition.x+220, 0);
                }
            }
            else
            {
                MergeGuideLogic.Instance.CheckMergeGuide();
            }
        }
        Callback?.Invoke();
        
        // CloseWindowWithinUIMgr(true);
        // Callback?.Invoke();
    }

    public static void Open(TableCardCollectionCardPackage cardPackageConfig,List<CardCollectionCardItemState> cardItemStateList,
        List<bool> newCardFlagList,Action callback = null)
    {
        var cardTheme = cardPackageConfig.ThemeId > 0 ? CardCollectionModel.Instance.GetCardThemeState(cardPackageConfig.ThemeId):CardCollectionModel.Instance.ThemeInUse;
        var uiPath = cardTheme.GetCardUIName(CardUIName.UIType.UIOpenCard);
        if (!UIManager.Instance.GetOpenedUIByPath(uiPath))
            UIManager.Instance.OpenUI( uiPath,cardPackageConfig,cardItemStateList,newCardFlagList,callback);
    }
}