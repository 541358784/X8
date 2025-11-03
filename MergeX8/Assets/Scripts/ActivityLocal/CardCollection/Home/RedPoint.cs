using DragonPlus;
using UnityEngine;

namespace ActivityLocal.CardCollection.Home
{
    public class RedPoint
    {
        private GameObject _root;
        private LocalizeTextMeshPro _text;

        public RedPoint(GameObject root)
        {
            _root = root;

            _text = _root.transform.Find("Label").GetComponent<LocalizeTextMeshPro>();
        }

        public void SetActive(bool active)
        {
            _root.SetActive(active);
        }

        public void RefreshText(int num)
        {
            _text.SetText(num.ToString());
        }
    }
}