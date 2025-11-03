using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStoreController
{
    public enum TabState
    {
        MainStore,
        VipStore,
        TeamShop,
    }

    public interface ITabContent
    {
        public void Show();
        public void Hide();
    }
    public class ViewGroup
    {
        public TabState State;
        public ITabContent Content;
        public Transform LabelNormal;
        public Transform LabelSelect;
    }
    private TabState State = TabState.MainStore;
    private Dictionary<TabState, ViewGroup> GroupDic = new Dictionary<TabState, ViewGroup>();
    private Dictionary<TabState, Button> TabBtnDic = new Dictionary<TabState, Button>();
    public void InitTab(TabState state,string name,ITabContent content)
    {
        var viewState = new ViewGroup();
        viewState.State = state;
        viewState.Content = content;
        viewState.LabelNormal = transform.Find("Root/LabelGroup/"+name+"/Normal");
        viewState.LabelSelect = transform.Find("Root/LabelGroup/"+name+"/Selected");
        GroupDic.Add(state,viewState);
        var btn = transform.Find("Root/LabelGroup/" + name).GetComponent<Button>();
        TabBtnDic.Add(state,btn);
        btn.onClick.AddListener(() =>
        {
            if (State != state)
            {
                GroupDic[State].Content.Hide();
                GroupDic[State].LabelSelect.gameObject.SetActive(false);
                GroupDic[State].LabelNormal .gameObject.SetActive(true);
                State = state;
                GroupDic[State].Content.Show();
                GroupDic[State].LabelSelect.gameObject.SetActive(true);
                GroupDic[State].LabelNormal .gameObject.SetActive(false);
            }
        });
        if (state == State)
        {
            viewState.Content.Show();
        }
        else
        {
            viewState.Content.Hide();
        }
        viewState.LabelSelect.gameObject.SetActive(state == State);
        viewState.LabelNormal .gameObject.SetActive(state != State);
    }
}