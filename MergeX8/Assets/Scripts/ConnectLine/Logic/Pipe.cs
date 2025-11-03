using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using Framework;
using Spine.Unity;
using UnityEngine;

namespace ConnectLine.Logic
{
    public enum PipeType
    {
        None=-1,
        WaterInlet,
        Connect,
        WaterOutlet,
        Count,
        ChangeWaterType,
    }
    public class Pipe
    {
        private PipeType _pipeType = PipeType.None;
        private List<Port> _ports = new List<Port>();

        private Transform _root;
        
        private bool _isConnect = false;

        private SkeletonGraphic _skeletonGraphic;
        private Coroutine _coroutine;

        private string[] _skeletonAnimName = new[]
        {
            "sad",
            "sad_happy",
            "happy",
            "sad_methysis",
            "methysis",
        };
        
        public Pipe(Transform root, PipeType type)
        {
            _root = root;
            _pipeType = type;

            foreach (Transform child in _root)
            {
                if (type == PipeType.WaterOutlet)
                {
                    if (child.name.Contains("Spine"))
                    {
                        _skeletonGraphic = child.transform.GetComponent<SkeletonGraphic>();
                        _skeletonGraphic.AnimationState?.SetAnimation(0, _skeletonAnimName[0], true);
                        _skeletonGraphic.Update(0);
                    }
                }

                Port port = null;
                if (child.name.Contains("inPort"))
                {
                    port = new Port(child,PortType.WaterIn);
                }
                else if (child.name.Contains("outPort"))
                {
                    port = new Port(child,PortType.WaterOut);
                }
                else if (child.name.Contains("port"))
                {
                    if (type == PipeType.WaterOutlet)
                    {
                        port = new Port(child,PortType.WaterIn);
                    }
                    else if (type == PipeType.WaterInlet)
                    {
                        port = new Port(child,PortType.WaterOut);
                    }
                    else if (type == PipeType.ChangeWaterType)
                    {
                        port = new Port(child,PortType.WaterOut);
                    }
                    else
                    {
                        port = new Port(child,PortType.DoublePass);   
                    }
                }
                else
                    continue;
                _ports.Add(port);
            }

            if (_pipeType == PipeType.WaterInlet)
            {
                SrcColorType = Line.LineModel.Blue;
                if (_root.name.Contains("blue"))
                {
                    SrcColorType = Line.LineModel.Blue;
                }
                else if (_root.name.Contains("purple"))
                {
                    SrcColorType = Line.LineModel.Purple;
                }
            }
            else if (_pipeType == PipeType.WaterOutlet)
            {
                DstColorType = Line.LineModel.Blue;
                if (_root.name.Contains("blue"))
                {
                    DstColorType = Line.LineModel.Blue;
                }
                else if (_root.name.Contains("purple"))
                {
                    DstColorType = Line.LineModel.Purple;
                }
            }
        }

        public Port Contains(Vector3 position)
        {
            foreach (var port in _ports)
            {
                if(port.IsConnect())
                    continue;
                
                if(!port.Contains(position))
                    continue;

                return port;
            }

            return null;
        }

        public void SetPipeActiveWithSelectPort(PortType selectPortType)
        {
            foreach (var port in _ports)
            {
                if (!port.IsConnect())
                {
                    switch (selectPortType)
                    {
                        case PortType.WaterIn:
                        {
                            if (port.GetPortType() != PortType.WaterIn)
                                port.SetConnectActive(true);
                            break;
                        }
                        case PortType.WaterOut:
                        {
                            if (port.GetPortType() != PortType.WaterOut)
                                port.SetConnectActive(true);
                            break;
                        }
                        default:
                        {
                            port.SetConnectActive(true);
                            break;
                        }
                    }   
                }
            }
        }
        public void SetPortActive(bool isActive, bool isForce = false)
        {
            foreach (var port in _ports)
            {
                if (!isForce)
                {
                    if (port.IsConnect())
                        continue;
                }

                port.SetConnectActive(isActive);
            }
        }

        public bool IsPipePort(Port port)
        {
            if (_ports.Contains(port))
                return true;

            return false;
        }

        public Line.LineModel SrcColorType = Line.LineModel.None;//waterIn特有的属性,水源类型
        public Line.LineModel DstColorType = Line.LineModel.None;//waterOut特有的属性,需求水类型
        public Line.LineModel ColorType = Line.LineModel.White;
        public void SetConnect(Line.LineModel colorType)
        {
            _isConnect = true;
            ColorType = colorType;
        }

        public Line.LineModel GetColorType()
        {
            return ColorType;
        }

        public Line.LineModel OutColorType()
        {
            if (GetPipeType() == PipeType.ChangeWaterType && ColorType == Line.LineModel.Purple)
                return Line.LineModel.Blue;
            return ColorType;
        }

        public void RestConnect()
        {
            _isConnect = false;
            ColorType = Line.LineModel.White;
            foreach (var port in _ports)
            {
                port.ResetPort();
            }

            RestAnim();
        }

        // public bool HaveNoConnectPort()
        // {
        //     foreach (var port in _ports)
        //     {
        //        if(port.IsConnect())
        //            continue;
        //
        //        return true;
        //     }
        //
        //     return false;
        // }
        
        public bool IsConnect()
        {
            return _isConnect;
        }

        public PipeType GetPipeType()
        {
            return _pipeType;
        }

        public int GetActivePortNum()
        {
            int num = 0;
            foreach (var port in _ports)
            {
                if(port.IsConnect())
                    continue;

                num += 1;
            }

            return num;
        }
        public int GetActiveWaterOutPortNum()
        {
            int num = 0;
            foreach (var port in _ports)
            {
                if(port.IsConnect())
                    continue;
                if (port.GetPortType() == PortType.DoublePass || 
                    port.GetPortType() == PortType.WaterOut)
                    num += 1;
            }

            return num;
        }
        public int GetConnectWaterInPortNum()
        {
            int num = 0;
            foreach (var port in _ports)
            {
                if(port.IsConnect() && (port.GetPortType() == PortType.WaterIn || port.GetPortType() == PortType.DoublePass))
                    num += 1;
            }

            return num;
        }

        private void RestAnim()
        {
            if(_skeletonGraphic == null)
                return;
            
            _skeletonGraphic.AnimationState?.SetAnimation(0, _skeletonAnimName[0], true);
            _skeletonGraphic.Update(0);
            
            CoroutineManager.Instance.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        public int PlayAnim(int playType)
        {
            var audioId = GetAnimAudioId(playType);
            if(_skeletonGraphic == null)
                return audioId;

            _coroutine = CoroutineManager.Instance.StartCoroutine(PlaySpineAnim(playType));
            return audioId;
        }

        public int GetAnimAudioId(int playType)
        {
            if (_skeletonGraphic != null)
            {
                if (_skeletonGraphic.SkeletonDataAsset.name.Contains("horse"))
                {
                    if (playType == 0)
                        return 101;
                    else if (playType == 1)
                        return -1;
                }
                else if (_skeletonGraphic.SkeletonDataAsset.name.Contains("fish"))
                {
                    if (playType == 0)
                        return 110;
                    else if (playType == 1)
                        return -1;
                }   
            }
            return 101;
        }

        private IEnumerator PlaySpineAnim(int playType)//0:胜利,1:失败
        {
            if (playType == 0)
            {
                _skeletonGraphic.AnimationState?.SetAnimation(0, _skeletonAnimName[1], false);
                _skeletonGraphic.Update(0);

                yield return new WaitForSeconds(1.66f);
            
                _skeletonGraphic.AnimationState?.SetAnimation(0, _skeletonAnimName[2], true);
                _skeletonGraphic.Update(0);   
            }
            else if (playType == 1)
            {
                _skeletonGraphic.AnimationState?.SetAnimation(0, _skeletonAnimName[3], false);
                _skeletonGraphic.Update(0);

                yield return new WaitForSeconds(1.66f);
            
                _skeletonGraphic.AnimationState?.SetAnimation(0, _skeletonAnimName[4], true);
                _skeletonGraphic.Update(0);
            }
        }

        public Vector3 GetFirstPortPosition()
        {
            return _ports[0].Position;
        }
    }
}