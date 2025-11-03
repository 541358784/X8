using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DragonPlus;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace ActivityLocal.ClimbTower.Model
{
    public class WindowsGroup
    {
        public int _index;
        public Transform _root;
        public List<WindowCell> _windowCells = new List<WindowCell>();
        public Button _button;
            
        public int _rewardId;
        public int _rewardNum;
        public int _openState;

        private Transform _spineRoot;
        private SkeletonGraphic _skeletonGraphic;

        public WindowsGroup(int index, Transform parent, Transform root, int rewardId, int rewardNum, int openState, Action<int> action)
        {
            _root = root;
            _index = index;
            _rewardId = rewardId;
            _rewardNum = rewardNum;
            _openState = openState;
            
            _button = _root.GetComponent<Button>();

            _spineRoot = parent.Find("Root/Spine/"+(index+1));
            _skeletonGraphic = _spineRoot.Find("Spine").GetComponent<SkeletonGraphic>();
            
            _button.onClick.AddListener(() =>
            {
                action?.Invoke(_index);
            });
                
            _windowCells.Clear();
            for (int j = 1; j <= 5; j++)
            {
                _windowCells.Add(root.Find(j.ToString()).gameObject.GetOrCreateComponent<WindowCell>());
                _windowCells.Last().InitData(j-1, _rewardId, _rewardNum, _openState);
            }

            RefreshSpine((ClimbTowerModel.OpenState)_openState == ClimbTowerModel.OpenState.Open);
        }

        public void RefreshState(ClimbTowerModel.OpenState state, bool isSelect)
        {
            _windowCells.ForEach(a=>a.RefreshState(state));
            
            RefreshSpine(state, isSelect);
        }

        private void RefreshSpine(ClimbTowerModel.OpenState state, bool isSelect)
        {
            if((ClimbTowerModel.OpenState)_openState == state)
                return;
            
            _openState = (int)state;
            RefreshSpine(isSelect);
        }

        public async UniTask RefreshSpine(bool isSelect)
        {
            switch ((ClimbTowerModel.OpenState)_openState)
            {
                case ClimbTowerModel.OpenState.Close:
                {
                    _spineRoot.gameObject.SetActive(false);
                    break;
                }
                case ClimbTowerModel.OpenState.Open:
                {
                    if(_rewardId != -1)
                        return;
                    
                    await UniTask.WaitForSeconds(0.5f);
                    _spineRoot.gameObject.SetActive(true);

                    string appearAnimName="";
                    string loopAnimName="";
                    
                    if (isSelect)
                    {
                        appearAnimName = "inhale_appear";
                        loopAnimName = "inhale";
                    }
                    else
                    {
                        appearAnimName = "angry_appear";
                        loopAnimName = "angry";
                    }
                    
                    var trackEntry = _skeletonGraphic.AnimationState?.SetAnimation(0, appearAnimName, false);
                    _skeletonGraphic.Update(0);

                    await UniTask.WaitForSeconds(trackEntry.AnimationTime);
                    
                    if(loopAnimName == "angry")
                        AudioManager.Instance.PlaySound(248);
                    else
                        AudioManager.Instance.PlaySound(247);
                        
                    _skeletonGraphic.AnimationState?.SetAnimation(0, loopAnimName, true);
                    _skeletonGraphic.Update(0);
                    break;
                }
                case ClimbTowerModel.OpenState.None:
                {
                    break;
                }
            } 
        }

        public RewardData GetActiveRewardData()
        {
           var cell =  _windowCells.Find(a => a._rewardData.gameObject.activeInHierarchy);
           if (cell == null)
               return _windowCells.First()._rewardData;
           
           return cell._rewardData;
        }

        public void UpdateReward(int rewardId, int rewardNum)
        {
            _rewardId = rewardId;
            _rewardNum = rewardNum;
            
            for (var i = 0; i < _windowCells.Count; i++)
            {
                _windowCells[i].UpdateReward(rewardId, rewardNum);
            } 
        }
        
        public void RestWindowInfo(int rewardId, int rewardNum, int openState)
        {
            _rewardId = rewardId;
            _rewardNum = rewardNum;
            _openState = openState;
            
            for (var i = 0; i < _windowCells.Count; i++)
            {
                _windowCells[i].InitData(i, _rewardId, _rewardNum, _openState);
            }

            RefreshSpine(false);
        }

        public void SetGaryObjActive(bool active)
        {
            if (active)
            {
                if(_openState == (int)ClimbTowerModel.OpenState.Open && _rewardId == -1)
                     return;
            }
            
            _windowCells.ForEach(a=>a._garyItem.gameObject.SetActive(active));
        }
    }
    
    public class WindowCell : MonoBehaviour
    {
        private Transform _openItem;
        private Transform _closeItem;
        private Transform _failItem;
        public Transform _garyItem;
        
        public RewardData _rewardData = new RewardData();

        public int _index;
        private int _rewardId;
        private int _rewardNum;
        private int _openState;

        private Animator _animator;
        
        private void Awake()
        {
            _animator = transform.GetComponent<Animator>();
            
            _closeItem = transform.Find("Close");
            _openItem = transform.Find("Open");
            _failItem = transform.Find("Fail");
            _garyItem = transform.Find("Gray");
            _garyItem.gameObject.SetActive(false);
            
            _rewardData.gameObject = transform.Find("Open/Item").gameObject;
            _rewardData.image = transform.Find("Open/Item/Icon").GetComponent<Image>();
            _rewardData.numText = transform.Find("Open/Item/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void InitData(int index, int rewardId, int rewardNum, int openState)
        {
            _index = index;
            _rewardNum = rewardNum;
            _rewardId = rewardId;
            _openState = openState;

            _rewardData.UpdateReward(rewardId,rewardNum);
            RefreshState();
        }

        public void UpdateReward(int rewardId, int rewardNum)
        {
            _rewardNum = rewardNum;
            _rewardId = rewardId;

            _rewardData.UpdateReward(rewardId,rewardNum);
        }
        public void RefreshState(ClimbTowerModel.OpenState state)
        {
            if(_openState == (int)state)
                return;

            _openState = (int)state;
            RefreshState();
        }
        private void RefreshState()
        {
            _rewardData.gameObject.SetActive(true);
            
            _openItem.gameObject.SetActive(false);
            _failItem.gameObject.SetActive(false);
            _closeItem.gameObject.SetActive(true);
            _garyItem.gameObject.SetActive(false);
            
            switch (_openState)
            {
                case (int)ClimbTowerModel.OpenState.Close:
                {
                    _closeItem.gameObject.SetActive(true);
                    _animator.Play("Normal", -1, 0);
                    break;
                }
                case (int)ClimbTowerModel.OpenState.Open:
                {
                    if (_rewardId == -1)
                    {
                        _failItem.gameObject.SetActive(true);
                        _animator.Play("Open", -1, 0);
                    }
                    else
                    {
                        _openItem.gameObject.SetActive(true);
                        _animator.Play("Open", -1, 0);
                    }
                    break;
                }
            }
        }
    }
}