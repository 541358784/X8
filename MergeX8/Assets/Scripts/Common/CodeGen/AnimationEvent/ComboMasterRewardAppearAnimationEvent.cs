/// <summary>
/// ComboMasterReward_appear
/// </summary>
interface ComboMasterRewardAppearAnimationEvent : CodeGenAnimationEvent
{
    /// <summary>
    /// ComboMasterReward_appear *OnActivatedPlaySound*
    /// </summary>
    void OnActivatedPlaySound();

    /// <summary>
    /// ComboMasterReward_appear *OnSuccessPlaySound*
    /// </summary>
    void OnSuccessPlaySound();

    /// <summary>
    /// ComboMasterReward_appear *OnSkipDisable*
    /// </summary>
    void OnSkipDisable();
}