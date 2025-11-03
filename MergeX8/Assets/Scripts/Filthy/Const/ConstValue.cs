namespace Filthy.ConstValue
{
    public class ConstValue
    {
        private static string _filthyLevelPath = "Filthy/Level{0}/Prefabs";

        public static string FilthyMainLevelPath(int levelId)
        {
            return string.Format(_filthyLevelPath, levelId)+"/UIFilthy";
        }
        
        private static string _filthyAudioPath = "Filthy/Level{0}/Audio/{1}";

        public static string FilthyAudioPath(int levelId, string audioName)
        {
            return string.Format(_filthyAudioPath, levelId, audioName);
        }

        public static string FilthyPrefabPath(int level, string prefabName)
        {
            return string.Format(_filthyLevelPath,level)+"/"+prefabName;
        }
    } 
}
