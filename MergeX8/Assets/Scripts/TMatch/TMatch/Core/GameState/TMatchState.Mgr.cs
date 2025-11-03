using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Framework;

namespace TMatch
{


    public enum TMatchStateType
    {
        Prepare, //准备
        Create, //创建
        Play, //操作
        Finish, //结束
        Destory, //销毁
    }

    public abstract class TMatchStateParam
    {
    }

    public interface TMatchBaseState
    {
        TMatchStateType Type { get; }
        Task Enter(TMatchStateParam param);
        void Update(float deltaTime);
        Task Exit();
    }

    public class TMatchStateSystem : GlobalSystem<TMatchStateSystem>, IInitable, IUpdatable
    {
        private Dictionary<TMatchStateType, TMatchBaseState>
            states = new Dictionary<TMatchStateType, TMatchBaseState>();

        private TMatchBaseState preState;
        private TMatchBaseState curState;

        public void Init()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_CHANGE_STATE, ChangeState);
        }

        public void Release()
        {
            preState = null;
            curState?.Exit();
            curState = null;

            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_CHANGE_STATE, ChangeState);
        }

        public void Update(float deltaTime)
        {
            curState?.Update(deltaTime);
        }

        public void RegisterState(TMatchStateType type)
        {
            states.Add(type, Assembly.GetExecutingAssembly().CreateInstance($"TMatch.TMatch{type}State") as TMatchBaseState);
        }

        private void ChangeState(BaseEvent evt)
        {
            TMatchGameChangeStateEvent realEvt = evt as TMatchGameChangeStateEvent;
            ChangeStateAsync(realEvt.stateType, realEvt.param);
        }

        private async void ChangeStateAsync(TMatchStateType stateType, TMatchStateParam param)
        {
            preState = curState;
            curState = null;

            if (preState != null) await preState.Exit();
            TMatchBaseState tempState = states[stateType];
            await tempState.Enter(param);
            curState = tempState;
        }
    }
}