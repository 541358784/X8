using System.Collections.Generic;
using Deco.World;
using DragonU3DSDK;
using UnityEngine;
using Decoration;

namespace SomeWhere
{
    public partial class PathPoint
    {
        private float[] _cdRange;
        private int[] _countRange;
        private ulong _nextOpenTime;
        private bool _closeForWander;
        public bool CloseForWander => _closeForWander;
        
        public void Order()
        {
        }
    }
}