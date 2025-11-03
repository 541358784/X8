using System;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using Farm.Order;
using GamePool;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public partial class OrderCell : MonoBehaviour, IInitContent
    {
        private CellReward _cellReward;
        public CellNeed _cellNeed;
        private Button _finishButton;
        
        public StorageFarmOrderItem _storage;

        private GameObject _personRoot;
        private GameObject _portraitSpineObj;
        private SkeletonGraphic _skeletonGraphic;
        private int _headIndex;

        public bool _isClickFinish = false;
        
        private void Awake()
        {
            _finishButton = transform.Find("FinishBtn").GetComponent<Button>();
            _finishButton.onClick.AddListener(OnClickFinish);

            _personRoot = transform.Find("Person/PortraitSpine").gameObject;
            
            InitDebug();
            
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_DEBUG_ORDER_OPEN, Event_DebugOrderOpen);

            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Farm_FinishTask))
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(_finishButton.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Farm_FinishTask, _finishButton.transform as RectTransform, topLayer:topLayer);
            }

            Awake_TimeOrder();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_DEBUG_ORDER_OPEN, Event_DebugOrderOpen);
            Recycle();
        }

        public void InitContent(object content)
        {
            _storage = (StorageFarmOrderItem)content;
            
            _cellReward = new CellReward(transform.Find("Reward"));
            _cellReward.InitContent(content);
            _cellReward.UpdateData();
            
            _cellNeed = new CellNeed(transform.Find("Need"), this);
            _cellNeed.InitContent(content);

            _isClickFinish = false;

            InitContentNotify();

            CancelInvoke("InvokeUpdate");   
            if (_storage.Slot == (int)OrderSlot.Activity_TimeOrder)
                InvokeRepeating("InvokeUpdate", 0, 1);   
        }

        public void UpdateData(params object[] param)
        {
            if (param != null && param.Length > 0)
            {
                _storage = (StorageFarmOrderItem)param[0];
                _isClickFinish = false;
            }
            
            InitContentNotify();
            
            _cellNeed.InitContent(_storage);
            _cellNeed.UpdateData(param);

            _cellReward.InitContent(_storage);
            _cellReward.UpdateData();
            
            bool isEnough = _cellNeed.IsEnough();
            _finishButton.gameObject.SetActive(isEnough);

            if (_headIndex != _storage.HeadIndex)
            {
                Recycle();
            }
            InitHeadSpine();
            
            PlaySkeletonAnimation(OrderConfigManager.Instance.GetSpineName(_headIndex, isEnough ? 2 : 1));
            UpdateDebugInfo();
        }

        public GameObject GetRewardIcon(int id)
        {
            return _cellReward.GetRewardIcon(id);
        }
        public bool IsFinish()
        {
            return _cellNeed.IsEnough();
        }
        
        private void OnClickFinish()
        {
            if(!IsFinish())
                return;

            _isClickFinish = true;
            _finishButton.gameObject.SetActive(false);
            FarmOrderManager.Instance.FinishOrder(this);

            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Farm_FinishTask, "");
        }
        
        private void InitHeadSpine()
        {
            if (_personRoot == null)
                return;

            if (_portraitSpineObj != null)
                return;

            if (_storage == null)
                return;
        
            _headIndex = _storage.HeadIndex;
            if (_headIndex <= 0 || _headIndex > OrderConfigManager.Instance._orderHeadSpines.Count)
            {
                _headIndex = _storage.HeadIndex = FarmOrderManager.Instance.RandomHeadIndex();
            }

            _portraitSpineObj = GamePool.ObjectPoolManager.Instance.Spawn(string.Format(ObjectPoolName.PortraitSpine, OrderConfigManager.Instance.GetSpineName(_headIndex)));
            _portraitSpineObj.transform.SetParent(_personRoot.transform);
            _portraitSpineObj.transform.localScale = Vector3.one;
            _portraitSpineObj.transform.localRotation = Quaternion.identity;
            ((RectTransform)_portraitSpineObj.transform).anchoredPosition = Vector3.zero;

            _skeletonGraphic = _portraitSpineObj.transform.GetComponentInChildren<SkeletonGraphic>();
            PlaySkeletonAnimation(OrderConfigManager.Instance.GetSpineName(_headIndex, 1));
        }

        public void Recycle()
        {
            if(_portraitSpineObj == null)
                return;
            
            GamePool.ObjectPoolManager.Instance.DeSpawn(string.Format(ObjectPoolName.PortraitSpine, OrderConfigManager.Instance.GetSpineName(_headIndex)), _portraitSpineObj);
            _portraitSpineObj = null;
            _skeletonGraphic = null;
            _headIndex = 0;
            _isClickFinish = false;
        }
        
        private void PlaySkeletonAnimation(string animName)
        {
            if (_skeletonGraphic == null)
                return;

            TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
            if (trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
                return;

            _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
            _skeletonGraphic.Update(0);
        }
    }
}