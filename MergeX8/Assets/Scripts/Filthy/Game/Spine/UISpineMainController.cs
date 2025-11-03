using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Filthy;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Filthy.Model;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Filthy.Game.Spine
{
    public class UISpineMainController: UIWindowController
    {
        private FilthySpine _spineConfig;
        private FilthyNodes _nodeConfig;
        private SpineGameLogic _spineGameLogic;
        
        private GameObject _finishObject;
        private GameObject _butItem;
        private SkeletonAnimation _skeletonAnimation;
        private bool _canExit = true;

        private int _index = 0;
        private List<GameObject> _buttonObjs = new List<GameObject>();
        private GameObject _errorObj;
        private GameObject _linkObj;
        private Action action;

        private Button _singleButton;
        
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
                if(_nodeConfig != null)
                    FilthyGameLogic.Instance.ExitGame(_nodeConfig);
                    
                action?.Invoke();
            });
            
            _singleButton = transform.Find("Root/SigleButton").GetComponent<Button>();
            _singleButton.onClick.AddListener(() =>
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StimulateChoseOperate, _index.ToString());
                OnClickButton();
            });
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            _nodeConfig = (FilthyNodes)objs[0];
            _spineConfig = (FilthySpine)objs[1];
            _spineGameLogic = (SpineGameLogic)objs[2];
            if(objs .Length >= 4)
                action = (Action)objs[3];
            
            _skeletonAnimation = _spineGameLogic.skeletonAnimation;

            _index = 0;
            UpdateButtonIcons();
        }

        private void OnClickButton()
        {
            PlaySound(_index);

            _linkObj?.gameObject.SetActive(false);
            _butItem.transform.parent.gameObject.SetActive(false);
            _canExit = false;
            SetSpineAnimation(_spineConfig.SpineAnims[_index], () =>
            {
                _index++;
                if (_index >= _spineConfig.SpineAnims.Count)
                {
                    if(_nodeConfig != null)
                        FilthyModel.Instance.OwnedNode(_nodeConfig.LevelId, _nodeConfig.Id);
                            
                    _finishObject.gameObject.SetActive(true);
                        
                    FilthyGameLogic.Instance.PlaySound("sfx_miserable_congratulate");
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
        
        private void UpdateButtonIcons()
        {
            foreach (var obj in _buttonObjs)
            {
                GameObject.DestroyImmediate(obj);
            }
            _buttonObjs.Clear();

            _linkObj = null;
            if (_spineConfig.LinkObjs != null && _spineConfig.LinkObjs.Count - 1 >= _index)
            {
                _linkObj = _spineGameLogic.transform.Find(_spineConfig.LinkObjs[_index]).gameObject;
                _linkObj.gameObject.SetActive(true);
            }
            
            if (_spineConfig.ButtonIcons != null && _spineConfig.ButtonIcons.Count > 0)
            {
                var icons = _spineConfig.ButtonIcons[_index];
                var buttonIcons = icons.Split(";");
            
                string atlasName = $"FilthyLevel{_spineConfig.LevelId}Atlas";
                int index = 0;
                foreach (var iconName in buttonIcons)
                {
                    string name = iconName;
                    var itemButton = Instantiate(_butItem, _butItem.transform.parent, false) as GameObject;
                    itemButton.gameObject.SetActive(true);
                    _buttonObjs.Add(itemButton);

                    var localIndex = index++;
                    var errorBtn = itemButton.transform.Find("Error").gameObject;
                    
                    itemButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StimulateChoseOperate, name);
                        OnIconButton(name, localIndex, errorBtn);
                    });

                    if (_spineConfig.Id == 101 && _index == 0)
                    {
                        var topLayer = new List<Transform>();
                        topLayer.Add(itemButton.transform);
                        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.StimulateChoseOperate, itemButton.transform as RectTransform, targetParam:name, topLayer: topLayer);
                        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StimulateChoseOperate, name,name);
                    }
                
                    itemButton.transform.Find("BG").GetComponent<Image>().sprite = ResourcesManager.Instance.GetSpriteVariant(atlasName, iconName); 
                }
            }
            else
            {
                FlowLinkObject();
            }
        }

        private void FlowLinkObject()
        {
            if (_linkObj == null)
            {
                _singleButton.gameObject.SetActive(false);
                return;
            }
                
            _singleButton.gameObject.SetActive(true);
                
            var position = DecoSceneRoot.Instance.mSceneCamera.WorldToScreenPoint(_linkObj.transform.position);
            var screenPos = UIRoot.Instance.mUICamera.ScreenToWorldPoint(position);
            screenPos.z = 0;
            
            _singleButton.transform.position= screenPos;
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
                    if(_nodeConfig != null)
                     FilthyGameLogic.Instance.ExitGame(_nodeConfig);
                    
                    action?.Invoke();
                },
                HasCloseButton = true,
                HasCancelButton = true,
                IsHighSortingOrder = true,
            });
        }

        private void OnIconButton(string iconName, int index, GameObject errorBtn)
        {
            if(_nodeConfig != null)
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrChoose, _nodeConfig.Id.ToString(), _index.ToString(), _spineConfig.CorrectIcons[_index]+"_"+iconName+"_"+iconName.Equals(_spineConfig.CorrectIcons[_index]));
           
            PlaySound(index);
            if (iconName.Equals(_spineConfig.CorrectIcons[_index]))
            {
                _linkObj?.gameObject.SetActive(false);
                _butItem.transform.parent.gameObject.SetActive(false);
                _singleButton.transform.gameObject.SetActive(false);
                _canExit = false;
                SetSpineAnimation(_spineConfig.SpineAnims[_index], () =>
                {
                    _index++;
                    if (_index >= _spineConfig.CorrectIcons.Count)
                    {
                        if(_nodeConfig != null)
                            FilthyModel.Instance.OwnedNode(_nodeConfig.LevelId, _nodeConfig.Id);
                            
                        _finishObject.gameObject.SetActive(true);
                        
                        FilthyGameLogic.Instance.PlaySound("sfx_miserable_congratulate");
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
                if (_spineConfig.ErrorAnim != null && _spineConfig.ErrorAnim.Count > 0)
                {
                    _butItem.transform.parent.gameObject.SetActive(false);
                    _canExit = false;
                    var anims = _spineConfig.ErrorAnim[_index].Split(';');
                    SetSpineAnimation(anims[index], () =>
                    {
                        _canExit = true;
                        SetSpineAnimation(_spineConfig.DefaultAnim, isLoop:true);
                        _butItem.transform.parent.gameObject.SetActive(true);
                    });
                }
                else
                {
                    errorBtn.gameObject.SetActive(false);
                    errorBtn.gameObject.SetActive(true);
                
                    _errorObj.gameObject.SetActive(false);
                    _errorObj.gameObject.SetActive(true);
                    FilthyGameLogic.Instance.PlaySound("sfx_miserable_wrong");
                }
            }
        }

        private async void SetSpineAnimation(string animName, Action animEndCall = null, bool isLoop = false)
        {
            if (_skeletonAnimation == null)
            {
                animEndCall?.Invoke();
                return;
            }
            
            var trackEntry = _skeletonAnimation.AnimationState.SetAnimation(
                0,
                animName,
                isLoop
            );
            _skeletonAnimation.Update(0);

            await Task.Delay((int)(trackEntry.AnimationEnd*1000)+1);
            
            animEndCall?.Invoke();
        }

        private async void PlaySound(int index)
        {
            if (_spineConfig.ErrorAnim != null)
            {
                var anims = _spineConfig.AudioNames[_index].Split(';');
                
                await Task.Delay(500);
            
                var audioName = anims[index];
                FilthyGameLogic.Instance.PlaySound(audioName);
            }
            else
            {
                if(_spineConfig.AudioNames == null || _spineConfig.AudioNames.Count <= _index)
                    return;

                await Task.Delay(500);
            
                var audioName = _spineConfig.AudioNames[_index];
                FilthyGameLogic.Instance.PlaySound(audioName);
            }
        }
    }
}