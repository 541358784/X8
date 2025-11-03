using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CKEmptyButton : MaskableGraphic
{
    public event UnityAction OnPointClick;

    protected CKEmptyButton()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }

    public void OnClick()
    {
        if (OnPointClick != null) OnPointClick();
    }
}
