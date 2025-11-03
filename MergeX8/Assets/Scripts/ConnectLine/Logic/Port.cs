using UnityEngine;

namespace ConnectLine.Logic
{
    public enum PortType
    {
        None=-1,
        WaterOut,
        WaterIn,
        DoublePass,
    }
    public class Port
    {
        private PortType _baseType;
        private PortType _type;
        private GameObject _connect;
        private Rect _rect;
        private bool _isConnect;
        private Transform _root;

        public Vector3 Position
        {
            get
            {
                return _connect.transform.position;
            }
        }
        
        public Port(Transform root,PortType type)
        {
            _baseType = type;
            _type = type;
            _root = root;
            _connect = _root.transform.Find("connect").gameObject;
            Vector3[] corners = new Vector3[4];
            ((RectTransform)(_connect.transform)).GetWorldCorners(corners);
            var leftDownPoint = new Vector2(corners[0].x, corners[0].y);
            var rightUpPoint = new Vector2(corners[0].x, corners[0].y);
            for (var i = 1; i < corners.Length; i++)
            {
                if (corners[i].x < leftDownPoint.x)
                {
                    leftDownPoint.x = corners[i].x;
                }
                if (corners[i].x > rightUpPoint.x)
                {
                    rightUpPoint.x = corners[i].x;
                }
                if (corners[i].y < leftDownPoint.y)
                {
                    leftDownPoint.y = corners[i].y;
                }
                if (corners[i].y > rightUpPoint.y)
                {
                    rightUpPoint.y = corners[i].y;
                }
            }
            _rect = new Rect(leftDownPoint.x, leftDownPoint.y, rightUpPoint.x - leftDownPoint.x, rightUpPoint.y - leftDownPoint.y);
            
            SetConnectActive(false);
        }

        public void SetConnectActive(bool isActive)
        {
            _connect.gameObject.SetActive(isActive);
        }

        public bool Contains(Vector3 position)
        {
            return _rect.Contains(position);
        }

        public void SetConnect(bool isConnect)
        {
            _isConnect = isConnect;
        }

        public void ResetPort()
        {
            _isConnect = false;
            _type = _baseType;
        }

        public bool IsConnect()
        {
            return _isConnect;
        }
        public PortType GetPortType() => _type;

        public void SetPortType(PortType type)
        {
            _type = type;
        }

        public bool CanConnect(Port otherPort)
        {
            if (GetPortType() == PortType.DoublePass || otherPort.GetPortType() == PortType.DoublePass)
                return true;
            if (GetPortType() == PortType.WaterIn && otherPort.GetPortType() == PortType.WaterIn)
                return false;
            if (GetPortType() == PortType.WaterOut && otherPort.GetPortType() == PortType.WaterOut)
                return false;
            return true;
        }
    }
}