using System.Collections.Generic;
using DragonPlus.Config.TMatch;

namespace TMatch
{


    public partial class TMatchLevelData
    {
        public int level; //关卡ID
        public Layout layoutCfg; //关卡布局
        public bool win; //是否胜利
        public float LastTimes; //剩余时间

        public bool isSettlement; //是否已经结算了

        public Dictionary<int, int> tripleItems = new Dictionary<int, int>(); //被消除的item

        public void Init()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_TRIPLE, OnTriple);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_TRIPLE_BOOST_HANDLE, OnTripleBoostHandle);
        }

        public void Release()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TRIPLE, OnTriple);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TRIPLE_BOOST_HANDLE,
                OnTripleBoostHandle);
        }

        private void OnTriple(BaseEvent evt)
        {
            TMatchTripleEvent realEvt = evt as TMatchTripleEvent;
            if (!tripleItems.ContainsKey(realEvt.id)) tripleItems.Add(realEvt.id, 0);
            tripleItems[realEvt.id] -= realEvt.deltaCnt;
        }

        private void OnTripleBoostHandle(BaseEvent evt)
        {
            TMatchTripleBoostHandleEvent realEvt = evt as TMatchTripleBoostHandleEvent;
            if (!tripleItems.ContainsKey(realEvt.id)) tripleItems.Add(realEvt.id, 0);
            tripleItems[realEvt.id] += realEvt.cnt;
        }
    }
}