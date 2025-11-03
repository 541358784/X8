using UnityEngine;

namespace Framework
{
    public static class MaterialPropertyBlockVisitor
    {
        static private MaterialPropertyBlock _instance;

        public static MaterialPropertyBlock Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MaterialPropertyBlock();
                }

                _instance.Clear();
                return _instance;
            }
        }
    }
}