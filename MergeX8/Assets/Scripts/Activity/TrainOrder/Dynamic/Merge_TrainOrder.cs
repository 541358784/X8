using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.TrainOrder
{
    public class Merge_TrainOrder : MonoBehaviour
    {
        private Button _buttonSelf;
        private LocalizeTextMeshProUGUI _textTime;

        protected void Awake()
        {
            _buttonSelf = transform.GetComponent<Button>();
            _buttonSelf.onClick.AddListener(TrainOrderModel.Instance.TryOpenMain);

            _textTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating(nameof(UpdateView), 0, 1);
        }

        private void UpdateView()
        {
            gameObject.SetActive(TrainOrderModel.Instance.CanShowEntrance());
            if (gameObject.activeInHierarchy)
            {
                _textTime.SetText(TrainOrderModel.Instance.GetActivityLeftTimeString());
            }
        }
    }
}