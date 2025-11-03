using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Scripts.UI;
using TMatch;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGuildJoinController
{
    public class CreateGroup : MonoBehaviour
    {
        public TeamIconNode TeamIcon;
        public InputField NameText;
        public InputField DescText;
        public TMP_InputField RequireLevelText;
        public int BadgeId=1;
        private TeamIconViewState viewState;
        private const int RequireLevelMin = 5;
        private const int RequireLevelMax = 40;
        public int RequireLevel;
        private Transform ErrorText;
        public void Init()
        {
            viewState = new TeamIconViewState(BadgeId, -1);
            TeamIcon = TeamIconNode.BuildTeamIconNode(transform.Find("Icon/GuildIcon") as RectTransform,viewState);
            transform.Find("Icon/Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIPopupGuildSetIconController.Open((badge) =>
                {
                    if (badge != BadgeId)
                    {
                        BadgeId = badge;
                        viewState = new TeamIconViewState(BadgeId, -1);
                        TeamIcon.SetTeamIconViewState(viewState);
                    }
                });
            });
            var createBtn = transform.Find("Button").GetComponent<Button>();
            NameText = transform.Find("Name/Input").GetComponent<InputField>();
            NameText.SetTextWithoutNotify("");
            NameText.onValueChanged.AddListener((str) =>
            {
                ErrorText.gameObject.SetActive(NameText.text.Length < 3);
                createBtn.interactable = CanCreate();
            });
            NameText.onEndEdit.AddListener((str) =>
            {
                if (str.Length > NameText.characterLimit)
                {
                    NameText.SetTextWithoutNotify(str.Substring(0,NameText.characterLimit));
                }
            });
            DescText = transform.Find("Message/Input").GetComponent<InputField>();
            DescText.SetTextWithoutNotify("");
            DescText.onEndEdit.AddListener((str) =>
            {
                if (str.Length > DescText.characterLimit)
                {
                    DescText.SetTextWithoutNotify(str.Substring(0,DescText.characterLimit));
                }
            });
            RequireLevelText = transform.Find("Lv/Input").GetComponent<TMP_InputField>();
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
                }
                else
                {
                    RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString());
                }
            });
            RequireLevel = RequireLevelMin;
            RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString());
            transform.Find("Lv/Button+").GetComponent<Button>().onClick.AddListener(() =>
            {
                RequireLevel++;
                if (RequireLevel > RequireLevelMax)
                {
                    RequireLevel = RequireLevelMax;
                }
                RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString());
            });
            transform.Find("Lv/Button-").GetComponent<Button>().onClick.AddListener(() =>
            {
                RequireLevel--;
                if (RequireLevel < RequireLevelMin)
                {
                    RequireLevel = RequireLevelMin;
                }
                RequireLevelText.SetTextWithoutNotify(RequireLevel.ToString()); 
            });
            var price = TeamManager.Instance.CreateCoins();
            transform.Find("Button/Text").GetComponent<LocalizeTextMeshProUGUI>().SetTermFormats(price.ToString());
            transform.Find("Button/GreyText").GetComponent<LocalizeTextMeshProUGUI>().SetTermFormats(price.ToString());
            createBtn.interactable = CanCreate();
            createBtn.onClick.AddListener(() =>
            {
                var nameText = NameText.text;
                if (nameText.IsEmptyString())
                {
                    Debug.LogError("公会名不能为空");
                    return;   
                }
                var descText = DescText.text;
                
                var itemChangeReason =
                    new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TeamConsume);
                if (UserData.Instance.CanAford(UserData.ResourceId.Coin, price))
                {
                    WaitingManager.Instance.OpenWindow(5f);
                    TeamManager.Instance.CreateNewTeam(nameText,descText,BadgeId,RequireLevel, (s) =>
                    {
                        WaitingManager.Instance.CloseWindow();
                        if (!s)
                        {
                            Debug.LogError("创建公会失败");
                            ErrorText.gameObject.SetActive(true);
                        }
                        else
                        {
                            UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, price, itemChangeReason);
                            Instance.AnimCloseWindow();
                            UIPopupGuildMainController.Open();
                        }
                    });
                }
            });
            ErrorText = transform.Find("ErrorText");
            ErrorText.gameObject.SetActive(NameText.text.Length < 3);
        }

        public bool CanCreate()
        {
            return UserData.Instance.CanAford(UserData.ResourceId.Coin, TeamManager.Instance.CreateCoins()) &&
                   NameText.text.Length >= 3;
        }
    }
}