using System;
using System.Collections.Generic;
using DragonPlus;
using Framework;
using UnityEngine;

namespace ConnectLine.Logic
{
    public partial class ConnectLineLogic
    {
        private List<LinkLine> _linkLines = new List<LinkLine>();
        
        public class LinkLine
        {
            public Port _startPort;
            public Port _endPort;
            public Line _line;

            public Pipe _startPipe;
            public Pipe _endPipe;
            
            public LinkLine(Port start, Port end, Line line, Pipe startPipe, Pipe endPipe)
            {
                _startPort = start;
                _endPort = end;
                _line = line;

                _startPipe = startPipe;
                _endPipe = endPipe;
            }
        }

        public void BuildLink(Port start, Port end, Line line)
        {
            LinkLine linkLine = new LinkLine(start, end, line, GetPipeByPort(start), GetPipeByPort(end));
            if (start.GetPortType() != PortType.DoublePass)
            {
                if (start.GetPortType() == PortType.WaterIn)
                {
                    end.SetPortType(PortType.WaterOut);
                }
                else
                {
                    end.SetPortType(PortType.WaterIn);
                }
            }
            else if(end.GetPortType() != PortType.DoublePass)
            {
                if (end.GetPortType() == PortType.WaterIn)
                {
                    start.SetPortType(PortType.WaterOut);
                }
                else
                {
                    start.SetPortType(PortType.WaterIn);
                }
            }
            _linkLines.Add(linkLine);

            if (linkLine._startPipe.GetPipeType() == PipeType.WaterInlet || linkLine._endPipe.GetPipeType() == PipeType.WaterInlet)
            {
                var colorType = Line.LineModel.None;
                if (linkLine._startPipe.GetPipeType() == PipeType.WaterInlet)
                {
                    colorType = linkLine._startPipe.SrcColorType;
                    linkLine._startPipe.SetConnect(colorType);
                }
                else
                {
                    colorType = linkLine._endPipe.SrcColorType;
                    linkLine._endPipe.SetConnect(colorType);
                }
                // linkLine._startPipe.SetConnect(colorType);
                // linkLine._endPipe.SetConnect(colorType);
                // line.SetLineModel(colorType);
            }

            ConnectLine();
            AudioManager.Instance.PlaySound(100);
        }

        private void ConnectLine()
        {
            bool isLoop = true;
            List<int> audioList = new List<int>();
            while (isLoop)
            {
                isLoop = false;
                foreach (var linkLine in _linkLines)
                {
                    if (!linkLine._startPipe.IsConnect() && !linkLine._endPipe.IsConnect())
                        continue;
                    if (linkLine._startPort.GetPortType() == PortType.DoublePass &&
                        linkLine._startPort.GetPortType() == PortType.DoublePass)
                    {
                        var colorType = (Line.LineModel)Math.Max((int)linkLine._startPipe.OutColorType(),(int)linkLine._endPipe.OutColorType());
                        if (linkLine._startPipe.GetColorType() != colorType)
                        {
                            isLoop = true;
                            linkLine._startPipe.SetConnect(colorType);   
                        }
                        if (linkLine._endPipe.GetColorType() != colorType)
                        {
                            isLoop = true;
                            linkLine._endPipe.SetConnect(colorType);   
                        }
                        linkLine._line.SetLineModel(colorType);
                        if (isLoop)
                            break;
                    }
                    else
                    {
                        if (linkLine._startPort.GetPortType() == PortType.WaterOut)
                        {
                            var colorType = linkLine._startPipe.OutColorType();
                            var targetColorType = linkLine._endPipe.GetColorType();
                            if (colorType != targetColorType && (int)colorType > (int)targetColorType)
                            {
                                isLoop = true;
                                linkLine._endPipe.SetConnect(colorType);
                                linkLine._line.SetLineModel(colorType);
                                if (linkLine._endPipe.GetPipeType() == PipeType.WaterOutlet)
                                {
                                    if (linkLine._endPipe.DstColorType == colorType)
                                    {
                                        var audioId = linkLine._endPipe.PlayAnim(0);
                                        if (audioId > 0 && !audioList.Contains(audioId))
                                            audioList.Add(audioId);
                                        
                                    }
                                    else
                                    {
                                        var audioId = linkLine._endPipe.PlayAnim(1);
                                        if (audioId > 0 && !audioList.Contains(audioId))
                                            audioList.Add(audioId);
                                    }
                                }
                            }
                            else
                            {
                                linkLine._line.SetLineModel(colorType);
                            }
                        }
                        else
                        {
                            var colorType = linkLine._endPipe.OutColorType();
                            var targetColorType = linkLine._startPipe.GetColorType();
                            if (colorType != targetColorType && (int)colorType > (int)targetColorType)
                            {
                                isLoop = true;
                                linkLine._startPipe.SetConnect(colorType);
                                linkLine._line.SetLineModel(colorType);
                                if (linkLine._startPipe.GetPipeType() == PipeType.WaterOutlet)
                                {
                                    if (linkLine._startPipe.DstColorType == colorType)
                                    {
                                        var audioId = linkLine._startPipe.PlayAnim(0);
                                        if (audioId > 0 && !audioList.Contains(audioId))
                                            audioList.Add(audioId);
                                    }
                                    else
                                    {
                                        var audioId = linkLine._startPipe.PlayAnim(1);
                                        if (audioId > 0 && !audioList.Contains(audioId))
                                            audioList.Add(audioId);
                                    } 
                                }
                            }
                            else
                            {
                                linkLine._line.SetLineModel(colorType);
                            }
                        }
                        if (isLoop)
                            break; 
                    }
                }
            }
            if (audioList.Count > 0)
            {
                foreach (var audioId in audioList)
                {
                    if (audioId == 101)
                    {
                        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1.66f, () =>
                        {
                            AudioManager.Instance.PlaySound(audioId);
                        }));       
                    }
                    else if(audioId == 110)
                    {
                        AudioManager.Instance.PlaySound(audioId);
                    }
                }
            }
        }

        public bool HaveValidPort()
        {
            foreach (var line in _linkLines)
            {
                if (line._startPipe.GetPipeType() != PipeType.WaterInlet && line._endPipe.GetPipeType() != PipeType.WaterInlet)
                    continue;
                
                if (line._startPipe.GetPipeType() != PipeType.Connect && line._endPipe.GetPipeType() != PipeType.Connect)
                    return true;
            }

            return false;
        }

        // private bool IsLoopPort()
        // {
        //     foreach (var line in _linkLines)
        //     {
        //         if (line._startPipe.HaveNoConnectPort() || line._endPipe.HaveNoConnectPort())
        //             return false;
        //     }
        //
        //     return true;
        // }
    }
}