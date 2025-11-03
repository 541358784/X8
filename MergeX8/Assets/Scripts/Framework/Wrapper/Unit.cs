using System;
using System.Collections;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Framework.Wrapper
{
    public class Unit : IDisposable
    {
        public Skeleton Skeleton;
        public RendererWrapper RendererWrapper;
        public BoxCollider[] BoxColliders;
        public ParticleSystem[] ParticleSystems;
        public MeshFilter MeshFilter;

        public Node Node
        {
            get { return _node; }
        }

        enum LoadStatus
        {
            NotLoad,
            Loading,
            Loaded,
        }

        private int _id;
        private Node _node = new Node();
        private GameObjectWrapper _goWrpper;

        private LoadStatus _loadStatus = LoadStatus.NotLoad;

        // private bool isDisposed = false;
        private string _path;
        private Action<GameObject> _onLoadCallback;
        private ResObject _resObject;

        public string Path
        {
            get { return _path; }
        }


        private static int currentGUID = 1;


        public Unit()
        {
            _id = currentGUID++;
        }

        public int GetID()
        {
            return _id;
        }

        public void Load(string path, Action<GameObject> onLoad = null)
        {
            _loadStatus = LoadStatus.Loading;
            _path = path;
            _onLoadCallback = onLoad;
            _node.Name = $"unit({_id}):{_path}";


#if RENDER_SERVER
            NewDecoAssetManager.Instance.GetRes(path, (o, s, arg3) => _onLoaded(o as GameObject));
#else
            //CoroutineManager.Instance.StartCoroutine(_CoLoad(path));
            var asset = ResourcesManager.Instance.LoadResource<GameObject>(path);
            if (asset != null)
            {
                var go = asset.Instantiate();
                _onLoaded(go);
            }
            else
            {
                DebugUtil.LogError($"unit({_id}) load resource failed,  path = {path}");
            }

#endif
        }

        public bool IsLoaded()
        {
            return _loadStatus == LoadStatus.Loaded;
        }

        public void Dispose()
        {
            _goWrpper?.SetActive(false);
            if (_goWrpper != null) GameplayInfoManager.Instance.UnRegister(_goWrpper.GetInstanceID());
            GameObjectFactory.Destroy(_goWrpper?.GameObject);
            _resObject?.Dispose();
            _resObject = null;
            _node?.Dispose();
            _node = null;
            _loadStatus = LoadStatus.NotLoad;
        }

        public bool IsDispose()
        {
            return _node == null;
        }

        private void _onLoaded(GameObject o)
        {
            if (IsDispose())
            {
                if (o)
                {
                    o.SetActive(false);
                    GameObjectFactory.Destroy(o);
                }

                return;
            }

            var go = o as GameObject;
            if (go != null)
            {
                _goWrpper = new GameObjectWrapper(go);
                var collector = new ComponentCollector(go);
                Skeleton = new Skeleton(collector.Transforms);
                RendererWrapper = new RendererWrapper(collector.Renderers);
                BoxColliders = collector.BoxColliders;
                ParticleSystems = collector.ParticleSystems;
                MeshFilter = collector.FirstMeshFilter;

#if UNITY_ANDROID
                if (Application.isEditor)
                {
                    var renderer = collector.Renderers;
                    if (renderer != null)
                    {
                        foreach (var r in renderer)
                        {
                            if (r.sharedMaterials != null)
                            {
                                foreach (var m in r.sharedMaterials)
                                {
                                    if (m.shader != null)
                                    {
                                        Shader s = Shader.Find(m.shader.name);
                                        if (s != null)
                                        {
                                            m.shader = s;
                                            DebugUtil.Log("qushuang -----> unit({0}) reset shader {1}", _id,
                                                m.shader.name);
                                        }
                                        else
                                        {
                                            DebugUtil.Log("qushuang -----> unit({0}) reset shader failed {1}", _id,
                                                m.shader.name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //_replaceUnsupportedShader(collector);
#endif
#if !RENDER_SERVER
                var info = new GameplayInfoManager.GameplayInfo();
                info.id = _id;
                info.type = 0;
                GameplayInfoManager.Instance.Register(_goWrpper.GetInstanceID(), info);
#endif
                //DebugUtil.LogError("------------unit({0}) attach to node, path = {1},  go = {2}", _id, path, _goWrpper);
                _node.AttachChild(_goWrpper);
                DebugUtil.Log("------------unit({0}) loaded, path = {1}", _id, _path);
            }

            _loadStatus = LoadStatus.Loaded;
            _onLoadCallback?.Invoke(go);
        }


        private void _replaceUnsupportedShader(ComponentCollector collector)
        {
            var renderer = collector.Renderers;
            if (renderer != null)
            {
                foreach (var r in renderer)
                {
                    if (r.sharedMaterials != null)
                    {
                        foreach (var m in r.sharedMaterials)
                        {
                            if (m.shader != null)
                            {
                                if (!m.shader.isSupported)
                                {
                                    //r.enabled = false;
                                    Shader s = Shader.Find("Unlit/Texture");
                                    if (s != null)
                                    {
                                        m.shader = s;
                                        DebugUtil.Log(
                                            "qushuang -----> unit({0}) replace shader to Unlit/Texture, Becasue {1} is not supported",
                                            _id, m.shader.name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}