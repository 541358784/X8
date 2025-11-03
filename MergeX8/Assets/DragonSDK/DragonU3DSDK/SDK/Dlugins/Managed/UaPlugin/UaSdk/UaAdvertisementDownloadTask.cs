namespace DragonPlus.Ad.UA
{
    public class UaAdvertisementDownloadTask
    {
        #region Properties

        public UaAdvertisement Advertisement { get; set; }
        public bool            IsCompleted   { get; set; }
        public bool            IsFailed      { get; set; }
        public bool            IsDownloading   { get; set; }

        #endregion
    }
}