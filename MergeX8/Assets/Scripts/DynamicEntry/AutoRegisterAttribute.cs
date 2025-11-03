using System;

namespace Dynamic
{
    // [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    // public class AutoRegister: Attribute { }
    
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CancelAutoRegister : Attribute { }
}