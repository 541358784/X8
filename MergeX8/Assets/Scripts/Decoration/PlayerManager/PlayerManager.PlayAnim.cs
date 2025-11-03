using System.Collections;
using System.Collections.Generic;
using Deco.Node;
using Decoration;
using UnityEngine;

public partial class PlayerManager
{
    private Dictionary<int, float> _animTime = new Dictionary<int, float>()
    {
        {888001, 11f},
        {888002, 8f},
        {888003, 2.5f},
    };

    private bool _isPlayAnim = false;
    
    public void PlayAnimation(DecoNode decoNode)
    {
        if(_isPlayAnim)
            return;

        _isPlayAnim = true;
        DecoManager.Instance.CurrentWorld.LookAtSuggestNodeBySpeed(decoNode, true, DecoManager.Instance.CurrentWorld.PinchMap.CurrentCameraScale, 10,
            () =>
            {
                if (decoNode.Id == 888003)
                {
                    var parent = decoNode._currentItem.GameObject.transform.Find("spine_Building/people");
                    var playerObj = GetPlayer(PlayerType.Chief);
                    playerObj.transform.SetParent(parent);
                    
                    playerObj = GetPlayer(PlayerType.Hero);
                    playerObj.transform.SetParent(parent);
                }
                
                SwitchPlayerStatus(PlayerType.Chief, StatusType.Fade, 0, -1, true, false, 0f);
                SwitchPlayerStatus(PlayerType.Dog, StatusType.Fade, 0, -1, true, false, 0f);
                SwitchPlayerStatus(PlayerType.Hero, StatusType.Fade, 0, -1, true, false, 0f);
                
                PlayerManager.Instance.PlayAnimation(decoNode, false, 0f, false, false);
                
                StopAllCoroutines();
                StartCoroutine(WaitPlayAnim(_animTime[decoNode.Id]));
            });
    }

    private IEnumerator WaitPlayAnim(float time)
    {
        float animTime = 0.2f;
        yield return new WaitForSeconds(time-animTime);

        SwitchPlayerStatus(PlayerType.Chief, StatusType.Fade, animTime, -1, false, true, animTime);
        SwitchPlayerStatus(PlayerType.Dog, StatusType.Fade, animTime, -1, false, true, animTime);
        SwitchPlayerStatus(PlayerType.Hero, StatusType.Fade, animTime, -1, false, true, animTime);
        
        yield return new WaitForSeconds(animTime);
        
        DecoNode decoNode = DecoManager.Instance.CurrentWorld.GetSuggestNode();
        
        UpdatePlayersFadeState(decoNode, 0.1f, true);
        
        SetPosition(decoNode, false);
        SetRotation(decoNode, false, false);
        SetActive(decoNode);
        _isPlayAnim = false;
    }
}