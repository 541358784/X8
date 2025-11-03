using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStoryController
{
    public static string LEFT_MAINROLE_PREFAB_PATH = "Prefabs/UIStory/UIStoryItem1";
    public static string LEFT_CHIEF_PREFAB_PATH = "Prefabs/UIStory/UIStoryItem2_2";

    public static string RIGHT_MAINROLE_PREFAB_PATH = "Prefabs/UIStory/UIStoryItem1_1";
    public static string RIGHT_CHIEF_PREFAB_PATH = "Prefabs/UIStory/UIStoryItem2";


    public static string LEFT_PREFAB_PATH = "Prefabs/UIStory/UIStoryItem1";
    public static string RIGHT_PREFAB_PATH = "Prefabs/UIStory/UIStoryItem2";
    public static string Image_PREFAB_PATH = "Prefabs/UIStory/UIStoryItem3";
    public static string ImageNo_PREFAB_PATH = "Prefabs/UIStory/UIStoryItem4";
    

    private UnityEngine.UI.Button m_MaskButton;



    private Transform _dialog;
    private RectTransform _dialog_Rect;
    //private float viewportHieght=0;
    private GameObject _skipObj;

    private GameObject prefab_dialog_left;
    private GameObject prefab_dialog_right;

    private GameObject prefab_dialog_left_mainRole;
    private GameObject prefab_dialog_left_chief;
    private GameObject prefab_dialog_right_mainRole;
    private GameObject prefab_dialog_right_chief;

    private Button m_skip;
    private const float AutoPlayDuration = 3f;
    private float _autoPlayTick;

    private SkeletonGraphic _leftNpcGraphic;
    private SkeletonGraphic _rightNpcGraphic;

    private Sequence _currentSequence;
    private SkeletonAnimation skeletonAnimation;

    private float _dialogHeight = 100;

    private Image _dynamicBg = null;
    private Image _dynamicImage = null;
    
    private void privateAwakeDialog()
    {
        _dialog = transform.Find("Dialog/MiddleGroup/Scroll View/Viewport/Content");
        m_skip = transform.Find("Dialog/TopGroup/Play").GetComponent<Button>();
        m_MaskButton = transform.GetComponent<Button>();

        _dynamicBg = transform.Find("Dialog/dynamicBg").GetComponent<Image>();
        _dynamicImage = transform.Find("Dialog/dynamic").GetComponent<Image>();
        
        _dialog_Rect = _dialog.GetComponent<RectTransform>();

        m_MaskButton.gameObject.SetActive(true);
        m_MaskButton.onClick.RemoveAllListeners();
        
        m_MaskButton.onClick.AddListener(delegate
        {
            tryNext();
        });

        m_skip.onClick.RemoveAllListeners();
        m_skip.onClick.AddListener(() =>
        {
            skip();
            m_skip.gameObject.SetActive(false);
            m_MaskButton.onClick.RemoveAllListeners();
        });
        m_skip.gameObject.SetActive(false);

        //_npcDicLeft = new Dictionary<string, SkeletonGraphic>();
        //_npcDicRight = new Dictionary<string, SkeletonGraphic>();
    }

    private void setDialogData(TableStory storyConfig, bool fade)
    {
        var roleConfig = GlobalConfigManager.Instance.GetTableRole(storyConfig.role_id);
        if (roleConfig == null)
        {
            DebugUtil.LogError("未查询到roleConfig:" + storyConfig.role_id + " StoryID:" + storyConfig.id);
            exitStory(null);
            return;
        }

        var pos = storyConfig.position;
        
        GameObject dialog = null;
        
        void DialogImageInit()
        {
            var npcImage = dialog.transform.Find("Root/Image").GetComponent<Image>();
            string npcImagePath = $"{roleConfig.rolePicName}";
            npcImage.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.UIStoryAtlas, npcImagePath);
        }

        switch (pos)
        {
            case 1:
            {
                prefab_dialog_left = ResourcesManager.Instance.LoadResource<GameObject>(LEFT_PREFAB_PATH);
                dialog = GameObject.Instantiate<GameObject>(prefab_dialog_left, _dialog, false);
                CommonUtils.GetOrCreateComponent<UIStoryDialogItem>(dialog).Init(storyConfig,roleConfig);
                break;
            } 
            case 2:
            {
                
                prefab_dialog_right = ResourcesManager.Instance.LoadResource<GameObject>(RIGHT_PREFAB_PATH);
                dialog = GameObject.Instantiate<GameObject>(prefab_dialog_right, _dialog, false);
                CommonUtils.GetOrCreateComponent<UIStoryDialogItem>(dialog).Init(storyConfig,roleConfig);
                break;
            }
            case 3:
            {
                prefab_dialog_right = ResourcesManager.Instance.LoadResource<GameObject>(Image_PREFAB_PATH);
                dialog = GameObject.Instantiate<GameObject>(prefab_dialog_right, _dialog, false);
                
                DialogImageInit();
                break;
            }
            case 4:
            {
                prefab_dialog_right = ResourcesManager.Instance.LoadResource<GameObject>(ImageNo_PREFAB_PATH);
                dialog = GameObject.Instantiate<GameObject>(prefab_dialog_right, _dialog, false);
                
                DialogImageInit();
                break;
            }
            case 5:
            {
                _dynamicBg.gameObject.SetActive(false);
                _dynamicImage.gameObject.SetActive(true);
                _dynamicBg.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.UIStoryAtlas, roleConfig.rolePicName);
                _dynamicImage.sprite = _dynamicBg.sprite;
                _dynamicImage.color =  new Color(1,1,1,0);
                
                _currentStoryConfig = storyConfig;
                StartCoroutine(Delay_Dynamic());
                break;
            }
        }
        
        if(dialog == null)
            return;
        
        StartCoroutine(Delay_ScrollView(dialog));
        _currentStoryConfig = storyConfig;
    }

    private void startDialog(TableStory config)
    {
        StopAllCoroutines();
        stopAllDialogTween();

        if(_dynamicBg.sprite != null)
            _dynamicBg.gameObject.SetActive(true);
        _dynamicImage.gameObject.SetActive(false);
        
        setDialogData(config, false);
        AudioManager.Instance.PlaySound(40);
        StartCoroutine(Delay_tryNext());

    }


    private void resetDialog()
    {
        try
        {
            stopAllDialogTween();
            CommonUtils.ClearChildGameObject(_dialog);
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    private void stopAllDialogTween()
    {
        // foreach (var tween in _currentTweens)
        // {
        //     tween.Kill();
        // }

        // _currentTweens.Clear();

        // contentRect.DOKill();
        // contentRect.DOAnchorPosX(-curAnchoredX, moveTime).OnComplete(() =>
        // {
        //     moveEndCall?.Invoke();
        // });
    }
    IEnumerator Delay_tryNext()
    {
        yield return new WaitForSeconds(AutoPlayDuration);
        tryNext();
    }
    IEnumerator Delay_ScrollView(GameObject itemCell)
    {
        RectTransform cellRect = itemCell.transform as RectTransform;
        _dialogHeight  += Math.Abs(cellRect.sizeDelta.y)-16;
        
        float viewportHeight = _dialog.parent.GetComponent<RectTransform>().rect.height;
        float height = _dialogHeight - viewportHeight - Mathf.Abs(_dialog_Rect.anchoredPosition.y);
        if (height <= 0) 
            yield break;
        
        _dialog_Rect.DOAnchorPosY(_dialog_Rect.anchoredPosition.y+height, 0.3f);
    }
    IEnumerator Delay_Dynamic()
    {
        _dynamicImage.DOFade(1f, 1f);
        yield return new WaitForSeconds(AutoPlayDuration-1);
        _dynamicImage.DOFade(0f, 1f);
        _dynamicBg.gameObject.SetActive(true);
        _dynamicImage.gameObject.SetActive(true);
    }
}