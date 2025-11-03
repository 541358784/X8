using Dlugin.PluginStructs;



class AdUnit
{
    public enum AdUnitProgress { Inited, Loading, Loaded, Watching, Watched }
    public AdUnitProgress m_Progress = AdUnitProgress.Inited;
    public AdUnit()
    {
        m_AdsUnitDefine = new AdsUnitDefine();
        disposed = false;
    }
    public virtual void Cache()
    {
        //no implement
    }
    public virtual void Play()
    {
        //no implement
    }
    public virtual bool IsReady()
    {
        //no implement
        return false;
    }
    public virtual void Dispose()
    {
        //no implement
    }
    public virtual string GetPlacement()
    {
        return "";
    }
    public AdsUnitDefine m_AdsUnitDefine;
    public bool disposed;
}