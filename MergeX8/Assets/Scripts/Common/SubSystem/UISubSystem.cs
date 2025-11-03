using System;
using DragonU3DSDK;
using Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class UISubSystem : GlobalSystem<UISubSystem>, IInitable
    {
        public void Init()
        {
            UIManager.Instance.Init();
        }

        public void Release()
        {
        }
    }
}