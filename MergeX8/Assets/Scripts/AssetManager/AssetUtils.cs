using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroupEnum
{
    UI,
    Data,
    Room,
    LocalSpriteAtlas,
    RemoteSpriteAtlas,
    RemoteConfig,
    RoomSounds,
    TMatch,
    OnePath,
    CardCollection,
    ExtraOrderRewardCoupon,
    KeepPet,
    BlueBlock,
    BlindBox,
    Screw,
    Farm,
    DigTrenchNew,
    TileMatch,
    SaveTheWhales,
    PigBox,
}

public static class AssetBundleEnum
{
    public static Dictionary<GroupEnum, string> GroupNames = new Dictionary<GroupEnum, string>
    {
        {GroupEnum.UI, "UI"},
        {GroupEnum.Data, "Data"},
        {GroupEnum.Room, "Scene"},
        {GroupEnum.RemoteSpriteAtlas, "RemoteSpriteAtlas"},
        {GroupEnum.LocalSpriteAtlas, "LocalSpriteAtlas"},
        {GroupEnum.RemoteConfig, "RemoteConfig"},
        {GroupEnum.RoomSounds, "RoomSounds"},
        {GroupEnum.TMatch, "TMatch"},
        {GroupEnum.OnePath, "OnePath"},
        {GroupEnum.CardCollection, "CardCollection"},
        {GroupEnum.ExtraOrderRewardCoupon, "ExtraOrderRewardCoupon"},
        {GroupEnum.KeepPet, "KeepPet"},
        {GroupEnum.BlueBlock,"BlueBlock"},
        { GroupEnum.BlindBox,"BlindBox"},
        { GroupEnum.Screw,"Screw"},
        { GroupEnum.Farm,"Farm"},
        { GroupEnum.DigTrenchNew,"DigTrenchNew"},
        { GroupEnum.TileMatch,"TileMatch"},
        {GroupEnum.SaveTheWhales, "SaveTheWhales"},
        {GroupEnum.PigBox, "PigBox"},
    };
}


public static class AtlasName
{
    public const string MenAvatar = "MenAvatarAtlas";
    public const string MysteryBox = "MysteryBoxAtlas";
    public const string WomenAvatar = "WomenAvatarAtlas";
    public const string MapFormat = "Map{0}Atlas";
    public const string UIShop = "ShopAtlas";
    public const string UpgradeUI = "UpgradeUIAtlas";
    public const string CommonDecorationAtlas = "UIDecorationAtlas"; 

    public const string UICommon_UIIconAll_MapUIPicture_WorldCommon_LevelStart =
        "UICommon_UIIconAll_MapUIPicture_WorldCommon_LevelStart";

    public const string UIRestaurantRankIcon = "UIRestaurantRankIconAtlas";
    public const string UIDecorationMain_IconBrush = "UIDecorationMainAtlas";
    public const string UIDailyBonus = "UIDailyBonus_UIDailyTaskAtlas";
    public const string UIStory = "UIStoryAtlas";
    public const string UIPortrait = "PortraitAtlas";
    public const string RoomFormat = "{0}";
    public const string Normal = "Login_Popup_PlayerAvatar_NormalAtlas";
    public const string UIDailyTask = "UIDailyBonus_UIDailyTaskAtlas";
    public const string GameUIPicture = "GameUIPictureAtlas";
    public const string MapGameFormat = "Map{0}GameAtlas";
    public const string Guide = "GuideAtlas";
    public const string RoomIconFormat = "Icon{0}";
    public const string GallerySpin = "GallerySpinAtlas";
    public const string CrazyTruckMap = "CrazyTruckMapAtlas";
    public const string FestivalXmasBundleAtlas = "FestivalXmasBundleAtlas";
    public const string FestivalHalloweenBundleAtlas = "FestivalHalloweenBundleAtlas";
    public const string CustomerCabinAtlas = "CustomerCabinAtlas";
    public const string PlayerAvatarAtlas = "Login_Popup_PlayerAvatar_NormalAtlas";
    public const string HappyShoppingAtlas = "HappyShoppingAtlas";

    //loading atlas
    public const string LoadingAtlas = "LoadingAtlas";

    public const string SolitaireAtlas = "SolitaireAtlas";
    public const string RoomAtlas = "RoomAtlas";

    public const string CommonAtlas = "CoomonAtlas";
    public const string HeadAtlas = "HeadAtlas";

    public const string ThemeBGAtlas = "ThemeBGAtlas";

    public const string ThemeAtlas = "ThemeAtlas";
    public const string ThemeAnimAtlas = "ThemeAnimAtlas";

    public const string CollectionAtlas = "CollectionAtlas";
    public const string ShopAtlas = "ShopAtlas";

    public const string MergeIconAtlas = "MergeIconAtlas";

    public const string HappyGoAtlas = "HappyGoAtlas";
    public const string Easter2024Atlas = "DonutAtlas";
    public const string SnakeLadderAtlas = "SnakeLadderAtlas";
    public const string MonopolyAtlas = "MonopolyAtlas";
    public const string ZumaAtlas = "ZumaAtlas";
    public const string KeepPetTurkeyAtlas = "KeepPetTurkeyAtlas";
}