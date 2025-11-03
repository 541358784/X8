using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine.UI;

public partial class UIKapibalaMainController
{
    [RequireComponent(typeof(RectTransform))]
    public class IndependentBee : MonoBehaviour
    {
        public SkeletonGraphic Spine;
        public bool IsPause;

        public void Pause()
        {
            IsPause = true;
        }

        public void Consume()
        {
            IsPause = false;
        }
        public void Attack()
        {
            Spine.PlaySkeletonAnimation("attack",true);
        }
        public void StopAttack()
        {
            Spine.PlaySkeletonAnimation("idle",true);
        }
        public void ChangeOrbitCenter(Transform newCenter)
        {
            orbitCenter = newCenter;
    
            // 重置参数使过渡更自然
            InitializeParameters();
        }
        [Header("环绕设置")] public Transform orbitCenter;
        [Tooltip("基础环绕半径（像素）")] public float baseOrbitRadius = 0.5f;
        [Tooltip("半径变化范围")] [Range(0f, 1f)] public float radiusVariation = 0f;
        [Tooltip("环绕速度")] [Range(0.5f, 5f)] public float baseSpeed = 2f;
        [Tooltip("速度变化范围")] [Range(0.1f, 2f)] public float speedVariation = 0.5f;

        [Header("运动参数")] [Tooltip("随机变化间隔")] [Range(0.1f, 1f)]
        public float randomChangeInterval = 0.3f;

        [Tooltip("运动平滑度（值越大越平滑）")] [Range(1f, 15f)]
        public float smoothFactor = 2f;

        [Tooltip("垂直波动幅度")] [Range(0f, 1f)] public float verticalAmplitude = 0f;
        [Tooltip("垂直波动速度")] [Range(0.5f, 3f)] public float verticalSpeed = 1.5f;
        
        [Tooltip("随机偏移范围")] [Range(0f, 1f)] public float randomOffsetRange = 0.5f;
        // 运动参数
        private float currentRadius;
        private float currentAngle;
        private float currentSpeed;
        private float nextChangeTime;
        private Vector2 randomOffset;
        private Vector2 currentVelocity;
        private float verticalOffset;

        // 方向控制
        private bool facingRight = true;

        void Start()
        {
            Spine = transform.gameObject.GetComponent<SkeletonGraphic>();
            Spine.PlaySkeletonAnimation("idle",true);
            InitializeParameters();

            // 初始随机位置
            if (orbitCenter != null)
            {
                Vector2 randomPos = (Vector2)orbitCenter.position +
                                    Random.insideUnitCircle * baseOrbitRadius;
                transform.position = randomPos;
            }
        }

        void InitializeParameters()
        {
            // 随机初始角度
            currentAngle = Random.Range(0f, Mathf.PI * 2f);

            // 随机半径
            currentRadius = baseOrbitRadius + Random.Range(-radiusVariation, radiusVariation);

            // 随机速度
            currentSpeed = baseSpeed + Random.Range(-speedVariation, speedVariation);

            // 随机偏移
            randomOffset = new Vector2(Random.Range(-randomOffsetRange, randomOffsetRange),Random.Range(-randomOffsetRange, randomOffsetRange));

            // 随机垂直波动偏移
            verticalOffset = Random.Range(0f, 100f);

            // 设置首次变化时间
            nextChangeTime = Time.time + Random.Range(0.1f, randomChangeInterval);

            // 初始随机朝向
            facingRight = Random.value > 0.5f;
            UpdateDirection();
        }

        void Update()
        {
            if (IsPause)
                return;
            if (orbitCenter == null) return;

            // 定期更新随机参数
            if (Time.time >= nextChangeTime)
            {
                UpdateRandomParameters();
                nextChangeTime = Time.time + randomChangeInterval + Random.Range(-0.1f, 0.1f);
            }

            // 计算目标位置
            Vector2 targetPosition = CalculateOrbitPosition();

            // 平滑移动
            transform.position = Vector2.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                1f / smoothFactor
            );

            // 更新朝向
            UpdateDirection();
        }

        void UpdateDirection()
        {
            return;
            // 根据水平速度方向设置朝向
            if (transform.position.x > orbitCenter.position.x && !facingRight)
            {
                facingRight = true;
                ApplyDirection();
            }
            else if (transform.position.x < orbitCenter.position.x && facingRight)
            {
                facingRight = false;
                ApplyDirection();
            }
        }

        void ApplyDirection()
        {
            // 使用scaleX控制左右朝向
            var scaleOld = transform.localScale;
            var scaleOldX = scaleOld.x;
            if (scaleOldX < 0)
                scaleOldX = -scaleOldX;
            transform.localScale = new Vector3((facingRight ? 1 : -1)*scaleOldX, scaleOld.y, scaleOld.z);
        }

        Vector2 CalculateOrbitPosition()
        {
            // 更新角度
            currentAngle += currentSpeed * Time.deltaTime;

            // 添加垂直波动
            float verticalWave = Mathf.Sin(Time.time * verticalSpeed + verticalOffset) * verticalAmplitude;

            // 计算基础轨道位置
            Vector2 centerPos = orbitCenter.position;
            Vector2 orbitPos = centerPos + new Vector2(
                Mathf.Cos(currentAngle) * currentRadius,
                Mathf.Sin(currentAngle) * currentRadius * 0.7f + verticalWave
            );

            // 添加随机偏移
            orbitPos += randomOffset;

            return orbitPos;
        }

        void UpdateRandomParameters()
        {
            // 随机变化半径
            currentRadius = Mathf.Clamp(
                currentRadius + Random.Range(- radiusVariation, radiusVariation),
                baseOrbitRadius - radiusVariation,
                baseOrbitRadius + radiusVariation
            );

            // 随机变化速度
            currentSpeed = Mathf.Clamp(
                currentSpeed + Random.Range(- speedVariation, speedVariation),
                baseSpeed - speedVariation,
                baseSpeed + speedVariation
            );

            // 微调随机偏移
            randomOffset = new Vector2(Random.Range(-randomOffsetRange, randomOffsetRange),Random.Range(-randomOffsetRange, randomOffsetRange));

            // 随机变化垂直波动参数
            verticalSpeed = Mathf.Clamp(verticalSpeed + Random.Range(-0.2f, 0.2f), 0.5f, 3f);
        }

        // 在编辑器中可视化轨道范围
#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (orbitCenter == null) return;

            // 绘制中心点
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(orbitCenter.position, 10f);

            // 绘制轨道范围
            Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.2f);
            float minRadius = baseOrbitRadius - radiusVariation;
            float maxRadius = baseOrbitRadius + radiusVariation;
            Gizmos.DrawWireSphere(orbitCenter.position, minRadius);
            Gizmos.DrawWireSphere(orbitCenter.position, maxRadius);

            // 绘制当前轨道位置
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(CalculateOrbitPosition(), 5f);
            }
        }
#endif
    }
}