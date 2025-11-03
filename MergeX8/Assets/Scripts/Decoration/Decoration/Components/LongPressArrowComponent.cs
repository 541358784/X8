using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace Decoration
{
    public class LongPressArrowComponent : MonoBehaviour
    {
        private Image m_FillArrowImage;

        private void Awake()
        {
            m_FillArrowImage = transform.Find("Arrow").GetComponent<Image>();
        }

        public void OnDespawn()
        {
            m_FillArrowImage.DOKill(true);
            m_FillArrowImage.fillAmount = 0f;
        }

        public void OnSpawn()
        {
            m_FillArrowImage.fillAmount = 0f;
        }

        public void Show(Vector2 screenPos)
        {
            this.gameObject.SetActive(true);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)UIRoot.Instance.mRoot.transform, screenPos, UIRoot.Instance.mUICamera, out Vector2 uiPos);
            transform.localPosition = uiPos;
            if(m_FillArrowImage==null){
                m_FillArrowImage = transform.Find("Arrow").GetComponent<Image>();
            }
            m_FillArrowImage.fillAmount = 0f;
            m_FillArrowImage.DOFillAmount(1.0f,0.5f).OnComplete(Hide);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
