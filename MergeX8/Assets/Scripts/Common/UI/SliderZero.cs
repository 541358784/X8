using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class SliderZero : MonoBehaviour
{
    public Slider m_Slider;
    public LocalizeTextMeshProUGUI m_Text;

    private void Awake()
    {
        m_Slider = transform.Find("Slider").GetComponent<Slider>();
        m_Text = transform.Find("Slider/progressinfo").GetComponent<LocalizeTextMeshProUGUI>();
    }

    private void OnDisable()
    {
        m_Slider.value = 0f;
        m_Text.SetText(
            LocalizationManager.Instance.GetLocalizedStringWithFormats("&key.UI_loading_progress_text",
                0.ToString("#0.0")));
    }
}