using System.Threading.Tasks;

namespace TMatch
{
    public class TMatchFinishState : TMatchBaseState
    {
        TMatchStateType TMatchBaseState.Type => TMatchStateType.Finish;

        public async Task Enter(TMatchStateParam param)
        {
            EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_FINISH);

            EventDispatcher.Instance.DispatchEvent(new TMatchGameChangeStateEvent(TMatchStateType.Destory,
                new TMatchPlayStateParam()));
        }

        public void Update(float deltaTime)
        {

        }

        public async Task Exit()
        {

        }
    }
}