using System;
using System.Collections.Generic;
using Decoration;
using Difference;
using DragonPlus;
using Farm.Model;
using Manager;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class MergeDailyTaskItem:MonoBehaviour
{
    public GameObject _guideArrow;
    private LocalizeTextMeshProUGUI dailyTaskText;
    private Transform dailyTaskRedPoint;
    public GuideArrowController GuideArrow2;
    private void Awake()
    {
        _guideArrow = transform.Find("Root/Arrow").gameObject;
        GuideArrow2 = transform.Find("Root/GuideArrow").gameObject.AddComponent<GuideArrowController>();
        GuideArrow2.Init();
        dailyTaskText = transform.Find("Root/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        dailyTaskRedPoint = transform.Find("Root/RedPoint");
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            GuideArrow2?.Hide();
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.UnlockDeco);
            UIManager.Instance.OpenUI(UINameConst.UIPopupTask);
        });
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.UnlockDeco, transform as RectTransform, topLayer:topLayer);
    }
    
    public void UpdateDailyTaskStatus()
    {
        //var nodes = DecoManager.Instance.CurrentWorld.GetUnlockAndNotOwnedNodes();
        if (GameModeManager.Instance.GetCurrenGameMode() == GameModeManager.CurrentGameMode.MiniGame)
        {
            gameObject.SetActive(DecoManager.Instance.CanBuyOrGet() && !FarmModel.Instance.IsFarmModel());
        }
        else if (DifferenceManager.Instance.IsDiffPlan_C())
        {
            gameObject.SetActive(MainOrderManager.Instance.StorageTaskGroup.CompleteNormalNum >= 5 && DecoManager.Instance.CanBuyOrGet() && !FarmModel.Instance.IsFarmModel());
        }
        else if (DifferenceManager.Instance.IsDiffPlan_B())
        {
            gameObject.SetActive(MainOrderManager.Instance.StorageTaskGroup.CompleteNormalNum >= 4 && DecoManager.Instance.CanBuyOrGet() && !FarmModel.Instance.IsFarmModel());
        }   
        else
        {
            gameObject.SetActive(DecoManager.Instance.CanBuyOrGet() && !FarmModel.Instance.IsFarmModel());
        }
        dailyTaskRedPoint.gameObject.SetActive(DecoManager.Instance.CanBuyOrGet());
        dailyTaskText.SetText("");
    }
}