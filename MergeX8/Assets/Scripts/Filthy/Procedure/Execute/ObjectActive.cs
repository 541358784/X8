using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Filthy.Procedure
{
    public class ObjectActive : IExecute
    {
        public Transform _root { get; set; }
        public ProcedureBase _procedureBase { get; set; }

        private GameObject _logicObject;
        private bool _isShow;
        
        public void Init(Transform root, ProcedureBase procedureBase)
        {
            _root = root;
            _procedureBase = procedureBase;

            var paramArray = procedureBase._config.ExecuteParam.Split(',');

            _logicObject = root.transform.Find(paramArray[0]).gameObject;
            _isShow = paramArray[1] == "1" ? true : false;
            
            //_logicObject.gameObject.SetActive(!_isShow);
        }

        public bool Execute()
        {
            _root.transform.Find("Root/SkipButton").gameObject.SetActive(false);
            _logicObject.gameObject.SetActive(_isShow);
            return true;
        }
    }
}