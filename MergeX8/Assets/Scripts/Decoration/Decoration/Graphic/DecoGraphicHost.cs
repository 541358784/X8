using System;
using UnityEngine;

namespace Deco.Graphic
{
    public abstract class DecoGraphicHost<T> where T : DecoGraphic
    {
        public T Graphic;
        public abstract void LoadGraphic(GameObject parentObj);
        public abstract void UnloadGraphic();
        public abstract void AsyncLoadGraphic(GameObject parentObj, bool isPreview, Action onFinished);
    }
}