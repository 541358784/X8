using System;
using System.Resources;
using Activity.CollectStone.Model;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.CollectStone.View
{
    public class StoneItem : MonoBehaviour
    {
        private const string normalStoneImage = "stone_{0}";
        private const string grayStoneImage = "stone_gray_{0}";
        
        private Animator _animator;
        private Image _normalImage;
        private Image _grayImage;

        private Transform _effect;
        
        private int _index;
        private void Awake()
        {
            _effect = transform.Find("Root/FX_Confetti");
            _normalImage = transform.Find("Root/Have").GetComponent<Image>();
            _grayImage = transform.Find("Root/Normal").GetComponent<Image>();
        }

        public void Init(int index)
        {
            _index = index;

            RefreshUI();
            
            _normalImage.sprite = ResourcesManager.Instance.GetSpriteVariant("CollectStoneAtlas", string.Format(normalStoneImage, _index+1));
            _grayImage.sprite = ResourcesManager.Instance.GetSpriteVariant("CollectStoneAtlas", string.Format(grayStoneImage, _index+1));
        }

        public void RefreshUI()
        {
            bool canHave = CollectStoneModel.Instance.IsHave(_index);
            _normalImage.gameObject.SetActive(canHave);
            _grayImage.gameObject.SetActive(!canHave);
            
            _effect.gameObject.SetActive(false);
        }

        public Image GetNormalSprite()
        {
            return _normalImage;
        }

        public void ActiveEffect()
        {
            _effect.gameObject.SetActive(false);
            _effect.gameObject.SetActive(true);
        }
    }
}