using System;
using System.Collections.Generic;
using ActivityLocal.CardCollection.Home;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class UIUpGradeCardController:UIWindowController
{
    public override void PrivateAwake()
    {
        
    }
    public static void Open(CardCollectionCardItemState cardItemState,Action callback = null)
    {
        var uiPath = cardItemState.CardBookStateList[0].CardThemeStateList[0].GetCardUIName(CardUIName.UIType.UIUpGradeCard);
        if (!UIManager.Instance.GetOpenedUIByPath(uiPath))
            UIManager.Instance.OpenUI( uiPath,cardItemState,callback);
    }

    private CardCollectionCardItemState CardItemState;
    private Action Callback;
    private HaveGroup Card1;
    private HaveGroup Card2;
    private HaveGroup CardNew;
    private Button CloseBtn;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CardItemState = objs[0] as CardCollectionCardItemState;
        if (objs.Length > 1)
        {
            Callback = objs[1] as Action;
        }

        Card1 = transform.Find("Root/Card1").gameObject.AddComponent<HaveGroup>();
        Card1.BindCardItemState(CardItemState);
        Card2 = transform.Find("Root/Card2").gameObject.AddComponent<HaveGroup>();
        Card2.BindCardItemState(CardItemState);
        CardNew = transform.Find("Root/CardNew").gameObject.AddComponent<HaveGroup>();
        CardNew.BindCardItemState(CardItemState);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(Callback);
        });
    }
    
    public class BaseGroup : MonoBehaviour
        {
            public CardCollectionCardItemState CardItemState;
            public List<Transform> StarList = new List<Transform>();

            public virtual void Awake()
            {
                for (var i = 1; transform.Find("Star/" + i); i++)
                {
                    StarList.Add(transform.Find("Star/" + i));
                }
            }

            public void BindCardItemState(CardCollectionCardItemState cardItemState)
            {
                CardItemState = cardItemState;
                UpdateViewState();
            }

            public virtual void UpdateViewState()
            {
                if (gameObject.activeInHierarchy)
                {
                    for (var i = 0; i < StarList.Count; i++)
                    {
                        StarList[i].gameObject.SetActive(CardItemState.CardItemConfig.Level == i+1);
                    }   
                }
            }
        }
        public class HaveGroup : BaseGroup
            {
                private Image Icon;
                private Dictionary<int, Transform> BGList = new Dictionary<int, Transform>();

                public override void Awake()
                {
                    base.Awake();
                    Icon = transform.Find("Mask/Icon").GetComponent<Image>();
                    
                    for (var i = 1; i <= 5; i++)
                    {
                        BGList.Add(i,transform.Find("BGGroup/"+i));
                    }
                    EventDispatcher.Instance.AddEvent<EventViewNewCard>(OnViewNewCard);
                }

                private void OnDestroy()
                {
                    EventDispatcher.Instance.RemoveEvent<EventViewNewCard>(OnViewNewCard);
                }

                public void OnViewNewCard(EventViewNewCard evt)
                {
                    if (evt.CardItemState != CardItemState)
                        return;
                    UpdateViewState();
                }

                public override void UpdateViewState()
                {
                    gameObject.SetActive(CardItemState.CollectCount > 0);
                    base.UpdateViewState();
                    if (gameObject.activeInHierarchy)
                    {
                        Icon.sprite = CardItemState.GetCardSprite();
                        foreach (var pair in BGList)
                        {
                            pair.Value.gameObject.SetActive(pair.Key == CardItemState.CardItemConfig.Level);
                        }   
                    }
                }
            }
}