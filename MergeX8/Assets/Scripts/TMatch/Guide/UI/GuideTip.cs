using System.Collections;
using DragonPlus;
using UnityEngine;

namespace OutsideGuide
{
    public class GuideTip : GuideGraphicBase
    {
        private GameObject selfGO;
        private Animator _animator;
        private LocalizeTextMeshProUGUI _txt;
        private bool isInit;

        protected override void Init()
        {
            if (isInit) return;
            gameObject.AddComponent<Canvas>();
            selfGO = this.gameObject;
            _animator = transform.GetComponent<Animator>();
            _txt = transform.Find("DescriptionText").GetComponent<LocalizeTextMeshProUGUI>();
            isInit = true;
        }

        public override bool IsShow()
        {
            return gameObject.activeSelf;
        }

        public override void Show()
        {
            Init();
            _txt.SetTerm("UI_in_tutorial");
            if (!selfGO.activeSelf) selfGO.SetActive(true);
            _animator.Play("GuideMask_BottomGroup_appear");
        }

        public override void Hide()
        {
            Hide(false);
        }

        public void Hide(bool immediately = false)
        {
            Init();
            if (!selfGO.activeSelf) return;
            if (immediately)
            {
                selfGO.SetActive(false);
            }
            else
            {
                _animator.Play("GuideMask_BottomGroup_disappear");
                StartCoroutine(StartHide());
            }
        }

        private IEnumerator StartHide()
        {
            yield return new WaitForSeconds(1F);
            if (selfGO.activeSelf) selfGO.SetActive(false);
        }
    }
}