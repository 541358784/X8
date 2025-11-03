using System;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyMainController
{
    public class PlayerNode:MonoBehaviour
    {
        Animator Animtor;
        public int CurBlockIndex;
        private RectTransform HeadIconRoot;
        private HeadIconNode HeadIcon;

        private void Awake()
        {
            Animtor = transform.parent.GetComponent<Animator>();
            HeadIconRoot = transform.Find("Head") as RectTransform;
            HeadIcon = HeadIconNode.BuildMyHeadIconNode(HeadIconRoot);
        }

        public void SetBlockIndex(int index)
        {
            CurBlockIndex = index;
        }
    }
}