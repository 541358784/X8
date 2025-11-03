
using UnityEngine;

public class TreasureMapBox : MonoBehaviour
{
    public UIPopupTreasureMapController _controller;
    public void Init( UIPopupTreasureMapController controller)
    {
        _controller = controller;
    }
    
    public void OnAnimationFinished()
    {
        StartCoroutine(_controller.PlayRewardAnimation());
    }
}
