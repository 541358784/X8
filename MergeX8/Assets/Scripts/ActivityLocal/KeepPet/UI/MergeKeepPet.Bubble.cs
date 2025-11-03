using DG.Tweening;
using DragonU3DSDK.Network.API;
using UnityEngine;

public partial class MergeKeepPet
{
    private Animator Bubble;
    private Transform BubbleHappy;
    private Transform BubbleHunger;
    private Transform BubbleSteak;
    private Transform BubbleGift;
    private ulong LastShowBubbleTime;
    private ulong ShowBubbleInterval = XUtility.Second * 10;
    private KeepPetBaseState CurState => Storage.GetCurState();
    public void InitBubble()
    {
        Bubble = transform.Find("Root/Bubble").GetComponent<Animator>();
        Bubble.gameObject.SetActive(false);
        Bubble.transform.Find("Sleep").gameObject.SetActive(false);
        Bubble.transform.Find("Sick").gameObject.SetActive(false);
        BubbleHappy = Bubble.transform.Find("Happy");
        BubbleHunger = Bubble.transform.Find("Bone");
        BubbleSteak = Bubble.transform.Find("Steak");
        BubbleGift = Bubble.transform.Find("Gift");
        InvokeRepeating("LoopShowBubble",0f,1f);
        var curTime = APIManager.Instance.GetServerTime();
        LastShowBubbleTime = curTime;
    }
    
    
    public void HideBubble()
    {
        Bubble.gameObject.SetActive(false);
        Bubble.transform.DOKill(false);
        LastShowBubbleTime = APIManager.Instance.GetServerTime();
    }
    public void ShowBubble()
    {
        Bubble.gameObject.SetActive(true);
        Bubble.PlayAnimation("Appear");
        Bubble.transform.DOKill(false);
        DOVirtual.DelayedCall(6f, () => Bubble.gameObject.SetActive(false)).SetTarget(Bubble.transform);
        BubbleSteak.gameObject.SetActive(CurState.Enum == KeepPetStateEnum.Happy && Storage.SearchPropCount > 0);
        BubbleHappy.gameObject.SetActive(CurState.Enum == KeepPetStateEnum.Happy && Storage.SearchPropCount <= 0 && Storage.FrisbeeCount >= 5);
        BubbleHunger.gameObject.SetActive(CurState.Enum == KeepPetStateEnum.Hunger);
        BubbleGift.gameObject.SetActive(CurState.Enum == KeepPetStateEnum.SearchFinish);
    }

    public bool CanShowBubble()
    {
        var showSteakBubble = CurState.Enum == KeepPetStateEnum.Happy && Storage.SearchPropCount > 0;
        var showHappyBubble = CurState.Enum == KeepPetStateEnum.Happy && Storage.SearchPropCount <= 0 &&
                              Storage.FrisbeeCount >= 5;
        var showHungerBubble = CurState.Enum == KeepPetStateEnum.Hunger;
        var showGiftBubble = CurState.Enum == KeepPetStateEnum.SearchFinish;
        return showSteakBubble || showHappyBubble || showHungerBubble || showGiftBubble;
    }
    public void LoopShowBubble()
    {
        if (!CanShowBubble())
            return;
        var curTime = APIManager.Instance.GetServerTime();
        if (curTime - LastShowBubbleTime > ShowBubbleInterval)
        {
            LastShowBubbleTime = curTime;
            ShowBubble();
        }
    }
}