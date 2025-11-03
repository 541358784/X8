using Framework.Utils;
using UnityEngine;

namespace Framework.UI.TableView
{
    public interface ITableViewCell
    {
        RectTransform RectTrans { get; }
        Transform     Transform { get; }
        void RemoveFromParent();
    }
}