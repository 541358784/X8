using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FishEatFishSpace
{


    public class ArrowLine : MonoBehaviour
    {
        [SerializeField] SpriteRenderer arrow;

        [SerializeField] SpriteRenderer hand;
        // [SerializeField] Transform hand;

        Camera fishCamera;
        Transform playerTrans;
        Vector3 touchBeginPos;
        float scale = 1f;
        float ppu = 1f;
        Sequence tipSeq;

        public void Init(float _scale, float _ppu, Transform _player, Camera _camera)
        {
            // Debug.Log($"{_scale},{_ppu}");
            hand.color = new Color(1f, 1f, 1f, 0f);
            playerTrans = _player;
            scale = _scale;
            ppu = _ppu;
            fishCamera = _camera;
        }

        public void StartDraw()
        {
            transform.position = playerTrans.position;
            touchBeginPos = fishCamera.WorldToScreenPoint(transform.position);
        }

        public void Draw(Vector3 touch_pos)
        {
            Vector3 direction = touch_pos - touchBeginPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            arrow.transform.localEulerAngles = new Vector3(0, 0, angle);
            arrow.size = new Vector2(0.55f, new Vector2(direction.x, direction.y).magnitude / ppu / scale);
        }

        public void ShowTip(Vector3 enemy_pos)
        {
            StartDraw();
            Vector3 target_pos = new Vector3(enemy_pos.x, enemy_pos.y, -1f);
            tipSeq = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    hand.transform.localPosition = new Vector3(0, 0, -1f);
                    arrow.color = new Color(1f, 1f, 1f, 1f);
                    arrow.size = new Vector2(0.55f, 0);
                })
                .Append(hand.DOFade(1f, 0.5f))
                .Append(hand.transform.DOMove(target_pos, 1.5f).SetEase(Ease.InOutSine).OnUpdate(() =>
                {
                    Vector2 direction = new Vector2(hand.transform.localPosition.x, hand.transform.localPosition.y);
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                    arrow.transform.localEulerAngles = new Vector3(0, 0, angle);
                    arrow.size = new Vector2(0.55f, direction.magnitude);
                }))
                .AppendInterval(0.3f)
                .Append(DOVirtual.Float(1f, 0f, 0.5f, (value) =>
                {
                    hand.color = new Color(1f, 1f, 1f, value);
                    arrow.color = new Color(1f, 1f, 1f, value);
                }))
                .AppendInterval(1f)
                .SetLoops(-1);

        }

        public void Reset()
        {
            arrow.size = new Vector2(0.55f, 0);
        }

        public void HideTip()
        {
            if (tipSeq != null)
            {
                tipSeq.Kill();
            }

            hand.transform.localPosition = new Vector3(0, 0, 0);
            hand.color = new Color(1, 1, 1, 0);
            arrow.color = new Color(1f, 1f, 1f, 1f);
            arrow.size = new Vector2(0.55f, 0);
        }
    }
}
