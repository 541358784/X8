
using System;
using System.Collections.Generic;
using Deco.Item;
using Deco.World;
using DragonPlus;
using DragonPlus.Config.HappyGo;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class HappyGoRewardItem : MonoBehaviour
{
    private GameObject _lock;
    private GameObject _get;
    private Transform _rewardItem;
    private GameObject _finish;
    private LocalizeTextMeshProUGUI _lvText;
    private HGVDLevel _level;
    private HappyGoLevelStatus _status;
    private GameObject _redPoint;
    private LocalizeTextMeshProUGUI _tagText;
    private GameObject _getBtn;
    private string buildIcon;
    private void Awake()
    {
        _lock = transform.Find("Lock").gameObject;
        _get = transform.Find("Get").gameObject;
        _rewardItem = transform.Find("ItemGroup/1");
        _rewardItem.gameObject.SetActive(false);
        _finish = transform.Find("Finish").gameObject;
        _lvText = transform.Find("LvText").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint").gameObject;
        _tagText = transform.Find("Tag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _getBtn=transform.Find("GetButton").gameObject;
        Button _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClaimBtn);
    }

    public void Init(HGVDLevel level)
    {
        _level = level;
        UpdateStatus();
        _lvText.SetText(level.lv.ToString());
        for (int i = 0; i < level.reward.Length; i++)
        {
            var item= Instantiate(_rewardItem, _rewardItem.parent);
            item.gameObject.SetActive(true);
            InitItem(item,level.reward[i],level.amount[i]);
        }
        _tagText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(_level.label));
        _tagText.SetTerm(level.label);
    }
    public void UpdateStatus(){
        _status = HappyGoModel.Instance.GetLevelStatus(_level);
        _getBtn.SetActive(false);
        switch (_status)
        {
            case HappyGoLevelStatus.Get:
            {
                bool isCanGet = HappyGoModel.Instance.IsCanGet(_level.lv);
                _lock.SetActive(false);
                _finish.SetActive(false);
                _get.SetActive(true);
                _redPoint.SetActive(isCanGet);
                _getBtn.SetActive(isCanGet);

                if (isCanGet)
                {                    
                    List<Transform> topLayer = new List<Transform>();
                    topLayer.Add(transform);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.HappyGoLevelUpReward, transform as RectTransform, isReplace:false, topLayer: topLayer);
                }
            }
                break;
            case HappyGoLevelStatus.Finish :
                _lock.SetActive(false);
                _finish.SetActive(true);
                _get.SetActive(false);
                _redPoint.SetActive(false);
                break;
            case HappyGoLevelStatus.UnLock:
                _lock.SetActive(false);
                _finish.SetActive(false);
                _get.SetActive(false);
                _redPoint.SetActive(false);
                break;   
            case HappyGoLevelStatus.Lock:
                _lock.SetActive(true);
                _finish.SetActive(false);
                _get.SetActive(false);
                _redPoint.SetActive(false);
                break;
        }
    }
    private void OnClaimBtn()
    {
        if (_status == HappyGoLevelStatus.Get)
        {
            if (HappyGoModel.Instance.IsCanGet(_level.lv))
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.HappyGoLevelUpReward);
                HappyGoModel.Instance.ClaimRewards(_level);
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdRewardsClaim, _level.lv.ToString());
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(buildIcon))
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoyuilan, buildIcon);
            }
        }
    }
    private void InitItem(Transform item, int itemID, int ItemCount)
    {
        LocalizeTextMeshProUGUI text =  item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        text.SetText(ItemCount.ToString());
        
        if (UserData.Instance.IsResource(itemID))
        {
            item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID, UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
            if (itemConfig != null)
            {
                item.Find("Icon").GetComponent<Image>().sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else
            {
                DecoItem decoItem = null;
                if (DecoWorld.ItemLib.ContainsKey(itemID))
                    decoItem = DecoWorld.ItemLib[itemID];

                if (decoItem != null)
                {
                    item.Find("vfx").gameObject.SetActive(true);
                    Image image = item.Find("Icon").GetComponent<Image>();
                    image.transform.localPosition = new Vector3(0, image.transform.localPosition.y, image.transform.localPosition.z);
                    ((RectTransform)(image.transform)).sizeDelta = new Vector2(210, 152);
                    image.sprite = ResourcesManager.Instance.GetSpriteVariant("HappyGoAtlas", decoItem.Config.buildingIcon);
                    buildIcon = decoItem.Config.buildingIcon;
                    text.gameObject.SetActive(false);
                } 
            }
        }

        item.gameObject.SetActive(true);
    }
}
