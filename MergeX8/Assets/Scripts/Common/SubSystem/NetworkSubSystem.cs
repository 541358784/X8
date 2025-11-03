using System.Collections;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using Framework;
using UnityEngine;

namespace Gameplay
{
    public class NetworkSubSystem : GlobalSystem<NetworkSubSystem>, IInitable
    {
        private bool _useNetwork;
        private const float updateInterval = 3f;

        public bool UseNetwork
        {
            get { return _useNetwork; }
        }

        public void Init()
        {
            APIManager manager = APIManager.Instance;
            NetworkReachability networkReachability = Application.internetReachability;
            _useNetwork = (networkReachability != NetworkReachability.NotReachable);
            DebugUtil.Log($"use network : {UseNetwork}");

            if (!_useNetwork)
            {
                CoroutineManager.Instance.StartCoroutine(_CoUpdateNetworkStatus());
            }
        }


        public void Release()
        {
        }


        IEnumerator _CoUpdateNetworkStatus()
        {
            bool loading = false;
            while (!loading)
            {
                yield return new WaitForSeconds(updateInterval);

                if (!_useNetwork && APIManager.Instance.HasNetwork)
                {
                    loading = true;
                    // 先更新version文件
                    CoroutineManager.Instance.StartCoroutine(VersionManager.Instance.LoadRemoteVersionFile(
                        (bool success) =>
                        {
                            DebugUtil.Log($" {GetType()}, Online LoadVersionFile result : {success}");
                            loading = false;
                            if (success)
                            {
                                _useNetwork = true;
                            }
                        }));
                }
            }
        }
    }
}