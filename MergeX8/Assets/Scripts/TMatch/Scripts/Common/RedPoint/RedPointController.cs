/*
 * @file RedPointController
 * 红点 - 控制器
 * @author lu
 */

using DragonPlus;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{


    public class RedPointController : MonoBehaviour
    {
        public string RedPointKey;
        public Text NumText;

        int Index { get; set; }

        RedPointConfig Cfg { get; set; }

        // Use this for initialization
        void Start()
        {
            Cfg = RedPointCenter.Instance.FindKey(RedPointKey, false);

            if (Cfg == null)
            {
                //DebugUtil.Log(CKer.lu, "红点中心: 试图监听不存在红点Key: " + RedPointKey);
                return;
            }

            Index = Cfg.Index;

            EventDispatcher.Instance.AddEventListener(EventEnum.REDPOINT, OnValueChange);
            // 设置显示状态
            transform.gameObject.SetActive(RedPointCenter.Instance.Get(RedPointKey) > 0);
        }

        public void SetData(string redPointKey, Text numText = null)
        {
            RedPointKey = redPointKey;
            NumText = numText;
        }

        void OnValueChange(BaseEvent e)
        {
            var index = ((RedPointEvent) e).Index;
            if (index == Index)
            {
                //DebugUtil.Log(CKer.lu, "红点控制器: 收到请求修改红点值,红点Key: " + Cfg.Key + ",值为: " + Cfg.Value);

                transform.gameObject.SetActive(Cfg.Value > 0);


                if (Cfg.Value > 0 && NumText != null)
                {
                    NumText.text = Cfg.Value.ToString();
                }
            }
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.REDPOINT, OnValueChange);
        }
    }
}
