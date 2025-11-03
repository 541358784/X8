public class SimpleRollUpdaterEasingConfig
{
    public float speedUpDuration; //加速时长
    public float spinSpeed; //滚轴滚动速度
    public float slowDownDuration; //减速需要的时间
    public int slowDownStepCount; //减速阶段滚动的步数
    public float leastSpinDuration; //最少SPIN时间
    public float overShootAmount; //回弹量

    public SimpleRollUpdaterEasingConfig()
    {
        spinSpeed = 25f;
        speedUpDuration = 0.5f;
        slowDownDuration = 0.5f;
        leastSpinDuration = 2;
        slowDownStepCount = 3;
        overShootAmount = 1.5f;
    }
}