using DragonPlus;

namespace TMatch
{


    public class CommonEvent : BaseEvent
    {
        public CommonEvent(string _eventEnum) : base(_eventEnum)
        {
        }

        public static bool DispatchEvent(string _eventEnum)
        {
            return EventDispatcher.Instance.DispatchEvent(new CommonEvent(_eventEnum));
        }
    }

    public class CommonEvent<T> : BaseEvent
    {
        public T Data { get; set; }

        public CommonEvent(string _eventEnum, T _data) : base(_eventEnum)
        {
            Data = _data;
        }

        public static bool DispatchEvent<T_1>(string _eventEnum, T_1 _data)
        {
            return EventDispatcher.Instance.DispatchEvent(new CommonEvent<T_1>(_eventEnum, _data));
        }
    }

    public class CommonEvent<T1, T2> : BaseEvent
    {
        public T1 Data1 { get; set; }
        public T2 Data2 { get; set; }

        public CommonEvent(string _eventEnum, T1 _data1, T2 _data2) : base(_eventEnum)
        {
            Data1 = _data1;
            Data2 = _data2;
        }

        public static bool DispatchEvent<T_1, T_2>(string _eventEnum, T_1 _data1, T_2 _data2)
        {
            return EventDispatcher.Instance.DispatchEvent(new CommonEvent<T_1, T_2>(_eventEnum, _data1, _data2));
        }
    }

    public class CommonEvent<T1, T2, T3> : BaseEvent
    {
        public T1 Data1 { get; set; }
        public T2 Data2 { get; set; }
        public T3 Data3 { get; set; }

        public CommonEvent(string _eventEnum, T1 _data1, T2 _data2, T3 _data3) : base(_eventEnum)
        {
            Data1 = _data1;
            Data2 = _data2;
            Data3 = _data3;
        }

        public static bool DispatchEvent<T_1, T_2, T_3>(string _eventEnum, T_1 _data1, T_2 _data2, T_3 _data3)
        {
            return EventDispatcher.Instance.DispatchEvent(
                new CommonEvent<T_1, T_2, T_3>(_eventEnum, _data1, _data2, _data3));
        }
    }

    public class CommonEvent<T1, T2, T3, T4> : BaseEvent
    {
        public T1 Data1 { get; set; }
        public T2 Data2 { get; set; }
        public T3 Data3 { get; set; }
        public T4 Data4 { get; set; }

        public CommonEvent(string _eventEnum, T1 _data1, T2 _data2, T3 _data3, T4 _data4) : base(_eventEnum)
        {
            Data1 = _data1;
            Data2 = _data2;
            Data3 = _data3;
            Data4 = _data4;
        }

        public static bool DispatchEvent<T_1, T_2, T_3, T_4>(string _eventEnum, T_1 _data1, T_2 _data2, T_3 _data3,
            T_4 _data4)
        {
            return EventDispatcher.Instance.DispatchEvent(
                new CommonEvent<T_1, T_2, T_3, T_4>(_eventEnum, _data1, _data2, _data3, _data4));
        }
    }

    public class CommonEvent<T1, T2, T3, T4, T5> : BaseEvent
    {
        public T1 Data1 { get; set; }
        public T2 Data2 { get; set; }
        public T3 Data3 { get; set; }
        public T4 Data4 { get; set; }
        public T5 Data5 { get; set; }

        public CommonEvent(string _eventEnum, T1 _data1, T2 _data2, T3 _data3, T4 _data4, T5 _data5) : base(_eventEnum)
        {
            Data1 = _data1;
            Data2 = _data2;
            Data3 = _data3;
            Data4 = _data4;
            Data5 = _data5;
        }

        public static bool DispatchEvent<T_1, T_2, T_3, T_4, T_5>(string _eventEnum, T_1 _data1, T_2 _data2, T_3 _data3,
            T_4 _data4, T_5 _data5)
        {
            return EventDispatcher.Instance.DispatchEvent(
                new CommonEvent<T_1, T_2, T_3, T_4, T_5>(_eventEnum, _data1, _data2, _data3, _data4, _data5));
        }
    }
}