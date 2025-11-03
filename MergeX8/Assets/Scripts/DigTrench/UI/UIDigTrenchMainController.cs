using System;
using System.Collections.Generic;
using DG.Tweening;
using DigTrench;
using DigTrench.UI;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using GamePool;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

namespace DigTrench.UI
{
    public class LifeModel : TransformHolder
    {
        public LifeModel(Transform inTransform) : base(inTransform)
        {

        }
    }

    public class TimeCountModel : TransformHolder
    {
        public TimeCountModel(Transform inTransform) : base(inTransform)
        {

        }
    }

    public class PropsModel : TransformHolder
    {
        public class PropsItem : TransformHolder
        {
            public int Count;
            public int Id;
            public Text CountNumText;
            public Image Icon;
            public PropsItem(Transform inTransform,int id,Sprite propsSprite) : base(inTransform)
            {
                Id = id;
                Count = 0;
                Icon = transform.Find("Icon").GetComponent<Image>();
                Icon.sprite = propsSprite;
                CountNumText = transform.Find("CountNum").GetComponent<Text>();
                UpdateView();
            }

            public async void GetProps(int count,Vector3 screenPosition)
            {
                Show();
                Icon.gameObject.SetActive(false);
                CountNumText.gameObject.SetActive(false);
                var worldPosition = UIRoot.Instance.mUICamera.ScreenToWorldPoint(screenPosition);
                for (var i = 0; i < count; i++)
                {
                    var flyObject = GameObject.Instantiate(Icon.gameObject, transform.parent.parent);
                    flyObject.SetActive(true);
                    flyObject.transform.position = worldPosition;
                    var time = 0.5f;
                    var lastProgress = 0f;
                    DOTween.To(() => 0f, (progress) =>
                    {
                        flyObject.transform.position +=
                            (Icon.transform.position - flyObject.transform.position) *
                            ((progress - lastProgress) / (1 - lastProgress));
                        lastProgress = progress;
                    }, 1f, time).SetTarget(flyObject.transform).SetEase(Ease.InCubic).OnComplete(() =>
                    {
                        GameObject.Destroy(flyObject);
                        AddCount();
                    });
                    
                    await XUtility.WaitSeconds(0.1f);
                }

            }

            public void AddCount()
            {
                Count++;
                CheckReduce();
                UpdateView();
            }

            private int _reduceCount=0;
            public void TryReduceCount(int reduceCount,Vector3 worldPosition)
            {
                _reduceCount += reduceCount;
                CheckReduce();
            }

            public void CheckReduce()
            {
                var updateFlag = false;
                while (_reduceCount > 0 && Count > 0)
                {
                    updateFlag = true;
                    _reduceCount--;
                    Count--;
                }
                if (updateFlag)
                    UpdateView();
            }
            void UpdateView()
            {
                if (Count <= 0)
                {
                    Hide();
                }
                else if (Count == 1)
                {
                    Show();
                    Icon.gameObject.SetActive(true);
                    CountNumText.gameObject.SetActive(false);
                }
                else
                {
                    Show();
                    Icon.gameObject.SetActive(true);
                    CountNumText.gameObject.SetActive(true);
                    CountNumText.text = Count.ToString();
                }
            }
        }
        private Transform _defaultItem;
        private Dictionary<int, PropsItem> _itemDic = new Dictionary<int, PropsItem>();
        public PropsModel(Transform inTransform) : base(inTransform)
        {
            _defaultItem = transform.Find("DefaultItem");
            _defaultItem.gameObject.SetActive(false);
        }
        
        public void GetProps(PropsGroup propsGroup,Sprite propsSprite,Vector3 screenPosition)
        {
            if (!_itemDic.ContainsKey(propsGroup.PropsId))
            {
                var propsItemObj = GameObject.Instantiate(_defaultItem.gameObject,_defaultItem.parent);
                propsItemObj.name = "Props_" + propsGroup.PropsId;
                propsItemObj.SetActive(true);
                var newPropsItem = new PropsItem(propsItemObj.transform,propsGroup.PropsId,propsSprite);
                _itemDic.Add(propsGroup.PropsId,newPropsItem);
            }
            var propsItem = _itemDic[propsGroup.PropsId];
            propsItem.GetProps(propsGroup.Count, screenPosition);
        }

        public void UseProps(PropsGroup propsGroup,Vector3 worldPosition)
        {
            _itemDic[propsGroup.PropsId].TryReduceCount(propsGroup.Count,worldPosition);
        }
    }
}

public class UIDigTrenchMainController:UIWindowController
    {
        private Button _startBtn;
        private Button _finishBtn;
        private Button _closeBtn;
        private Transform _showTargetView;
        private LifeModel _lifeModel;
        private TimeCountModel _timeCountModel;
        private PropsModel _propsModel;
        private DigTrenchMainUIProgressModel _progressModel;
        private DigTrenchGameConfig _config;
        public override void PrivateAwake()
        {
            _startBtn = GetItem<Button>("Root/Start");
            _finishBtn = GetItem<Button>("Root/Finish");
            _closeBtn = GetItem<Button>("Root/PauseButton");
            _showTargetView = transform.Find("Root/Tip");
            if (_showTargetView)
                _showTargetView.gameObject.SetActive(false);
            _lifeModel = new LifeModel(GetItem<Transform>("Root/TopGroup/HitPoint"));
            _timeCountModel = new TimeCountModel(GetItem<Transform>("Root/TopGroup/Time"));
            _progressModel = new DigTrenchMainUIProgressModel(GetItem<Transform>("Root/OperateGroup"));
            _lifeModel.Hide();//第一版本先不上生命模块
            _timeCountModel.Hide();//第一版本先不上倒计时模块
            
            _startBtn.onClick.AddListener(() =>
            {
                _startBtn.gameObject.SetActive(false);
            });
            _startBtn.gameObject.SetActive(false);
            
            _finishBtn.onClick.AddListener(OnClickFinish);
            _finishBtn.gameObject.SetActive(false);
            
            _closeBtn.onClick.AddListener(OnClickQuitGame);
            _propsModel = new PropsModel(GetItem<Transform>("Root/PropsGroup"));
        }

        private void OnDestroy()
        {
            _progressModel.Destroy();
        }

        private int _levelId;
        private bool _isFirstTimePlay;
        public void BindLevelConfig(DigTrenchGameConfig config,int levelId,bool isFirstTimePlay)
        {
            _isFirstTimePlay = isFirstTimePlay;
            _levelId = levelId;
            _config = config;
            _progressModel.UpdateProgressPointConfig(_config.ProgressPoints);
            _startBtn.transform.Find("Group").gameObject.SetActive(false);
            _startBtn.transform.Find("Text (1)").gameObject.SetActive(false);
            if (!_config.StartWordsKey.IsEmptyString())
                _startBtn.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm(_config.StartWordsKey);
            if (!_config.EndWordsKey.IsEmptyString())
                _finishBtn.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm(_config.EndWordsKey);
            if (_showTargetView && !_config.ShowTargetWordsKey.IsEmptyString())
                _showTargetView.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm(_config.ShowTargetWordsKey);
            _finishBtn.transform.Find("Bag")?.gameObject.SetActive(levelId == 99);
        }

        public void GetProps(PropsGroup propsGroup,Sprite propsSprite,Vector3 screenPosition)=>_propsModel.GetProps(propsGroup,propsSprite,screenPosition);
        public void UseProps(PropsGroup propsGroup, Vector3 worldPosition) => _propsModel.UseProps(propsGroup, worldPosition);
        public void OnClickFinish()
        {
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.FinishDigTrenchGame,true);
            if (_levelId != 99)
            {
                if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_AsmrContinue))
                {
                    AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_AsmrContinue, b =>
                    {
                    });
                }   
            }
        }

        public void OnClickQuitGame()
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupGameTips,_levelId);
        }

        public void SetFinishLevel()
        {
            _finishBtn.gameObject.SetActive(true);
        }

        public void HideCloseBtn()
        {
            _closeBtn.gameObject.SetActive(false);
        }
        public void SetStartLevel()
        {
            if (!_config.StartWordsKey.IsEmptyString())
            {
                _startBtn.GetComponent<Image>().enabled = false;
                _startBtn.gameObject.SetActive(true);
            }
            else
            {
                _startBtn.gameObject.SetActive(false);
            }
        }

        public async void ShowTargetText()
        {
            if (_showTargetView && !_config.ShowTargetWordsKey.IsEmptyString())
            {
                _showTargetView.gameObject.SetActive(true);
                await XUtility.WaitSeconds(3f);
                if (_showTargetView)
                {
                    _showTargetView.gameObject.SetActive(false);
                }
            }
        }
    }