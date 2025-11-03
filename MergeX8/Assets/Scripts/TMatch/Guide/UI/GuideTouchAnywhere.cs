using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OutsideGuide
{
	public class GuideTouchAnywhere : GuideGraphicBase, IPointerDownHandler
	{

		public RectTransform Target;
		public Action<GuideTouchMaskEnum, Vector2> OnTouchMaskEvent { get; set; }

		public void OnPointerDown(PointerEventData eventData)
		{
			OnTouchMaskEvent?.Invoke(GuideTouchMaskEnum.Within, Vector2.zero);
		}

		public void SetActive(bool b)
		{
			if (gameObject.activeSelf == b) return;
			gameObject.SetActive(b);
		}

		protected override void Init()
		{
			
		}

		public override bool IsShow()
		{
			return gameObject.activeSelf;
		}

		public override void Show()
		{
			if (IsShow()) return;
			gameObject.SetActive(true);
		}

		public override void Hide()
		{
			if (!IsShow()) return;
			gameObject.SetActive(false);
		}
	}
}