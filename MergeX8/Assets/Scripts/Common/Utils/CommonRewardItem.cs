using System;
using DragonPlus;
using Gameplay;
using Screw;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

public class CommonRewardItem : MonoBehaviour
{
    public ResData _resData;
    public Button _clickinfo;
    public Image _image;
    public TableMergeItem _mergeItem;
    public LocalizeTextMeshProUGUI _numText;
    private bool IsAwake = false;
    public void Awake()
    {
        if (IsAwake)
            return;
        IsAwake = true;
        _numText = transform.Find("Text")?.GetComponent<LocalizeTextMeshProUGUI>();
        if (!_numText)
            _numText = transform.Find("Num")?.GetComponent<LocalizeTextMeshProUGUI>();
        if (transform.Find("Icon"))
        {
            _image = transform.Find("Icon").GetComponent<Image>();   
        }
        else if(transform.Find("Item/Icon"))
        {
            _image = transform.Find("Item/Icon").GetComponent<Image>();   
        }
        else if(transform.Find("RewardIcon"))
        {
            _image = transform.Find("RewardIcon").GetComponent<Image>();   
        }
        _clickinfo = transform.Find("TipsBtn")?.GetComponent<Button>();
        if (!_clickinfo)
            _clickinfo = transform.Find("HelpButton")?.GetComponent<Button>();
        if (_clickinfo)
        {
            _clickinfo.onClick.AddListener(() =>
            {
                MergeInfoView.Instance.OpenMergeInfo(_mergeItem);
            });   
        }
    }

    public void Init(ResData resData, bool isShowNum = false,bool isShowClickTips = true)
    {
        _resData = resData;
        if (!IsAwake)
            Awake();
        if (TMatchModel.Instance.IsTMatchResId(_resData.id))
        {
            _image.sprite = TMatch.ItemModel.Instance.GetItemSprite(TMatchModel.Instance.ChangeToTMatchId(_resData.id), false);
            _clickinfo?.gameObject.SetActive(false);
        }
        else if (ScrewGameModel.Instance.IsScrewResId(_resData.id))
        {
            _image.sprite = Screw.UserData.UserData.GetResourceIcon(ScrewGameModel.Instance.ChangeToScrewId(_resData.id));
            _clickinfo?.gameObject.SetActive(false);
        }
        else if (!UserData.Instance.IsResource(_resData.id))
        {
            _mergeItem = GameConfigManager.Instance.GetItemConfig(_resData.id);
            _image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(_mergeItem.image);
            _clickinfo?.gameObject.SetActive(isShowClickTips);
        }
        else
        {
            _image.sprite = UserData.GetResourceIcon(_resData.id,UserData.ResourceSubType.Big);
            _clickinfo?.gameObject.SetActive(false);
        }
        _numText.SetText(_resData.count.ToString());
        _numText.gameObject.SetActive(_resData.count > 1 || isShowNum);
    }
}