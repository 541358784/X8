using UnityEngine;

namespace ConnectLine.Logic
{
    public class Line
    {
        public enum LineModel
        {
            None,
            White,
            Black,
            Blue,
            Purple,
        }
        
        
        private LineRenderer[] _lineRenders = new LineRenderer[5];
        private Transform _root;

        private bool _isFree = true;

        private LineModel _lineModel = LineModel.None;
        
        public void Init(Transform root)
        {
            _root = root;
            
            for (int i = 1; i <= 5; i++)
            {
                var lineNode = _root.transform.Find("line" + i);
                if (lineNode)
                {
                    var lineRender = lineNode.GetComponent<LineRenderer>();
                    _lineRenders[i - 1] = lineRender;   
                }
            }

            SetLineModel(LineModel.None);
        }

        void HideAllLineRender()
        {
            foreach (var lineRenderer in _lineRenders)
            {
                if (lineRenderer)
                    lineRenderer.gameObject.SetActive(false);
            }
        }
        public void SetLineModel(LineModel model)
        {
            _lineModel = model;
            switch (model)
            {
                case LineModel.None:
                {
                    HideAllLineRender();
                    break;
                }
                case LineModel.White:
                {
                    HideAllLineRender();
                    _lineRenders[1].gameObject.SetActive(true);
                    _lineRenders[2].gameObject.SetActive(true);
                    break;
                }
                case LineModel.Black:
                {
                    HideAllLineRender();
                    _lineRenders[1].gameObject.SetActive(true);
                    _lineRenders[2].gameObject.SetActive(true);
                    break;
                }
                case LineModel.Blue:
                {
                    HideAllLineRender();
                    _lineRenders[0].gameObject.SetActive(true);
                    _lineRenders[3].gameObject.SetActive(true);
                    break;
                }
                case LineModel.Purple:
                {
                    HideAllLineRender();
                    _lineRenders[0].gameObject.SetActive(true);
                    _lineRenders[4].gameObject.SetActive(true);
                    break;
                }
            }
        }
        public bool IsFree()
        {
            return _isFree;
        }

        public void SetFree(bool isFree)
        {
            _isFree = isFree;
            
            if(!_isFree)
                return;
            
            foreach (var lineRenderer in _lineRenders)
            {
                if (lineRenderer) 
                    lineRenderer.positionCount = 0;
            }
            SetLineModel(LineModel.None);
        }
        
        public void SetLineRenderPosition(Vector3 position)
        {
            foreach (var lineRenderer in _lineRenders)
            {
                if (!lineRenderer)
                    continue;
                int index = lineRenderer.positionCount;
                lineRenderer.positionCount = index + 1;

                lineRenderer.SetPosition(index, position);
            }
        }

        public void Release()
        {
            foreach (var lineRenderer in _lineRenders)
            {
                if (lineRenderer)
                    GameObject.DestroyImmediate(lineRenderer.gameObject);
            }

            _lineRenders = null;
        }

        public LineModel GetLineModel()
        {
            return _lineModel;
        }

        public Bounds GetBounds()
        {
            return _lineRenders[1].bounds;
        }

        public LineRenderer GetLineRenderer()
        {
            return _lineRenders[1];
        }
    }
}