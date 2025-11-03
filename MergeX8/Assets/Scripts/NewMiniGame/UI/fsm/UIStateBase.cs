using System;
using DragonU3DSDK;
using Framework.Utils;
using NewMiniGame.Fsm;
using UnityEngine;

namespace Framework.UI.fsm
{
    public abstract class UIStateBase : State<UIStateBase>
    {
        protected UIStateData _data;
        private   float       _tickDuration;


        public override void Enter(StateData param)
        {
            _data = param as UIStateData;

            base.Enter(param);

            _tickDuration = 1f;
        }

        public override string ToString()
        {
            return $"{_data.view.GetType()}:{GetType()}";
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _tickDuration += Time.deltaTime;
            if (_tickDuration >= 1f)
            {
                _tickDuration -= 1f;

#if !UNITY_EDITOR
                try
                {
#endif
                TickUpdate();
#if !UNITY_EDITOR
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
#endif
            }
        }

        protected virtual void TickUpdate()
        {
        }
    }

    public class UIStateData : StateData
    {
        public UIElement view;

        private UIStateData()
        {
        }

        public UIStateData(UIElement view)
        {
            this.view = view;
        }
    }


    public class UIStateDataParams<P1> : UIStateData
    {
        public P1 Params1;

        public UIStateDataParams(UIElement view, P1 params1) : base(view)
        {
            Params1 = params1;
        }
    }

    public class UIStateDataParams<P1, P2> : UIStateData
    {
        public P1 Params1;
        public P2 Params2;

        public UIStateDataParams(UIElement view, P1 params1, P2 params2) : base(view)
        {
            Params1 = params1;
            Params2 = params2;
        }
    }

    public class UIStateDataParams<P1, P2, P3> : UIStateData
    {
        public P1 Params1;
        public P2 Params2;
        public P3 Params3;

        public UIStateDataParams(UIElement view, P1 params1, P2 params2, P3 params3) : base(view)
        {
            Params1 = params1;
            Params2 = params2;
            Params3 = params3;
        }
    }
}