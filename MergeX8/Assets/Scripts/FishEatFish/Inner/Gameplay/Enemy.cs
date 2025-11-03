using UnityEngine;
using TMPro;

namespace FishEatFishSpace
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected BoxCollider enemyBox;
        [SerializeField] protected Animator animator;
        [SerializeField] protected TextMeshPro hpTxt;
        protected virtual Transform BodyNode => transform;
        protected int hp;
        protected float size;

        public Vector3 speed;
        public virtual Vector3 Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                if (speed.x != 0 && 
                (speed.x < 0 != BodyNode.localScale.x > 0))
                {
                    var tempScale = BodyNode.localScale;
                    tempScale.x *= -1;
                    BodyNode.localScale = tempScale;
                    
                    tempScale = hpTxt.transform.localScale;
                    tempScale.x *= -1;
                    hpTxt.transform.localScale = tempScale;
                }
            }
        }

        public virtual Vector3 GetBoxSize()
        {
            return enemyBox.size * size;
        }

        public virtual float Size
        {
            get { return size; }
            set
            {
                size = value;
                transform.localScale = new Vector3(size, size, size);
            }
        }
        public virtual int HP
        {
            get { return hp; }
            set
            {
                hp = value;
                hpTxt.SetText($"{hp}");
            }
        }
    }
}