using UnityEngine;

namespace Filthy.Procedure
{
    public interface IExecute
    {
        public Transform _root { get; set; }
        public ProcedureBase _procedureBase { get; set; }

        public void Init(Transform root, ProcedureBase procedureBase);
        public bool Execute();
    }
}