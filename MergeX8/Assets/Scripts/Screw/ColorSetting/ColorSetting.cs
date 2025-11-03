using System;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using Screw;
using UnityEngine;

namespace Screw
{
    public class ColorSetting : ScriptableObject
    {
        [Serializable]
        public struct ColorData
        {
            public Color panel;

            public Color panelShadow;

            public Color screw;

            public Color module;
        }

        public enum ColorChannel
        {
            R = 0,
            G = 1,
            B = 2,
            Alpha = 3
        }

        public Vector2 aeroEffectOffsetPerLayer;

        [SerializeField] public ColorData lightGreen;

        [SerializeField] public ColorData blue;

        [SerializeField] public ColorData yellow;

        [SerializeField] public ColorData cyan;

        [SerializeField] public ColorData lilac;

        [SerializeField] public ColorData red;

        [SerializeField] public ColorData pink;

        [SerializeField] public ColorData purple;

        [SerializeField] public ColorData orange;

        [SerializeField] public ColorData darkGreen;

        public static ColorSetting _instance;

        public static ColorSetting Instance
        {
            get
            {
                if (_instance == null)
                {
                    var scriptableObject= ResourcesManager.Instance.LoadResource<ScriptableObject>("Screw/Scriptable/ColorSetting");
                    
                    _instance = (ColorSetting)scriptableObject;
                    _instance.InitMap();
                }

                return _instance;
            }
        }

        private Dictionary<ColorType, ColorSetting.ColorData> _colorMap;

        public Dictionary<ColorType, ColorSetting.ColorData> ColorMap => _colorMap;
        private void InitMap()
        {
            _colorMap = new Dictionary<ColorType, ColorData>();
            _colorMap.Add(ColorType.Green, lightGreen);
            _colorMap.Add(ColorType.Cyan, cyan);
            _colorMap.Add(ColorType.Yellow, yellow);
            _colorMap.Add(ColorType.Blue, blue);
            _colorMap.Add(ColorType.Lilac, lilac);
            _colorMap.Add(ColorType.Red, red);
            _colorMap.Add(ColorType.Pink, pink);
            _colorMap.Add(ColorType.Purple, purple);
            _colorMap.Add(ColorType.Orange, orange);
            _colorMap.Add(ColorType.Grey, darkGreen);
        }
    }
}