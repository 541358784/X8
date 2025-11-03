using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

namespace DigTrench.UI
{
    public class DigTrenchMainUIProgressPoint : TransformHolder
    {
        public enum ProgressPointState
        {
            UnStarted,
            InProcessed,
            Completed,
        }
        public ProgressPointConfig ProgressPointConfig;
        public ProgressPointState State;//节点的状态
        public Transform UnCompletedView1;
        public Transform UnCompletedView2;
        public Transform CompletedView;
        
        public DigTrenchMainUIProgressPoint(Transform inTransform,ProgressPointConfig config) : base(inTransform)
        {
            ProgressPointConfig = config;
            var iconAsset = ResourcesManager.Instance.LoadResource<Sprite>("DigTrench/Texture/Items/"+ProgressPointConfig.IconName);
            UnCompletedView1 = transform.Find("Status_1");
            UnCompletedView1.Find("Icon").GetComponent<Image>().sprite = iconAsset;
            UnCompletedView1.Find("Title").gameObject.SetActive(false);
            UnCompletedView2 = transform.Find("Status_2");
            UnCompletedView2.Find("Icon").GetComponent<Image>().sprite = iconAsset;
            UnCompletedView2.Find("Title").gameObject.SetActive(false);
            CompletedView = transform.Find("Status_3");
            State = ProgressPointState.UnStarted;
            UpdateViewState();
        }

        public void StartProcess()
        {
            if (State != ProgressPointState.UnStarted)
            {
                Debug.LogError("StartProcess状态错误,应为"+ProgressPointState.UnStarted+",State为"+State);
                return;
            }
            State = ProgressPointState.InProcessed;
            UpdateViewState();
        }
        public void OnCompleted()
        {
            if (State != ProgressPointState.InProcessed)
            {
                Debug.LogError("OnCompleted状态错误,应为"+ProgressPointState.UnStarted+",State为"+State);
                return;
            }
            State = ProgressPointState.Completed;
            UpdateViewState();
        }
        public void UpdateViewState()
        {
            UnCompletedView1.gameObject.SetActive(State == ProgressPointState.UnStarted);
            UnCompletedView2.gameObject.SetActive(State == ProgressPointState.InProcessed);
            CompletedView.gameObject.SetActive(State == ProgressPointState.Completed);
        }
    }
    public class DigTrenchMainUIProgressModel:TransformHolder
    {
        private Slider _slider;
        private Transform _defaultProgressItem;
        private List<DigTrenchMainUIProgressPoint> _progressPointList;
        private int _pointCount;
        private int _currentCompletedCount;
        public DigTrenchMainUIProgressModel(Transform inTransform) : base(inTransform)
        {
            _slider = transform.Find("Slider").GetComponent<Slider>();
            _defaultProgressItem = transform.Find("Operate_1");//设置为起始点
            _defaultProgressItem.gameObject.SetActive(true);
            _defaultProgressItem.name = "StartPoint";
            foreach (Transform children in _defaultProgressItem)
            {
                children.gameObject.SetActive(false);
            }
            _slider.value = 0f;
            _currentCompletedCount = 0;
            EventDispatcher.Instance.AddEventListener(EventEnum.OnDigTile,OnDigTile);
        }

        public override void Destroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.OnDigTile,OnDigTile);
            base.Destroy();
        }

        public void UpdateProgressPointConfig(List<ProgressPointConfig> configs)
        {
            _progressPointList = new List<DigTrenchMainUIProgressPoint>();
            foreach (var config in configs)
            {
                AddProgressPoint(config);
            }
            _pointCount = _progressPointList.Count;
            if (_progressPointList.Count > 0)
                _progressPointList[0].StartProcess();//默认开始第一个节点的进程
            else
                Hide();
        }

        public DigTrenchMainUIProgressPoint AddProgressPoint(ProgressPointConfig config)
        {
            var newProgressPointObj =
                GameObject.Instantiate(_defaultProgressItem.gameObject, _defaultProgressItem.parent);
            var newProgressPoint = new DigTrenchMainUIProgressPoint(newProgressPointObj.transform,config);
            _progressPointList.Add(newProgressPoint);
            return newProgressPoint;
        }

        public void CompletedProgressPoint(int pointIndex)
        {
            _progressPointList[pointIndex - 1].OnCompleted();
            if (pointIndex < _progressPointList.Count)
            {
                _progressPointList[pointIndex].StartProcess();
            }
        }

        public void OnDigTile(BaseEvent evt)
        {
            var openTile = (DigTrenchTile)evt.datas[0];
            var newProgressIndex = 0;
            for (var i=0;i<_progressPointList.Count;i++)
            {
                var point = _progressPointList[i];
                if (point.ProgressPointConfig.Position == openTile.Position)
                {
                    if (point.State == DigTrenchMainUIProgressPoint.ProgressPointState.InProcessed)
                    {
                        newProgressIndex = i+1;   
                    }
                    else
                    {
                        Debug.LogError("进度条节点的状态不对1，ProgressPointConfig="+point.ProgressPointConfig+" State="+point.State);
                    }
                    break;
                }
            }

            if (newProgressIndex > 0)
            {
                if (newProgressIndex > _currentCompletedCount)
                {
                    _slider.transform.DOKill(false);
                    var startValue = _slider.value;
                    var endValue = newProgressIndex * 1f / _pointCount;
                    DOTween.To(() => startValue, (v) =>
                    {
                        _slider.value = v;
                        var nowCompletedCount = (int)Mathf.Floor(v / 1f * _pointCount);
                        if (nowCompletedCount > _currentCompletedCount)
                        {
                            _currentCompletedCount = nowCompletedCount;
                            CompletedProgressPoint(nowCompletedCount);
                        }
                    }, endValue, 0.5f).SetTarget(_slider.transform);
                }
                else
                {
                    Debug.LogError("进度条节点的状态不对2，_currentCompletedCount="+_currentCompletedCount+" newProgressIndex="+newProgressIndex);
                }
            }
        }
    }
}