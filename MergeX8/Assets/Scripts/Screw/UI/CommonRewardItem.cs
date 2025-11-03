using System;
using DragonPlus;
using Gameplay;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    public class CommonRewardItem : MonoBehaviour
    {
        public ResData _resData;
        public Image _image;
        public LocalizeTextMeshProUGUI _numText;
        private bool IsAwake = false;

        public void Awake()
        {
            if (IsAwake)
                return;
            IsAwake = true;
            _numText = transform.GetComponentInChildren<LocalizeTextMeshProUGUI>();
            _image = transform.Find("Icon").GetComponent<Image>();
        }

        public void Init(ResData resData)
        {
            _resData = resData;
            if (!IsAwake)
                Awake();
            if (!UserData.UserData.ItemDic.TryGetValue(resData.id, out var itemConfig))
            {
                Debug.LogError("未找到ScrewItem配置"+resData.id);
                return;   
            }

            if (itemConfig.Infinity)
            {
                var time = itemConfig.InfinityTime * resData.count * (long)XUtility.Second;
                _numText.SetText(UserData.UserData.Instance.GetInfinityTimeString(time));
                _numText.gameObject.SetActive(true);
            }
            else
            {
                _numText.SetText(_resData.count.ToString());
                _numText.gameObject.SetActive(_resData.count > 1);
            }
            _image.sprite = UserData.UserData.GetResourceIcon(resData.id);
        }
    }
}