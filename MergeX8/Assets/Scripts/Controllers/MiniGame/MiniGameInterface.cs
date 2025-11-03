using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.MiniGame
{
    public interface MiniGameInterface
    {
        public UIPopupMiniGameController.MiniGameType _miniGameType { get; set; }
        public void Init(GameObject scrollView, GameObject selectObj, UIPopupMiniGameController.MiniGameType type, TableMiniGameSetting config, Action<UIPopupMiniGameController.MiniGameType> action);

        public void Select(bool isSelect);
    }

    public struct MiniGameInfo
    {
        public Sprite _sprite;
        public string _name;
        public bool _isFinish;
        public bool needRv;
        public bool _isUnLock;
        public int unlockNum;
        public int _id;
        public bool _isComingSoon;
    }
    
    public abstract class MiniGameViewBase : MiniGameInterface
    {
        private GameObject _scrollView;
        private GameObject _selectObj;
        private TableMiniGameSetting _config;
        private Action<UIPopupMiniGameController.MiniGameType> _action;
        private Button _selectButton;
        private GameObject _selectNormal;
        private GameObject _selectChose;
        protected List<MiniGameInfo> _infos = new List<MiniGameInfo>();

        public UIPopupMiniGameController.MiniGameType _miniGameType { get; set; }

        public void Init(GameObject scrollView, GameObject selectObj,  UIPopupMiniGameController.MiniGameType type, TableMiniGameSetting config, Action<UIPopupMiniGameController.MiniGameType> action)
        {
            _scrollView = scrollView;
            _selectObj = selectObj;
            _miniGameType = type;
            _action = action;
            _config = config;
            
            InitSelect();
            InitInfos();
            InitCell();
        }

        public void Select(bool isSelect)
        {
            _selectNormal.gameObject.SetActive(!isSelect);
            _selectChose.gameObject.SetActive(isSelect);
            _scrollView.gameObject.SetActive(isSelect);
        }
        
        private void InitSelect()
        {
            _selectButton = _selectObj.transform.GetComponent<Button>();
            _selectNormal = _selectObj.transform.Find("Normal").gameObject;
            _selectChose = _selectObj.transform.Find("Select").gameObject;
            
            var normal = _selectObj.transform.Find("Normal/Icon").GetComponent<Image>();
            var chose = _selectObj.transform.Find("Select/Icon").GetComponent<Image>();
            
            normal.sprite = ResourcesManager.Instance.GetSpriteVariant("HomeAtlas", _config.imageName);
            chose.sprite = normal.sprite;
            
            //_selectObj.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm(_config.name);
            _selectObj.transform.Find("Text").gameObject.SetActive(false);
            
            _selectButton.onClick.AddListener(() =>
            {
                _action?.Invoke(_miniGameType);
            });
        }

        private void InitCell()
        {
            MiniGameInfo comingSoonInfo = new MiniGameInfo();
            comingSoonInfo._isComingSoon = true;
            _infos.Add(comingSoonInfo);
            
            var item = _scrollView.transform.Find("Viewport/Content/Level");
            item.gameObject.SetActive(false);
            
            foreach (var info in _infos)
            {
                var cloneItem = GameObject.Instantiate(item, item.parent, false);
                cloneItem.gameObject.SetActive(true);
                var script = cloneItem.gameObject.AddComponent<MiniGameCell>();
                script.Init(info, OnClickPlay);
            }
        }
        protected abstract void InitInfos();
        protected abstract void OnClickPlay(MiniGameInfo info);
    }
}