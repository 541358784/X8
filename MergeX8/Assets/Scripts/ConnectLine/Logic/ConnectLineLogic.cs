using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectLine.Logic
{
    public partial class ConnectLineLogic : MonoBehaviour
    {
        private List<Pipe> _pipes = new List<Pipe>();
        private Rect _controlArea;
        private int _order;
        private Canvas _canvas;
        private Canvas _bgCanvas;


        private bool _canDrawLine = false;
        private Vector3 _currentPosition = Vector3.zero;
        private float _moveMinDes = 0.03f;
        private Port _startPort;
        private Port _endPort;
        private Line _currentLine;

        private bool _canInput = false;
        public bool CanInput
        {
            set
            {
                _canInput = value;
            }
            get
            {
                return _canInput;
            }
        }
        
        private void Awake()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = 0;
            
            _bgCanvas = transform.Find("bg").gameObject.AddComponent<Canvas>();
            _bgCanvas.overrideSorting = true;
            _bgCanvas.sortingOrder = 0;
        }

        public void InitOrder(int order)
        {
            _order = order;
        }
        private void Start()
        {
            _bgCanvas.sortingOrder = _order+1;
            _canvas.sortingOrder = _order+6;

            _guideRoot.GetComponent<Canvas>().sortingOrder = _order + 10;
            
            RectTransform area = (RectTransform)transform.Find("ControlArea");
            Vector3[] corners = new Vector3[4];
            area.GetWorldCorners(corners);
            _controlArea = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

            area.gameObject.SetActive(false);
            
            InitPipes(transform.Find("WaterInlet"), PipeType.WaterInlet);
            InitPipes(transform.Find("Pipes"), PipeType.Connect);
            InitPipes(transform.Find("WaterOutlet"), PipeType.WaterOutlet);
            InitPipes(transform.Find("CleanWaterPipes"), PipeType.ChangeWaterType);

            PathLineManager.Instance.InitPathLine(transform.Find("LineRender/Lines"), _order+1);

            CheckGuide();
        }

        private void InitPipes(Transform root, PipeType type)
        {
            if (!root)
                return;
            foreach (Transform child in root)
            {
                Pipe pipe = new Pipe(child, type);
                _pipes.Add(pipe);
            }
        }

        public void Update()
        {
            if(!CanInput)
                return;
            
            if (Input.GetMouseButtonDown(0))
            {
                StopGuide();
                _canDrawLine = false;
                _startPort = null;
                _endPort = null;
                _currentLine = null;
                
                Vector3 mousePosition = UIRoot.Instance.mUICamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;
                if(!InControlArea(mousePosition))
                    return;

                _currentLine = PathLineManager.Instance.Spawn();
                
                _startPort = Contains(mousePosition);
                if (_startPort != null)
                {
                    _startPort.SetConnect(true);
                    _currentLine.SetLineModel(Line.LineModel.Black);
                    Vector3 position = _startPort.Position;
                    position.z = 0;
                    _currentLine.SetLineRenderPosition(position);
                }
                else
                {
                    _currentLine.SetLineModel(Line.LineModel.White);
                    _currentLine.SetLineRenderPosition(mousePosition);
                }

                if (_startPort != null)
                {
                    SetPipeActiveWithSelectPort(_startPort.GetPortType());
                }
                else
                {
                    SetPipeActive(true);   
                }

                _currentPosition = mousePosition;
                _canDrawLine = true;
            }

            if (Input.GetMouseButton(0))
            {
                if(!_canDrawLine)
                    return;
                
                // StopGuide();
                
                Vector3 mousePosition = UIRoot.Instance.mUICamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;

                mousePosition = ControlAreaLimit(mousePosition);
                if(Vector3.Distance(_currentPosition, mousePosition) < _moveMinDes)
                    return;

                if (_startPort != null)
                {
                    var tempEndPort = Contains(mousePosition);
                    if (tempEndPort != null && _startPort.CanConnect(tempEndPort))
                    {
                        _endPort = tempEndPort;
                        if (PathLineManager.Instance.CheckIntersections(_currentLine))
                        {
                            //MouseButtonUp();
                            EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECTLINE_FAILED);
                            return;
                        }
                        SetPipeActive(false);
                        _canDrawLine = false;
                        _endPort.SetConnect(true);
                        
                        Vector3 position = _endPort.Position;
                        position.z = 0;
                        _currentLine.SetLineRenderPosition(position);
                        
                        BuildLink(_startPort, _endPort, _currentLine);
                        if (CheckWaterOutDisMatch())
                        {
                            EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECTLINE_FAILED);
                            return;
                        }

                        if (!HaveValidWaterOutlet())
                        {
                            EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECTLINE_SUCCESS);
                            return;
                        }
                        
                        int inPortNum = 0;
                        int outPortNum = 0;
                        
                        CalculatePort(ref inPortNum, ref outPortNum);
                        if (outPortNum == 0)
                        {
                            EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECTLINE_FAILED);
                            return;
                        }
                        if (outPortNum < inPortNum)
                        {
                            EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECTLINE_FAILED);
                            return;
                        }
                        return;
                    } 
                }
                
                _currentPosition = mousePosition;
                _currentLine.SetLineRenderPosition(mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                MouseButtonUp();
            }
        }

        private void MouseButtonUp()
        {
            if(!_canDrawLine)
                return;

            if (_endPort != null)
                _endPort.SetConnect(false);
                
            if(_startPort != null)
                _startPort.SetConnect(false);
                
            _currentLine.SetFree(true);
            SetPipeActive(false);
            _canDrawLine = false;
        }
        private bool InControlArea(Vector3 position)
        {
            return _controlArea.Contains(position);
        }
        
        private Vector3 ControlAreaLimit(Vector3 position)
        {
            position.x = Mathf.Min(position.x, _controlArea.xMax);
            position.x = Mathf.Max(position.x, _controlArea.xMin);
            
            position.y = Mathf.Min(position.y, _controlArea.yMax);
            position.y = Mathf.Max(position.y, _controlArea.yMin);

            return position;
        }

        private Port Contains(Vector3 position)
        {
            foreach (var pipe in _pipes)
            {
                Port port = pipe.Contains(position);
                if(port == null)
                    continue;

                return port;
            }

            return null;
        }

        private void SetPipeActive(bool isActive, bool isForce = false)
        {
            foreach (var pipe in _pipes)
            {
                pipe.SetPortActive(isActive, isForce);
            } 
        }
        public void SetPipeActiveWithSelectPort(PortType selectPortType)
        {
            foreach (var pipe in _pipes)
            {
                pipe.SetPipeActiveWithSelectPort(selectPortType);
            } 
        }

        private void RestConnect()
        {
            foreach (var pipe in _pipes)
            {
                pipe.RestConnect();
            } 
        }
        public void Reset()
        {
            RestConnect();
            _linkLines.Clear();
            
            SetPipeActive(false, true);
        }

        private Pipe GetPipeByPort(Port port)
        {
            foreach (var pipe in _pipes)
            {
                if(!pipe.IsPipePort(port))
                    continue;

                return pipe;
            }

            return null;
        }

        private void CalculatePort(ref int inPortNum, ref int outPortNum)
        {
            var hasWaterSource = false;
            foreach (var pipe in _pipes)
            {
                switch (pipe.GetPipeType())
                {
                    case PipeType.WaterInlet:
                    {
                        outPortNum += pipe.GetActivePortNum();
                        if (!hasWaterSource && pipe.GetActivePortNum() > 0)
                        {
                            hasWaterSource = true;
                        }
                        break;
                    }
                    case PipeType.WaterOutlet:
                    {
                        inPortNum += pipe.GetActivePortNum();
                        break;
                    }
                    case PipeType.Connect:
                    {
                        if (pipe.IsConnect())
                        {
                            outPortNum += pipe.GetActivePortNum();
                            if (!hasWaterSource && pipe.GetActivePortNum() > 0)
                            {
                                hasWaterSource = true;
                            }   
                        }
                        else
                        {
                            if (pipe.GetConnectWaterInPortNum() > 0)
                            {
                                outPortNum += pipe.GetActiveWaterOutPortNum();
                            }
                            else
                            {
                                outPortNum += pipe.GetActivePortNum()-1;
                            }
                        }
                        break;
                    }
                    case PipeType.ChangeWaterType:
                    {
                        if (pipe.IsConnect())
                        {
                            outPortNum += pipe.GetActivePortNum();   
                            if (!hasWaterSource && pipe.GetActivePortNum() > 0)
                            {
                                hasWaterSource = true;
                            }
                        }
                        else
                        {
                            if (pipe.GetConnectWaterInPortNum() > 0)
                            {
                                outPortNum += pipe.GetActiveWaterOutPortNum();
                            }
                            else
                            {
                                outPortNum += pipe.GetActivePortNum()-1;
                            }
                        }
                        break;
                    }
                }
            }
            if (!hasWaterSource)
                outPortNum = 0;
        }

        private bool HaveValidWaterOutlet()
        {
            foreach (var pipe in _pipes)
            {
                if(pipe.GetPipeType() != PipeType.WaterOutlet)
                    continue;

                if (!pipe.IsConnect())
                    return true;
            }

            return false;
        }
        
        private bool HaveValidWaterInlet()
        {
            foreach (var pipe in _pipes)
            {
                if(pipe.GetPipeType() != PipeType.WaterInlet)
                    continue;

                if (!pipe.IsConnect())
                    return true;
            }

            return false;
        }
        
        // private bool HaveNoConnectWaterOutlet()
        // {
        //     foreach (var pipe in _pipes)
        //     {
        //         if(pipe.GetPipeType() != PipeType.WaterOutlet)
        //             continue;
        //
        //         if (!pipe.HaveNoConnectPort())
        //             continue;
        //
        //         return true;
        //     }
        //
        //     return false;
        // }

        public bool CheckWaterOutDisMatch()
        {
            foreach (var pipe in _pipes)
            {
                if (pipe.GetPipeType() == PipeType.WaterOutlet && pipe.IsConnect())
                {
                    if (pipe.DstColorType != pipe.GetColorType())
                        return true;
                }
            }
            return false;
        }
    }
}