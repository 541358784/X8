using System;
using Screw.UI;

public partial class UIManager : Manager<UIManager>
{
    public UIWindow OpenUI(Type type, params object[] datas)
    {
        WindowAttribute attribute = Attribute.GetCustomAttribute(type, typeof(WindowAttribute)) as WindowAttribute;
        if (attribute == null)
            throw new Exception($"Window {type.FullName} not found {nameof(WindowAttribute)} attribute.");

        return OpenUI(attribute.AssetPath, attribute.WindowType, attribute.WindowLayer, type, attribute.AddUIMask, datas);
    }

    public void CloseUI(Type type, bool destroy = true)
    {
        WindowAttribute attribute = Attribute.GetCustomAttribute(type, typeof(WindowAttribute)) as WindowAttribute;
        if (attribute == null)
            throw new Exception($"Window {type.FullName} not found {nameof(WindowAttribute)} attribute.");

        CloseUI(attribute.AssetPath, destroy);
    }
}