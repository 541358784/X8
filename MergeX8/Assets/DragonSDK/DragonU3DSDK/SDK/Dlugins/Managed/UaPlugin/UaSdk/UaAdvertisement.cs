using DragonU3DSDK.Network.API.Protocol;

namespace DragonPlus.Ad.UA
{
    public class UaAdvertisement
    {
        #region Properties

        public Advertisement       Advertisement    { get; set; }
        public string              Id               { get; set; }
        public bool                Loaded           { get; set; }
        public UaAdvertisementType Type             { get; set; }
        public string              VideoName        { get; set; }
        public string              AdUnitIdentifier { get; set; }
        public string              ExtraData        { get; set; }

        #endregion
    }
}