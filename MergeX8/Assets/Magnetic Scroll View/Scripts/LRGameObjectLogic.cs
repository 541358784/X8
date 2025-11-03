
using MagneticScrollView;
using UnityEngine;
public class LRGameObjectLogic : MonoBehaviour
{
    private int index = 0;
    
    public GameObject leftObj = null;
    public GameObject rightObj = null;

    public MagneticScrollRect magneticScrollRect;
    private void Awake()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(leftObj != null)
            leftObj.SetActive(index != 0);
        
        if(rightObj)
            rightObj.SetActive(index != 5);
    }

    public void But_Left()
    {
        index = magneticScrollRect.CurrentSelectedIndex;
        
        UpdateUI();
    }

    public void But_Right()
    {
        index = magneticScrollRect.CurrentSelectedIndex;
        
        UpdateUI();
    }
}
