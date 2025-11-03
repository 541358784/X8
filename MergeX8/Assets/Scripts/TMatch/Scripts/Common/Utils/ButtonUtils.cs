using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{


    public class ButtonUtils : MonoBehaviour
    {
        public static ButtonUtils Add(GameObject go, System.Action action = null, bool isPlayDefaultAudio = true)
        {
            if (go == null) return null;
            var btn = go.GetComponent<ButtonUtils>();
            if (btn == null) btn = go.AddComponent<ButtonUtils>();
            btn.isPlayDefaultAudio = isPlayDefaultAudio;
            btn.onClick = action;
            return btn;
        }

        public static ButtonUtils Add(Button go, System.Action action = null, bool isPlayDefaultAudio = true)
        {
            if (go == null) return null;
            var btn = go.GetComponent<ButtonUtils>();
            if (btn == null) btn = go.gameObject.AddComponent<ButtonUtils>();
            btn.isPlayDefaultAudio = isPlayDefaultAudio;
            btn.onClick = action;
            return btn;
        }

        private static bool cannotTouch = false;
        public static float LimitTouchTime { get; set; } = 0.5f;

        private System.Action onClick = null;

        private Button btn;
        private bool isPlayDefaultAudio;

        private void Awake()
        {
            btn = transform.GetComponent<Button>();
            if (btn == null) return;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickCall);
        }

        private void OnClickCall()
        {
            if (Input.touchCount > 1) return;
            if (cannotTouch) return;
            cannotTouch = true;
            if (isPlayDefaultAudio) AudioManager.Instance.PlayBtnTap();
            onClick?.Invoke();
            CommonUtils.DelayedCall(LimitTouchTime, () => { cannotTouch = false; });
        }
    }
}