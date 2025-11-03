using System.Collections.Generic;

namespace DragonPlus.Config
{
    public class ContactUsFaqConfig
    {
        public int Id { get; set; }
        
        public List<int> AndroidQuestionList { get; set; }
        
        public List<int> IosQuestionList { get; set; }
        
        public string Question { get; set; }
        
        public string Answer { get; set; }
        
        public int YN { get; set; }
        
        public int Successor { get; set; }
        
        public int SuccessorY { get; set; }
        
        public int SuccessorN { get; set; }
    }
}


