namespace Screw.Configs
{
    public class ConstConfig
    {
        public static string ScrewConfigPath = "Assets/Export/Configs/Screw/{0}";

        public static string FolderNameConfigPath(string finderName)
        {
            return string.Format(ScrewConfigPath, finderName);
        }
    }
}