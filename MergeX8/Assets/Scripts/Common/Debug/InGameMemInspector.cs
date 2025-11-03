using UnityEngine;

namespace Framework
{
    public class InGameMemInspector
    {
        public static bool On
        {
            set
            {
                if (value)
                {
                    if (_instance == null)
                    {
                        _instance = new InGameMemInspector();
                        _instance._Init();
                    }
                }
                else
                {
                    if (_instance != null)
                    {
                        _instance._Release();
                    }

                    _instance = null;
                }
            }
            get { return _instance != null; }
        }

        private static InGameMemInspector _instance;
        private GameObject _go = null;

        private void _Init()
        {
            _go = GameObjectFactory.Create(true);
            _go.name = GetType().ToString();
            _go.AddComponent<MBCodeMemInspector>();
        }

        private void _Release()
        {
            if (_go != null)
            {
                GameObjectFactory.Destroy(_go);
            }

            _go = null;
        }
    }
}