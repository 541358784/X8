using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonU3DSDK.Asset;
using DG.Tweening;
using DragonPlus.Config.Farm;
using Farm.Model;
using Gameplay;

public class UIPopupFarmLevelTipsController : UIWindow
{
    private Transform _awardsRoot;
    private Slider _slider, _slider2;
    private LocalizeTextMeshProUGUI _level, _experenceProgross, _experenceProgross2;
    private Button _getBtn;
    private Button _notGetBtn;
    private GameObject _packageItem;
    private Transform DefaultResourceItem;
    private List<GameObject> tempGo = new List<GameObject>();
    private GameObject _vfxBoxReward;
    bool _isCanClick = true;
    private Animator _expAnimator;
    private Animator _animator;
    private UIShiny _uiShiny;

    public override void PrivateAwake()
    {
        _packageItem = transform.Find("Root/ContentGroup/UnitList/Item").gameObject;
        _packageItem.gameObject.SetActive(false);
        
        Transform content = this.transform.Find("Root/ContentGroup");
        _awardsRoot = content.Find("UnitList");
        DefaultResourceItem = _awardsRoot.Find("Item");
        DefaultResourceItem.gameObject.SetActive(false);
        _getBtn = content.GetComponentDefault<Button>("Button");
        _notGetBtn = content.GetComponentDefault<Button>("ButtonGray");
        _slider = content.GetComponentDefault<Slider>("Slider");

        _uiShiny = content.Find("Slider/Fill Area/Fill").GetComponent<UIShiny>();
        _uiShiny.enabled = false;

        _experenceProgross = _slider.GetComponentDefault<LocalizeTextMeshProUGUI>("ExperienceText");

        _slider2 = content.GetComponentDefault<Slider>("StarSlider");
        _level = _slider2.GetComponentDefault<LocalizeTextMeshProUGUI>("Text");
        _expAnimator = _slider2.gameObject.GetComponent<Animator>();
        _animator = gameObject.GetComponent<Animator>();
        BindClick("Root/ContentGroup/CloseButton", (go) =>
        {
            if (!_isCanClick)
                return;

            _isCanClick = false;
            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
                () => { CloseWindowWithinUIMgr(true); }));
        });
        _getBtn = this.transform.GetComponentDefault<Button>("Root/ContentGroup/Button");
        _getBtn.onClick.AddListener(OnLevelUpClick);
        InitAwards();
        _isCanClick = true;
    }

    private void InitAwards()
    {
        var config = FarmConfigManager.Instance.TableFarmLevelList.Find(a => a.Id == FarmModel.Instance.GetLevel());
        if(config == null)
            config = FarmConfigManager.Instance.TableFarmLevelList[FarmConfigManager.Instance.TableFarmLevelList.Count - 1];
        
        _experenceProgross.SetText(string.Format("{0}/{1}", FarmModel.Instance.storageFarm.Exp, config.LevelExp));

        float value = 1.0f * FarmModel.Instance.storageFarm.Exp / config.LevelExp;
        _level.SetText(FarmModel.Instance.GetLevel().ToString());
        _slider.value = value;
        _slider2.value = value;

        if (FarmModel.Instance.IsMaxLevel())
        {
            _experenceProgross.SetTerm("UI_max");
            _slider.value = 1.0f;
            _slider2.value = 1.0f;
        }
        
        tempGo.Clear();
        _getBtn.gameObject.SetActive(false);
        _notGetBtn.gameObject.SetActive(false);
        
        for (int i = 0; i < config.RewardIds.Count; i++)
        {
            GameObject go = Instantiate(_packageItem, _awardsRoot);
            go.gameObject.SetActive(true);

            int id = config.RewardIds[i];
            if (UserData.Instance.IsFarmProp(id))
            {
                var propConfig = FarmConfigManager.Instance.TableFarmPropList.Find(a => a.Id == id);
                if(propConfig != null)
                    go.transform.Find("Icon").GetComponent<Image>().sprite = FarmModel.Instance.GetFarmIcon(propConfig.Image);
            }
            else
            {
                var productConfig = FarmConfigManager.Instance.TableFarmProductList.Find(a => a.Id == id);
                if (productConfig != null)
                {
                    go.transform.Find("Icon").GetComponent<Image>().sprite = FarmModel.Instance.GetFarmIcon(productConfig.Image);
                }
                else
                {
                    go.transform.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(config.RewardIds[i]);
                }
            }
            go.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText("x" + config.RewardNums[i].ToString());
            tempGo.Add(go);

            StartCoroutine(PlayTween(go, i));
        }
    }

    public void OnLevelUpClick()
    {
        if (!_isCanClick)
            return;
       
        CloseWindowWithinUIMgr(true);
    }

    IEnumerator PlayTween(GameObject tweenObj, int index)
    {
        tweenObj.transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.2f);

        float step = 1.0f / 60f;

        float delay = index * 8 * step;
        yield return new WaitForSeconds(delay);

        tweenObj.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(tweenObj.transform.DOScale(1.2f, 7 * step));
        sequence.Append(tweenObj.transform.DOScale(0.9f, 11 * step));
        sequence.Append(tweenObj.transform.DOScale(1f, 10 * step));
        sequence.onComplete = () =>
        {
        };
        sequence.Play();
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }
}