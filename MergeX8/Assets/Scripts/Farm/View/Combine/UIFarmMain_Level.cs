using System;
using System.Collections;
using System.Collections.Generic;
using Decoration;
using Decoration.DynamicMap;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.Farm;
using Farm.Model;
using Farm.Order;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public class UIFarmMain_Level : MonoBehaviour, IInitContent
    {
        private LocalizeTextMeshProUGUI _levelText;
        private LocalizeTextMeshProUGUI _expText;
        private Slider _slider;
        private Button _button;

        private UIFarmMainController _content;

        private int _currentExp = 0;
        private int _currentLevel = 0;

        private Coroutine _coroutine;
        private Tween _tween;
        
        private void Awake()
        {
            _button = transform.GetComponent<Button>();
            _button.onClick.AddListener(OnClickLevel);
            
            _levelText = transform.Find("LvText").GetComponent<LocalizeTextMeshProUGUI>();
            _expText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _slider = transform.Find("Slider").GetComponent<Slider>();

            
            FarmModel.Instance.LevelUpTransform = transform;
            
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_REFRESH_LEVEL_EXP, Event_RefreshLevelExp);
        }

        public void InitContent(object content)
        {
            _content = (UIFarmMainController)content;

            _currentLevel = FarmModel.Instance.storageFarm.Level;
            _currentExp = FarmModel.Instance.storageFarm.Exp;
            
            InitView();
        }

        private void InitView()
        {
            var config = FarmConfigManager.Instance.TableFarmLevelList.Find(a => a.Id == _currentLevel);
            if(config == null)
                config = FarmConfigManager.Instance.TableFarmLevelList[FarmConfigManager.Instance.TableFarmLevelList.Count - 1];

            _levelText.SetText(_currentLevel.ToString());
            _slider.value = 1.0f * _currentExp / config.LevelExp;
            _expText.SetText(_currentExp + "/" + config.LevelExp);

            if (FarmModel.Instance.IsMaxLevel())
            {
                _expText.SetTerm("UI_max");
                _slider.value = 1.0f;
            }
        }
        
        public void UpdateData(params object[] param)
        {
            
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_REFRESH_LEVEL_EXP, Event_RefreshLevelExp);
        }
        private void OnClickLevel()
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupFarmLevelTips);
        }
        private void Event_RefreshLevelExp(BaseEvent e)
        {
            if(_coroutine != null)
                return;
            
            if(_tween != null)
             _tween.Kill();
            
            _coroutine = StartCoroutine(LevelExpAnim());
        }

        private IEnumerator LevelExpAnim()
        {
            if (_currentExp == FarmModel.Instance.storageFarm.Exp && _currentLevel >= FarmModel.Instance.storageFarm.Level)
            {
                _coroutine = null;
                yield break;
            }
            
            var config = FarmConfigManager.Instance.TableFarmLevelList.Find(a => a.Id == _currentLevel);
            if(config == null)
            {
                _coroutine = null;
                yield break;
            }

            int animExp = config.LevelExp;
            bool isLevelUp = false;
            if (_currentLevel == FarmModel.Instance.storageFarm.Level)
                animExp = FarmModel.Instance.storageFarm.Exp;
            else
            {
                isLevelUp = true;
            }

            float time = (animExp - _currentExp) / 100;
            time = Mathf.Clamp(time, 0.3f, 1.5f);
            _tween = DOTween.To(() => _currentExp, x => _currentExp = x, animExp, time)
                .OnUpdate(() =>
            {
                _slider.value = 1.0f * _currentExp / config.LevelExp;
                _expText.SetText(_currentExp + "/" + config.LevelExp);
            })
                .OnComplete(() =>
            {
                if (isLevelUp)
                {
                    _currentLevel++;
                    _currentExp = 0;
                    InitView();

                    DynamicMapManager.Instance.ForceLoadCurrentChunk();
                    FarmModel.Instance.UpdateAllUnLockTipsStatus();
                    
                    Action action = () =>
                    {
                        _coroutine = null;
                        _tween = null;
                        Event_RefreshLevelExp(null);

                        DecoManager.Instance.CurrentWorld.LookAtSuggestNodeBySpeed(true);
                        var orders = FarmOrderManager.Instance.TryCreateOrder();
                        EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_ORDER_REFRESH, null, orders);

                        StartCoroutine(AutoPopupManager.AutoPopupManager.Instance.FarmPopUIViewLogic());
                    };
                    UIManager.Instance.OpenUI(UINameConst.UIPopupFarmLevelTipsShow, _currentLevel, action);
                }
            });
        }
    }
}