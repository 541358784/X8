using UnityEngine;

namespace DragonPlus.UI
{
    public class ReuseItemBase : MonoBehaviour
    {
        private ReuseScrollView scrollview;

        public string fbId;

        private int index = int.MinValue;

        public int Index
        {
            private set { index = value; }
            get { return index; }
        }

        public ReuseScrollView GetInfinityScrollView()
        {
            return scrollview;
        }

        public virtual void Reload(ReuseScrollView sv, int _index)
        {
            fbId = string.Empty;
            scrollview = sv;
            Index = _index;
        }

        public virtual void SelfReload()
        {
            if (Index != int.MinValue)
            {
            }
        }
    }
}