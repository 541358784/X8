using DG.Tweening;
using DragonU3DSDK.Network.API;
using UnityEngine;

public partial class UIKeepPetMainController
{
    private Animator Bubble;
    private Transform BubbleHappy;
    private Transform BubbleHunger;
    private Animator BubbleSleep;
    private ulong LastShowBubbleTime;
    private ulong ShowBubbleInterval = XUtility.Second * 30;

    public void InitBubble()
    {
        Bubble = GetItem<Animator>("Root/Bubble");
        Bubble.gameObject.SetActive(false);
        Bubble.transform.Find("Sleep").gameObject.SetActive(false);
        Bubble.transform.Find("Sick").gameObject.SetActive(false);
        Bubble.transform.Find("Steak").gameObject.SetActive(false);
        BubbleHappy = Bubble.transform.Find("Happy");
        BubbleHunger = Bubble.transform.Find("Bone");
        BubbleSleep = GetItem<Animator>("Root/BubbleSleep");
        BubbleSleep.gameObject.SetActive(false);
        InvokeRepeating("LoopShowBubble",0f,1f);
        var curTime = APIManager.Instance.GetServerTime();
        LastShowBubbleTime = curTime;
    }
    
    
    public void HideBubble()
    {
        Bubble.gameObject.SetActive(false);
        Bubble.transform.DOKill(false);
        LastShowBubbleTime = APIManager.Instance.GetServerTime();
        BubbleSleep.gameObject.SetActive(false);
        BubbleSleep.transform.DOKill(false);
    }
    public void ShowBubble()
    {
        if (CurState.Enum == KeepPetStateEnum.Sleep)
        {
            BubbleSleep.gameObject.SetActive(true);
            BubbleSleep.PlayAnimation("Appear");
            BubbleSleep.transform.DOKill(false);
            DOVirtual.DelayedCall(6f, () => BubbleSleep.gameObject.SetActive(false)).SetTarget(BubbleSleep.transform);
        }
        else
        {
            Bubble.gameObject.SetActive(true);
            Bubble.PlayAnimation("Appear");
            Bubble.transform.DOKill(false);
            DOVirtual.DelayedCall(6f, () => Bubble.gameObject.SetActive(false)).SetTarget(Bubble.transform);
            BubbleHappy.gameObject.SetActive(CurState.Enum == KeepPetStateEnum.Happy);
            BubbleHunger.gameObject.SetActive(CurState.Enum == KeepPetStateEnum.Hunger);   
        }
    }

    public void LoopShowBubble()
    {
        if (CurState.Enum != KeepPetStateEnum.Happy && CurState.Enum != KeepPetStateEnum.Hunger)
            return;
        var curTime = APIManager.Instance.GetServerTime();
        if (curTime - LastShowBubbleTime > ShowBubbleInterval)
        {
            LastShowBubbleTime = curTime;
            ShowBubble();
        }
    }
}