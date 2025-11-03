using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.UI;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardTipController : UIWindow
{
    public static List<RectTransform> ListAllTips = new List<RectTransform>();

    private Transform _rewardItem;
    private RectTransform _right;
    private RectTransform _bottom;
    private RectTransform _left;
    private RectTransform _top;
    private RectTransform _root;

    private RectTransform _bg;

    private Dictionary<CommonRewardManager.TipDirection, RectTransform>
        DirectionTranslateDictionary = new Dictionary<CommonRewardManager.TipDirection, RectTransform>();

    private Dictionary<CommonRewardManager.TipDirection, Vector3>
        DirectionInitLocalPositionDictionary = new Dictionary<CommonRewardManager.TipDirection, Vector3>();

    private CommonRewardManager.TipDirection _direction;
    private Vector3 _popupPosition;
    private List<ResData> _resDatas;
    private GridLayoutGroup _layout;
    private List<CommonRewardItem> _commonRewardItems;
    private IEnumerator _enumerator;

    public override void PrivateAwake()
    {
        _root = transform.Find("Root").GetComponent<RectTransform>();
        _layout = _root.gameObject.GetComponent<GridLayoutGroup>();
        _rewardItem = _root.Find("Item");
        _rewardItem.gameObject.SetActive(false);
        _bg = _root.Find("Image").GetComponent<RectTransform>();
        _right = _root.Find("Image/Right").GetComponent<RectTransform>();
        _bottom = _root.Find("Image/Bottom").GetComponent<RectTransform>();
        _left = _root.Find("Image/Left").GetComponent<RectTransform>();
        _top = _root.Find("Image/Top").GetComponent<RectTransform>();
        DirectionTranslateDictionary.Add(CommonRewardManager.TipDirection.Right, _left);
        DirectionInitLocalPositionDictionary.Add(CommonRewardManager.TipDirection.Right, _left.anchoredPosition);
        DirectionTranslateDictionary.Add(CommonRewardManager.TipDirection.Left, _right);
        DirectionInitLocalPositionDictionary.Add(CommonRewardManager.TipDirection.Left, _right.anchoredPosition);
        DirectionTranslateDictionary.Add(CommonRewardManager.TipDirection.Top, _bottom);
        DirectionInitLocalPositionDictionary.Add(CommonRewardManager.TipDirection.Top, _bottom.anchoredPosition);
        DirectionTranslateDictionary.Add(CommonRewardManager.TipDirection.Bottom, _top);
        DirectionInitLocalPositionDictionary.Add(CommonRewardManager.TipDirection.Bottom, _top.anchoredPosition);
        _commonRewardItems = new List<CommonRewardItem>();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (_enumerator != null)
            StopCoroutine(_enumerator);

        _resDatas = (List<ResData>)objs[0];
        _popupPosition = (Vector3)objs[1];
        _direction = (CommonRewardManager.TipDirection)objs[2];
        foreach (var pair in DirectionTranslateDictionary)
        {
            pair.Value.gameObject.SetActive(pair.Key == _direction);
        }

        _layout.constraintCount = Math.Min(3, _resDatas.Count);

        _commonRewardItems.ForEach(delegate(CommonRewardItem item) { item.gameObject.SetActive(false); });
        for (int i = 0; i < _resDatas.Count; i++)
        {
            if (_commonRewardItems.Count > i)
            {
                _commonRewardItems[i].gameObject.SetActive(true);
                _commonRewardItems[i].Init(_resDatas[i], true);
            }
            else
            {
                var item = Instantiate(_rewardItem, _rewardItem.parent);
                item.gameObject.SetActive(true);
                CommonRewardItem rewardItem = item.gameObject.AddComponent<CommonRewardItem>();
                rewardItem.Init(_resDatas[i], true);
                _commonRewardItems.Add(rewardItem);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
        RectTransform popupRectTransform = _root;
        switch (_direction)
        {
            case CommonRewardManager.TipDirection.Left:
                popupRectTransform.pivot = new Vector2(1, 0.5f);
                break;
            case CommonRewardManager.TipDirection.Right:
                popupRectTransform.pivot = new Vector2(0, 0.5f);
                break;
            case CommonRewardManager.TipDirection.Top:
                popupRectTransform.pivot = new Vector2(0.5f, 0);
                break;
            case CommonRewardManager.TipDirection.Bottom:
                popupRectTransform.pivot = new Vector2(0.5f, 1);
                break;
        }

        (transform as RectTransform).anchoredPosition =
            CommonUtils.WorldToCanvasPos(UIRoot.Instance.mRootCanvas.transform as RectTransform, _popupPosition);
        SetWindowInScreen();

        _enumerator = AutoClosePopup();
        StartCoroutine(_enumerator);
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.REWARD_POPUP);

    }

    private IEnumerator AutoClosePopup()
    {
        yield return new WaitForSeconds(3f); // 等待3秒钟

        Debug.Log("=================RewardTipController  自动关闭");
        AnimCloseWindow();
    }

    void Update()
    {
        // 检测点击任意位置关闭
        if (Input.GetMouseButtonDown(0))
        {
            //点击了其他Tips
            if (_isTipsBtnClick())
                return;
            //范围内
            if (UIRoot.Instance.IsPointInArea(Input.mousePosition, _bg))
                return;
            Debug.Log("=================RewardTipController  点击关闭");
            AnimCloseWindow();
        }
    }

    public void SetStaticScale(float staticScale)
    {
        transform.localScale = Vector3.one;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        var lossyScale = _root.lossyScale;
        var defaultScale = 1 / UIRoot.Instance.mRootCanvas.referencePixelsPerUnit;
        _root.localScale = new Vector3(defaultScale / lossyScale.x * staticScale,
            defaultScale / lossyScale.y * staticScale, 1);
        SetWindowInScreen();
    }

    public void SetLocalScale(float localScale)
    {
        _root.localScale = new Vector3(localScale, localScale, 1);
    }

    public void ResetRootPosition()
    {
        _root.anchoredPosition = Vector3.zero;
        foreach (CommonRewardManager.TipDirection direction in Enum.GetValues(typeof(CommonRewardManager.TipDirection)))
        {
            DirectionTranslateDictionary[direction].anchoredPosition = DirectionInitLocalPositionDictionary[direction];
        }
    }

    public void SetWindowInScreen()
    {
        ResetRootPosition();
        RectTransform popupRectTransform = _root;
        var moveDistance = XUtility.SetRectTransformInScreen(popupRectTransform);
        switch (_direction)
        {
            case CommonRewardManager.TipDirection.Top:
            case CommonRewardManager.TipDirection.Bottom:
                DirectionTranslateDictionary[_direction].position -= new Vector3(moveDistance.x, 0, 0);
                break;
            case CommonRewardManager.TipDirection.Right:
            case CommonRewardManager.TipDirection.Left:
                DirectionTranslateDictionary[_direction].position -= new Vector3(0, moveDistance.y, 0);
                break;
        }
    }

    public static void RegisterTips(RectTransform rect)
    {
        ListAllTips.Add(rect);
    }

    public static void UnRegisterTips(RectTransform rect)
    {
        ListAllTips.Remove(rect);
    }

    private bool _isTipsBtnClick()
    {
        for (int i = 0; i < ListAllTips.Count; i++)
        {
            if (UIRoot.Instance.IsPointInArea(Input.mousePosition, ListAllTips[i]))
                return true;
        }

        return false;
    }
}