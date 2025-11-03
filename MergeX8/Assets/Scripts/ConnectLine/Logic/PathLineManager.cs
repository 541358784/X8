using System.Collections.Generic;
using UnityEngine;

namespace ConnectLine.Logic
{
    public class PathLineManager : Singleton<PathLineManager>
    {
        private Transform _lineRoot;
        private int _order;

        private List<Line> _lines = new List<Line>();

        public void InitPathLine(Transform root, int order)
        {
            _order = order;
            _lineRoot = root;

            for (int i = 1; i <= 5; i++)
            {
                var lineRenderTrans = _lineRoot.transform.Find("line" + i);
                if (!lineRenderTrans)
                    continue;
                var lineRender = lineRenderTrans.GetComponent<LineRenderer>();

                lineRender.useWorldSpace = true;
                lineRender.sortingOrder = _order + i;
                lineRender.startWidth = lineRender.endWidth = 0.25f;

                if (i == 1 || i == 2)
                    lineRender.startWidth = lineRender.endWidth = 0.32f;
            }

            for (int i = 0; i < 5; i++)
            {
                NewLine();
            }
        }

        public Line Spawn()
        {
            foreach (var line in _lines)
            {
                if (line.IsFree())
                {
                    line.SetFree(false);
                    return line;
                }
            }

            var newLine = NewLine();
            newLine.SetFree(false);
            
            return newLine;
        }

        public void CleanPath()
        {
            foreach (var lineRenderer in _lines)
            {
                lineRenderer.SetFree(true);
                lineRenderer.SetLineModel(Line.LineModel.None);
            }
        }

        private Line NewLine()
        {
            GameObject cloneLine = GameObject.Instantiate(_lineRoot.gameObject);
            CommonUtils.AddChild(_lineRoot.transform.parent, cloneLine.transform);

            Line line = new Line();
            line.Init(cloneLine.transform);

            _lines.Add(line);

            return line;
        }

        public void Release()
        {
            foreach (var lineRenderer in _lines)
            {
                lineRenderer.Release();
            }

            _lines.Clear();
        }

        public bool CheckIntersections(Line line)
        {
            foreach (var lineRenderer in _lines)
            {
                if(lineRenderer.IsFree() || lineRenderer == line)
                    continue;

                if (line.GetBounds().Intersects(lineRenderer.GetBounds()))
                {
                    if(!DoIntersect(line.GetLineRenderer(), lineRenderer.GetLineRenderer()))
                        continue;
                    
                    //Debug.LogError("相交");
                    return true;
                }
            }

            //Debug.LogError("不相交");
            return false;
        }
        
        bool DoIntersect(LineRenderer line1, LineRenderer line2)
        {
            Vector3[] points1 = new Vector3[line1.positionCount];
            Vector3[] points2 = new Vector3[line2.positionCount];

            line1.GetPositions(points1);
            line2.GetPositions(points2);

            for (int i = 0; i < points1.Length - 1; i++)
            {
                for (int j = 0; j < points2.Length - 1; j++)
                {
                    if (SegmentsIntersect(points1[i], points1[i + 1], points2[j], points2[j + 1]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool SegmentsIntersect(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
        {
            Vector3 p = p2 - p1;
            Vector3 q = q2 - q1;
            Vector3 r = q1 - p1;
            float pqCrossR = Vector3.Cross(p, r).magnitude;
            float pqCrossQ = Vector3.Cross(p, q).magnitude;

            // 如果两条线段平行，则不相交
            if (pqCrossQ < float.Epsilon)
                return false;

            float t = pqCrossR / pqCrossQ;
            float u = Vector3.Cross(r, q).magnitude / pqCrossQ;

            // 如果 t 和 u 大于等于 0 且小于等于 1，则两条线段相交
            return (t >= 0 && t <= 1 && u >= 0 && u <= 1);
        }
    }
}