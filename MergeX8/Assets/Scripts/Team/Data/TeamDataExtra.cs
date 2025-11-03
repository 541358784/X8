using Newtonsoft.Json;

namespace Scripts.UI
{
    public class TeamDataExtra
    {
        public int MemberMaxCount;
        public int TeamLevel;
        public int BadgeFrame;
        // public int Exp;
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static TeamDataExtra FromJson(string json)
        {
            var result = JsonConvert.DeserializeObject<TeamDataExtra>(json);
            return result != null ?result:new TeamDataExtra(){MemberMaxCount = 10,TeamLevel = 1,BadgeFrame = 0};
        }
    }
}