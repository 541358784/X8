using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public partial class UIKapiScrewMainController
{
    private ScoreBoard MyScoreBoard;
    private ScoreBoard EnemyScoreBoard;

    public void InitScoreBoard()
    {
        MyScoreBoard = transform.Find("Root/MiddleGroup/ContestItem/FractionGroup/MineItem").gameObject
            .AddComponent<ScoreBoard>();
        MyScoreBoard.Init(Storage.SmallLevel);
        EnemyScoreBoard = transform.Find("Root/MiddleGroup/ContestItem/FractionGroup/PlayerItem").gameObject
            .AddComponent<ScoreBoard>();
        EnemyScoreBoard.Init(Storage.SmallLevel);
    }

    public void UpdateScore(bool isWin)
    {
        if (isWin)
        {
            if (Storage.SmallLevel == 0 || KapiScrewModel.Instance.IsFinished()) //通关
            {
                MyScoreBoard.SetValue(Storage.PlayingSmallLevel+10);
                EnemyScoreBoard.SetValue(Storage.PlayingSmallLevel);
            }
            else
            {
                MyScoreBoard.SetValue(Storage.SmallLevel);
                EnemyScoreBoard.SetValue(Storage.SmallLevel);
            }
        }
        else
        {
            MyScoreBoard.SetValue(Storage.PlayingSmallLevel);
            EnemyScoreBoard.SetValue(Storage.PlayingSmallLevel + 10);
        }
    }
    public class ScoreBoard : MonoBehaviour
    {
        private Animator Anim1;
        private Animator Anim2;
        private List<Image> OldText1 = new List<Image>();
        private List<Image> NewText1 = new List<Image>();
        private List<Image> OldText2 = new List<Image>();
        private List<Image> NewText2 = new List<Image>();
        private bool IsAwake = false;
        public int ScoreValue;
        private void Awake()
        {
            if (IsAwake)
                return;
            IsAwake = true;
            Anim1 = transform.Find("01").GetComponent<Animator>();
            Anim2 = transform.Find("02").GetComponent<Animator>();
            OldText1.Add(transform.Find("01/Text1_chushi").GetComponent<Image>());
            OldText1.Add(transform.Find("01/Ani_chushi_1/Root/Text1").GetComponent<Image>());
            OldText1.Add(transform.Find("01/Ani_chushi_2/Root/Text1").GetComponent<Image>());
            NewText1.Add(transform.Find("01/Text1_zuizhong").GetComponent<Image>());
            NewText1.Add(transform.Find("01/Ani_zuizhong_1/Root/Text1").GetComponent<Image>());
            NewText1.Add(transform.Find("01/Ani_zuizhong_2/Root/Text1").GetComponent<Image>());
            OldText2.Add(transform.Find("02/Text1_chushi").GetComponent<Image>());
            OldText2.Add(transform.Find("02/Ani_chushi_1/Root/Text1").GetComponent<Image>());
            OldText2.Add(transform.Find("02/Ani_chushi_2/Root/Text1").GetComponent<Image>());
            NewText2.Add(transform.Find("02/Text1_zuizhong").GetComponent<Image>());
            NewText2.Add(transform.Find("02/Ani_zuizhong_1/Root/Text1").GetComponent<Image>());
            NewText2.Add(transform.Find("02/Ani_zuizhong_2/Root/Text1").GetComponent<Image>());
        }

        public void Init(int value)
        {
            Awake();
            ScoreValue = value;
            var num2Value = ScoreValue % 10;
            var leftNum = ScoreValue / 10;
            var num1Value = leftNum % 10;
            var num2Sprite = GetNumSprite(num2Value);
            var num1Sprite = GetNumSprite(num1Value);
            foreach (var image in OldText1)
            {
                image.sprite = num1Sprite;
            }
            foreach (var image in NewText1)
            {
                image.sprite = num1Sprite;
            }
            foreach (var image in OldText2)
            {
                image.sprite = num2Sprite;
            }
            foreach (var image in NewText2)
            {
                image.sprite = num2Sprite;
            }
            Anim1.PlayAnimation("normal");
            Anim2.PlayAnimation("normal");
        }

        public void SetValue(int value)
        {
            var num2ValueLast = ScoreValue % 10;
            var leftNumLast = ScoreValue / 10;
            var num1ValueLast = leftNumLast % 10;
            ScoreValue = value;
            var num2Value = ScoreValue % 10;
            var leftNum = ScoreValue / 10;
            var num1Value = leftNum % 10;
            if (num2ValueLast != num2Value)
            {
                var num2Sprite = GetNumSprite(num2Value);
                foreach (var image in NewText2)
                {
                    image.sprite = num2Sprite;
                }
                Anim2.PlayAnimation("change", () =>
                {
                    foreach (var image in OldText2)
                    {
                        image.sprite = num2Sprite;
                    }
                    Anim2.PlayAnimation("normal");
                });
            }
            if (num1ValueLast != num1Value)
            {
                var num1Sprite = GetNumSprite(num1Value);
                foreach (var image in NewText1)
                {
                    image.sprite = num1Sprite;
                }
                Anim1.PlayAnimation("change", () =>
                {
                    foreach (var image in OldText1)
                    {
                        image.sprite = num1Sprite;
                    }
                    Anim1.PlayAnimation("normal");
                });
            }
        }

        public Sprite GetNumSprite(int num)
        {
            return ResourcesManager.Instance.GetSpriteVariant("KapiScrewAtlas", num.ToString());
        }
    }
}