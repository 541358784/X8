using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class SubSystem
    {
        public bool Enable = true;

        public virtual void Update()
        {
        }
        public bool destory;

        protected async Task<bool> DelayCareDestory(int millisecondsDelay)
        {
            await Task.Delay(millisecondsDelay);
            return !destory;
        }
    }

    interface IInitable
    {
        // 游戏启动时
        void Init();

        // 游戏终止
        void Release();
    }

    interface IStart
    {
        // 第一帧之前
        void Start();
    }

    interface IUpdatable
    {
        // 每帧
        void Update(float deltaTime);
    }

    interface ILateUpdatable
    {
        // 每帧
        void LateUpdate(float deltaTime);
    }
    
    interface IOnApplicationPause
    {
        void OnApplicationPause(bool pauseStatus);
    }


    interface IGlobal
    {
        void ToGlobal();
    }

    public class GlobalSystem<T> : SubSystem, IGlobal where T : GlobalSystem<T>
    {
        public static T Instance;

        public void ToGlobal()
        {
            Instance = this as T;
        }
    }
}