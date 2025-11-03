using System;
using System.Collections.Generic;
using DragonPlus;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    public class UIWidgets : MonoBehaviour
    {
        public struct UserData
        {
            public int Id;
        }

        private readonly List<Action> _onUpdateCalls = new List<Action>();

        public UserData Data;
        public Text[] texts;
        public Image[] images;
        public Button[] buttons;
        public Slider[] sliders;
        public Toggle[] toggles;
        public Animator[] animators;
        public Dropdown[] dropdowns;
        public RawImage[] rawImages;
        public InputField[] inputFields;
        public GameObject[] gameObjects;
        public ScrollRect[] scrollRects;
        public ToggleGroup[] toggleGroups;
        public RectTransform[] transforms;
        public SkeletonGraphic[] skeletonGraphics;
        public TextMeshProUGUI[] textMeshProUGuis;
        public LocalizeTextMeshProUGUI[] localizeTextMeshProUGuis;

        private void Update()
        {
            foreach (var action in _onUpdateCalls)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// 注册Update回调
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterUpdate(Action callback)
        {
            _onUpdateCalls.Add(callback);
        }

        /// <summary>
        /// 解注册Update回调
        /// </summary>
        /// <param name="callback"></param>
        public void UnregisterUpdate(Action callback)
        {
            if (_onUpdateCalls.Contains(callback))
            {
                _onUpdateCalls.Remove(callback);
            }
        }

        /// <summary>
        /// 解注册所有Update回调
        /// </summary>
        public void UnregisterAllUpdate()
        {
            _onUpdateCalls.Clear();
        }
    }
}