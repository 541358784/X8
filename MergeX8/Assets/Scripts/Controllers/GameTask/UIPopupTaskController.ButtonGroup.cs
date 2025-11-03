using System.Collections.Generic;
using Decoration;
using Ditch.Model;
using DragonPlus;
using Manager;
using MiniGame;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupTaskController
{
    private Transform _buttonsRoot;
    
    private class ButtonData
    {
        public GameObject _normalObj;
        public GameObject _selectObj;
        public GameObject _redPointObj;
        
        public Transform _root;

        public Button _button;

        private Transform _logicRoot;
        
        public void Init(Transform root, Transform logicRoot)
        {
            _root = root;
            _logicRoot = logicRoot;
            
            _button = _root.transform.GetComponent<Button>();
            
            _normalObj = _root.Find("Normal").gameObject;
            _selectObj = _root.Find("Selected").gameObject;
            _redPointObj = _root.Find("RedPoint").gameObject;
            _redPointObj.gameObject.SetActive(false);
        }

        public void SetSelectStatus(bool isSelect)
        {
            _normalObj.gameObject.SetActive(!isSelect);
            _selectObj.gameObject.SetActive(isSelect);
            _logicRoot.gameObject.SetActive(isSelect);
        }

        public void RefreshRedPoint(bool isActive)
        {
            _redPointObj.gameObject.SetActive(isActive);
        }
    }

    private List<ButtonData> _buttonDatas = new List<ButtonData>();

    private string[] _logicPath = new string[]
    {
        "Root/Task",
        "Root/MiniGame",
        "Root/Task2"
    };
    private void Awake_Buttons()
    {
        _buttonsRoot = transform.Find("Root/LabelGroup");
        _buttonsRoot.gameObject.SetActive(true);

        for (int i = 1; i <= 3; i++)
        {
            ButtonData data = new ButtonData();
            data.Init(_buttonsRoot.transform.Find("Label" + i), transform.Find(_logicPath[i-1]));
            
            data._button.onClick.AddListener(() =>
            {
                OnClickButton(data._root);
            });
            _buttonDatas.Add(data);
        }
        
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MiniGame_Button, _buttonDatas[0]._button.transform as RectTransform, targetParam:GuideKey, topLayer:_buttonDatas[0]._button.transform);

        if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.DecoAndMerge)
        {
            _buttonDatas[1]._root.gameObject.SetActive(false);
        }
        
        InvokeRepeating("RedPointInvokeUpdate", 0, 1);
    }

    private void OnClickButton(Transform obj)
    {
        if (obj == _buttonDatas[0]._root)
        {
            if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
            {
                if (ExperenceModel.Instance.GetLevel() < UnlockManager.GetUnlockParam(UnlockManager.MergeUnlockType.MiniGame_Deo))
                {
                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                    {
                        DescString = string.Format(LocalizationManager.Instance.GetLocalizedString("&key.UI_unlock_tips"), UnlockManager.GetUnlockParam(UnlockManager.MergeUnlockType.MiniGame_Deo)),
                        OKCallback = () =>
                        {
                        },
                        HasCloseButton = false
                    });
                    return;
                }
            }
        }
        else if (obj == _buttonDatas[2]._root)
        {
            if (!DecoManager.Instance.IsOwnedNode(DecoNodeId))
            {
                CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                {
                    DescString = LocalizationManager.Instance.GetLocalizedString("&key.ui_wish_unlock"),
                    OKCallback = () =>
                    {
                    },
                    HasCloseButton = false
                });
                return;
            }
        }
        _buttonDatas.ForEach(a=>a.SetSelectStatus(a._root == obj));
        
        if (obj == _buttonDatas[0]._root)
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MiniGame_Button, GuideKey);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.GoDeco, "");
        }
        else
        {
            InitLocation();
        }
    }

    private void RedPointInvokeUpdate()
    {
        for (var i = 0; i < _buttonDatas.Count; i++)
        {
            bool isActive = false;
            if (i == 2)
            {
                isActive = IsShowWishingRedPoint();
            }
            else if (i == 0)
            {
                if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.DecoAndMerge)
                {
                    isActive = DecoManager.Instance.CanBuyOrGet_Deco();
                }
                else
                {
                    if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.MiniGame_Deo))
                        isActive = false;
                    else
                        isActive = DecoManager.Instance.CanBuyOrGet_Deco();
                }
            }
            else
                isActive = DecoManager.Instance.CanBuyOrGet_Mini();
            
            _buttonDatas[i].RefreshRedPoint(isActive);
        }
    }
}