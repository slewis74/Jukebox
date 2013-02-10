namespace Slew.WinRT.Data.Navigation
{
    public interface INavigationStackStorage
    {
        void StoreUris(string[] uris);
        string[] RetrieveUris();
    }
}