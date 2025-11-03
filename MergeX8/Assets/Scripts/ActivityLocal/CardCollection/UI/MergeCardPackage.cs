using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeCardPackage : MonoBehaviour
{
    private Button _butCoinRush;
    private Image Icon;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    // private Transform _rewardGroup;

    private void Awake()
    {
        _butCoinRush = transform.GetComponent<Button>();
        _butCoinRush.onClick.AddListener(OnClick);
        Icon = transform.Find("Root/Icon").GetComponent<Image>();
        _redPoint = transform.Find("Root/RedPoint");
        _redPointText = transform.Find("Root/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventGetCardPackage>(OnGetCard);
        InvokeRepeating("RefreshView", 0, 1f);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventGetCardPackage>(OnGetCard);
    }

    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CardPackageEntrance);
        var unOpenCardPackages = CardCollectionModel.Instance.ThemeInUse.GetUpGradeTheme().GetUnOpenCardPackages();
        if (unOpenCardPackages.Count > 0)
        {
            var showPackage = unOpenCardPackages.Last();
            CardCollectionModel.Instance.TryOpenSingleCardPackage(showPackage.Id).WrapErrors();
            RefreshView();
        }
    }
    public void OnGetCard(EventGetCardPackage evt)
    {
        RefreshView();
    }

    public void RefreshView()
    {
        var unOpenCardPackages = CardCollectionModel.Instance.ThemeInUse.GetUpGradeTheme().GetUnOpenCardPackages();
        gameObject.SetActive(unOpenCardPackages.Count > 0);
        if (gameObject.activeInHierarchy)
        {
            var showPackage = unOpenCardPackages.Last();
            Icon.sprite = CardCollectionModel.Instance.TableCardPackage[showPackage.Id].GetCommonCardPackageSprite();
            _redPoint.gameObject.SetActive(true);
            _redPointText.gameObject.SetActive(true);
            _redPointText.SetText(unOpenCardPackages.Count.ToString());
        }
    }
}