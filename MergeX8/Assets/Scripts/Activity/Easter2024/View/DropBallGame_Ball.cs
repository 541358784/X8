using System;
using DragonU3DSDK.Asset;
using TMPro;
using UnityEngine;

public partial class DropBallGame
{
    public class Ball : MonoBehaviour
    {
        public Rigidbody2D Rigidbody;
        public Easter2024BallType BallType;
        public int MultiValue = 1;
        public DropBallGame Game;
        private SpriteRenderer Icon;
        private TextMeshPro Text;
        private Vector3 DefaultScale;
        private void Awake()
        {
            var triggerTools = gameObject.AddComponent<ColliderTriggerTools>();
            triggerTools.RegisterCallback(OnTrigger);
            triggerTools.RegisterEndCallback(OnTriggerEnd);
            Rigidbody = gameObject.GetComponent<Rigidbody2D>();
            Icon = transform.Find("Image").GetComponent<SpriteRenderer>();
            Text = transform.Find("Text").GetComponent<TextMeshPro>();
            DefaultScale = transform.localScale;
        }

        public bool Simulated
        {
            get => Rigidbody.simulated;
            set => Rigidbody.simulated = value;
        }
        public void Init(DropBallGame game,Easter2024BallType ballType)
        {
            Game = game;
            BallType = ballType;
            UpdateUI();
        }

        public void UpdateUI()
        {
            Icon.sprite = GetIcon();
            Text.gameObject.SetActive(BallType == Easter2024BallType.Multi);
            Text.SetText("x"+MultiValue);
        }

        public Sprite GetIcon()
        {
            return ResourcesManager.Instance.GetSpriteVariant(AtlasName.Easter2024Atlas, GetAssetName());
        }
        public string GetAssetName()
        {
            if (BallType == Easter2024BallType.Normal)
                return "egg3";
            else if (BallType == Easter2024BallType.Extra)
                return "egg1";
            else if (BallType == Easter2024BallType.Multi)
                return "egg2";
            return "egg3";
        }
        public bool TriggerLuckyPoint = false;
        public int TriggerResult = -1;
        public bool IsEnterTriggerLuckyPoint = false;
#if UNITY_EDITOR
        public void ResetBallState()
        {
            TriggerLuckyPoint = false;
            TriggerResult = -1;
            IsEnterTriggerLuckyPoint = false;
            SamePositionCount = 0;
            UpdateCount = 0;
        }
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        public int LastBigBallScale = 1;
        public void Update()
        {
            if (LastBigBallScale == Easter2024Model.Instance.DebugBigBall)
            {
                return;
            }
            LastBigBallScale = Easter2024Model.Instance.DebugBigBall;
            transform.localScale = DefaultScale * LastBigBallScale;
        }
#endif
        public void OnTrigger(Collider2D other)
        {
            if (TriggerResult > 0)
                return;
            
            if (other == Game.LuckyGroupCollider)
            {
                if (!TriggerLuckyPoint && other.transform.position.y < transform.position.y)
                {
                    IsEnterTriggerLuckyPoint = true;
                }
                return;
            }
            
            for (var i = 0; i < Game.ResultColliderList.Count; i++)
            {
                if (other == Game.ResultColliderList[i])
                {
                    TriggerResult = i;
                    Game.TriggerResult(this,i);
                    return;
                }
            }
        }
        public void OnTriggerEnd(Collider2D other)
        {
            if (other == Game.LuckyGroupCollider)
            {
                if (IsEnterTriggerLuckyPoint && !TriggerLuckyPoint && other.transform.position.y > transform.position.y)
                {
                    TriggerLuckyPoint = true;
                    Game.TriggerLuckPoint(this);
                }
                IsEnterTriggerLuckyPoint = false;
            }
        }

        private Vector3 BallLastPosition;
        private int SamePositionCount = 0;
        private int UpdateCount = 0;
        public bool CheckSamePosition()
        {
            if (!Simulated)
                return false;
            UpdateCount++;
            if (BallLastPosition.Near(transform.localPosition,0.1f))
            {
                SamePositionCount++;
                if (SamePositionCount > 100)
                {
                    return true;
                }
            }
            else
            {
                BallLastPosition = transform.localPosition;
                SamePositionCount = 0;
            }
            if (UpdateCount > 6000)
            {
                return true;   
            }
            return false;
        }
    }
}