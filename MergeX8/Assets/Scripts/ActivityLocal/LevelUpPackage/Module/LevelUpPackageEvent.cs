using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string LevelUpPackageStart = "LevelUpPackageStart";
    public const string LevelUpPackageEnd = "LevelUpPackageEnd";
}
public class EventLevelUpPackageStart : BaseEvent
{
    public StorageLevelUpPackageSinglePackage PackageStorage;
    public EventLevelUpPackageStart() : base(EventEnum.LevelUpPackageStart) { }

    public EventLevelUpPackageStart(StorageLevelUpPackageSinglePackage storage) : base(EventEnum.LevelUpPackageStart)
    {
        PackageStorage = storage;
    }
}
public class EventLevelUpPackageEnd : BaseEvent
{
    public StorageLevelUpPackageSinglePackage PackageStorage;
    public EventLevelUpPackageEnd() : base(EventEnum.LevelUpPackageEnd) { }

    public EventLevelUpPackageEnd(StorageLevelUpPackageSinglePackage storage) : base(EventEnum.LevelUpPackageEnd)
    {
        PackageStorage = storage;
    }
}