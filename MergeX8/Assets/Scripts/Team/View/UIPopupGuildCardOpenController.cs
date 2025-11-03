using System;
using System.Collections.Generic;
using Activity.BattlePass;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupGuildCardOpenController:UIWindowController
{
    
    public static UIPopupGuildCardOpenController Instance;
    public static UIPopupGuildCardOpenController Open(PassGiftData passGiftData,int index,Action callback)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildCardOpen,passGiftData,index,callback) as UIPopupGuildCardOpenController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        
    }

    private PassGiftData passData;
    private int index;
    private Button CloseBtn;
    private Action Callback;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        passData = objs[0] as PassGiftData;
        index = (int)objs[1];
        if (objs.Length > 2)
            Callback = objs[2] as Action;
        var package = passData.ExtraData.CardPackageId;
        var packageConfig = CardCollectionModel.Instance.TableCardPackage[package];
        var theme = CardCollectionModel.Instance.GetCardThemeState(packageConfig.ThemeId);
        var cardPackSprite = theme.GetCardBackSprite();
        transform.Find("Root/Card/Card/Back").GetComponent<Image>().sprite = cardPackSprite;
        
        var defaultCardItem = transform.Find("Root/Card");
        defaultCardItem.gameObject.SetActive(false);
        var cardCount = passData.ExtraData.CardList.Count;
        var group = transform.Find("Root/" + cardCount);
        for (var i = 0; i < cardCount; i++)
        {
            var cardItemObj = Instantiate(defaultCardItem, defaultCardItem.parent);
            cardItemObj.gameObject.SetActive(true);
            var cardItem = cardItemObj.gameObject.AddComponent<CardItemContainer>();
            cardItem.Init(passData.ExtraData.CardList[i],this);
            cardItem.transform.position = group.Find((i + 1).ToString()).position;
            ContainerList.Add(cardItem);
        }

        CloseBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
        CloseBtn.gameObject.SetActive(false);
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                UIPopupGuildMainController.Instance?.CheckLifeGuide(passData.ExtraData.Price);
            });
            UIPopupGuildCardGetController.Instance?.AnimCloseWindow();
            TeamManager.Instance.UpdateBattlePassList(null);
            UIPopupGuildMainController.Instance?.Chat.InitMessageGroup();
            UIPopupGuildMainController.Instance?.Chat.SetContentToBottom();
        });
    }

    private List<CardItemContainer> ContainerList = new List<CardItemContainer>();

    public async void OnClick(CardItemContainer container)
    {
        var copyList = passData.ExtraData.CardList.DeepCopy();
        var result = copyList[index];
        copyList.RemoveAt(index);
        container.SetIndex(result);
        var j = 0;
        for (var i = 0; i < ContainerList.Count; i++)
        {
            if (ContainerList[i] != container)
            {
                ContainerList[i].SetIndex(copyList[j]);
                j++;
            }
        }
        container.Select();
        await XUtility.WaitSeconds(0.2f);
        for (var i = 0; i < ContainerList.Count; i++)
        {
            if (ContainerList[i] != container)
            {
                ContainerList[i].Open();
                await XUtility.WaitSeconds(0.1f);
            }
        }
        await XUtility.WaitSeconds(0.2f);
        container.Open();
        // await XUtility.WaitSeconds(0.3f);
        Callback?.Invoke();
        CloseBtn.gameObject.SetActive(true);
    }

    public class CardItemContainer : MonoBehaviour
    {
        private Animator Animator;
        public CardItem Card;
        private Transform SelectFlag;
        private UIPopupGuildCardOpenController Controller;
        private bool IsOpen = false;
        public void Init(int cardIndex,UIPopupGuildCardOpenController controller)
        {
            Animator = transform.GetComponent<Animator>();
            Controller = controller;
            Card = transform.Find("Card/Content").gameObject.AddComponent<CardItem>();
            Card.Init(cardIndex);
            SelectFlag = transform.Find("Card/Selected");
            SelectFlag.gameObject.SetActive(false);
            transform.Find("Card").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (IsOpen)
                    return;
                Controller.OnClick(this);
            });
        }

        public void Open()
        {
            IsOpen = true;
            Animator.PlayAnimation("Open");
        }

        public void SetIndex(int cardIndex)
        {
            Card.Init(cardIndex);
        }

        public void Select()
        {
            SelectFlag.gameObject.SetActive(true);
        }
        public class CardItem : MonoBehaviour
        {
            private Image Icon;
            private Dictionary<int, Transform> BGDic = new Dictionary<int, Transform>();
            private Dictionary<int, Transform> StarDic = new Dictionary<int, Transform>();
            // private Transform HaveFlag;
            private CardCollectionCardItemState CardItemState;

            public void Init(int cardId)
            {
                CardItemState = CardCollectionModel.Instance.GetCardItemState(cardId);
                Icon = transform.Find("Mask/Icon").GetComponent<Image>();
                Icon.sprite = CardItemState.GetCardSprite();
                BGDic.Clear();
                StarDic.Clear();
                var starIndex = 1;
                while (true)
                {
                    var bg = transform.Find("BGGroup/" + starIndex);
                    var star = transform.Find("Star/" + starIndex);
                    if (bg == null || star == null)
                    {
                        break;
                    }
                    BGDic.Add(starIndex,bg);
                    StarDic.Add(starIndex,star);
                    bg.gameObject.SetActive(CardItemState.CardItemConfig.Level == starIndex);
                    star.gameObject.SetActive(CardItemState.CardItemConfig.Level >= starIndex);
                    starIndex++;
                }
                // HaveFlag = transform.Find("Have");
                // HaveFlag.gameObject.SetActive(CardItemState.CollectCount > 0);
            }
        }
    }
}