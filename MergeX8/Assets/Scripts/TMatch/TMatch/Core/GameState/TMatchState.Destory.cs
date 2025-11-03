using System.Threading.Tasks;

namespace TMatch
{
    public class TMatchDestoryState : TMatchBaseState
    {
        TMatchStateType TMatchBaseState.Type => TMatchStateType.Destory;

        public async Task Enter(TMatchStateParam param)
        {

        }

        public void Update(float deltaTime)
        {

        }

        public async Task Exit()
        {

        }
    }
}