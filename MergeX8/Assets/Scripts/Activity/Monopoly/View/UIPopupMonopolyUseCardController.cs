using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMonopolyUseCardController:UIWindowController
{
    private Dictionary<int, SelectNode> SelectNodeDic = new Dictionary<int, SelectNode>();
    private SelectNode CurSelect;
    private Button StartBtn;
    private Button CloseBtn;
    private StorageMonopoly Storage;
    public override void PrivateAwake()
    {
        for (var i=1;i<=6;i++)
        {
            var selectNode = transform.Find("Root/" + i).gameObject.AddComponent<SelectNode>();
            var stepCount = i;
            selectNode.SetStepCount(stepCount,this);
            SelectNodeDic.Add(stepCount,selectNode);
        }

        StartBtn = transform.Find("Root/Button").GetComponent<Button>();
        StartBtn.onClick.AddListener(OnClickStartBtn);
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageMonopoly;
        UpdateBtnState();
        
        // if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyUseWildCard))
        // {
        //     List<Transform> topLayer = new List<Transform>();
        //     foreach (var nodePair in SelectNodeDic)
        //     {
        //         topLayer.Add(nodePair.Value.transform);
        //     }
        //     topLayer.Add(StartBtn.transform);
        //     GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MonopolyUseWildCard, StartBtn.transform as RectTransform, topLayer: topLayer);
        //     GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyUseWildCard,null);
        // }
    }

    public void OnClickStartBtn()
    {
        if (CurSelect == null)
            return;
        if (Storage.WildCardCount == 0)
            return;
        var stepCount = CurSelect.StepCount;
        AnimCloseWindow(() =>
        {
            if (Storage.ReduceWildCard())
            {
                var biStr = "UseWildCard";
                Storage.AddStep(stepCount,biStr,out biStr);
            }
        });
    }
    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    public void Select(int stepCount)
    {
        if (CurSelect)
        {
            CurSelect.SetSelect(false);
        }

        if (CurSelect != SelectNodeDic[stepCount])
        {
            CurSelect = SelectNodeDic[stepCount];
            CurSelect.SetSelect(true);   
        }
        else
        {
            CurSelect = null;
        }
        UpdateBtnState();
    }

    public void UpdateBtnState()
    {
        var selectFlag = CurSelect!=null && Storage.WildCardCount > 0;
        StartBtn.transform.Find("BG").gameObject.SetActive(selectFlag);
        StartBtn.transform.Find("Text").gameObject.SetActive(selectFlag);
        StartBtn.transform.Find("GreyBG").gameObject.SetActive(!selectFlag);
        StartBtn.transform.Find("GreyText").gameObject.SetActive(!selectFlag);
        StartBtn.interactable = selectFlag;
    }
    public class SelectNode:MonoBehaviour
    {
        public Transform SelectState;
        public Button SelectBtn;
        public int StepCount;
        // public LocalizeTextMeshProUGUI StepCountText;
        public UIPopupMonopolyUseCardController Controller;

        private void Awake()
        {
            SelectState = transform.Find("Selected");
            SelectBtn = transform.GetComponent<Button>();
            SelectBtn.onClick.AddListener(() =>
            {
                // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MonopolyUseWildCard);
                Controller.Select(StepCount);
            });
            if (SelectBtn.transform.TryGetComponent<ShieldButtonOnClick>(out var shield))
            {
                shield.isUse = false;
            }
            // StepCountText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void SetStepCount(int stepCount,UIPopupMonopolyUseCardController controller)
        {
            Controller = controller;
            StepCount = stepCount;
            // StepCountText.SetText(StepCount.ToString());
        }

        public void SetSelect(bool state)
        {
            SelectState.gameObject.SetActive(state);
        }
    }
    public static UIPopupMonopolyUseCardController Open(StorageMonopoly storageMonopoly)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupMonopolyUseCard, storageMonopoly) as
            UIPopupMonopolyUseCardController;
    }
}