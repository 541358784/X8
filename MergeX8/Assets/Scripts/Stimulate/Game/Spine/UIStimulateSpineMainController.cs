using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Stimulate.Model.Spine
{
    public class UIStimulateSpineMainController : UIWindowController
    {
        private TableStimulateSpine _spineConfig;
        private TableStimulateNodes _nodeConfig;
        private SpineGameLogic _spineGameLogic;
        
        private GameObject _finishObject;
        private GameObject _butItem;
        private SkeletonAnimation _skeletonAnimation;
        private bool _canExit = true;

        private int _index = 0;
        private List<GameObject> _buttonObjs = new List<GameObject>();
        private GameObject _errorObj;
        private GameObject _linkObj;
        
        public override void PrivateAwake()
        {
            transform.Find("Root/ExitButton").GetComponent<Button>().onClick.AddListener(OnExitButton);
            
            _butItem = transform.Find("Root/Buttons/btn").gameObject;
            _butItem.gameObject.SetActive(false);

            _errorObj = transform.Find("Root/Errow").gameObject;
            _errorObj.gameObject.SetActive(false);
            
            _finishObject = transform.Find("Root/Finish").gameObject;
            _finishObject.gameObject.SetActive(false);
            _finishObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                StimulateGameLogic.Instance.ExitGame(_nodeConfig);
            });
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            _nodeConfig = (TableStimulateNodes)objs[0];
            _spineConfig = (TableStimulateSpine)objs[1];
            _spineGameLogic = (SpineGameLogic)objs[2];
            
            _skeletonAnimation = _spineGameLogic.skeletonAnimation;

            _index = 0;
            UpdateButtonIcons();
        }

        private void UpdateButtonIcons()
        {
            foreach (var obj in _buttonObjs)
            {
                GameObject.DestroyImmediate(obj);
            }
            _buttonObjs.Clear();

            var icons = _spineConfig.buttonIcons[_index];
            var buttonIcons = icons.Split(";");
            
            string atlasName = $"Level{_nodeConfig.levelId}Atlas";
            foreach (var iconName in buttonIcons)
            {
                string name = iconName;
                var itemButton = Instantiate(_butItem, _butItem.transform.parent, false) as GameObject;
                itemButton.gameObject.SetActive(true);
                _buttonObjs.Add(itemButton);

                var errorBtn = itemButton.transform.Find("Error").gameObject;
                    
                itemButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StimulateChoseOperate, name);
                    OnIconButton(name, errorBtn);
                });

                if (_spineConfig.id == 101 && _index == 0)
                {
                    var topLayer = new List<Transform>();
                    topLayer.Add(itemButton.transform);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.StimulateChoseOperate, itemButton.transform as RectTransform, targetParam:name, topLayer: topLayer);
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StimulateChoseOperate, name,name);
                }
                
                itemButton.transform.Find("BG").GetComponent<Image>().sprite = ResourcesManager.Instance.GetSpriteVariant(atlasName, iconName); 
            }

            _linkObj = null;
            if (_spineConfig.linkObjs != null && _spineConfig.linkObjs.Length - 1 >= _index)
            {
                _linkObj = _spineGameLogic.transform.Find(_spineConfig.linkObjs[_index]).gameObject;
                _linkObj.gameObject.SetActive(true);
            }
        }
        private void OnExitButton()
        {
            if(!_canExit)
                return;
            
            CommonUtils.OpenCommon1ConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("ui_minigame_11"),
                OKCallback = () =>
                {
                    StimulateGameLogic.Instance.ExitGame(_nodeConfig);
                },
                HasCloseButton = true,
                HasCancelButton = true,
                IsHighSortingOrder = true,
            });
        }

        private void OnIconButton(string iconName, GameObject errorBtn)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrChoose, _nodeConfig.id.ToString(), _index.ToString(), _spineConfig.correctIcons[_index]+"_"+iconName+"_"+iconName.Equals(_spineConfig.correctIcons[_index]));
           
            if (iconName.Equals(_spineConfig.correctIcons[_index]))
            {
                PlaySound();

                _linkObj?.gameObject.SetActive(false);
                _butItem.transform.parent.gameObject.SetActive(false);
                _canExit = false;
                SetSpineAnimation(_spineConfig.spineAnims[_index], () =>
                {
                    _index++;
                    if (_index >= _spineConfig.correctIcons.Length)
                    {
                        StimulateModel.Instance.OwnedNode(_nodeConfig.levelId, _nodeConfig.id);
                            
                        _finishObject.gameObject.SetActive(true);
                        
                        StimulateGameLogic.Instance.PlaySound("sfx_miserable_congratulate");
                    }
                    else if (_index == _spineConfig.correctIcons.Length - 1)//最后一步自动
                    {
                        OnIconButton(_spineConfig.correctIcons[_index], errorBtn);
                    }
                    else
                    {
                        _canExit = true;
                        UpdateButtonIcons();
                        _butItem.transform.parent.gameObject.SetActive(true);
                        _linkObj?.gameObject.SetActive(true);
                    }
                });
            }
            else
            {
                errorBtn.gameObject.SetActive(false);
                errorBtn.gameObject.SetActive(true);
                
                _errorObj.gameObject.SetActive(false);
                _errorObj.gameObject.SetActive(true);
                
                StimulateGameLogic.Instance.PlaySound("sfx_miserable_wrong");
            }
        }

        private async void SetSpineAnimation(string animName, Action animEndCall = null)
        {
            if (_skeletonAnimation == null)
            {
                animEndCall?.Invoke();
                return;
            }
            
            var trackEntry = _skeletonAnimation.AnimationState.SetAnimation(
                0,
                animName,
                false
            );
            _skeletonAnimation.Update(0);

            await Task.Delay((int)(trackEntry.AnimationEnd*1000)+1);
            
            animEndCall?.Invoke();
        }

        private async void PlaySound()
        {
            if(_spineConfig.audioNames == null || _spineConfig.audioNames.Length <= _index)
                return;

            await Task.Delay(1000);
            
            var audioName = _spineConfig.audioNames[_index];
            StimulateGameLogic.Instance.PlaySound(audioName);
        }
    }
}