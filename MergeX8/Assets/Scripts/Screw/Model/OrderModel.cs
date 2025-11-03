using System.Collections.Generic;
using Screw;
using Screw.Configs;

namespace Screw
{
    public class OrderModel
    {
        public ColorType ColorType { get; }

        public int SlotCount { get; }

        public List<ScrewShape> Shapes { get; }

        private List<bool> _taskRecover;
        private List<bool> _taskModelRecover;

        public OrderModel(ColorType colorType, int slotCount, List<ScrewShape> shapes)
        {
            ColorType = colorType;
            SlotCount = slotCount;
            Shapes = shapes;
            _taskRecover = new List<bool>();
            _taskModelRecover = new List<bool>();

            for (int i = 0; i < SlotCount; i++)
            {
                _taskRecover.Add(false);
                _taskModelRecover.Add(false);
            }
        }

        public void SetSlotState(int slotIndex)
        {
            _taskRecover[slotIndex] = true;
        }

        public bool IsComplete()
        {
            for (int i = 0; i < _taskRecover.Count; i++)
            {
                if (!_taskRecover[i])
                    return false;
            }
            return true;
        }

        public bool GetSlotState(int index)
        {
            return _taskRecover[index];
        }

        public bool IsModelComplete()
        {
            for (int i = 0; i < _taskModelRecover.Count; i++)
            {
                if (!_taskModelRecover[i])
                    return false;
            }
            return true;
        }

        public void SetModelSlotState(int slotIndex)
        {
            _taskModelRecover[slotIndex] = true;
        }
    }
}