using System;
using DragonPlus;
using UnityEngine;
public class CardCollectionEntranceRedPoint:MonoBehaviour
{
    private LocalizeTextMeshProUGUI Text;

    private void Awake()
    {
        
        Text = transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventViewNewCard>(OnViewNewCard);
        EventDispatcher.Instance.AddEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
        UpdateViewState();
        if (!CardCollectionModel.Instance.ShowTaskEntrance())
        {
            InvokeRepeating("RepeatRefresh",0f,1f);
        }
    }

    public void RepeatRefresh()
    {
        if (CardCollectionModel.Instance.ShowTaskEntrance())
        {
            UpdateViewState();
            CancelInvoke("RepeatRefresh");
        }
    }
    private bool ShowState = false;
    public void OnCollectNewCardItem(EventCollectNewCardItem evt)
    {
        UpdateViewState();
    }
    public void OnViewNewCard(EventViewNewCard evt)
    {
        UpdateViewState();
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventViewNewCard>(OnViewNewCard);
        EventDispatcher.Instance.RemoveEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
    }

    public void UpdateViewState()
    {
        var unViewedCardCount = CardCollectionModel.Instance.GetUnViewedCardCount();
        Text.gameObject.SetActive(unViewedCardCount > 0);
        Text.SetText(unViewedCardCount.ToString());
        gameObject.SetActive(unViewedCardCount > 0);
    }
}

public class CardCollectionEntranceThemeRedPoint:MonoBehaviour
{
    private LocalizeTextMeshProUGUI Text;
    private CardCollectionCardThemeState Theme;

    public void SetTheme(CardCollectionCardThemeState theme)
    {
        Theme = theme;
        UpdateViewState();
    }

    private void Awake()
    {
        
        Text = transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventViewNewCard>(OnViewNewCard);
        EventDispatcher.Instance.AddEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
        UpdateViewState();
        if (!CardCollectionModel.Instance.ShowTaskEntrance())
        {
            InvokeRepeating("RepeatRefresh",0f,1f);
        }
    }

    public void RepeatRefresh()
    {
        if (CardCollectionModel.Instance.ShowTaskEntrance())
        {
            UpdateViewState();
            CancelInvoke("RepeatRefresh");
        }
    }
    private bool ShowState = false;
    public void OnCollectNewCardItem(EventCollectNewCardItem evt)
    {
        UpdateViewState();
    }
    public void OnViewNewCard(EventViewNewCard evt)
    {
        UpdateViewState();
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventViewNewCard>(OnViewNewCard);
        EventDispatcher.Instance.RemoveEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
    }

    public void UpdateViewState()
    {
        var unViewedCardCount = CardCollectionModel.Instance.GetUnViewedCardCount(Theme);
        Text.gameObject.SetActive(unViewedCardCount > 0);
        Text.SetText(unViewedCardCount.ToString());
        gameObject.SetActive(unViewedCardCount > 0);
    }
}