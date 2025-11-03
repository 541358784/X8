using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.Ditch;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public partial class UIDigTrenchNewMainController
{
    public ProgressGroup ProgressGroupController;
    public void InitProgressGroup()
    {
        ProgressGroupController = transform.Find("Root/OperateGroup").gameObject.AddComponent<ProgressGroup>();
        ProgressGroupController.Init(this);
    }

    public class ProgressGroup : MonoBehaviour
    {
        public UIDigTrenchNewMainController Window;
        public List<ProgressPoint> Points = new List<ProgressPoint>();
        public int CurLevel;
        public Slider Slider;
        private TableDitchLevel LevelConfig => Window.Config;

        public void Init(UIDigTrenchNewMainController window)
        {
            Window = window;
            Slider = transform.Find("Slider").GetComponent<Slider>();
            var defaultNode = transform.Find("Operate_1");
            defaultNode.gameObject.SetActive(false);
            var emptyNode = Instantiate(defaultNode, defaultNode.parent).gameObject.AddComponent<ProgressPoint>();
            emptyNode.gameObject.SetActive(true);
            emptyNode.HideAllStates();
            if (LevelConfig.PointIcons != null)
            {
                foreach (var iconName in LevelConfig.PointIcons)
                {
                    var progressNode = Instantiate(defaultNode, defaultNode.parent).gameObject.AddComponent<ProgressPoint>();
                    progressNode.gameObject.SetActive(true);
                    progressNode.Init(iconName);
                    Points.Add(progressNode);
                }
            }
            CurLevel = 0;
            Slider.maxValue = Points.Count;
            Slider.value = CurLevel;
            UpdatePointsState();
        }

        public void UpdatePointsState()
        {
            for (var i = 0; i < Points.Count; i++)
            {
                if (i == CurLevel)
                {
                    Points[i].ShowState2();
                }
                else if (i < CurLevel)
                {
                    Points[i].ShowState3();
                }
                else
                {
                    Points[i].ShowState1();
                }
            }
        }
        public void AddLevel()
        {
            CurLevel++;
            Slider.DOKill();
            Slider.DOValue(CurLevel, 0.5f).OnComplete(UpdatePointsState);
        }
    }

    public class ProgressPoint : MonoBehaviour
    {
        public string IconPath = "";
        public Transform State1;
        public Image State1Icon;
        public LocalizeTextMeshProUGUI State1Text;
        public Transform State2;
        public Image State2Icon;
        public LocalizeTextMeshProUGUI State2Text;
        public Transform State3;

        private void Awake()
        {
            State1 = transform.Find("Status_1");
            State1Icon = transform.Find("Status_1/Icon").GetComponent<Image>();
            State1Text = transform.Find("Status_1/Title").GetComponent<LocalizeTextMeshProUGUI>();
            State2 = transform.Find("Status_2");
            State2Icon = transform.Find("Status_2/Icon").GetComponent<Image>();
            State2Text = transform.Find("Status_2/Title").GetComponent<LocalizeTextMeshProUGUI>();
            State3 = transform.Find("Status_3");
        }

        public void Init(string iconName)
        {
            IconPath = iconName;
            var sprite =ResourcesManager.Instance.GetSpriteVariant("DigTrenchNewAtlas",IconPath);
            State1Icon.sprite = sprite;
            State2Icon.sprite = sprite;
            State1Text.gameObject.SetActive(false);
            State2Text.gameObject.SetActive(false);
        }
        public void ShowState1()
        {
            State1.gameObject.SetActive(true);
            State2.gameObject.SetActive(false);
            State3.gameObject.SetActive(false);
        }
        public void ShowState2()
        {
            State1.gameObject.SetActive(false);
            State2.gameObject.SetActive(true);
            State3.gameObject.SetActive(false);
        }
        public void ShowState3()
        {
            State1.gameObject.SetActive(false);
            State2.gameObject.SetActive(false);
            State3.gameObject.SetActive(true);
        }
        public void HideAllStates()
        {
            State1.gameObject.SetActive(false);
            State2.gameObject.SetActive(false);
            State3.gameObject.SetActive(false);
        }
    }
}