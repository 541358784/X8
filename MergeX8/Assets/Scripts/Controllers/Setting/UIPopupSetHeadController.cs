using System.Collections.Generic;
using System.Resources;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSetHeadController : UIWindowController
{
    private Button _buttonClose;
    
    private Button _buttonTrue;
    
    private GameObject _content = null;

    private Animator _animator;
    private int _headIconId = 0;
    private StorageAvatar _storageAvatar;
    private Dictionary<int,GameObject> _choiceObjs = new Dictionary<int,GameObject>();
    public override void PrivateAwake()
    {
        _storageAvatar = StorageManager.Instance.GetStorage<StorageHome>().AvatarData;
        
        _animator = transform.GetComponent<Animator>();
        
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseClick);
        
        _buttonTrue = GetItem<Button>("Root/OKButton");
        _buttonTrue.onClick.AddListener(() =>
        {
            if (_headIconId != _storageAvatar.AvatarIconId)
            {
                var avatar = GlobalConfigManager.Instance.GetTableAvatar(_headIconId);
                if (!avatar.isNeedCollect || _storageAvatar.CollectedAvatarList.Contains(_headIconId))
                {
                    _storageAvatar.AvatarIconId = _headIconId;
                    EventDispatcher.Instance.DispatchEvent(EventEnum.UPDATE_HEAD,_storageAvatar.GetViewState());
                    TeamManager.Instance.UploadMyInfo();
                }
            }
            OnCloseClick();
        });
        
        _content = GetItem("Root/ScrollView/Viewport/Content");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        _headIconId = _storageAvatar.AvatarIconId;
        InitHeadIcons();
        UpdateChoice(_headIconId);
    }

    private void OnCloseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        _buttonClose.interactable = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () =>
            {
                CloseWindowWithinUIMgr(true);
                UIManager.Instance.OpenUI(UINameConst.UIPopupSet1, false);
            }));
    }

    private void InitHeadIcons()
    {
       GameObject cloneObj = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Home/UIPopupSetHeadCell");
       
       
        for(int i = 0; i < GlobalConfigManager.Instance._tableAvatars.Count; i++)
        {
            var data = GlobalConfigManager.Instance._tableAvatars[i];
            if (data.isNeedCollect && !_storageAvatar.CollectedAvatarList.Contains(data.id))
            {
                continue;
            }
            Transform instanObj = GameObject.Instantiate(cloneObj.transform);
            CommonUtils.AddChild(_content.transform, instanObj);
            instanObj.Find("BG").gameObject.SetActive(false);
            var headIconRoot = instanObj.Find("Head") as RectTransform;
            var headIcon = HeadIconNode.BuildHeadIconNode(headIconRoot,new AvatarViewState(data.id,-1,"旗鼓相当的对手",false));
            
            
            _choiceObjs.Add(data.id,instanObj.Find("Choice").gameObject);
            var redPoint = instanObj.Find("RedPoint").gameObject.AddComponent<UIPopupSetHeadCellRedPoint>();
            redPoint.Init(data.id);
            int index = data.id;
            instanObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (data.isNeedCollect && !_storageAvatar.CollectedAvatarList.Contains(data.id))
                    return;
                _headIconId = index;
                if (_storageAvatar.UnViewedAvatarList.Contains(_headIconId))
                {
                    _storageAvatar.UnViewedAvatarList.Remove(_headIconId);
                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.VIEW_NEW_HEAD,_headIconId);
                }
                UpdateChoice(_headIconId);
            });
        }
    }

    private void UpdateChoice(int index)
    {
        foreach (var choicePair in _choiceObjs)
        {
            choicePair.Value.SetActive(choicePair.Key == index);
        }
    }
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;
        
        OnCloseClick();
    }
}