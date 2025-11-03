using System;
using System.Drawing;
using System.Linq;
using DragonPlus.UI;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Mosframe;
using SomeWhere;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class HeadIconNode:MonoBehaviour
{
    private Vector2 DefaultSize;
    private AvatarViewState ViewState;
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

    public void SetAvatarViewState(AvatarViewState avatar)
    {
        if (ViewState != null && ViewState.IsMe)
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_HEAD,OnChangeHeadIcon);
        }
        if (ViewState.PrivateEquals(avatar))
            return;
        ViewState = avatar;
        if (ViewState.IsMe)
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.UPDATE_HEAD,OnChangeHeadIcon);
        }
        UpdateHeadIcon();
    }

    private void OnDestroy()
    {
        if (ViewState != null && ViewState.IsMe)
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_HEAD,OnChangeHeadIcon);
        }
    }

    public void OnChangeHeadIcon(BaseEvent evt)
    {
        var newViewState = evt.datas[0] as AvatarViewState;
        SetAvatarViewState(newViewState);
    }

    public void UpdateHeadIcon()
    {
        var avatar = ViewState.GetUserAvatar();
        if (avatar.isPrefab)
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
            HeadIconImage.gameObject.SetActive(true);
        }

        var avatarFrame = ViewState.GetUserAvatarFrame();
        if (avatarFrame.isPrefab)
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
            HeadIconFrameImage.gameObject.SetActive(true);
        }
    }
    public void SetSize(Vector2 size)
    {
        var scaleX = size.x/DefaultSize.x;
        var scaleY = size.y/DefaultSize.y;
        transform.localScale = new Vector3(scaleX, scaleY);
    }

    public static HeadIconNode GetHeadIconNode(AvatarViewState avatarViewState,Vector3 size)
    {
        var asset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Common/HeadIconNode");
        var  headIconNodeObj = Instantiate(asset);
        var headIconNode = headIconNodeObj.AddComponent<HeadIconNode>();
        headIconNode.SetSize(size);
        headIconNode.SetAvatarViewState(avatarViewState);
        return headIconNode;
    }

    public static HeadIconNode GetMyHeadIconNode(Vector3 size)
    {
        return GetHeadIconNode(StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetViewState(), size);
    }
    
    public static HeadIconNode BuildHeadIconNode(RectTransform root,AvatarViewState avatarViewState)
    {
        CommonUtils.DestroyAllChildren(root);
        if (root.TryGetComponent<Image>(out var image))
        {
            image.enabled = false;
        }
        var size = root.getSize();
        var headIconNode = GetHeadIconNode(avatarViewState, size);
        headIconNode.transform.SetParent(root,false);
        return headIconNode;
    }
    public static HeadIconNode BuildMyHeadIconNode(RectTransform root)
    {
        return BuildHeadIconNode(root,StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetViewState());
    }
}

public static class HeadIconUtils
{
    public static Sprite GetHeadIcon(this AvatarViewState avatarViewState)
    {
        var avatar = avatarViewState.GetUserAvatar();
        if (avatar == null)
            return null;
        if (avatar.isPrefab)
            return null;
        return ResourcesManager.Instance.GetSpriteVariant(avatar.headIconAtlas, avatar.headIconName);
    }
    public static Sprite GetHeadIconFrame(this AvatarViewState avatarViewState)
    {
        var avatarFrame = avatarViewState.GetUserAvatarFrame();
        if(avatarFrame == null)
            return null;
        if (avatarFrame.isPrefab)
            return null;
        return ResourcesManager.Instance.GetSpriteVariant(avatarFrame.headIconFrameAtlas, avatarFrame.headIconFrameName);
    }
    public static GameObject GetHeadIconPrefab(this AvatarViewState avatarViewState)
    {
        var avatar = avatarViewState.GetUserAvatar();
        if (avatar == null)
            return null;
        if (!avatar.isPrefab)
            return null;
        return ResourcesManager.Instance.LoadResource<GameObject>(avatar.headIconName);
    }
    
    public static GameObject GetHeadIconFramePrefab(this AvatarViewState avatarViewState)
    {
        var avatar = avatarViewState.GetUserAvatarFrame();
        if (avatar == null)
            return null;
        if (!avatar.isPrefab)
            return null;
        return ResourcesManager.Instance.LoadResource<GameObject>(avatar.headIconFrameName);
    }
    public static AvatarViewState GetMyViewState()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetViewState();
    }
    public static AvatarViewState GetViewState(this StorageAvatar avatarStorage)
    {
        return new AvatarViewState(avatarStorage);
    }
    public static void GetAvatar(int avatarId)
    {
        if (!StorageManager.Instance.GetStorage<StorageHome>().AvatarData.CollectedAvatarList.Contains(avatarId))
        {
            StorageManager.Instance.GetStorage<StorageHome>().AvatarData.CollectedAvatarList.Add(avatarId);
            StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UnViewedAvatarList.Add(avatarId);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.GET_NEW_HEAD,avatarId);
        }
    }
    public static TableAvatar GetUserAvatar(this StorageAvatar storageAvatar)
    {
        return storageAvatar.GetViewState().GetUserAvatar();
    }
    public static TableAvatar GetUserAvatar(this AvatarViewState viewState)
    {
        return GlobalConfigManager.Instance.GetTableAvatar(viewState.HeadIconId);
    }
    public static TableAvatarFrame GetUserAvatarFrame(this StorageAvatar storageAvatar)
    {
        return storageAvatar.GetViewState().GetUserAvatarFrame();
    }
    public static TableAvatarFrame GetUserAvatarFrame(this AvatarViewState viewState)
    {
        var avatar = viewState.GetUserAvatar();
        GlobalConfigManager.Instance.TableAvatarFrames.TryGetValue(avatar.defaultHeadIconFrameId,out var avatarDefaultFrame);
        GlobalConfigManager.Instance.TableAvatarFrames.TryGetValue(viewState.HeadIconFrameId,out var overrideAvatarFrame);
        if (overrideAvatarFrame == null)
        {
            return avatarDefaultFrame;
        }
        else
        {
            if (overrideAvatarFrame.headIconFrameType > avatarDefaultFrame.headIconFrameType)
                return overrideAvatarFrame;
            else
                return avatarDefaultFrame;
        }
    }

    public static int GetRandomHead()
    {
        return GlobalConfigManager.Instance._tableAvatars.RandomPickOne().id;
    }
    
    public static int GetRandomHeadFrame()
    {
        var keys = GlobalConfigManager.Instance.TableAvatarFrames.Keys.ToList();
        var key = keys.RandomPickOne();
        return GlobalConfigManager.Instance.TableAvatarFrames[key].id;
    }
    
    public static bool SameAs(this AvatarViewState viewState1,AvatarViewState viewState2)
    {
        if (viewState1 == null && viewState2 == null)
            return true;
        if (viewState1 == null || viewState2 == null)
            return false;
        return viewState1.HeadIconId == viewState2.HeadIconId &&
               viewState1.HeadIconFrameId == viewState2.HeadIconFrameId &&
               viewState1.UserName == viewState2.UserName &&
               viewState1.IsMe == viewState2.IsMe;
    }
}

public class AvatarViewState
{
    public int HeadIconId;
    public int HeadIconFrameId;
    public string UserName;
    public bool IsMe;

    public AvatarViewState()
    { }
    public AvatarViewState(StorageAvatar avatarStorage)
    {
        HeadIconId = avatarStorage.AvatarIconId;
        HeadIconFrameId = avatarStorage.AvatarIconFrameId;
        UserName = avatarStorage.UserName;
        IsMe = true;
    }

    public AvatarViewState(int headIconId,int headIconFrameId,string userName,bool isMe)
    {
        HeadIconId = headIconId;
        HeadIconFrameId = headIconFrameId;
        UserName = userName;
        IsMe = isMe;
    }
}
public static class AvatarViewStateUtils
{
    public static bool PrivateEquals(this AvatarViewState a,AvatarViewState b)
    {
        if (a!=null && b!= null &&
            a.HeadIconId == b.HeadIconId &&
            a.HeadIconFrameId == b.HeadIconFrameId &&
            a.UserName == b.UserName &&
            a.IsMe == b.IsMe)
            return true;
        else
            return false;
    }
}