using System.Collections.Generic;
using Activity.CollectStone.Model;
using DragonPlus;
using UnityEngine;

namespace Activity.CollectStone.View
{
    public class Aux_CollectStone : Aux_ItemBase
    {
        private LocalizeTextMeshProUGUI _timeText;
        private GameObject _redPoint;
        private LocalizeTextMeshProUGUI _redPointText;

        protected override void Awake()
        {
            base.Awake();

            _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _redPoint = transform.Find("RedPoint").gameObject;
            _redPointText = _redPoint.transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();

            InvokeRepeating("UpdateUI", 0, 1);

            UpdateUI();
        }

        public override void UpdateUI()
        {
            gameObject.SetActive(CollectStoneModel.Instance.IsOpened());
            _redPoint.gameObject.SetActive(CollectStoneModel.Instance.GetStone() > 0);
            if (!gameObject.activeSelf)
                return;

            _timeText.SetText(CollectStoneModel.Instance.GetEndTimeString());
            _redPointText.SetText(CollectStoneModel.Instance.GetStone().ToString());
        }

        protected override void OnButtonClick()
        {
            base.OnButtonClick();
          
            UIManager.Instance.OpenUI(UINameConst.UIPopupCollectStoneMain);
        }

        private void OnDestroy()
        {
        }
    }
}