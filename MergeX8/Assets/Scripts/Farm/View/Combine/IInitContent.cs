namespace Farm.View
{
    public interface IInitContent
    {
        public void InitContent(object content);
        
        public void UpdateData(params object[] param);
    }
}