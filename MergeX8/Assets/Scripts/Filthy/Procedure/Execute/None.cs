using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Filthy.Procedure
{
    public class None: IExecute
    {
        public Transform _root { get; set; }
        public ProcedureBase _procedureBase { get; set; }
        public void Init(Transform root, ProcedureBase procedureBase)
        { 
            _root = root;
            _procedureBase = procedureBase;
        }

        public bool Execute()
        {
            _root.transform.Find("Root/SkipButton").gameObject.SetActive(false);
            return true;
        }
    }
}