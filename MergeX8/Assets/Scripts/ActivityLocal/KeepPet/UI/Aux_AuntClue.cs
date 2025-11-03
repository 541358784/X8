using System;
using UnityEngine;
using UnityEngine.UI;

public class Aux_AuntClue:MonoBehaviour
{
    private Button Btn;
    private void Awake()
    {
        Btn = gameObject.GetComponent<Button>();
        Btn.onClick.AddListener(UIPopupKeepPetClueController.OpenUI);
    }
}