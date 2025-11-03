using Framework;


namespace TMatch
{
    public class TMatchPruchaseBoostSystem : GlobalSystem<TMatchPruchaseBoostSystem>, IInitable
    {
        public int RetainPurchaseCntInSingleLevel;

        public void Init()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_ENTER, OnEnterEvt);
        }

        public void Release()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_ENTER, OnEnterEvt);
        }

        private void OnEnterEvt(BaseEvent evt)
        {
            RetainPurchaseCntInSingleLevel = 0;
        }
    }
}