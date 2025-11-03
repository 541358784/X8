using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.Team;
using DragonU3DSDK.Network.API.Protocol;
using Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupGuildSetController:UIWindowController
{
    
    public static UIPopupGuildSetController Instance;
    public static UIPopupGuildSetController Open(TeamData teamData)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildSet,teamData) as UIPopupGuildSetController;
        return Instance;
    }

    private OwnerGroup Owner;
    private OtherGroup Other;
    
    
    public override void PrivateAwake()
    {
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var teamData = objs[0] as TeamData;
        Owner = transform.Find("Root/President").gameObject.AddComponent<OwnerGroup>();
        Owner.Init(teamData);
        
        Other = transform.Find("Root/Other").gameObject.AddComponent<OtherGroup>();
        Other.Init(teamData);
        
        Owner.gameObject.SetActive(teamData.LeaderId == (long)TeamManager.Instance.MyPlayerId);
        Other.gameObject.SetActive(teamData.LeaderId != (long)TeamManager.Instance.MyPlayerId);
    }


    public class OwnerGroup:MonoBehaviour
    {
        private TeamData TeamData => TeamManager.Instance.MyTeamInfo;
        private int IconIndex;
        private TeamIconViewState viewState;
        private TeamIconNode Icon;
        private LocalizeTextMeshProUGUI LevelText;
        private Button ChangeIconBtn;
        private InputField NameInput;
        private Button NameInputBtn;
        private InputField DescInput;
        private Button DescInputBtn;
        private int RequireLevel;
        private TMP_InputField RequireLevelText;
        private const int RequireLevelMin = 5;
        private const int RequireLevelMax = 40;
        private Transform ErrorText;

        public void ShowError()
        {
            ErrorText.DOKill();
            ErrorText.gameObject.SetActive(true);
            DOVirtual.DelayedCall(1f, () =>
            {
                ErrorText.gameObject.SetActive(false);
            }).SetTarget(ErrorText);
        }
        public void Init(TeamData inteamdata)
        {
            transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
            {
                TeamManager.Instance.PushMyTeamInfo();
                Instance.AnimCloseWindow();
            });
            ErrorText = transform.Find("");
            ErrorText.gameObject.SetActive(false);
            IconIndex = TeamData.Badge;
            viewState = new TeamIconViewState(IconIndex, 0);
            Icon = TeamIconNode.BuildTeamIconNode(transform.Find("Set/Icon/GuildIcon") as RectTransform,
                TeamData.GetViewState());
            transform.Find("Set/Icon/GuildBG").gameObject.SetActive(false);
            ChangeIconBtn = transform.Find("Set/Icon/Button").GetComponent<Button>();
            ChangeIconBtn.onClick.AddListener(()=>
            {
                UIPopupGuildSetIconController.Open((result) =>
                {
                    if (IconIndex != result)
                    {
                        IconIndex = result;
                        viewState = new TeamIconViewState(IconIndex, 0);
                        Icon.SetTeamIconViewState(viewState);
                        TeamData.Badge = IconIndex;
                        TeamManager.Instance.PushMyTeamInfo();
                    }
                });
            });
            LevelText = transform.Find("Set/Icon/LvText").GetComponent<LocalizeTextMeshProUGUI>();
            LevelText.SetText(TeamData.ExtraData.TeamLevel.ToString());
            NameInput = transform.Find("Set/Name/Input").GetComponent<InputField>();
            NameInput.SetTextWithoutNotify(TeamData.Name);
            NameInput.onEndEdit.AddListener((text) =>
            {
                if (text.Length < 3)
                {
                    NameInput.SetTextWithoutNotify(TeamData.Name);
                }
                else
                {
                    var oldName = TeamData.Name;
                    TeamData.Name = text;
                    TeamManager.Instance.PushMyTeamInfo((b) =>
                    {
                        if (!b)
                        {
                            TeamData.Name = oldName;
                            NameInput.SetTextWithoutNotify(TeamData.Name);
                            ShowError();
                        }
                    });
                }
            });
            NameInputBtn = transform.Find("Set/Name/Button").GetComponent<Button>();
            NameInputBtn.onClick.AddListener(() =>
            {
                NameInput.ActivateInputField();
            });
            DescInput = transform.Find("Set/Message/Input").GetComponent<InputField>();
            DescInput.SetTextWithoutNotify(TeamData.Description);
            DescInput.onEndEdit.AddListener((text) =>
            {
                TeamData.Description = text;
                TeamManager.Instance.PushMyTeamInfo();
            });
            DescInputBtn = transform.Find("Set/Message/Button").GetComponent<Button>();
            DescInputBtn.onClick.AddListener(() =>
            {
                DescInput.ActivateInputField();
            });
            
            RequireLevelText = transform.Find("Set/Lv/Input").GetComponent<TMP_InputField>();
            RequireLevelText.onEndEdit.AddListener((num) =>
            {
                if (int.TryParse(num, out var level))
                {
                    if (level > RequireLevelMax)
                    {
                        RequireLevel = RequireLevelMax;
                        RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString());
                    }
                    else if (level < RequireLevelMin)
                    {
                        RequireLevel = RequireLevelMin;
                        RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString());
                    }
                    else
                    {
                        RequireLevel = level;   
                    }
                    TeamData.RequireLevel = RequireLevel;
                    TeamManager.Instance.PushMyTeamInfo();
                }
                else
                {
                    RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString());
                }
            });
            RequireLevel = TeamData.RequireLevel;
            RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString());
            transform.Find("Set/Lv/Button+").GetComponent<Button>().onClick.AddListener(() =>
            {
                RequireLevel++;
                if (RequireLevel > RequireLevelMax)
                {
                    RequireLevel = RequireLevelMax;
                }
                TeamData.RequireLevel = RequireLevel;
                RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString());
            });
            transform.Find("Set/Lv/Button-").GetComponent<Button>().onClick.AddListener(() =>
            {
                RequireLevel--;
                if (RequireLevel < RequireLevelMin)
                {
                    RequireLevel = RequireLevelMin;
                }
                TeamData.RequireLevel = RequireLevel;
                RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString());   
            });
            
            transform.Find("Set/ButtonGroup/ButtonExit").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIPopupGuildDisbandTipController.Open((s) =>
                {
                    if (s)
                    {
                        var memberList = TeamData.PlayerList;
                        for (var i = 0; i < memberList.Count; i++)
                        {
                            if (memberList[i].PlayerId != (long)TeamManager.Instance.MyPlayerId)
                            {
                                var member = memberList[i];
                                TeamManager.Instance.KickMember(member.PlayerId,null);   
                            }
                        }
                        TeamManager.Instance.LeaveTeam((b) =>
                        {
                            if (b)
                            {
                                Instance?.AnimCloseWindow();
                                UIPopupGuildMainController.Instance?.AnimCloseWindow();
                            }
                        });
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTeamDisband,data1:TeamData.TeamId.ToString());
                    }
                });
            });
            
            transform.Find("Set/ButtonGroup/ButtonLvUp").GetComponent<Button>().onClick.AddListener(() =>
            {
                var curLevelConfig = TeamConfigManager.Instance.LevelConfigList.Find(a => a.Id == TeamManager.Instance.MyTeamInfo.ExtraData.TeamLevel);
                if (curLevelConfig.UpPrice > 0) 
                    UIPopupGuildLevelUpController.Open();
            });

        }
    }
    
    public class OtherGroup:MonoBehaviour
    {
        private TeamData TeamData;
        private TeamIconNode Icon;
        private LocalizeTextMeshProUGUI LevelText;
        public void Init(TeamData teamData)
        {
            TeamData = teamData;
            transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
            {
                Instance.AnimCloseWindow();
            });
            Icon = TeamIconNode.BuildTeamIconNode(transform.Find("Set/Icon/GuildIcon") as RectTransform,TeamData.GetViewState());
            transform.Find("Set/Icon/GuildBG").gameObject.SetActive(false);
            LevelText = transform.Find("Set/Icon/LvText").GetComponent<LocalizeTextMeshProUGUI>();
            LevelText.SetText(TeamData.ExtraData.TeamLevel.ToString());
            transform.Find("Set/Name/Input/Text").GetComponent<Text>().text = TeamData.Name;
            transform.Find("Set/Message/Input/Text").GetComponent<Text>().text = TeamData.Description;
            transform.Find("Set/Lv/Input/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(TeamData.RequireLevel.ToString());
            transform.Find("ButtonGroup/ButtonExit").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIPopupGuildExitTipController.Open((s) =>
                {
                    if (s)
                    {
                        TeamManager.Instance.LeaveTeam((b) =>
                        {
                            if (b)
                            {
                                Instance?.AnimCloseWindow();
                                UIPopupGuildMainController.Instance?.AnimCloseWindow();
                            }
                        });
                    }
                });
            });

        }
    }
}