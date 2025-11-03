namespace Merge.Order
{
    public partial class MainOrderManager
    {
        private const int _sealTaskId = 1101110;
        private const int _dolphinTaskId = 1201110;
        private const int _capybaraTaskId = 1301110;
        
        public bool IsFinishSealTask()
        {
            return IsCompleteOrder(_sealTaskId);
        }
    
        public bool IsSealActiveTask()
        {
            return HaveTask(_sealTaskId);
        }
        
        public bool IsSealTask(int taskId)
        {
            return _sealTaskId == taskId;
        }
        
        public bool IsCapybaraTask(int taskId)
        {
            return _capybaraTaskId == taskId;
        }
        
        public bool IsFinishCapybaraTask()
        {
            return IsCompleteOrder(_capybaraTaskId);
        }

        public bool IsCapybaraActiveTask()
        {
            return HaveTask(_capybaraTaskId);
        }
    
        public bool IsFinishDolphinTask()
        {
            return IsCompleteOrder(_dolphinTaskId);
        }
    
        public bool IsDolphinActiveTask()
        {
            return HaveTask(_dolphinTaskId);
        }

        public bool IsDolphinTask(int taskId)
        {
            return _dolphinTaskId == taskId;
        }

        public bool IsSpecialTask(int taskId)
        {
            return taskId == _dolphinTaskId || taskId == _sealTaskId || taskId == _capybaraTaskId;
        }
    }
}