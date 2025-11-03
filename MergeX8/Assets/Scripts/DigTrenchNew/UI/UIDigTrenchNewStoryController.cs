using System;
using DragonPlus;
using DragonU3DSDK.Asset;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UIDigTrenchNewStoryController:UIWindowController
{
    public static UIDigTrenchNewStoryController Instance;
    public static UIDigTrenchNewStoryController Open(TableStory config,Action callback = null)
    {
        if (Instance)
        {
            Instance.CloseWindowWithinUIMgr(true);
            Instance.Callback?.Invoke();
        }
        Instance =
            UIManager.Instance.OpenUI(UINameConst.UIDigTrenchNewStory, config,callback) as UIDigTrenchNewStoryController;
        return Instance;
    }
    public Button SkipBtn;
    public Transform LeftBig;
    public Transform LeftSmall;
    public Transform RightBig;
    public Transform RightSmall;
    public Role LeftRole;
    public Role RightRole;
    public TableStory CurStoryConfig;
    public LocalizeTextMeshProUGUI StoryText;
    public LocalizeTextMeshProUGUI LeftNameText;
    public Image LeftNameBG;
    public LocalizeTextMeshProUGUI RightNameText;
    public Image RightNameBG;
    private Image TextBG;
    
    public Button NextBtn;
    public Action Callback;
    public override void PrivateAwake()
    {
        SkipBtn = GetItem<Button>("Root/SkipButton");
        SkipBtn.onClick.AddListener(() =>
        {
            CloseWindowWithinUIMgr(true);
            Callback?.Invoke();
        });
        LeftBig = transform.Find("Root/Story/LeftSpine/Normal/StorySpine");
        LeftSmall = transform.Find("Root/Story/LeftSpine/Behind/StorySpine");
        RightBig = transform.Find("Root/Story/RightSpine/Normal/StorySpine");
        RightSmall = transform.Find("Root/Story/RightSpine/Behind/StorySpine");
        LeftRole = new Role();
        RightRole = new Role();
        StoryText = transform.Find("Root/Story/Text").GetComponent<LocalizeTextMeshProUGUI>();
        LeftNameText = transform.Find("Root/Story/LeftName/Text").GetComponent<LocalizeTextMeshProUGUI>();
        LeftNameText.transform.parent.gameObject.SetActive(false);
        RightNameText = transform.Find("Root/Story/RightName/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RightNameText.transform.parent.gameObject.SetActive(false);
        LeftNameBG = transform.Find("Root/Story/LeftName/BG").GetComponent<Image>();
        RightNameBG = transform.Find("Root/Story/RightName/BG").GetComponent<Image>();
        TextBG = transform.Find("Root/Story/BG").GetComponent<Image>();
        NextBtn = transform.Find("Root/NextButton").GetComponent<Button>();
        NextBtn.onClick.AddListener(() =>
        {
            var nextStory = GlobalConfigManager.Instance.GetTableStory(CurStoryConfig.next_id);
            if (nextStory != null && CurStoryConfig.next_id > 0)
            {
                CurStoryConfig = nextStory;
                PlayStory(CurStoryConfig);
            }
            else
            {
                CloseWindowWithinUIMgr(true);
                Callback?.Invoke();
            }
        });
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CurStoryConfig = objs[0] as TableStory;
        if (objs.Length > 1)
        {
            Callback = objs[1] as Action;   
        }
        PlayStory(CurStoryConfig);
    }

    public void PlayStory(TableStory story)
    {
        SkipBtn.gameObject.SetActive(story.skip);
        var roleConfig =  GlobalConfigManager.Instance.GetTableRole(story.role_id);
        if (roleConfig == null)
        {
            Debug.LogError("role表中未找到id"+story.role_id+" storyId"+story.id);
            return;
        }
        StoryText.SetTerm(story.en);
        if (story.position == 1)
        {
            LeftRole.SetConfig(roleConfig);
            LeftRole.transform.SetParent(LeftBig,false);
            if (RightRole.transform)
                RightRole.transform.SetParent(RightSmall,false);
            LeftNameText.transform.parent.gameObject.SetActive(true);
            LeftNameText.SetTerm(roleConfig.roleName);
        }
        else if(story.position == 2)
        {
            RightRole.SetConfig(roleConfig);
            RightRole.transform.SetParent(RightBig,false);
            if (LeftRole.transform)
                LeftRole.transform.SetParent(LeftSmall,false);
            RightNameText.transform.parent.gameObject.SetActive(true);
            RightNameText.SetTerm(roleConfig.roleName);
        }
        
        var roleColor = GlobalConfigManager.Instance.GetRoleColor(roleConfig.roleColor);
        if (roleColor != null)
        {
            var nameBG = story.position == 1 ? LeftNameBG : RightNameBG;
            nameBG.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.UIStoryAtlas, roleColor.nameBg2);
            TextBG.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.UIStoryAtlas, roleColor.contentBg2);
        
            Material material = LocalizationManager.Instance.GetLocaleMaterial(roleColor.nameMaterial);
            if (material != null)
            {
                var text = story.position == 1 ? LeftNameText : RightNameText;
                text.isAutoChangeMatrial = false;
                text.m_TmpText.fontMaterial = material;      
            }
        }
        
    }
    public class Role
    {
        public TableRoles Config;
        public Transform transform;
        // public Transform Container;

        public void SetConfig(TableRoles config)
        {
            if (Config != null)
            {
                if (Config.rolePrefabName == config.rolePrefabName)
                {
                    var _skeletonAnimation= transform.Find("Mask/PortraitSpine").GetComponent<SkeletonGraphic>();
                    _skeletonAnimation.AnimationState.SetAnimation(0, config.roleAniName+"_appear", false);
                    _skeletonAnimation.AnimationState.AddAnimation(0, config.roleAniName, true,1.66f);
                    _skeletonAnimation.AnimationState.Update(0);
                }
                else
                {
                    DestroyImmediate(transform.gameObject);
                    var spinePrefab=ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/PortraitSpine/"+config.rolePrefabName);
                    transform =Instantiate(spinePrefab).transform;
                    var _skeletonAnimation= transform.Find("Mask/PortraitSpine").GetComponent<SkeletonGraphic>();
                    _skeletonAnimation.AnimationState.SetAnimation(0, config.roleAniName+"_appear", false);
                    _skeletonAnimation.AnimationState.AddAnimation(0, config.roleAniName, true,1.66f);
                    _skeletonAnimation.AnimationState.Update(0);
                }
            }
            else
            {
                var spinePrefab=ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/PortraitSpine/"+config.rolePrefabName);
                transform =Instantiate(spinePrefab).transform;
                var _skeletonAnimation= transform.Find("Mask/PortraitSpine").GetComponent<SkeletonGraphic>();
                _skeletonAnimation.AnimationState.SetAnimation(0, config.roleAniName+"_appear", false);
                _skeletonAnimation.AnimationState.AddAnimation(0, config.roleAniName, true,1.66f);
                _skeletonAnimation.AnimationState.Update(0);
            }
            Config = config;
        }
    }
}