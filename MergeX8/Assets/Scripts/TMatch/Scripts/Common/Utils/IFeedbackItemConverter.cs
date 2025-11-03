namespace TMatch
{
    public interface CommonIFeedbackItemConverter<T>
    {
        bool IsMapping(int itemId, T feedBackItem);
        T ConvertCreate(int itemId, int itemCount);
        void ConvertAddTo(T feedBackItem, int itemCount);
    }
}