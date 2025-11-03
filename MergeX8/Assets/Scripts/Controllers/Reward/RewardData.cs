using Deco.Item;
using Deco.World;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using Gameplay;
using Screw;
using TMatch;
using UnityEngine;
using UnityEngine.UI;


public class RewardData
{
    public GameObject gameObject;
    public Image image;
    public Image tipIcon;
    public LocalizeTextMeshProUGUI numText;
    public Animator animator;
    public int type;
    public int num;

    public void SetActive(bool isActive)
    {
        gameObject?.gameObject.SetActive(isActive);
        image?.gameObject.SetActive(isActive);
        numText?.gameObject.SetActive(isActive);
    }

    public void UpdateReward(int type, int num, MasterCardResource mcRes = null)
    {
        this.type = type;
        this.num = num;

        numText?.SetText(num.ToString());

        if (image == null)
            return;

        if (UserData.Instance.IsResource(type))
        {
            image.sprite = UserData.GetResourceIcon(type,UserData.ResourceSubType.Big);
            return;
        }

        if (MasterCardModel.Instance.IsMasterCardResource(type))
        {
            image.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.ShopAtlas, mcRes.Icon);
            ;

            numText.SetText(
                LocalizationManager.Instance.GetLocalizedStringWithFormats(mcRes.Description,
                    mcRes.RewardParam.ToString()));

            return;
        }

          
        DecoItem item = null;
        if (DecoWorld.ItemLib.ContainsKey(type))
            item = DecoWorld.ItemLib[type];
        if (item != null)
        {
            Sprite sprite = null;
            if (item._node.Config.costId == (int)UserData.ResourceId.Mermaid)
                sprite = ResourcesManager.Instance.GetSpriteVariant("MermaidAtlas", item.Config.buildingIcon);
            else if(item._node.Config.costId == (int)UserData.ResourceId.HappyGo)
                sprite = ResourcesManager.Instance.GetSpriteVariant("HappyGoAtlas", item.Config.buildingIcon);
            else if(item._node.Config.costId == 10001)
                sprite = CommonUtils.LoadDecoItemIconSprite(item._node._stage.Area.Id,item._data._config.buildingIcon);
            
            image.sprite = sprite;
            if(sprite != null)
                return;
        } 
        TableMergeItem mergeConfig = GameConfigManager.Instance.GetItemConfig(type);
        if (mergeConfig != null)
        {
            image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeConfig.image);
            return;
        }

        if (TMatchModel.Instance.IsTMatchResId(type))
        {
            image.sprite = TMatch.ItemModel.Instance.GetItemSprite(TMatchModel.Instance.ChangeToTMatchId(type), false);
            return;
        }
        if (ScrewGameModel.Instance.IsScrewResId(type))
        {
            image.sprite = Screw.UserData.UserData.GetResourceIcon(ScrewGameModel.Instance.ChangeToScrewId(type));
            return;
        }
    }

    public void UpdateReward(ResData resData)
    {
        if(resData == null)
            return;
        
        UpdateReward(resData.id, resData.count);
        tipIcon?.gameObject.SetActive(false);
    }

    public void PlayAnimation(string animName)
    {
        if (animator == null)
            return;

        animator.Play(animName, 0, 0f);
    }
}