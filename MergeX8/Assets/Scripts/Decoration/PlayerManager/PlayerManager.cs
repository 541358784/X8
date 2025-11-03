using System;
using System.Collections.Generic;
using Deco.Node;
using Decoration;
using Decoration.Player;
using DG.Tweening;
using UnityEngine;


public partial class PlayerManager : Manager<PlayerManager>
{
    public enum StatusType
    {
        None,
        Fade,
        AutoFade,
        PlayAnim,
        Idle,
    }

    public enum PlayerType
    {
        None = -1,
        Chief,
        Dog,
        Hero,
        Count
    }

    public enum ActionName
    {
        Idle,
        Happy,
        Walk,
        Sit,
        Surprise,
        Bbq,
    }

    private string[] playerPath = new[] { "Prefabs/PortraitSpine/NvZhu", "Prefabs/PortraitSpine/Dog","Prefabs/PortraitSpine/NanZhu" };

    private Dictionary<PlayerType, GameObject> _players = null;

    private Dictionary<PlayerType, Dictionary<ActionName, string>> _actionNames =
        new Dictionary<PlayerType, Dictionary<ActionName, string>>()
        {
            {
                PlayerType.Chief, new Dictionary<ActionName, string>()
                {
                    { ActionName.Idle, "stand" },
                    { ActionName.Happy, "happy" },
                    { ActionName.Walk, "walk" },
                    { ActionName.Sit, "weak02" },
                    { ActionName.Surprise, "surprised" },
                    { ActionName.Bbq, "bbq" },
                }
            },

            {
                PlayerType.Dog, new Dictionary<ActionName, string>()
                {
                    { ActionName.Idle, "idle" },
                    { ActionName.Happy, "idle2" },
                    { ActionName.Walk, "run" },
                    { ActionName.Sit, "idle2" },
                    { ActionName.Surprise, "idle2" },
                    { ActionName.Bbq, "idle" },
                }
            },

            {
                PlayerType.Hero, new Dictionary<ActionName, string>()
                {
                    { ActionName.Idle, "stand" },
                    { ActionName.Happy, "idle2" },
                    { ActionName.Walk, "walk" },
                    { ActionName.Sit, "idle2" },
                    { ActionName.Surprise, "idle" },
                    { ActionName.Bbq, "idle" },
                }
            },
        };

    private Dictionary<PlayerType, Animator> _animators = new Dictionary<PlayerType, Animator>();
    private Dictionary<PlayerType, List<Material>> _materials = new Dictionary<PlayerType, List<Material>>();
    private Dictionary<PlayerType, SpriteRenderer> _spriteRenderers = new Dictionary<PlayerType, SpriteRenderer>();
    private Dictionary<PlayerType, PlayerStatusBase> _playerStatus = new Dictionary<PlayerType, PlayerStatusBase>();

    public void InitPlayers()
    {
        if (_players != null)
            return;

        _players = new Dictionary<PlayerType, GameObject>();

        var parent = DecoSceneRoot.Instance.mRoot.transform;
        for (var i = PlayerType.Chief; i < PlayerType.Count; i++)
        {
            var obj = DragonU3DSDK.Asset.ResourcesManager.Instance.LoadResource<GameObject>(playerPath[(int)i]);
            if (obj == null)
                continue;

            var cloneObj = Instantiate(obj, parent);
            _players.Add(i, cloneObj);
            cloneObj.transform.localScale = Vector3.one;
            cloneObj.transform.localPosition = Vector3.zero;
            cloneObj.gameObject.SetActive(false);

            _animators.Add(i, cloneObj.gameObject.GetComponentInChildren<Animator>());
            var renders = cloneObj.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            _materials.Add(i, new List<Material>());
            foreach (var render in renders)
            {
                _materials[i].AddRange(render.materials);
            }

            _spriteRenderers.Add(i, cloneObj.gameObject.GetComponentInChildren<SpriteRenderer>());
        }
    }

    public void UnLoad()
    {
        if (_players == null)
            return;
        
        foreach (var player in _players)
        {
            DestroyImmediate(player.Value.gameObject);
        }
        
        _players.Clear();
        _animators.Clear();
        _materials.Clear();
        _spriteRenderers.Clear();
        _playerStatus.Clear();
        _players = null;
    }

    public void AnimShowPlayer(PlayerType type, float animTime = 1f, bool isAnim = true)
    {
        SwitchPlayerStatus(type, StatusType.Fade, 0.1f, -1, true, isAnim, animTime);
    }

    public void PlayAnimation(DecoNode decoNode, bool isDeco, float crossTime = 0.1f, bool isAnim = true, bool isRestParent = true)
    {
        StopAllCoroutines();
        SetActive(decoNode, isRestParent);
        
        if (decoNode.Config.npcConfigId <= 0)
            return;

        var npcConfig = DecorationConfigManager.Instance.NpcConfigList.Find(a => a.id == decoNode.Config.npcConfigId);
        if (npcConfig == null)
            return;

        PlayAnimation(PlayerType.Chief, npcConfig.chiefInfoId, isDeco, crossTime);
        PlayAnimation(PlayerType.Hero, npcConfig.heroInfoId, isDeco, crossTime);
        PlayAnimation(PlayerType.Dog, npcConfig.dogInfoId, isDeco, crossTime);
        
        SetPosition(decoNode, isDeco);
        SetRotation(decoNode, isDeco, isAnim);
    }
    
    public void UpdatePlayersState(bool isAnim = true)
    {
        _isPlayAnim = false;
        StopAllCoroutines();
        DecoNode decoNode = DecoManager.Instance.CurrentWorld.GetSuggestNode();

        UpdatePlayersState(decoNode, 0f, isAnim);
    }

    public string GetAnimationName(PlayerType type, ActionName actionName)
    {
        if (!_actionNames.ContainsKey(type))
            return null;

        if (!_actionNames[type].ContainsKey(actionName))
            return null;

        return _actionNames[type][actionName];
    }


    public void SwitchPlayerStatusIdle(DecoNode decoNode)
    {
        SwitchPlayerStatus(PlayerType.Chief, StatusType.Idle);
        SwitchPlayerStatus(PlayerType.Dog, StatusType.Idle);
        SwitchPlayerStatus(PlayerType.Hero, StatusType.Idle);  
        
        SetPosition(decoNode, false);
        SetRotation(decoNode, false);
    }

    public void StopPlayerStatus()
    {
        StopPlayerStatus(PlayerType.Chief);
        StopPlayerStatus(PlayerType.Dog);
        StopPlayerStatus(PlayerType.Hero);
    }

    public void StopPlayerStatus(PlayerType type)
    {
        PlayerStatusBase statusBase = GetPlayerStatus(type);
        if (statusBase == null)
            return;
        
        statusBase.OnStop();
        _playerStatus.Remove(type);
    }

    public void SwitchPlayerStatus(PlayerType type, StatusType statusType, float crossTime = 0.1f, int npcConfigId = -1, params object[] param)
    {
        PlayerStatusBase statusBase = GetPlayerStatus(type);
        if (statusBase != null)
        {
            //女主坐着动画特殊处理
            if (type == PlayerType.Chief)
            {
                if (statusType == StatusType.Idle)
                {
                    if (statusBase._statusType == StatusType.AutoFade)
                    {
                        if (statusBase._npcInfo != null && statusBase._npcInfo.animName ==
                            GetAnimationName(type, PlayerManager.ActionName.Sit))
                            return;
                    }
                    else if(statusBase._statusType == StatusType.PlayAnim)
                    {
                        string animName = ((PlayerStatusPlayAnim)statusBase)._animName;
                        if (animName.IsEmptyString() || animName == GetAnimationName(type, PlayerManager.ActionName.Sit))
                            return;
                    }
                }
            }
            statusBase.OnStop();
            _playerStatus.Remove(type);
        }

        statusBase = null;
        switch (statusType)
        {
            case StatusType.Fade:
            {
                statusBase = new PlayerStatusFade();
                break;
            } 
            case StatusType.AutoFade:
            {
                statusBase = new PlayerStatusAutoFade();
                break;
            }
            case StatusType.Idle:
            {
                statusBase = new PlayerStatusIdle();
                break;
            }
            case StatusType.PlayAnim:
            {
                statusBase = new PlayerStatusPlayAnim();
                break;
            }
        }

        if (statusBase == null)
            return;

        statusBase._statusType = statusType;
        statusBase.OnInit(type, npcConfigId, crossTime, param);
        _playerStatus.Add(type, statusBase);
        statusBase.OnStart();
    }

    public PlayerStatusBase GetPlayerStatus(PlayerType type)
    {
        if (!_playerStatus.ContainsKey(type))
            return null;

        return _playerStatus[type];
    }

    public GameObject GetPlayer(PlayerType type)
    {
        if (!_players.ContainsKey(type))
            return null;

        return _players[type];
    }

    public Animator GetAnimator(PlayerType type)
    {
        if (!_animators.ContainsKey(type))
            return null;

        return _animators[type];
    }

    public List<Material> GetMaterials(PlayerType type)
    {
        if (!_materials.ContainsKey(type))
            return null;

        return _materials[type];
    }

    public SpriteRenderer GetSpriteRenderer(PlayerType type)
    {
        if (!_spriteRenderers.ContainsKey(type))
            return null;

        return _spriteRenderers[type];
    }

    private void PlayAnimation(PlayerType type, int npcInfoId, bool isDeco, float crossTime = 0.1f)
    {
        var npcInfo = DecorationConfigManager.Instance.NpcInfoList.Find(a => a.id == npcInfoId);
        if (npcInfo == null)
            return;

        SwitchPlayerStatus(type, StatusType.PlayAnim, crossTime, -1, isDeco ? npcInfo.decoAnimName : npcInfo.finishAnimName,  isDeco ? npcInfo.playAnimNum : 1);
    }

    public void UpdatePlayersState(DecoNode decoNode, float crossTime, bool isAnim = true)
    {
        if (decoNode == null)
        {
            SwitchPlayerStatus(PlayerType.Chief, StatusType.AutoFade, crossTime, 30, true, isAnim, 0.5f);
            SwitchPlayerStatus(PlayerType.Dog, StatusType.AutoFade, crossTime, 30, true, isAnim, 0.5f);
            SwitchPlayerStatus(PlayerType.Hero, StatusType.AutoFade, crossTime, 30, true, isAnim, 0.5f);
            return;
        }

        if (decoNode.Config.npcConfigId <= 0)
            return;

        SwitchPlayerStatus(PlayerType.Chief, StatusType.AutoFade, crossTime, decoNode.Config.npcConfigId, true, isAnim, 0.5f);
        SwitchPlayerStatus(PlayerType.Dog, StatusType.AutoFade, crossTime, decoNode.Config.npcConfigId, true, isAnim, 0.5f);
        SwitchPlayerStatus(PlayerType.Hero, StatusType.AutoFade, crossTime, decoNode.Config.npcConfigId, true, isAnim, 0.5f);
    }
    
    public void UpdatePlayersFadeState(DecoNode decoNode, float crossTime, bool isAnim = true)
    {
        if (decoNode == null)
        {
            SwitchPlayerStatus(PlayerType.Chief, StatusType.Fade, crossTime, 30, true, isAnim, 0.5f);
            SwitchPlayerStatus(PlayerType.Dog, StatusType.Fade, crossTime, 30, true, isAnim, 0.5f);
            SwitchPlayerStatus(PlayerType.Hero, StatusType.Fade, crossTime, 30, true, isAnim, 0.5f);
            return;
        }

        if (decoNode.Config.npcConfigId <= 0)
            return;

        SwitchPlayerStatus(PlayerType.Chief, StatusType.Fade, crossTime, decoNode.Config.npcConfigId, true, isAnim, 0.5f);
        SwitchPlayerStatus(PlayerType.Dog, StatusType.Fade, crossTime, decoNode.Config.npcConfigId, true, isAnim, 0.5f);
        SwitchPlayerStatus(PlayerType.Hero, StatusType.Fade, crossTime, decoNode.Config.npcConfigId, true, isAnim, 0.5f);
    }
    
    
    

    public void SetPosition(DecoNode decoNode, bool isDeco)
    {
        if(decoNode == null)
            return;
        
        if (decoNode.Config.npcConfigId <= 0)
            return;

        var npcConfig = DecorationConfigManager.Instance.NpcConfigList.Find(a => a.id == decoNode.Config.npcConfigId);
        if (npcConfig == null)
            return;
        
        SetPosition(PlayerType.Chief, npcConfig.chiefInfoId, isDeco);
        SetPosition(PlayerType.Dog, npcConfig.dogInfoId, isDeco);
        SetPosition(PlayerType.Hero, npcConfig.heroInfoId, isDeco);
    }

    public void SetActive(DecoNode decoNode, bool isRestParent = true)
    {
        if(decoNode == null)
            return;
        
        if (decoNode.Config.npcConfigId <= 0)
            return;

        var npcConfig = DecorationConfigManager.Instance.NpcConfigList.Find(a => a.id == decoNode.Config.npcConfigId);
        if (npcConfig == null)
            return;
        
        SetActive(PlayerType.Chief, npcConfig.chiefInfoId, isRestParent);
        SetActive(PlayerType.Dog, npcConfig.dogInfoId, isRestParent);
        SetActive(PlayerType.Hero, npcConfig.heroInfoId, isRestParent);
    }
    
    public void SetRotation(DecoNode decoNode, bool isDeco, bool isAnim = true)
    {
        if(decoNode == null)
            return;
        
        if (decoNode.Config.npcConfigId <= 0)
            return;

        var npcConfig = DecorationConfigManager.Instance.NpcConfigList.Find(a => a.id == decoNode.Config.npcConfigId);
        if (npcConfig == null)
            return;
        
        SetRotation(PlayerType.Chief, npcConfig.chiefInfoId, isDeco, isAnim);
        SetRotation(PlayerType.Dog, npcConfig.dogInfoId, isDeco, isAnim);
        SetRotation(PlayerType.Hero, npcConfig.heroInfoId, isDeco, isAnim);
    }
    
    public void SetPosition(PlayerType type, int npcInfoId, bool isDeco)
    {
        var npcInfo = DecorationConfigManager.Instance.NpcInfoList.Find(a => a.id == npcInfoId);
        if (npcInfo == null)
            return;
        
        GameObject player = GetPlayer(type);
        if(player == null)
            return;

        float[] array = isDeco ? npcInfo.decoPosition : npcInfo.finishPosition != null && npcInfo.finishPosition.Length > 0 ? npcInfo.finishPosition : npcInfo.position;
        if(array == null || array.Length == 0)
            return;

        player.transform.localPosition = GetVector3(array);
    }

    public void SetRotation(PlayerType type, int npcInfoId, bool isDeco, bool isAnim = true)
    {
        var npcInfo = DecorationConfigManager.Instance.NpcInfoList.Find(a => a.id == npcInfoId);
        if (npcInfo == null)
            return;
            
        GameObject player = GetPlayer(type);
        if(player == null)
            return;
        
        float[] array = isDeco ? npcInfo.decoRotation : npcInfo.finishRotation != null && npcInfo.finishRotation.Length > 0 ? npcInfo.finishRotation : npcInfo.rotation;
        if(array == null || array.Length == 0)
            return;

        if (isAnim)
        {
            player.transform.DOLocalRotate(GetVector3(array), 0.5f, RotateMode.Fast);
        }
        else
        {
            player.transform.rotation =  Quaternion.Euler(GetVector3(array));
        }
    }

    public void SetActive(PlayerType type, int npcInfoId, bool isRestParent = true)
    {
        GameObject player = GetPlayer(type);
        if(player == null)
            return;
        
        if(isRestParent)
            player.transform.SetParent(DecoSceneRoot.Instance.transform);
        
        var npcInfo = DecorationConfigManager.Instance.NpcInfoList.Find(a => a.id == npcInfoId);
        if (npcInfo == null)
        {
            player.SetActive(false);
            return;
        }
        
        player.SetActive(npcInfo.isShow);    
    }
    
    private Vector3 GetVector3(float[] array)
    {
        Vector3 vector3 = Vector3.zero;

        if (array == null)
            return vector3;

        if (array.Length >= 1)
            vector3.x = array[0];
        if(array.Length >= 2)
            vector3.y = array[1];
        if(array.Length >= 3)
            vector3.z = array[2];

        return vector3;
    }

    public void HidePlayer()
    {
        PlayerStatusBase statusBase = GetPlayerStatus(PlayerType.Chief);
        statusBase?.HidePlayer();
        
        statusBase = GetPlayerStatus(PlayerType.Dog);
        statusBase?.HidePlayer();
        
        statusBase = GetPlayerStatus(PlayerType.Hero);
        statusBase?.HidePlayer();
    }
    
    public void RecoverPlayer()
    {
        PlayerStatusBase statusBase = GetPlayerStatus(PlayerType.Chief);
        statusBase?.RecoverPlayer();
        
        statusBase = GetPlayerStatus(PlayerType.Dog);
        statusBase?.RecoverPlayer();
        
        statusBase = GetPlayerStatus(PlayerType.Hero);
        statusBase?.RecoverPlayer();
    }
}