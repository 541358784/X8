using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FishEatFishSpace
{
    public class Bubble : Enemy
    {
        protected override Transform BodyNode => transform;
        public override Vector3 Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }
        void Start()
        {
            transform.DOScale(transform.localScale + new Vector3(0.25f, 0.25f, 0.25f), 0.5f).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }
        protected float multi;
        public virtual float Multi
        {
            get { return multi; }
            set
            {
                multi = value;
                hpTxt.SetText($"x{multi}");
            }
        }
    }
}
