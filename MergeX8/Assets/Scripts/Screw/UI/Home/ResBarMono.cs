using System;
using DG.Tweening;
using DragonPlus;
using Screw.Module;
using Screw.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    public enum ResBarType
    {
        None,
        Energy,
        Coin,
        MainPlay,
        ExtraSlot,
        BreakBody,
        TwoTask,
    }
    
    public abstract class ResBarMono : MonoBehaviour
    {
        protected Image _icon1;
        protected Image _icon2;

        protected Button _addButton;

        protected LocalizeTextMeshProUGUI _text1;
        protected LocalizeTextMeshProUGUI _text2;

        private string[] _iconName = { "Icon1", "Icon2" };
        private string[] _textName = { "Text1", "Text2" };

        public Image Icon1 => _icon1;
        public Image Icon2 => _icon2;

        protected Tween _animTween;

        public abstract ResBarType _resBarType { get; set; }
        
        private void Awake()
        {
            InitView();
            InitAwake();
            ResBarModule.Instance.RegisterResBar(_resBarType, _icon1.transform);
        }

        private void Start()
        {
            InitStart();
        }

        private void OnDestroy()
        {
            InitOnDestroy();
            ResBarModule.Instance.UnRegisterResBar(_resBarType, _icon1.transform);
        }

        protected virtual void InitView()
        {
            for (var i = 0; i < _iconName.Length; i++)
            {
               var obj = transform.Find(_iconName[i]);
               if(obj == null)
                   continue;

               if (i == 0)
                   _icon1 = obj.GetComponent<Image>();
               else
                   _icon2 = obj.GetComponent<Image>();
            }

            for (var i = 0; i < _textName.Length; i++)
            {
                var obj = transform.Find(_textName[i]);
                if(obj == null)
                    continue;

                if (i == 0)
                    _text1 = obj.GetComponent<LocalizeTextMeshProUGUI>();
                else
                    _text2 = obj.GetComponent<LocalizeTextMeshProUGUI>();
            }
            
            _addButton = transform.Find("ButtonAdd").GetComponent<Button>();
            _addButton.onClick.AddListener(OnButtonAdd);
        }
        
        public virtual void AnimChangeText(int finallyNum, int changeNum,  Action action = null)
        {
            if (_animTween != null)
            {
                _animTween.Kill();
                _animTween = null;
            }
            
            int newValue = finallyNum;
            int oldValue = newValue - changeNum;
            oldValue = Math.Max(oldValue, 0);
            newValue = Math.Max(newValue, 0);

            _text1.SetText(oldValue.ToString());

            float time = 1.0f * oldValue / 10f;
            time = Mathf.Clamp(time, 0.3f, 1.3f);
                
            _animTween = DOTween.To(() => oldValue, x => oldValue = x, newValue, time).OnUpdate(() =>
            {
                _text1.SetText(oldValue.ToString());
            }).OnComplete(() =>
            {
                _animTween = null;
                action?.Invoke();
            });
        }
        
        protected abstract void InitAwake();
        protected abstract void InitStart();
        protected abstract void InitOnDestroy();
        protected abstract void OnButtonAdd();
        protected T GetParam<T>(int index, object[] datas)
        {
            if (datas == null)
                return default(T);
            
            if(index < 0 || index >= datas.Length)
                return default(T);

            return (T)datas[index];
        }
 
        public void SetAddButtonActive(bool isActive)
        {
            _addButton?.gameObject.SetActive(isActive);
        }
    }
}