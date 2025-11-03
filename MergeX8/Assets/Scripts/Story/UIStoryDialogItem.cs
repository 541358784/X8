
using System;
using DragonPlus;
using DragonU3DSDK.Asset;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UIStoryDialogItem:MonoBehaviour
{
    private LocalizeTextMeshProUGUI _nameText;
    private LocalizeTextMeshProUGUI _dialogText;
    private Image _npcImage;
    private SkeletonGraphic _skeletonAnimation;
    private TableRoles _roleConfig;
    public void Init(TableStory storyConfig,TableRoles roleConfig)
    {

        _roleConfig = roleConfig;
        _nameText = transform.Find("HeadImage/Mask/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        _dialogText = transform.Find("Image/decBg/DecText").GetComponent<LocalizeTextMeshProUGUI>();
        //设置名字和对话
        _nameText.SetTerm(roleConfig.roleName);
        _dialogText.SetTerm(storyConfig.en);
            
        _npcImage= transform.Find("HeadImage/Mask/PortraitSpine/NpcImage").GetComponent<Image>();
        if (!string.IsNullOrEmpty(roleConfig.rolePrefabName))
        {
            _npcImage.gameObject.SetActive(false);
            var spinePrefab=ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/PortraitSpine/"+roleConfig.rolePrefabName);
            var spine =Instantiate(spinePrefab, _npcImage.transform.parent, false);
            _skeletonAnimation= spine.transform.Find("Mask/PortraitSpine").GetComponent<SkeletonGraphic>();
            _skeletonAnimation.AnimationState.SetAnimation(0, roleConfig.roleAniName+"_appear", false);
            _skeletonAnimation.AnimationState.AddAnimation(0, _roleConfig.roleAniName, true,1.66f);
            _skeletonAnimation.AnimationState.Update(0);
          
        }
        else
        {
            //设置头像
            _npcImage.useSpriteMesh = true;
            string npcImagePath = $"{roleConfig.rolePicName}";
            _npcImage.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.UIStoryAtlas, npcImagePath);
        }
        UpdateAllBg(roleConfig, gameObject);
    }

    private void UpdateAllBg(TableRoles role, GameObject item)
    {
        var roleColor = GlobalConfigManager.Instance.GetRoleColor(role.roleColor);
        if(roleColor == null)
            return;
        
        var image = item.transform.Find("HeadImage/Mask/headBg").GetComponent<Image>();
        image.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.UIStoryAtlas, roleColor.headBg);
        
        image = item.transform.Find("HeadImage/Mask/nameBg").GetComponent<Image>();
        image.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.UIStoryAtlas, roleColor.nameBg);
        
        image = item.transform.Find("Image/decBg").GetComponent<Image>();
        image.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.UIStoryAtlas, roleColor.contentBg);
        
        Material material = LocalizationManager.Instance.GetLocaleMaterial(roleColor.nameMaterial);
        if (material == null)
            return;
        
        var text = item.transform.Find("HeadImage/Mask/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        text.isAutoChangeMatrial = false;
        text.m_TmpText.fontMaterial = material;
        
        //item.transform.Find("Image/decBg/DecText").GetComponent<LocalizeTextMeshProUGUI>();
    }
}
