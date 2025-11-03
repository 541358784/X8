using UnityEngine;
using UnityEngine.U2D;
using TMPro;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Decoration
{
    public static class AnimationConfig
    {
        static public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        static public float MoveSpeed = 2.0f;
        static public float MoveDuration = 1.0f;
    }
}