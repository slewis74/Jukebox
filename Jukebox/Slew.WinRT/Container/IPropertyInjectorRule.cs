namespace Slew.WinRT.Container
{
    public interface IPropertyInjectorRule
    {
        void Process<T>(T obj);
    }
}