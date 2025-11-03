using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FishEatFishSpace
{
    public class Empty : Enemy
    {
        public override int HP
        {
            get { return 0; }
            set { }
        }
    }
}
