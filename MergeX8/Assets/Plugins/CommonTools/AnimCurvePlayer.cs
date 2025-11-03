using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// 脚本编辑：通过动画曲线让特效控制目标物体从起始位置运动到终点位置的过程变化
/// 
/// </summary>

public class AnimCurvePlayer : MonoBehaviour
{ 

    public  AnimationCurve TimeAnimationCurve       ; //创建动画曲线
    public  Transform      targetTransform          ; //需要被查找位置的目标物体
    private Vector3        initiallyPosition        ; //准备播放飞行动画的物体的位置


    [SerializeField] private float playTimer = 0    ; //计时
    [SerializeField] private bool  isPlay    = false; //是否进行播放
    [SerializeField] private bool  isPlaying = false; //当前是否进行播放

    private void Start()
    {
        isPlay    = false;
        isPlaying = false;
        initiallyPosition = this.transform.position;  //获取准备播放飞行动画的物体的初始位置

    }

    private void Update()
    {
        //如果勾选播放选项，就执行播放函数
        if (isPlay)
        {
            pathCurveAinmationPlay();
        }
        else
        {
            isPlaying = false;
            playTimer = 0.0f;
        }
        
    }

    /// <summary>
    /// 物体播放飞行动画的函数
    /// </summary>
    private void pathCurveAinmationPlay()
    {
        isPlaying = true;

        float DeltaThisPos2TargetPosDis = Vector3.Distance(targetTransform.position , initiallyPosition); //获取被飞行物体到目标物体的距离的变化量

        //判断：如果距离变化量 > 0.05f，就表示两者之间有一定的距离，所以执行飞行动画；
        //     否则则说明两者之间的距离十分接近，直接移动过去就好
        if (DeltaThisPos2TargetPosDis > 0.05f)
        {
            playTimer += Time.deltaTime;

            //0.5秒内到达AnimationCurve曲线Y轴最大值1（x轴 y轴最大值我设置的都是1）
            this.transform.position = Vector3.Lerp(initiallyPosition, targetTransform.position, TimeAnimationCurve.Evaluate(playTimer));
        }
        else
        {
            this.transform.position = targetTransform.position;
        }

        if (this.transform.position == targetTransform.position)
        {
            isPlay    = false;
            isPlaying = false;
            playTimer = 0.0f;
        }
    }
}
