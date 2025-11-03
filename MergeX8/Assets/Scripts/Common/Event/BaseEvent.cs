public class BaseEvent
{
    public string type;
    public object[] datas;

    public BaseEvent(string type)
    {
        this.type = type;
    }

    public BaseEvent(string type, params object[] datas)
    {
        this.type = type;
        this.datas = datas;
    }
}