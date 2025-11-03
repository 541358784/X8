using System;
using Framework.Utils;

namespace Framework.UI
{
    public struct EVENT_UI_CLOSE : IEvent
    {
        public Type t;

        public EVENT_UI_CLOSE(Type t)
        {
            this.t = t;
        }
    }

    public struct EVENT_UI_CLOSE_ALL_POPUP : IEvent
    {
    }

    public struct EVENT_UI_CLOSE_ALL_VIEW : IEvent
    {
    }

    public struct EventUIPopupShow : IEvent
    {
    }

    public struct EventUIPopupClose : IEvent
    {
    }
}