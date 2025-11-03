
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private bool _isPressing = false;
    public UnityEngine.UI.Button.ButtonClickedEvent onPress;


    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressing = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPressing = false;
        
    }

    private void Update()
    {
        if (_isPressing)
        {
            onPress?.Invoke();
        }
    }
}
