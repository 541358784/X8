using System;
using System.Drawing;
using System.Linq;
using DragonPlus.Config.Team;
using DragonPlus.UI;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Mosframe;
using Newtonsoft.Json;
using Scripts.UI;
using SomeWhere;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class TeamIconNode:MonoBehaviour
{
    private Vector2 DefaultSize;
    private TeamIconViewState ViewState;
    private Image HeadIconImage;//头像image
    private Image HeadIconFrameImage;//头像框image
    private Transform HeadIconHangNode;//头像预制体挂点
    private Transform HeadIconFrameHangNode;//头像框预制体挂点
    private Transform HeadIconRoot;//头像根节点
    private Transform HeadIconFrameRoot;//头像框根节点
    private bool ShowHeadIconFlag = true;
    private bool ShowHeadIconFrameFlag = true;

    private void Awake()
    {
        HeadIconImage = transform.Find("HeadIconRoot/HeadIcon").GetComponent<Image>();
        HeadIconFrameImage = transform.Find("HeadIconFrameRoot/HeadIconFrame").GetComponent<Image>();
        DefaultSize = (transform as RectTransform).getSize();
        HeadIconHangNode = transform.Find("HeadIconRoot/HeadIconHangNode");
        HeadIconFrameHangNode = transform.Find("HeadIconFrameRoot/HeadIconFrameHangNode");
        HeadIconRoot = transform.Find("HeadIconRoot");
        HeadIconFrameRoot = transform.Find("HeadIconFrameRoot");
    }

    public void ShowHeadIcon(bool isShow)
    {
        ShowHeadIconFlag = isShow;
        HeadIconRoot.gameObject.SetActive(ShowHeadIconFlag);
    }
    public void ShowHeadIconFrame(bool isShow)
    {
        ShowHeadIconFrameFlag = isShow;
        HeadIconFrameRoot.gameObject.SetActive(ShowHeadIconFrameFlag);
    }

    public void SetTeamIconViewState(TeamIconViewState avatar)
    {
        
        if (ViewState.PrivateEquals(avatar))
            return;
        ViewState = avatar;
        UpdateHeadIcon();
    }

    private void OnDestroy()
    {
    }

    public void UpdateHeadIcon()
    {
        var avatar = ViewState.GetUserAvatar();
        if (avatar.IsPrefab)
        {
            HeadIconHangNode.gameObject.RemoveAllChildren();
            var headIconObj = ViewState.GetHeadIconPrefab();
            Instantiate(headIconObj, HeadIconHangNode);
            HeadIconHangNode.gameObject.SetActive(true);
            HeadIconImage.gameObject.SetActive(false);
        }
        else
        {
            HeadIconHangNode.gameObject.RemoveAllChildren();
            HeadIconHangNode.gameObject.SetActive(false);
            HeadIconImage.sprite = ViewState.GetHeadIcon();
            HeadIconImage.gameObject.SetActive(HeadIconImage.sprite);
        }

        var avatarFrame = ViewState.GetUserAvatarFrame();
        if (avatarFrame.IsPrefab)
        {
            HeadIconFrameHangNode.gameObject.RemoveAllChildren();
            var headIconObj = ViewState.GetHeadIconFramePrefab();
            Instantiate(headIconObj, HeadIconFrameHangNode);
            HeadIconFrameHangNode.gameObject.SetActive(true);
            HeadIconFrameImage.gameObject.SetActive(false);
        }
        else
        {
            HeadIconFrameHangNode.gameObject.RemoveAllChildren();
            HeadIconFrameHangNode.gameObject.SetActive(false);
            HeadIconFrameImage.sprite = ViewState.GetHeadIconFrame();
            HeadIconFrameImage.gameObject.SetActive(HeadIconFrameImage.sprite);
        }
    }
    public void SetSize(Vector2 size)
    {
        var scaleX = size.x/DefaultSize.x;
        var scaleY = size.y/DefaultSize.y;
        transform.localScale = new Vector3(scaleX, scaleY);
    }

    public static TeamIconNode GetTeamIconNode(TeamIconViewState TeamIconViewState,Vector3 size)
    {
        var asset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Common/TeamIconNode");
        var  TeamIconNodeObj = Instantiate(asset);
        var TeamIconNode = TeamIconNodeObj.AddComponent<TeamIconNode>();
        TeamIconNode.SetSize(size);
        TeamIconNode.SetTeamIconViewState(TeamIconViewState);
        return TeamIconNode;
    }
    
    public static TeamIconNode BuildTeamIconNode(RectTransform root,TeamIconViewState TeamIconViewState)
    {
        CommonUtils.DestroyAllChildren(root);
        if (root.TryGetComponent<Image>(out var image))
        {
            image.enabled = false;
        }
        var size = root.getSize();
        var TeamIconNode = GetTeamIconNode(TeamIconViewState, size);
        TeamIconNode.transform.SetParent(root,false);
        return TeamIconNode;
    }
}

public static class TeamIconUtils
{
    public static Sprite GetHeadIcon(this TeamIconViewState TeamIconViewState)
    {
        var avatar = TeamIconViewState.GetUserAvatar();
        if (avatar == null)
            return null;
        if (avatar.IsPrefab)
            return null;
        return ResourcesManager.Instance.GetSpriteVariant(avatar.HeadIconAtlas, avatar.HeadIconName);
    }
    public static Sprite GetHeadIconFrame(this TeamIconViewState TeamIconViewState)
    {
        var avatarFrame = TeamIconViewState.GetUserAvatarFrame();
        if(avatarFrame == null)
            return null;
        if (avatarFrame.IsPrefab)
            return null;
        return ResourcesManager.Instance.GetSpriteVariant(avatarFrame.HeadIconFrameAtlas, avatarFrame.HeadIconFrameName);
    }
    public static GameObject GetHeadIconPrefab(this TeamIconViewState TeamIconViewState)
    {
        var avatar = TeamIconViewState.GetUserAvatar();
        if (avatar == null)
            return null;
        if (!avatar.IsPrefab)
            return null;
        return ResourcesManager.Instance.LoadResource<GameObject>(avatar.HeadIconName);
    }
    
    public static GameObject GetHeadIconFramePrefab(this TeamIconViewState TeamIconViewState)
    {
        var avatar = TeamIconViewState.GetUserAvatarFrame();
        if (avatar == null)
            return null;
        if (!avatar.IsPrefab)
            return null;
        return ResourcesManager.Instance.LoadResource<GameObject>(avatar.HeadIconFrameName);
    }
    public static TeamIconViewState GetViewState(this TeamData avatarStorage)
    {
        return new TeamIconViewState(avatarStorage);
    }
    public static TeamIconViewState GetViewState(this TeamInfo avatarStorage)
    {
        return new TeamIconViewState(avatarStorage);
    }
    public static TableTeamIconConfig GetUserAvatar(this TeamData TeamData)
    {
        return TeamData.GetViewState().GetUserAvatar();
    }
    public static TableTeamIconConfig GetUserAvatar(this TeamIconViewState viewState)
    {
        var config = TeamConfigManager.Instance.IconConfigList.Find(a => a.Id == viewState.HeadIconId);
        if (config == null)
            config = TeamConfigManager.Instance.IconConfigList.First();
        return config;
    }
    public static TableTeamIconFrameConfig GetUserAvatarFrame(this TeamData TeamData)
    {
        return TeamData.GetViewState().GetUserAvatarFrame();
    }
    public static TableTeamIconFrameConfig GetUserAvatarFrame(this TeamIconViewState viewState)
    {
        var avatar = viewState.GetUserAvatar();
        var avatarDefaultFrame =
            TeamConfigManager.Instance.IconFrameConfigList.Find(a => a.Id == avatar.DefaultHeadIconFrameId);
        var overrideAvatarFrame = TeamConfigManager.Instance.IconFrameConfigList.Find(a => a.Id == viewState.HeadIconFrameId);
        if (overrideAvatarFrame == null)
        {
            return avatarDefaultFrame;
        }
        else
        {
            if (overrideAvatarFrame.HeadIconFrameType > avatarDefaultFrame.HeadIconFrameType)
                return overrideAvatarFrame;
            else
                return avatarDefaultFrame;
        }
    }
    
    public static bool SameAs(this TeamIconViewState viewState1,TeamIconViewState viewState2)
    {
        if (viewState1 == null && viewState2 == null)
            return true;
        if (viewState1 == null || viewState2 == null)
            return false;
        return viewState1.HeadIconId == viewState2.HeadIconId &&
               viewState1.HeadIconFrameId == viewState2.HeadIconFrameId;
    }
}

public class TeamIconViewState
{
    public int HeadIconId;
    public int HeadIconFrameId;

    public TeamIconViewState()
    { }
    public TeamIconViewState(TeamData avatarStorage)
    {
        if (avatarStorage != null)
        {
            HeadIconId = avatarStorage.Badge;
            HeadIconFrameId = avatarStorage.BadgeFrame;   
        }
        else
        {
            HeadIconId = -1;
            HeadIconFrameId = -1;
        }
    }
    public TeamIconViewState(TeamInfo avatarStorage)
    {
        if (avatarStorage != null)
        {
            HeadIconId = avatarStorage.Badge;
            var extra = JsonConvert.DeserializeObject<TeamDataExtra>(avatarStorage.TeamExtra);
            HeadIconFrameId = extra.BadgeFrame;
        }
        else
        {
            HeadIconId = -1;
            HeadIconFrameId = -1;
        }
    }

    public TeamIconViewState(int headIconId,int headIconFrameId)
    {
        HeadIconId = headIconId;
        HeadIconFrameId = headIconFrameId;
    }
}
public static class TeamIconViewStateUtils
{
    public static bool PrivateEquals(this TeamIconViewState a,TeamIconViewState b)
    {
        if (a!=null && b!= null &&
            a.HeadIconId == b.HeadIconId &&
            a.HeadIconFrameId == b.HeadIconFrameId)
            return true;
        else
            return false;
    }
}