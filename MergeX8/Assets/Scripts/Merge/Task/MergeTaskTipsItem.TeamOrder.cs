using Activity.TimeOrder;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class MergeTaskTipsItem
{
    private Transform _teamOrderRoot = null;
    public Image _teamIcon;
    private Transform _teamBg;
    
    private GameObject _teamOrder;
    public LocalizeTextMeshProUGUI _teamOrderText;
    public Image _teamOrderIcon;

    private TeamIconNode _teamIconNode;
    
    private void AwakeTeamOrder()
    {
        _teamOrderRoot = transform.Find("Guild");
        _teamIcon = transform.Find("Guild/GuildIcon").GetComponent<Image>();
        _teamBg = transform.Find("BGGuild");

        _teamBg.gameObject.SetActive(false);
        _teamOrderRoot.gameObject.SetActive(false);
        
        _teamOrder = transform.Find("StartActivity/Guild").gameObject;
        _teamOrderText = _teamOrder.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        _teamOrderIcon = _teamOrder.transform.Find("Icon").GetComponent<Image>();
        _teamOrder.gameObject.SetActive(false);
        
        _teamOrderRoot.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.LogError("---------------打开工会");
        });
    }

    private void RefreshRepeatingTeamOrder()
    {
        // if(storageTaskItem.Type != (int)MainOrderType.Team)
        //     return;
        //
        // if (!TeamManager.Instance.HasTeam())
        // {
        //     _teamBg.gameObject.SetActive(false);
        //     _teamOrderRoot.gameObject.SetActive(false);
        // }
        // else
        // {
        //     _teamBg.gameObject.SetActive(true);
        //     _teamOrderRoot.gameObject.SetActive(true);
        // }
    }

    private void InitTeamOrder(StorageTaskItem storageItem)
    {
        _teamBg.gameObject.SetActive(false);
        _teamOrder.gameObject.SetActive(false);
        _teamOrderRoot.gameObject.SetActive(false);
        
        if(storageItem == null || storageItem.Type != (int)MainOrderType.Team)
            return;
        
        _teamBg.gameObject.SetActive(true);
        _teamOrderRoot.gameObject.SetActive(true);
        
        _teamIconNode = TeamIconNode.BuildTeamIconNode(_teamIcon.transform as RectTransform, TeamManager.Instance.MyTeamInfo.GetViewState());
    }
}