using DragonPlus;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGuildJoinPreviewController
{
    
    public class TeamMemberItem:MonoBehaviour
    {
        private HeadIconNode HeadIconNode;
        private Text NameText;
        private LocalizeTextMeshProUGUI LevelText;

        public void Init(PlayerData playerData)
        {
            LevelText = transform.Find("Lv/LvText").GetComponent<LocalizeTextMeshProUGUI>();
            NameText = transform.Find("NameText").GetComponent<Text>();
            LevelText.SetText(playerData.PlayerInfo.Level.ToString());
            NameText.text = playerData.PlayerInfo.UserName;
            var avatarData = new AvatarViewState(playerData.PlayerInfo.AvatarIcon,
                playerData.PlayerInfo.AvatarFrameIcon, playerData.PlayerInfo.UserName, false);
            HeadIconNode = HeadIconNode.BuildHeadIconNode(transform.Find("HeadGroup/Head") as RectTransform, avatarData);
            transform.Find("HeadGroup/BG").gameObject.SetActive(false);
            transform.Find("HeadGroup").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIPopupTeamMemberInfoController.Open(playerData.PlayerInfo);
            });
            
        }
    }
}