using UnityEngine.UI;

namespace OnePath.View
{
    public class UIPopupOnePathFailController : UIWindowController
    {
        public override void PrivateAwake()
        {
            transform.Find("Root/ButtonGroup/ButtonContinue").GetComponent<Button>().onClick.AddListener(() =>
            {
                CloseWindowWithinUIMgr(true);
                SceneFsm.mInstance.ChangeState(StatusType.ExitOnePath);
            });
            
            
            transform.Find("Root/ButtonGroup/ButtonRetry").GetComponent<Button>().onClick.AddListener(() =>
            {
                CloseWindowWithinUIMgr(true);
            });
        }
    }
}