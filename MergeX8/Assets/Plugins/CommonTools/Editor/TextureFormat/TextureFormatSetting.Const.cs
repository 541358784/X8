
public static class TextureFormatSettingConst
{
    
    public const string TextureFormatSettingsPath = "Assets/Plugins/CommonTools/Editor/TextureFormat/TextureFormatSettings.asset";
    
    public static string ExcludePostFix = "_cc";
    public static string NameFormatNone = "-";
    
    public static readonly string[] NameFormatPre =
    {
        NameFormatNone, // none, dont move
        
        "ui_",
        "tx_",
        "bg_",
        "ReflectionProbe_",
        "Room_",
        "room2d_",
    };

    public static readonly string[] NameFormatPost =
    {
        NameFormatNone,// none, dont move
        
        "_AlbedoTransparency",
        "_AO",
        "_MetallicSmoothness",
        "_Normal",
        "_bg_ABANDON",
        "_dir",
        "_light",
        "_AO_Metallic_Combin",
        "_Emission",
        "_2d_256",
    };

    public static readonly string[] TextureMaxSize =
    {
        "32",
        "64",
        "128",
        "256",
        "512",
        "1024",
        "2048",
        "4096",
        "8192",
    };

}
