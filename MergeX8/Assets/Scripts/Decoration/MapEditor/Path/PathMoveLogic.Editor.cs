#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SomeWhere
{
    public partial class PathMoveLogic
    {
        public void PlayPath(string pathId)
        {
            PathMapConfigManager.Instance.InitConfig();
            
            _pathId = pathId;
            InitPath();
            Moveing();
        }

        private async void Moveing()
        {
            while (gameObject.activeSelf)
            {
                Update();
                await Task.Delay(20);
            }
        }
    }
}
#endif