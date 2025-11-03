using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

namespace FishEatFishSpace
{
    public class Player : MonoBehaviour
    {
        [SerializeField] public BoxCollider playerBox;
        [SerializeField] Animator animator;
        [SerializeField] TextMeshPro hpTxt;
        [SerializeField] TextMeshPro rewardTxt;

        public Vector3 OriginPos { get; set; }
        protected virtual Transform BodyNode => transform;
        public RenderTexture targetTexture { get; set; }
        // [SerializeField] Renderer skin;

        int hp = 0;
        float size = 1f;

        void Start()
        {
            // skin.sortingLayerName = "Default";
            // skin.sortingOrder = 10;
            // originPos = transform.position;      
        }

        public Vector3 GetBoxSize()
        {
            return playerBox.size * size;;
        }

        public float Size
        {
            get { return size; }
            set
            {
                size = value;
                var lastLocalScale = transform.localScale;
                var newLocalScale = new Vector3(size, size, size);
                if (lastLocalScale.x < 0)
                {
                    newLocalScale.x *= -1;
                }
                if (lastLocalScale.y < 0)
                {
                    newLocalScale.y *= -1;
                }
                transform.localScale = newLocalScale;
            }
        }

        public int HP
        {
            get { return hp; }
            set
            {
                hp = value;
                hpTxt.SetText($"{hp}");
            }
        }

        public void GoEnemy(Vector3 target_pos, Action move_callback, Action eat_callback)
        {
            Vector3 direction = target_pos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Sequence goSeq = DOTween.Sequence();
            goSeq.SetTarget(transform);
            goSeq.AppendCallback(() =>
            {
                transform.DOLocalRotate(new Vector3(0, 0, angle), 0.5f);
                transform.DOMove(new Vector3(target_pos.x, target_pos.y, transform.position.z), 1.2f)
                    .SetEase(Ease.InCubic);
                DOVirtual.DelayedCall(0.7f, () => { animator.Play("attack"); });
            });
            goSeq.AppendInterval(1.1f);
            goSeq.AppendCallback(() => { move_callback(); });
            goSeq.AppendInterval(1.5f-1.2f);
            goSeq.AppendCallback(() => { eat_callback(); });
        }
        
        
        public void GoEnemyTwo(Enemy target, Action move_callback, Action eat_callback)
        {
            Sequence goSeq = DOTween.Sequence();
            goSeq.SetTarget(transform);
            goSeq.AppendCallback(() =>
            {
                var lastProgress = 0f;
                DOTween.To(() => 0f, progress =>
                {
                    var distance = target.transform.position - transform.position;
                    distance.z = 0;
                    ChangeDirection(distance);
                
                    var percent = (progress - lastProgress) / (1 - lastProgress);
                    transform.position += distance * percent;
                    lastProgress = progress;
                }, 1f, 1.2f).OnComplete(() =>
                {
                    transform.position = target.transform.position;
                    target.Speed = Vector3.zero;
                }).SetTarget(transform).SetEase(Ease.InCubic);
                DOVirtual.DelayedCall(0.7f, () => { animator.Play("attack"); });
            });
            goSeq.AppendInterval(1.1f);
            goSeq.AppendCallback(() => { move_callback(); });
            goSeq.AppendInterval(1.5f-1.2f);
            goSeq.AppendCallback(() => { eat_callback(); });
        }

        public void ChangeDirection(Vector3 distance)
        {
            var xPositive = true;
            float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
            if (angle > 90)
            {
                angle -= 180;
                xPositive = false;
            }
            else if (angle < -90)
            {
                angle += 180;
                xPositive = false;
            }
            transform.localRotation = Quaternion.Euler(0, 0, angle);
            if (xPositive != BodyNode.localScale.x > 0)
            {
                var tempScale = BodyNode.localScale;
                tempScale.x *= -1;
                BodyNode.localScale = tempScale;

                tempScale = hpTxt.transform.localScale;
                tempScale.x *= -1;
                hpTxt.transform.localScale = tempScale;
                        
                tempScale = rewardTxt.transform.localScale;
                tempScale.x *= -1;
                rewardTxt.transform.localScale = tempScale;

                textScaleX *= -1;
            }
        }

        private int textScaleX = 1;
        public void Win(int hp, Action win_callback)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            OriginPos = transform.localPosition;
            animator.Play("jump");
            rewardTxt.SetText($"+{hp}");
            Sequence winSeq = DOTween.Sequence();
            winSeq.AppendCallback(() =>
            {
                rewardTxt.transform.localPosition = new Vector3(0.5f, 0.5f, 0f);
                rewardTxt.gameObject.SetActive(true);
            });
            winSeq.AppendInterval(0.2f);
            winSeq.AppendCallback(() =>
            {
                rewardTxt.transform.DOLocalMoveX(hpTxt.transform.localPosition.x, 0.5f).OnComplete(() =>
                {
                    rewardTxt.gameObject.SetActive(false);
                });
            });
            winSeq.AppendInterval(0.3f);
            winSeq.AppendCallback(() => { HP += hp; });
            winSeq.Append(hpTxt.transform.DOScale(new Vector3(0.25f, 0.25f, 0.25f), 0.2f).SetEase(Ease.Linear).OnUpdate(
                () =>
                {
                    if (textScaleX < 0 && hpTxt.transform.localScale.x > 0 || 
                        textScaleX > 0 && hpTxt.transform.localScale.x < 0)
                    {
                        var tempScale = hpTxt.transform.localScale;
                        tempScale.x *= -1;
                        hpTxt.transform.localScale = tempScale;
                    }
                }).OnComplete(() =>
            {
                if (textScaleX < 0 && hpTxt.transform.localScale.x > 0 || 
                    textScaleX > 0 && hpTxt.transform.localScale.x < 0)
                {
                    var tempScale = hpTxt.transform.localScale;
                    tempScale.x *= -1;
                    hpTxt.transform.localScale = tempScale;
                }
            }));
            winSeq.Append(hpTxt.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f).SetEase(Ease.Linear).OnUpdate(
                () =>
                {
                    if (textScaleX < 0 && hpTxt.transform.localScale.x > 0 || 
                        textScaleX > 0 && hpTxt.transform.localScale.x < 0)
                    {
                        var tempScale = hpTxt.transform.localScale;
                        tempScale.x *= -1;
                        hpTxt.transform.localScale = tempScale;
                    }
                }).OnComplete(() =>
            {
                if (textScaleX < 0 && hpTxt.transform.localScale.x > 0 || 
                    textScaleX > 0 && hpTxt.transform.localScale.x < 0)
                {
                    var tempScale = hpTxt.transform.localScale;
                    tempScale.x *= -1;
                    hpTxt.transform.localScale = tempScale;
                }
            }));
            winSeq.OnComplete(() => { win_callback(); });
        }

        public void Bubble(float multi, Action win_callback)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            OriginPos = transform.localPosition;
            animator.Play("jump");
            // rewardTxt.SetText($"+{hp}");
            Sequence winSeq = DOTween.Sequence();
            winSeq.AppendCallback(() => { HP = (int)(HP * multi); });
            winSeq.Append(hpTxt.transform.DOScale(new Vector3(0.25f, 0.25f, 0.25f), 0.2f).SetEase(Ease.Linear).OnUpdate(
                () =>
                {
                    if (textScaleX < 0 && hpTxt.transform.localScale.x > 0 || 
                        textScaleX > 0 && hpTxt.transform.localScale.x < 0)
                    {
                        var tempScale = hpTxt.transform.localScale;
                        tempScale.x *= -1;
                        hpTxt.transform.localScale = tempScale;
                    }
                }).OnComplete(() =>
            {
                if (textScaleX < 0 && hpTxt.transform.localScale.x > 0 || 
                    textScaleX > 0 && hpTxt.transform.localScale.x < 0)
                {
                    var tempScale = hpTxt.transform.localScale;
                    tempScale.x *= -1;
                    hpTxt.transform.localScale = tempScale;
                }
            }));
            winSeq.Append(hpTxt.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f).SetEase(Ease.Linear).OnUpdate(
                () =>
                {
                    if (textScaleX < 0 && hpTxt.transform.localScale.x > 0 || 
                        textScaleX > 0 && hpTxt.transform.localScale.x < 0)
                    {
                        var tempScale = hpTxt.transform.localScale;
                        tempScale.x *= -1;
                        hpTxt.transform.localScale = tempScale;
                    }
                }).OnComplete(() =>
            {
                if (textScaleX < 0 && hpTxt.transform.localScale.x > 0 || 
                    textScaleX > 0 && hpTxt.transform.localScale.x < 0)
                {
                    var tempScale = hpTxt.transform.localScale;
                    tempScale.x *= -1;
                    hpTxt.transform.localScale = tempScale;
                }
            }));
            winSeq.OnComplete(() => { win_callback(); });
        }

        public void OpenBox()
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            animator.Play("jump");
        }
        public void Lose(Action lose_callback)
        {
            // transform.localPosition = OriginPos;
            // transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            // animator.Play("stand");
            // lose_callback(targetTexture);
            lose_callback();
        }

        public void Reset()
        {
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            animator.Play("stand");
        }
    }
}