using System;
using System.Collections;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine.UI;
using UnityEngine;


public class MergePigBoxController: MonoBehaviour
{

    private Image _pigBoxIcon;
    private LocalizeTextMeshProUGUI _pigAddNumText;
    private Animator _pigTexAnim;
    private Animator _pigAnim;
    private GameObject _pigEffect;
    private GameObject _coinFull;
    private int _curPigIndex = -1;
    private IEnumerator _pigIEnumerator;
    private Coroutine _pigDisappear;
    private Transform _full;
    private void Awake()
    {
        _pigBoxIcon = transform.Find("PigIcon").GetComponent<Image>();
        _pigAddNumText = transform.Find("TextGroup/Label").gameObject
            .GetComponent<LocalizeTextMeshProUGUI>();
        _pigTexAnim = transform.Find("TextGroup").gameObject.GetComponent<Animator>();
        _pigEffect = transform.Find("Vfx_open").gameObject;
        _pigEffect.gameObject.SetActive(false);      
        _coinFull = transform.Find("vfx_coin_full").gameObject;
        _coinFull.gameObject.SetActive(false);
        _pigAnim = gameObject.GetComponent<Animator>();
        _full = transform.Find("Full");

        //CommonUtils.NotchAdapte(transform);
    }

    private void OnEnable()
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.PIGBANK_VALUE_REFRESH, PigBankValueRefresh);
        EventDispatcher.Instance.AddEventListener(EventEnum.PIGBANK_UI_REFRESH, PigBankValueRefresh);
        EventDispatcher.Instance.AddEventListener(EventEnum.PIGBANK_PURCHASE_REFRESH, PigBankPurchaseRefresh);
        EventDispatcher.Instance.AddEventListener(EventEnum.PIGBANK_SHOW_BUTTON, PigBankShowButton);
        EventDispatcher.Instance.AddEventListener(EventEnum.PIGBANK_INITIMAGE, PigBankInitImage);
        UpdatePigBankImage();
    }

    private void UpdatePigBankImage()
    {
        if (!PigBankModel.Instance.IsOpened())
            return;

        if (_pigBoxIcon == null)
            return;

        PigBank pigBankTable = PigBankModel.Instance.GetAdPigBankTable();
        if (pigBankTable == null)
            return;
        _full.gameObject.SetActive(PigBankModel.Instance.IsFull());
        string imageName = PigBankModel.Instance._mergeBoxImageName[0];
        int index = PigBankModel.Instance.GetCurIndex();

        if (_curPigIndex > 0 && _curPigIndex != index)
        {
            _pigEffect.gameObject.SetActive(true);
            if (_pigIEnumerator != null)
                StopCoroutine(_pigIEnumerator);

            _pigIEnumerator = CommonUtils.DelayWork(2f, () =>
            {
                _pigEffect.gameObject.SetActive(false);
                _pigIEnumerator = null;
            });
            StartCoroutine(_pigIEnumerator);
        }

        _curPigIndex = index;
        if (index < PigBankModel.Instance._mergeBoxImageName.Length)
            imageName = PigBankModel.Instance._mergeBoxImageName[index];

        if (_pigBoxIcon.sprite == null || _pigBoxIcon.sprite.name != imageName)
        {
            _pigBoxIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.MergeAtlas, imageName);
        }
    }
    
    private void PigBankValueRefresh(BaseEvent e)
    {
        if(!gameObject.activeSelf)
            return;
        
        if (_pigDisappear != null)
        {
            StopCoroutine(_pigDisappear);
            _pigDisappear = null;
        }
        
        PlayPigBankAnim();
    }
    private void PigBankPurchaseRefresh(BaseEvent e)
    {    
        if(!gameObject.activeSelf)
            return;
        
        UpdatePigBankImage();
    }

    private void PigBankShowButton(BaseEvent e)
    {
        if(!gameObject.activeSelf)
            return;
        PigBankModel.Instance.UpdateActivityState();
        if (_pigDisappear != null)
        {
            StopCoroutine(_pigDisappear);
            _pigDisappear = null;
        }
        UpdatePigBankImage();
        _pigAnim.Play("appear", 0);
    }

    private void PigBankInitImage(BaseEvent e)
    {
        if(_pigBoxIcon != null && _pigBoxIcon.sprite == null)
            UpdatePigBankImage();
    }

    
    private void PlayPigBankAnim()
    {
        if (PigBankModel.Instance.IsFull())
        {
            _pigAddNumText.SetTerm("UI_store_energy_full_text");
            _coinFull.SetActive(false); 
            _coinFull.SetActive(true);
        }
        else
        {
            _pigAddNumText.SetText("+" + PigBankModel.Instance.GetCollectValue().ToString());
        }

        _pigAnim.Play("shake", 0);

        _pigDisappear = StartCoroutine(CommonUtils.PlayAnimation(_pigTexAnim, "appear", "", () =>
        {
            _pigAnim.Play("disappear", 0);
            _pigDisappear = null;
        }));
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.PIGBANK_VALUE_REFRESH, PigBankValueRefresh);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.PIGBANK_UI_REFRESH, PigBankValueRefresh);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.PIGBANK_PURCHASE_REFRESH, PigBankPurchaseRefresh);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.PIGBANK_SHOW_BUTTON, PigBankShowButton);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.PIGBANK_INITIMAGE, PigBankInitImage);
    }
}
