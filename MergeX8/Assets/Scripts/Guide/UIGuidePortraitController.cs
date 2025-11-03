using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Gameplay;
using Gameplay.UI;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGuidePortraitController : UIWindowController, IPointerClickHandler
{
    private TutorialMask _mask;
    private RectTransform _npcTips;
    private LocalizeTextMeshProUGUI _txtNpcTips;
    public GameObject _bg;
    private bool _isNormal;
    private bool _isSmallTipGuide;

    private GameObject _smallTip;
    private LocalizeTextMeshProUGUI _txtSmallTip;
    private TableGuide _config;

    private GameObject _arrowPrefab;

    private Button _skipBtn;

    private GameObject _arrowLonelyHand;
    private GameObject _arrowAnimHand;
    private Image _maskImage = null;
    private SkeletonGraphic _skeletonAnimation;
    private List<Transform> _topLayers;
    private List<Canvas> _addCanvasList = new List<Canvas>();
    private List<GraphicRaycaster> _addGraphicRaycasters = new List<GraphicRaycaster>();
    
    Transform target = null;
    private List<Canvas> _canvas = new List<Canvas>();
    private bool isUpdate = true;

    private Vector3 _bgLocalPosition = Vector3.zero;
    public override void PrivateAwake()
    {
        isPlayDefaultAudio = false;
        _smallTip = GetItem("UI_Guide");
        _txtSmallTip = GetItem<LocalizeTextMeshProUGUI>("UI_Guide/Text");
        _maskImage = gameObject.GetComponent<Image>();
        _maskImage.material =
            ResourcesManager.Instance.LoadResource<Material>(PathManager.MaterialsCommon + "/UI_TutorialMask");

        _mask = gameObject.AddComponent<TutorialMask>();

        _npcTips = GetItem<RectTransform>("UIGuideNPC");
        _txtNpcTips = GetItem<LocalizeTextMeshProUGUI>("UIGuideNPC/BgImage/Text");

        _skipBtn = GetItem<Button>("SkipButton");
        _skipBtn.onClick.AddListener(OnSkipClicked);

        CommonUtils.NotchAdapte(_skipBtn.transform);

        _bg = GetItem("UIGuideNPC/BgImage");
        _bgLocalPosition = _bg.transform.localPosition;
        _arrowPrefab = GetItem("Arrow");

        _arrowLonelyHand = _arrowPrefab.FindChild("hand");
        _arrowLonelyHand.gameObject.SetActive(false);

        _arrowAnimHand = _arrowPrefab.FindChild("vfx_guide");
        _arrowAnimHand.gameObject.SetActive(true);

        _skeletonAnimation=transform.Find("UIGuideNPC/BgImage/NPCAvater/Heroine/Mask/PortraitSpine").GetComponent<SkeletonGraphic>();
        _canvas.Add(transform.Find("UI_Guide").GetComponent<Canvas>());
        _canvas.Add(transform.Find("UIGuideNPC").GetComponent<Canvas>());
        _canvas.Add(transform.Find("Arrow").GetComponent<Canvas>());
        _canvas.Add(transform.Find("SkipButton").GetComponent<Canvas>());
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        OnDisable();
        
        isUpdate = true;
        target = null;
        
        _canvas.ForEach(a=>a.sortingOrder = canvas.sortingOrder+2);
        
        StopAllCoroutines();
        if (objs.Length > 0)
        {
            _config = objs[0] as TableGuide;

            string targetParam = string.Empty;
            if (objs.Length >= 2)
                targetParam = (string)objs[1];
            showGuide(targetParam);
        }

        _bg.transform.localPosition = _bgLocalPosition;
        UIRoot.Instance.FitUIPos(_bg, 60f);

        StartCoroutine(ShowSkip());
        
        if (MergeMainController.Instance != null)
            MergeMainController.Instance.SetGuideMaskActive(_config.showMergeMask);
    }

    private void Update()
    {
        if(!isUpdate)
            return;

        if(!GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.CdProductSpeedUp) && !GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.BoxSpeedUp))
            return;
        
        if(target == null)
            return;
        
        if(target.gameObject.activeInHierarchy)
            return;

        isUpdate = false;
        OnSkipClicked();
    }

    private IEnumerator ShowSkip()
    {
        if (_config.skip == 0 || !_config.delaySkip)
        {
            _skipBtn.gameObject.SetActive(_config.skip == 1);
            yield break;
        }

        _skipBtn.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);

        _skipBtn.gameObject.SetActive(true);
    }

    private void showGuide(string targetParam)
    {
        _arrowLonelyHand.SetActive(false);
        _arrowAnimHand.SetActive(false);

        bool isClearMask = _config.clearMask;
        _maskImage.enabled = _config.rayCast;
        _mask.enabled = !isClearMask;
        _mask.SetConfig(_config, targetParam);
        
        var mainTargetOffset = Vector3.zero;

        setTarget(targetParam, ref mainTargetOffset);
        setNpc(mainTargetOffset);
        setTopLayer(targetParam);
    }

    private void setTopLayer(string targetParam)
    {
        _topLayers = GuideSubSystem.Instance.GetTopLayers((GuideTargetType) _config.targetType,targetParam);
        if(_topLayers == null || _topLayers.Count == 0)
            return;
        var topLayerIndex = 0;
        foreach (var trans in _topLayers)
        {
            var addCanvas = trans.gameObject.GetComponent<Canvas>();
            if (addCanvas == null)
            {
                addCanvas = trans.gameObject.AddComponent<Canvas>();
                addCanvas.additionalShaderChannels = 
                    AdditionalCanvasShaderChannels.TexCoord1 |
                    AdditionalCanvasShaderChannels.TexCoord2 |
                    AdditionalCanvasShaderChannels.TexCoord3 |
                    AdditionalCanvasShaderChannels.Normal |
                    AdditionalCanvasShaderChannels.Tangent;
                _addCanvasList.Add(addCanvas);
            }

            var addRaycaster = trans.gameObject.GetComponent<GraphicRaycaster>();
            if (addRaycaster == null)
            {
                addRaycaster = trans.gameObject.AddComponent<GraphicRaycaster>();
                _addGraphicRaycasters.Add(addRaycaster);
            }
            
            addCanvas.overrideSorting = true;
            topLayerIndex++;
            addCanvas.sortingOrder = canvas.sortingOrder + topLayerIndex;
        }
    }
    private void setTarget(string targetParam, ref Vector3 mainTargetOffset)
    {
        if (_config.targetType == null)
            return;

        _arrowPrefab.transform.DOKill();
        _arrowPrefab.SetActive(false);
        _arrowPrefab.transform.localScale = Vector3.one;

        float mul = _config.expandMultiple;
        if (mul == 0)
            mul = 1;
        var dist = _mask.GetRadius() * mul;

        List<RectTransform> moveTargetList = null;

        target = GuideSubSystem.Instance.GetTarget((GuideTargetType) _config.targetType, targetParam);
        moveTargetList = GuideSubSystem.Instance.GetMoveTarget((GuideTargetType) _config.targetType);

        var offset = Vector3.zero;
        if (target != null)
        {
            var targetLocalPosition = transform.InverseTransformPoint(target.position);

            if (targetLocalPosition.y < 0)
            {
                offset = targetLocalPosition + new Vector3(-targetLocalPosition.x, dist + _npcTips.rect.height, 0);
            }
            else
            {
                offset = targetLocalPosition + new Vector3(-targetLocalPosition.x, -(dist + _npcTips.rect.height), 0);
            }
        }
        else
        {
            offset.y += mul;
        }

        mainTargetOffset = offset;


        // hand设置
        var handEnable = _config.handsGuide == 1;
        if (handEnable && target != null)
        {
            Vector3 offsetPos = Vector3.zero;
            if (_config.arrowOffset != null && _config.arrowOffset.Length > 0)
            {
                if (_config.arrowOffset.Length >= 1)
                    offsetPos.x = _config.arrowOffset[0];

                if (_config.arrowOffset.Length >= 2)
                    offsetPos.y = _config.arrowOffset[1];

                if (_config.arrowOffset.Length >= 3)
                    offsetPos.z = _config.arrowOffset[2];
            }

            _arrowAnimHand.gameObject.SetActive(true);
            _arrowPrefab.SetActive(true);
            _arrowPrefab.transform.position = target.position;
            _arrowPrefab.transform.localPosition += offsetPos;
            _arrowPrefab.transform.localRotation = Quaternion.Euler(0f, 0f, _config.arrowAngle);

            if (moveTargetList != null)
            {
                if (moveTargetList.Count == 1)
                {
                    DoMoveLogic(moveTargetList[0], offsetPos);
                }
                else
                {
                    StartCoroutine(DoSelectLogic(moveTargetList, offsetPos));
                }
            }
            else
            {
                StartCoroutine(DoSelectLogic(target, offsetPos));
            }

            if (_config.isWorldPos)
            {
                Vector3 spos = Vector3.zero; //M3GameManager.Instance.sceneCameras.WorldToScreenPoint(target.position);
                Vector2 tempV2 = new Vector2(spos.x, spos.y);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_arrowPrefab.transform.parent as RectTransform,
                    tempV2, UIRoot.Instance.mUICamera, out var srcWorldPos);
                _arrowPrefab.GetComponent<RectTransform>().anchoredPosition = srcWorldPos;
                _arrowPrefab.transform.localPosition += offsetPos;
            }
        }
    }

    private void DoMoveLogic(RectTransform moveTarget, Vector3 offsetPos)
    {
        _arrowLonelyHand.gameObject.SetActive(false);
        _arrowAnimHand.gameObject.SetActive(false);

        if (moveTarget == null)
            return;

        _arrowLonelyHand.gameObject.SetActive(true);
        _arrowAnimHand.gameObject.SetActive(false);
        Vector3 positon = moveTarget.localPosition + offsetPos;
        if (moveTarget.transform.parent != null)
            positon = moveTarget.transform.parent.TransformPoint(positon);
        else
            positon = moveTarget.transform.TransformPoint(positon);
        _arrowPrefab.transform.DOMove(positon, 1.5f).SetLoops(-1);
    }

    IEnumerator DoSelectLogic(List<RectTransform> moveTargets, Vector3 offsetPos)
    {
        _arrowLonelyHand.gameObject.SetActive(false);
        _arrowAnimHand.gameObject.SetActive(true);
        int index = 0;
        while (true)
        {
            _arrowAnimHand.gameObject.SetActive(false);
            _arrowAnimHand.gameObject.SetActive(true);
            RectTransform moveTarget = moveTargets[index];

            Vector3 positon = moveTarget.localPosition + offsetPos;
            if (moveTarget.transform.parent != null)
                positon = moveTarget.transform.parent.TransformPoint(positon);
            else
                positon = moveTarget.transform.TransformPoint(positon);

            _arrowPrefab.transform.position = positon;
            index++;

            if (index >= moveTargets.Count)
                index = 0;

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator DoSelectLogic(Transform target, Vector3 offsetPos)
    {
        _arrowAnimHand.gameObject.SetActive(true);
        _arrowPrefab.SetActive(true);
        _arrowPrefab.transform.localRotation = Quaternion.Euler(0f, 0f, _config.arrowAngle);

        while (true)
        {
            _arrowPrefab.transform.position = target.position;
            _arrowPrefab.transform.localPosition += offsetPos;

            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(0.1f);
        }
    }

    private void setNpc(Vector3 offset)
    {
        _npcTips.gameObject.SetActive(_config.tipType == 0);
        _smallTip.gameObject.SetActive(_config.tipType == 1);
        var roleConfig=GlobalConfigManager.Instance.GetTableRole(_config.npcHeadId);
        string aniName = roleConfig == null || string.IsNullOrEmpty(roleConfig.roleAniName) ? "normal" : roleConfig.roleAniName;
        _skeletonAnimation.AnimationState.SetAnimation(0, aniName+"_appear", false);
        _skeletonAnimation.AnimationState.AddAnimation(0, aniName, true,1.66f);
        _skeletonAnimation.AnimationState.Update(0);
        
        _npcTips.localPosition = offset;
        _smallTip.transform.localPosition = offset;

        if (_config != null && !string.IsNullOrEmpty(_config.textGuide))
        {
            string textContent = LocalizationManager.Instance.GetLocalizedString(_config.textGuide);
            _txtNpcTips.SetTerm(textContent);
            _txtSmallTip.SetTerm(textContent);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_config.blockAll)
            return;

        GuideSubSystem.Instance.FinishCurrent();
    }

    private void OnSkipClicked()
    {
        StopAllCoroutines();
     
        GuideSubSystem.Instance.FinishCurrent(true);
        MergeGuideLogic.Instance.CheckMergeGuide();
    }

    private void OnEnable()
    {
        if (_config == null)
            return;

        if (MergeMainController.Instance != null)
            MergeMainController.Instance.SetGuideMaskActive(_config.showMergeMask);
    }

    private void OnDisable()
    {
        foreach (var addGraphicRaycaster in _addGraphicRaycasters)
        {
            DestroyImmediate(addGraphicRaycaster);
        }
        
        foreach (var addCanvas in _addCanvasList)
        {
            DestroyImmediate(addCanvas);
        }
        
        _addCanvasList.Clear();
        _addGraphicRaycasters.Clear();
    }
}