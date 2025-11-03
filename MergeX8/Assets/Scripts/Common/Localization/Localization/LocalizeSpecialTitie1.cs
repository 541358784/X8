using DragonPlus;
using UnityEngine;

/// <summary>
/// 特殊标题显示控制
/// </summary>
public class LocalizeSpecialTitie1 : MonoBehaviour
{
    private GameObject _objEn;
    private GameObject _objOther;
    private string _curLocalize = string.Empty;

    private void Awake()
    {
        _objEn = transform.Find("En").gameObject;
        _objOther = transform.Find("Other").gameObject;
    }

    private void Start()
    {
        _curLocalize = LocalizationManager.Instance.GetCurrentLocale();
        _objEn.SetActive(_curLocalize == Locale.ENGLISH);
        _objOther.SetActive(_curLocalize != Locale.ENGLISH);
    }
}