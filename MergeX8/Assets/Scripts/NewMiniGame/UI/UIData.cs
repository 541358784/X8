using System;

namespace Framework.UI
{
    public class UIData
    {
        public bool canvasHeight;
    }

    public class UIDataParams<P1> : UIData
    {
        public P1 Params1;

        public UIDataParams(P1 params1)
        {
            Params1 = params1;
        }
    }

    public class UIDataParams<P1, P2> : UIData
    {
        public P1 Params1;
        public P2 Params2;

        public UIDataParams(P1 params1, P2 params2)
        {
            Params1 = params1;
            Params2 = params2;
        }
    }


    public class UIDataParams<P1, P2, P3> : UIData
    {
        public P1 Params1;
        public P2 Params2;
        public P3 Params3;

        public UIDataParams(P1 params1, P2 params2, P3 params3)
        {
            Params1 = params1;
            Params2 = params2;
            Params3 = params3;
        }
    }
}