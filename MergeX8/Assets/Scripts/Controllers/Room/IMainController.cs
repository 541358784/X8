using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public interface IMainController
{
    UIHomeMainController mainController { get; }

    void Init(UIHomeMainController mainController);
    void Show();
    void Hide();
    bool IsShow();
    void MoneyAnim(UserData.ResourceId resId, int subNum, float time);
    void InitMoney(UserData.ResourceId resId, int money);
    Transform GetTransform();
}