using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Gameplay;

namespace TMatch
{


    public class EffectManager : Manager<EffectManager>
    {
        public static Vector2 Bezier(float t, Vector2 a, Vector2 b, Vector2 c)
        {
            var ab = Vector2.Lerp(a, b, t);
            var bc = Vector2.Lerp(b, c, t);
            return Vector2.Lerp(ab, bc, t);
        }
    }
}